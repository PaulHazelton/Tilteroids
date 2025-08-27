using Tilteroids.Core.Data;
using Tilteroids.Core.Gameplay;

namespace Tilteroids.Core.Gui;

public class Hud
{
	private readonly IGamePlayer _gamePlayer;

	private readonly HudElement _safeZone;
	private readonly HudElement _respawnIndicator;
	private readonly HudElement _pauseButton;
	private readonly HudElement _playButton;
	private readonly HudElement[] _lives;
	private readonly HudElement[] _health;
	private readonly HudElement[] _currency;
	private readonly HudElement[] _weapons;

	public Hud(IGamePlayer gamePlayer, ContentBucket contentBucket)
	{
		_gamePlayer = gamePlayer;

		var spriteSheet = contentBucket.SpriteSheets.MainSpriteSheet;

		int unit = Constants.UiUnit;

		_safeZone = new(spriteSheet, SpriteIdentifiers.SafeZone, new(unit, unit));
		_respawnIndicator = new(spriteSheet, SpriteIdentifiers.RespawnIndicator, new(unit, 6 * unit));

		_pauseButton = new(spriteSheet, SpriteIdentifiers.Pause, new(_gamePlayer.ScreenSize.X - (5 * unit), unit));
		_playButton = new(spriteSheet, SpriteIdentifiers.Play, new(_gamePlayer.ScreenSize.X - (5 * unit), unit));

		_lives = new HudElement[Constants.MaxLives];
		for (int i = 0; i < _lives.Length; i++)
			_lives[i] = new(spriteSheet, SpriteIdentifiers.Life, new((8 + 2.5f * i) * unit, unit));

		_health = new HudElement[Constants.MaxHealth];
		for (int i = 0; i < _health.Length; i++)
			_health[i] = new(spriteSheet, SpriteIdentifiers.Health, new((8 + i) * unit, 4 * unit));

		// NOTE: If max currency is odd, this might not work right
		_currency = new HudElement[Constants.MaxCurrency];
		for (int i = 0; i < _currency.Length; i += 2)
		{
			_currency[i] = new(spriteSheet, SpriteIdentifiers.Currency, new((8 + (i / 2)) * unit, 7 * unit));
			_currency[i + 1] = new(spriteSheet, SpriteIdentifiers.Currency, new((8 + (i / 2)) * unit, 8 * unit));
		}

		_weapons = new HudElement[Constants.NumberOfWeaponTypes];
		for (int i = 0; i < _weapons.Length; i++)
			_weapons[i] = new(spriteSheet, SpriteIdentifiers.Weapon(i), new((22 + i * 5) * unit, unit));
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		_safeZone.Draw(spriteBatch);

		_respawnIndicator.Draw(spriteBatch);

		if (_gamePlayer.GameState.IsPaused)
			_playButton.Draw(spriteBatch);
		else
			_pauseButton.Draw(spriteBatch);

		for (int i = 0; i < _gamePlayer.GameState.Lives; i++)
			_lives[i].Draw(spriteBatch);

		for (int i = 0; i < _gamePlayer.GameState.Health; i++)
			_health[i].Draw(spriteBatch);

		for (int i = 0; i < _gamePlayer.GameState.Currency; i++)
			_currency[i].Draw(spriteBatch);

		for (int i = 0; i < _weapons.Length; i++)
		{
			_weapons[i].DrawFrame(spriteBatch, i == _gamePlayer.GameState.SelectedWeaponIndex);
			_weapons[i].Draw(spriteBatch);
		}

	}
}