using Microsoft.Xna.Framework.Audio;
using SpaceshipArcade.MG.Engine.Utilities;
using Tilteroids.Core.Settings;
using Tilteroids.Core.Data;
using Tilteroids.Core.Services.Interfaces;

namespace Tilteroids.Core.Services.Implementations;

public class UserSettingsService : IUserSettingsService
{
	private UserSettings _loadedUserSettings;

	public UserSettingsService()
	{
		// Load if exists, otherwise create default and create / save file
		try
		{
			_loadedUserSettings = LoadSettings();
		}
		catch
		{
			_loadedUserSettings = new();
			FileManager.SaveJson(RootPath.AppData, FilePaths.Settings, _loadedUserSettings);
		}
	}

	public UserSettings GetUserSettings() => _loadedUserSettings;

	public UserSettings ReloadUserSettings()
	{
		_loadedUserSettings = LoadSettings();
		return _loadedUserSettings;
	}

	public void SaveSettings(UserSettings settings)
	{
		_loadedUserSettings = settings;
		FileManager.SaveJson(RootPath.AppData, FilePaths.Settings, settings);
	}

	public void ApplySettings(UserSettings settings)
	{
		SoundEffect.MasterVolume = settings.MasterVolume;
	}

	private static UserSettings LoadSettings() => FileManager.LoadJson<UserSettings>(RootPath.AppData, FilePaths.Settings);
}