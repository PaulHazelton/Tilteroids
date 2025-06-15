using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using SpaceshipArcade.MG.Engine.Graphics;
using Tilteroids.Core.Data;

namespace Tilteroids.Core.Gameplay.Entities;

public class Asteroid : IGameObject, IPhysicsObject
{
	private const float Density = 1.0f;

	private readonly IGameObjectHandler _handler;
	private readonly int _size;

	public Body Body { get; private init; }
	public int Health { get; private set; }

	// For now, Asteroids will just be squares
	// Eventually, Asteroids will be polygons
	public Asteroid(IGameObjectHandler handler, int size, Vector2 initialPosition, float initialRotation, Vector2 initialVelocity, float initialAngularVelocity)
	{
		_handler = handler;
		_size = size;

		Health = size * 10;

		Body = CreateBody();

		Body CreateBody()
		{
			var body = new Body()
			{
				Tag = this,
				BodyType = BodyType.Dynamic,
				Position = initialPosition,
				Rotation = initialRotation,
			};

			body.CreateRectangle(size * 0.5f, size * 0.5f, Density, Vector2.Zero);

			// Angular velocity has to be set after fixtures are added, otherwise bugs.
			body.LinearVelocity = initialVelocity;
			body.AngularVelocity = initialAngularVelocity;

			body.FixtureList[0].Restitution = 0.5f;

			body.OnCollision += OnCollisionHandler;

			return body;
		}
	}

	private bool OnCollisionHandler(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		// _handler.RemoveGameObject(this);

		return true;
	}

	public void Update(GameTime gameTime)
	{

	}
	public void Draw(SpriteBatch spriteBatch)
	{
		Primitives.DrawRectangle(Body.Position * Constants.PixelsPerMeter,
			size: new Vector2(_size * 0.5f, _size * 0.5f) * Constants.PixelsPerMeter,
			Body.Rotation,
			Color.Blue,
			layerDepth: 1);
	}
}