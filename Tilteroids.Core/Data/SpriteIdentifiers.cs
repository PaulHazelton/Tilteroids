namespace Tilteroids.Core.Data;

public static class SpriteIdentifiers
{
	public const string Pause = "Pause";
	public const string Play = "Play";
	public const string Weapon1 = "Weapon1";
	public const string Weapon2 = "Weapon2";
	public const string Weapon3 = "Weapon3";
	public const string Weapon4 = "Weapon4";
	public const string SafeZone = "SafeZone";
	public const string RespawnIndicator = "RespawnIndicator";
	public const string Ship = "Ship";
	public const string Life = "Life";
	public const string Health = "Health";
	public const string Currency = "Currency";

	private const string WeaponPrefix = "Weapon";

	public static string Weapon(int index) => $"{WeaponPrefix}{index + 1}";
}