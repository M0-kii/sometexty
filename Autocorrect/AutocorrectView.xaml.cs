using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace SomeTexty.Autocorrect;

public partial class AutocorrectView : UserControl
{
    private const int MaxSuggestionsShown = 8;

    private readonly CorrectionService _service = new(SpellEngine.Instance);
    private readonly DispatcherTimer _debounce;

    private IReadOnlyList<Misspelling> _current = Array.Empty<Misspelling>();

    public AutocorrectView()
    {
        InitializeComponent();

        _debounce = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(350) };
        _debounce.Tick += (_, _) => { _debounce.Stop(); Analyze(); };

        Analyze();
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
            return;
        }

        _current = _service.Analyze(text);
        RenderIssues();

        Status.Text = _current.Count == 0
            ? "No spelling issues."
            : $"{_current.Count} word(s) to review — click one for suggestions.";

        UpdateButtons();
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

        if (misspelling.Suggestions.Count == 0)
        {
            menu.Items.Add(new MenuItem { Header = "(no suggestions)", IsEnabled = false });
            return menu;
        }

        foreach (string suggestion in misspelling.Suggestions.Take(MaxSuggestionsShown))
        {
            string word = misspelling.Word;
            string replacement = suggestion;
            var item = new MenuItem { Header = suggestion };
            item.Click += (_, _) => ApplySuggestion(word, replacement);
            menu.Items.Add(item);
        }

        return menu;
    }

    private void ApplySuggestion(string word, string replacement)
    {
        Input.Text = _service.ReplaceWord(Input.Text, word, replacement);
        // Setting Text re-fires OnInputChanged, which re-analyzes after the debounce.
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

        Input.Text = text;
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
