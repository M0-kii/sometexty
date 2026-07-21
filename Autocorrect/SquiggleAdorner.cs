using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace SomeTexty.Autocorrect;

/// <summary>
/// Draws red wavy underlines beneath misspelled words on a text box. Each segment
/// is supplied as a rectangle whose Left/Right give the horizontal span and whose
/// Top gives the baseline to draw along.
/// </summary>
public sealed class SquiggleAdorner : Adorner
{
    private const double WaveLength = 4;
    private const double Amplitude = 1.5;

    private static readonly Pen SquigglePen = CreatePen();

    private IReadOnlyList<Rect> _segments = Array.Empty<Rect>();

    public SquiggleAdorner(UIElement adornedElement) : base(adornedElement)
    {
        IsHitTestVisible = false;
    }

    public void SetSegments(IReadOnlyList<Rect> segments)
    {
        _segments = segments;
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        // Keep underlines inside the text box even when content is scrolled.
        drawingContext.PushClip(new RectangleGeometry(new Rect(AdornedElement.RenderSize)));

        foreach (Rect segment in _segments)
        {
            DrawSquiggle(drawingContext, segment.Left, segment.Right, segment.Top);
        }

        drawingContext.Pop();
    }

    private static void DrawSquiggle(DrawingContext dc, double left, double right, double y)
    {
        var geometry = new StreamGeometry();
        using (StreamGeometryContext ctx = geometry.Open())
        {
            ctx.BeginFigure(new Point(left, y), isFilled: false, isClosed: false);

            bool down = true;
            double x = left;
            while (x < right)
            {
                double next = Math.Min(x + WaveLength, right);
                ctx.LineTo(new Point(next, y + (down ? Amplitude : 0)), isStroked: true, isSmoothJoin: false);
                down = !down;
                x = next;
            }
        }

        geometry.Freeze();
        dc.DrawGeometry(null, SquigglePen, geometry);
    }

    private static Pen CreatePen()
    {
        var pen = new Pen(new SolidColorBrush(Color.FromRgb(0xE0, 0x55, 0x55)), 1.2);
        pen.Freeze();
        return pen;
    }
}
