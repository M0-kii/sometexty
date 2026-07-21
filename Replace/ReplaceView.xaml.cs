using System.Windows;
using System.Windows.Controls;
using SomeTexty.Tools;

namespace SomeTexty.Replace;

public partial class ReplaceView : UserControl, IClipboardAware
{
    public ReplaceView() => InitializeComponent();

    public void OfferClipboardText(string text)
    {
        if (string.IsNullOrEmpty(Input.Text))
        {
            Input.Text = text;
        }
    }

    private void OnReplaceAll(object sender, RoutedEventArgs e)
    {
        string find = FindBox.Text;
        if (string.IsNullOrEmpty(find))
        {
            Status.Text = "Enter text to find.";
            return;
        }

        bool matchCase = CaseToggle.IsChecked == true;
        int count = TextReplacer.CountOccurrences(Input.Text, find, matchCase);
        Input.Text = TextReplacer.ReplaceAll(Input.Text, find, ReplaceBox.Text, matchCase);

        Status.Text = count switch
        {
            0 => "No matches.",
            1 => "Replaced 1 match.",
            _ => $"Replaced {count} matches.",
        };
    }
}
