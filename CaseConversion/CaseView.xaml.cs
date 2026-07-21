using System.Windows;
using System.Windows.Controls;

namespace SomeTexty.CaseConversion;

public partial class CaseView : UserControl
{
    public CaseView() => InitializeComponent();

    private void OnUpper(object sender, RoutedEventArgs e) => Input.Text = CaseConverter.ToUpper(Input.Text);

    private void OnLower(object sender, RoutedEventArgs e) => Input.Text = CaseConverter.ToLower(Input.Text);

    private void OnTitle(object sender, RoutedEventArgs e) => Input.Text = CaseConverter.ToTitle(Input.Text);

    private void OnSentence(object sender, RoutedEventArgs e) => Input.Text = CaseConverter.ToSentence(Input.Text);

    private void OnCopy(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(Input.Text))
        {
            try { Clipboard.SetText(Input.Text); }
            catch (Exception) { /* clipboard busy; nothing to do */ }
        }
    }
}
