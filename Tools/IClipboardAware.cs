namespace SomeTexty.Tools;

/// <summary>
/// Accepts clipboard text when the panel opens. Only fill an empty input.
/// </summary>
public interface IClipboardAware
{
    void OfferClipboardText(string text);
}
