using Tilteroids.Core.Data;

namespace Tilteroids.Core.Gameplay;

public class GameState
{
	private readonly IGamePlayer _gamePlayer;

	public bool IsPaused { get; set; }

	public int Lives
	{
		get => field;
		private set => field = MathHelper.Clamp(value, 0, Constants.MaxLives);
	}
	public int Health
	{
		get => field;
		private set => field = MathHelper.Clamp(value, 0, Constants.MaxHealth);
	}
	public int Currency
	{
		get => field;
		private set => field = MathHelper.Clamp(value, 0, Constants.MaxCurrency);
	}

	public int SelectedWeaponIndex { get; set; } = 0;

	public GameState(IGamePlayer gamePlayer)
	{
		_gamePlayer = gamePlayer;

		IsPaused = false;

		Lives = Constants.InitialLives;
		Health = Constants.InitialHealth;
		Currency = Constants.InitialCurrency;
	}

	public void LoseLife()
	{
		Lives -= 1;

		if (Lives == 0)
			_gamePlayer.GameOver();
		else
		{
			_gamePlayer.LoseLife();
			Health = Constants.MaxHealth;
		}
	}

	public void AddHealth(int difference)
	{
		Health += difference;

		if (Health == 0)
			LoseLife();
	}

	public void AddCurrency(int difference)
	{
		Currency += difference;
	}
}