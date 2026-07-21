using System.IO;
using System.Text.Json;

namespace SomeTexty.Settings;

/// <summary>
/// Loads and saves <see cref="AppSettings"/> as JSON under %AppData%\SomeTexty.
/// Reads fall back to defaults on any error so a corrupt file can't block startup.
/// </summary>
public static class SettingsStore
{
    private static readonly string Directory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SomeTexty");

    private static readonly string FilePath = Path.Combine(Directory, "settings.json");

    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath)) ?? new AppSettings();
            }
        }
        catch (Exception)
        {
            // Unreadable or malformed settings: start fresh rather than fail.
        }

        return new AppSettings();
    }

    /// <summary>Writes settings to disk. Returns false if the write failed.</summary>
    public static bool Save(AppSettings settings)
    {
        try
        {
            System.IO.Directory.CreateDirectory(Directory);
            File.WriteAllText(FilePath, JsonSerializer.Serialize(settings, Options));
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
