using nkast.Aether.Physics2D.Dynamics;

namespace Tilteroids.Core.Gameplay;

public interface IPhysicsObject
{
	public Body Body { get; }
}