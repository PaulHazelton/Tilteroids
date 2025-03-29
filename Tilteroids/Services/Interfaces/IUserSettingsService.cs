using Tilteroids.Main.Settings;

namespace Tilteroids.Main.Services.Interfaces;

public interface IUserSettingsService
{
	UserSettings GetUserSettings();
	UserSettings ReloadUserSettings();
	void SaveSettings(UserSettings settings);
	void ApplySettings(UserSettings settings);
}