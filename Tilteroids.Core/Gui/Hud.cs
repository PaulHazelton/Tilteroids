using Tilteroids.Core.Data;

namespace Tilteroids.Core.Gui;

public class Hud
{
	private readonly HudElement _safeZone;
	private readonly HudElement _respawnIndicator;
	private readonly HudElement[] _lives;
	private readonly HudElement[] _health;
	private readonly HudElement[] _currency;
	private readonly HudElement[] _weapons;

	public Hud(ContentBucket contentBucket)
	{
		var spriteSheet = contentBucket.SpriteSheets.MainSpriteSheet;

		int unit = Constants.UiUnit;

		_safeZone = new(spriteSheet, SpriteIdentifiers.SafeZone, new(unit, unit));
		_respawnIndicator = new(spriteSheet, SpriteIdentifiers.RespawnIndicator, new(unit, 6 * unit));

		_lives = new HudElement[5];
		for (int i = 0; i < _lives.Length; i++)
			_lives[i] = new(spriteSheet, SpriteIdentifiers.Life, new((8 + 2.5f * i) * unit, unit));

		_health = new HudElement[12];
		for (int i = 0; i < _health.Length; i++)
			_health[i] = new(spriteSheet, SpriteIdentifiers.Health, new((8 + i) * unit, 4 * unit));

		// NOTE: If max currency is odd, this might not work right
		_currency = new HudElement[24];
		for (int i = 0; i < _currency.Length; i += 2)
		{
			_currency[i] = new(spriteSheet, SpriteIdentifiers.Currency, new((8 + (i / 2)) * unit, 7 * unit));
			_currency[i + 1] = new(spriteSheet, SpriteIdentifiers.Currency, new((8 + (i / 2)) * unit, 8 * unit));
		}

		_weapons = new HudElement[4];
		for (int i = 0; i < _weapons.Length; i++)
			_weapons[i] = new(spriteSheet, SpriteIdentifiers.Weapon(i), new((22 + i * 5) * unit, unit));
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		_safeZone.Draw(spriteBatch);

		_respawnIndicator.Draw(spriteBatch);

		foreach (var element in _lives)
			element.Draw(spriteBatch);

		foreach (var element in _health)
			element.Draw(spriteBatch);

		foreach (var element in _currency)
			element.Draw(spriteBatch);

		foreach (var element in _weapons)
			element.Draw(spriteBatch);
	}
}