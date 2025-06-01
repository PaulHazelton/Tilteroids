namespace Tilteroids.Core.Gameplay.Guns;

public class Clipper : Gun
{
	public override float RecoilMagnitude => 0.075f;
	public override float MuzzleOffset => 0.4f;
	public override float MuzzleVelocity => 20.0f;
	public override float Width => 0.05f;
	public override float Length => 0.4f;
	public override float Density => 1.0f;
	public override double Cooldown => 0.4f;
}