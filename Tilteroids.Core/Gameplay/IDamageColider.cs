using nkast.Aether.Physics2D.Dynamics;

namespace Tilteroids.Core.Gameplay;

interface IDamageColider
{
	int DamageMass { get; }
	Body Body { get; }
}