using System.Windows;
using System.Windows.Controls;
using SomeTexty.Tools;

namespace SomeTexty.Clean;

public partial class CleanView : UserControl, IClipboardAware
{
    public CleanView() => InitializeComponent();

    public void OfferClipboardText(string text)
    {
        if (string.IsNullOrEmpty(Input.Text))
        {
            Input.Text = text;
        }
    }

    private void OnTrimLines(object sender, RoutedEventArgs e) => Input.Text = TextCleaner.TrimLines(Input.Text);

    private void OnCollapseSpaces(object sender, RoutedEventArgs e) => Input.Text = TextCleaner.CollapseSpaces(Input.Text);

    private void OnRemoveBlankLines(object sender, RoutedEventArgs e) => Input.Text = TextCleaner.RemoveBlankLines(Input.Text);

    private void OnJoinLines(object sender, RoutedEventArgs e) => Input.Text = TextCleaner.JoinLines(Input.Text);

    private void OnCopy(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(Input.Text))
        {
            try { Clipboard.SetText(Input.Text); }
            catch (Exception) { /* clipboard busy; nothing to do */ }
        }
    }
}
