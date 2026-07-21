using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Threading;
using SomeTexty.Tools;

namespace SomeTexty.Autocorrect;

public partial class AutocorrectView : UserControl, IClipboardAware
{
    private const int MaxSuggestionsShown = 8;

    private readonly CorrectionService _service = new(SpellEngine.Instance);
    private readonly DispatcherTimer _debounce;

    private IReadOnlyList<Misspelling> _current = Array.Empty<Misspelling>();
    private SquiggleAdorner? _squiggle;

    public AutocorrectView()
    {
        InitializeComponent();

        _debounce = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(350) };
        _debounce.Tick += (_, _) => { _debounce.Stop(); Analyze(); };

        Input.Loaded += (_, _) => AttachSquiggleAdorner();
        Input.SizeChanged += (_, _) => RefreshSquiggles();
        Input.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler((_, _) => RefreshSquiggles()));

        Analyze();
    }

    private void AttachSquiggleAdorner()
    {
        var layer = AdornerLayer.GetAdornerLayer(Input);
        if (layer is null)
        {
            return;
        }

        _squiggle ??= new SquiggleAdorner(Input);

        var existing = layer.GetAdorners(Input);
        if (existing is null || Array.IndexOf(existing, _squiggle) < 0)
        {
            layer.Add(_squiggle);
        }

        RefreshSquiggles();
    }

    private void RefreshSquiggles()
    {
        if (_squiggle is null)
        {
            return;
        }

        var segments = new List<Rect>();

        foreach (var (start, length) in _service.FindMisspelledSpans(Input.Text))
        {
            Rect first = Input.GetRectFromCharacterIndex(start);
            Rect last = Input.GetRectFromCharacterIndex(start + length - 1);
            Rect afterWord = Input.GetRectFromCharacterIndex(start + length);

            if (first.IsEmpty || afterWord.IsEmpty || Math.Abs(first.Top - last.Top) > 0.5)
            {
                // Word wraps across lines (rare) or isn't laid out; skip it.
                continue;
            }

            double width = Math.Max(2, afterWord.Left - first.Left);
            segments.Add(new Rect(first.Left, first.Bottom - 1, width, 0));
        }

        _squiggle.SetSegments(segments);
    }

    public void OfferClipboardText(string text)
    {
        if (string.IsNullOrEmpty(Input.Text))
        {
            Input.Text = text;
        }
    }

    private void OnInputChanged(object sender, TextChangedEventArgs e)
    {
        _debounce.Stop();
        _debounce.Start();
    }

    private void Analyze()
    {
        string text = Input.Text;

        if (string.IsNullOrWhiteSpace(text))
        {
            _current = Array.Empty<Misspelling>();
            Issues.Children.Clear();
            Status.Text = "Type to check spelling.";
            UpdateButtons();
            RefreshSquiggles();
            return;
        }

        _current = _service.Analyze(text);
        RenderIssues();

        Status.Text = _current.Count == 0
            ? "No spelling issues."
            : $"{_current.Count} word(s) to review — click one for suggestions.";

        UpdateButtons();
        RefreshSquiggles();
    }

    private void RenderIssues()
    {
        Issues.Children.Clear();

        foreach (var misspelling in _current)
        {
            Issues.Children.Add(BuildChip(misspelling));
        }
    }

    private Button BuildChip(Misspelling misspelling)
    {
        var chip = new Button
        {
            Style = (Style)Resources["WordChip"],
            Content = misspelling.Word,
            ContextMenu = BuildSuggestionMenu(misspelling),
        };

        // Left-click opens the suggestion menu below the chip.
        chip.Click += (_, _) =>
        {
            if (chip.ContextMenu is { } menu)
            {
                menu.PlacementTarget = chip;
                menu.Placement = PlacementMode.Bottom;
                menu.IsOpen = true;
            }
        };

        return chip;
    }

    private ContextMenu BuildSuggestionMenu(Misspelling misspelling)
    {
        var menu = new ContextMenu();
        string word = misspelling.Word;

        if (misspelling.Suggestions.Count == 0)
        {
            menu.Items.Add(new MenuItem { Header = "(no suggestions)", IsEnabled = false });
        }
        else
        {
            foreach (string suggestion in misspelling.Suggestions.Take(MaxSuggestionsShown))
            {
                string replacement = suggestion;
                var item = new MenuItem { Header = suggestion };
                item.Click += (_, _) => ApplySuggestion(word, replacement);
                menu.Items.Add(item);
            }
        }

        menu.Items.Add(new Separator());

        var add = new MenuItem { Header = "Add to dictionary" };
        add.Click += (_, _) => AddToDictionary(word);
        menu.Items.Add(add);

        return menu;
    }

    private void AddToDictionary(string word)
    {
        _service.AddToDictionary(word);
        Analyze(); // The word is no longer flagged; refresh immediately.
    }

    private void ApplySuggestion(string word, string replacement)
    {
        ReplaceAllText(_service.ReplaceWord(Input.Text, word, replacement));
    }

    private void OnFixAll(object sender, RoutedEventArgs e)
    {
        string text = Input.Text;

        foreach (var misspelling in _current)
        {
            if (misspelling.Suggestions.Count > 0)
            {
                text = _service.ReplaceWord(text, misspelling.Word, misspelling.Suggestions[0]);
            }
        }

        ReplaceAllText(text);
    }

    /// <summary>
    /// Replaces the whole text through the edit pipeline (select-all then overwrite)
    /// rather than assigning Text directly, so the change stays on the undo stack and
    /// Ctrl+Z reverts it. Re-analysis follows from the resulting TextChanged.
    /// </summary>
    private void ReplaceAllText(string text)
    {
        if (text == Input.Text)
        {
            return;
        }

        Input.SelectAll();
        Input.SelectedText = text;
        Input.CaretIndex = Input.Text.Length;
    }

    private void OnCopy(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(Input.Text))
        {
            return;
        }

        try
        {
            Clipboard.SetText(Input.Text);
            Status.Text = "Copied to clipboard.";
        }
        catch (Exception)
        {
            Status.Text = "Couldn't access the clipboard.";
        }
    }

    private void UpdateButtons()
    {
        Copy.IsEnabled = !string.IsNullOrEmpty(Input.Text);
        FixAll.IsEnabled = _current.Any(m => m.Suggestions.Count > 0);
    }
}
