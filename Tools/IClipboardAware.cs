namespace SomeTexty.Tools;

/// <summary>
/// Implemented by tool views that can be pre-filled with clipboard text when the
/// panel opens. Implementations should only fill when their input is empty, so an
/// open never clobbers text the user is already working on.
/// </summary>
public interface IClipboardAware
{
    void OfferClipboardText(string text);
}
