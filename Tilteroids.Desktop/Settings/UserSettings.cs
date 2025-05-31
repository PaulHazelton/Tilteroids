namespace Tilteroids.Main.Settings;

public class UserSettings
{
	public float MasterVolume { get; set; }

	/// <summary>
	/// Creates a settings object with default settings
	/// </summary>
	internal UserSettings()
	{
		MasterVolume = 1.0f;
	}
}