using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using SpaceshipArcade.MG.Engine.Graphics;
using Tilteroids.Core.Data;

namespace Tilteroids.Core.Gameplay.Entities;

public class Asteroid : IGameObject, IPhysicsObject
{
	private const float Density = 1.0f;

	private readonly Vertices _vertices;

	public Body Body { get; private init; }
	public int Health { get; private set; }

	// For now, Asteroids will just be squares
	// Eventually, Asteroids will be polygons
	public Asteroid(int size, Vector2 initialPosition, float initialRotation, Vector2 initialVelocity, float initialAngularVelocity)
	{
		Health = size * 10;

		// Create Body
		{
			var body = new Body()
			{
				Tag = this,
				BodyType = BodyType.Dynamic,
				Position = initialPosition,
				Rotation = initialRotation,
			};

			float unit = size / 2.0f;

			_vertices = new Vertices([
				new(-unit * 0.7f, unit * 0.8f),
				new(-unit, 0),
				new(0, -unit * 0.7f),
				new(unit, 0),
				new(unit * 0.8f, unit * 0.6f)
			]);

			PolygonShape shipShape = new(_vertices, Density);

			var fixture = new Fixture(shipShape)
			{
				Restitution = 0.5f,
			};

			body.Add(fixture);

			// Angular velocity has to be set after fixtures are added, otherwise bugs.
			body.LinearVelocity = initialVelocity;
			body.AngularVelocity = initialAngularVelocity;

			body.FixtureList[0].Restitution = 0.5f;

			body.OnCollision += OnCollisionHandler;

			Body = body;
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
		var tf = Body.GetTransform();

		for (int i = 0; i < _vertices.Count - 1; i++)
		{
			Primitives.DrawLine(
				Transform.Multiply(_vertices[i], ref tf),
				Transform.Multiply(_vertices[i + 1], ref tf),
				thickness: 1.0f / Constants.PixelsPerMeter, Color.White, 0.1f);
		}
		Primitives.DrawLine(
			Transform.Multiply(_vertices[^1], ref tf),
			Transform.Multiply(_vertices[0], ref tf),
			thickness: 1.0f / Constants.PixelsPerMeter, Color.White, 0.1f);
	}
}