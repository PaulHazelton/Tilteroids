using Tilteroids.Core.Settings;

namespace Tilteroids.Core.Services.Interfaces;

public interface IUserSettingsService
{
	UserSettings GetUserSettings();
	UserSettings ReloadUserSettings();
	void SaveSettings(UserSettings settings);
	void ApplySettings(UserSettings settings);
}