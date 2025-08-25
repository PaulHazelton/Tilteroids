namespace Tilteroids.Core.Data;

public class Constants
{
	#region Display and Pixels

	// Ship should be about 1/24th of the screen
	// Ship is about 1 meter wide
	// Screen is about 24 meters wide

	public const float PixelsPerMeter = 80;
	public const float MetersPerPixel = 1 / 80f;

	public const int UiUnit = 24;

	#endregion

	#region Gameplay

	public const int MaxLives = 5;
	public const int MaxHealth = 12;
	public const int MaxCurrency = 24;

	public const int InitialLives = MaxLives;
	public const int InitialHealth = MaxHealth;
	public const int InitialCurrency = 0;

	public const int NumberOfWeaponTypes = 4;

	#endregion
}