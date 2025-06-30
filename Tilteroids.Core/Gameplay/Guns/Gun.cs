namespace Tilteroids.Core.Gameplay.Guns;

public abstract class Gun
{
	public abstract float RecoilMagnitude { get; }
	public abstract float MuzzleOffset { get; }
	public abstract float MuzzleVelocity { get; }
	public abstract float Width { get; }
	public abstract float Length { get; }
	public abstract float Density { get; }
	public abstract int Damage { get; }

	// Time in seconds between firings
	public abstract double Cooldown { get; }

	public double RemainingCooldown { get; private set; }

	public bool ReadyToFire => RemainingCooldown <= 0;

	public void Update(GameTime gameTime)
	{
		if (RemainingCooldown >= 0)
			RemainingCooldown -= gameTime.ElapsedGameTime.TotalSeconds;
	}

	public void ResetCooldown() => RemainingCooldown = Cooldown;
}