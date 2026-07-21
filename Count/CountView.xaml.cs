using System.Windows.Controls;
using SomeTexty.Tools;

namespace SomeTexty.Count;

public partial class CountView : UserControl, IClipboardAware
{
    public CountView()
    {
        InitializeComponent();
        UpdateStats();
    }

    public void OfferClipboardText(string text)
    {
        if (string.IsNullOrEmpty(Input.Text))
        {
            Input.Text = text;
        }
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e) => UpdateStats();

    private void UpdateStats()
    {
        var stats = TextStats.From(Input.Text);
        WordsValue.Text = stats.Words.ToString();
        CharsValue.Text = stats.Characters.ToString();
        NoSpaceValue.Text = stats.CharactersNoSpaces.ToString();
        LinesValue.Text = stats.Lines.ToString();
    }
}
