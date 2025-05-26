using nkast.Aether.Physics2D.Dynamics;

namespace Tilteroids.Main.Gameplay;

public interface IPhysicsObject
{
	public Body Body { get; }
}