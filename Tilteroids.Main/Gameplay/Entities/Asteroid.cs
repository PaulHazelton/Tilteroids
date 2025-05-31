using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;

namespace Tilteroids.Main.Gameplay.Entities;

public class Asteroid : IGameObject, IPhysicsObject
{
	public Body Body { get; private init; }

	// For now, Asteroids will just be squares
	// Eventually, Asteroids will be polygons
	public Asteroid(IGameObjectHandler handler, Vector2 startingPos)
	{
		Body = CreateBody();



		Body CreateBody()
		{
			return new Body();
		}
	}

	public void Update(GameTime gameTime)
	{

	}
	public void Draw(SpriteBatch spriteBatch)
	{

	}
}