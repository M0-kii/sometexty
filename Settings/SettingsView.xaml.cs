using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using SomeTexty.Autocorrect;
using SomeTexty.Startup;

namespace SomeTexty.Settings;

public partial class SettingsView : UserControl
{
    // Segoe MDL2 Assets "Cancel" (X) glyph.
    private const string CancelGlyph = "";

    private static readonly Brush MutedBrush = Freeze("#FF8A8A8A");
    private static readonly Brush TextBrush = Freeze("#FFE6E6E6");

    // Guards toggles so refreshing their state doesn't re-trigger a write.
    private bool _suppressEvents;

    public SettingsView()
    {
        InitializeComponent();
        AboutText.Text = $"SomeTexty {AppVersion()}";
    }

    /// <summary>Reloads all settings state. Call each time the view is shown.</summary>
    public void Refresh()
    {
        var settings = SettingsStore.Load();

        _suppressEvents = true;
        StartupToggle.IsChecked = StartupManager.IsEnabled();
        AutoHideToggle.IsChecked = settings.HideOnFocusLoss;
        ClipboardToggle.IsChecked = settings.GrabClipboardOnOpen;
        _suppressEvents = false;

        BuildWordList();
    }

    private void OnStartupToggled(object sender, RoutedEventArgs e)
    {
        if (_suppressEvents)
        {
            return;
        }

        StartupManager.SetEnabled(StartupToggle.IsChecked == true);
    }

    private void OnAutoHideToggled(object sender, RoutedEventArgs e)
    {
        if (_suppressEvents)
        {
            return;
        }

        var settings = SettingsStore.Load();
        settings.HideOnFocusLoss = AutoHideToggle.IsChecked == true;
        SettingsStore.Save(settings);
    }

    private void OnClipboardToggled(object sender, RoutedEventArgs e)
    {
        if (_suppressEvents)
        {
            return;
        }

        var settings = SettingsStore.Load();
        settings.GrabClipboardOnOpen = ClipboardToggle.IsChecked == true;
        SettingsStore.Save(settings);
    }

    private void OnClearAll(object sender, RoutedEventArgs e)
    {
        SpellEngine.Instance.ClearPersonal();
        BuildWordList();
    }

    private void OnNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        e.Handled = true;
    }

    private void BuildWordList()
    {
        WordList.Children.Clear();

        var words = SpellEngine.Instance.PersonalWords;
        ClearAllButton.Visibility = words.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

        if (words.Count == 0)
        {
            WordList.Children.Add(new TextBlock
            {
                Text = "No words added yet.",
                Foreground = MutedBrush,
                FontSize = 12,
                Margin = new Thickness(6, 6, 6, 6),
            });
            return;
        }

        foreach (string word in words)
        {
            WordList.Children.Add(BuildWordRow(word));
        }
    }

    private Grid BuildWordRow(string word)
    {
        var row = new Grid { Margin = new Thickness(4, 2, 2, 2) };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var label = new TextBlock
        {
            Text = word,
            Foreground = TextBrush,
            FontSize = 13,
            VerticalAlignment = VerticalAlignment.Center,
        };
        Grid.SetColumn(label, 0);

        var remove = new Button
        {
            Style = (Style)Application.Current.Resources["IconButton"],
            Content = CancelGlyph,
            FontFamily = new FontFamily("Segoe MDL2 Assets"),
            FontSize = 11,
            ToolTip = $"Remove \"{word}\"",
        };
        remove.Click += (_, _) =>
        {
            SpellEngine.Instance.RemoveFromPersonal(word);
            BuildWordList();
        };
        Grid.SetColumn(remove, 1);

        row.Children.Add(label);
        row.Children.Add(remove);
        return row;
    }

    private static string AppVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        return version is null ? string.Empty : $"v{version.Major}.{version.Minor}.{version.Build}";
    }

    private static Brush Freeze(string hex)
    {
        var brush = (Brush)new BrushConverter().ConvertFromString(hex)!;
        brush.Freeze();
        return brush;
    }
}
