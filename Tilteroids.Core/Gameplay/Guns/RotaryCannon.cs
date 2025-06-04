namespace Tilteroids.Core.Gameplay.Guns;

public class RotaryCannon : Gun
{
	public override float RecoilMagnitude => 0.02f;
	public override float MuzzleOffset => 0.4f;
	public override float MuzzleVelocity => 30.0f;
	public override float Width => 0.05f;
	public override float Length => 0.4f;
	public override float Density => 1.0f;
	public override double Cooldown => 0.1f;
}