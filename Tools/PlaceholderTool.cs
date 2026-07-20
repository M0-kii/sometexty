using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SomeTexty.Tools;

/// <summary>
/// A stand-in tool that shows a short message. Used to populate the shell before
/// real tools land; each instance is replaced by a working tool as it's built.
/// </summary>
public sealed class PlaceholderTool : ITextTool
{
    private static readonly Brush MutedBrush = CreateFrozen("#FF8A8A8A");

    private readonly string _message;

    public PlaceholderTool(string name, string message)
    {
        Name = name;
        _message = message;
    }

    public string Name { get; }

    public UserControl CreateView()
    {
        var label = new TextBlock
        {
            Text = _message,
            Foreground = MutedBrush,
            FontFamily = new FontFamily("Segoe UI"),
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
            TextAlignment = TextAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };

        return new UserControl { Content = label };
    }

    private static Brush CreateFrozen(string hex)
    {
        var brush = (Brush)new BrushConverter().ConvertFromString(hex)!;
        brush.Freeze();
        return brush;
    }
}
