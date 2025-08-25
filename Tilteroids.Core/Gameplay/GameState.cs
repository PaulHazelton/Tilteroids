using Tilteroids.Core.Data;

namespace Tilteroids.Core.Gameplay;

public class GameState
{
	public int Lives { get; private set; }
	public int Health { get; private set; }
	public int Currency { get; private set; }

	public GameState()
	{
		Lives = Constants.InitialLives;
		Health = Constants.InitialHealth;
		Currency = Constants.InitialCurrency;
	}

	public void AddLives(int difference)
	{
		Lives = MathHelper.Clamp(Lives + difference, 0, Constants.MaxLives);
	}

	public void AddHealth(int difference)
	{
		Health = MathHelper.Clamp(Health + difference, 0, Constants.MaxHealth);

		if (Health == 0)
			AddLives(-1);
	}

	public void AddCurrency(int difference)
	{
		Currency = MathHelper.Clamp(Currency + difference, 0, Constants.MaxCurrency);
	}
}