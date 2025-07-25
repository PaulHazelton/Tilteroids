using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using SpaceshipArcade.MG.Engine.Extensions;
using SpaceshipArcade.MG.Engine.Graphics;
using Tilteroids.Core.Data;
using Tilteroids.Core.Gameplay.Torus;

namespace Tilteroids.Core.Gameplay.Entities;

public class Asteroid : IGameObject, IPhysicsObject, IWrappable
{
	private const float Density = 1.0f;

	private readonly IGamePlayer _handler;
	private readonly Random _generator;
	private readonly Vertices _vertices;
	private readonly int _splitCount;

	public Body Body { get; private init; }
	public int Size { get; private init; }
	public int Health { get; private set; }
	
	public float Radius { get; private set; }
	public Vector2 WorldCenter
	{
		get => Body.WorldCenter;
		set => Body.Position = value;
	}

	// For now, Asteroids will just be squares
	// Eventually, Asteroids will be polygons
	public Asteroid(IGamePlayer gamePlayer, int size, Vector2 initialPosition, float initialRotation, Vector2 initialVelocity, float initialAngularVelocity)
	{
		_handler = gamePlayer;
		_generator = new(DateTime.Now.Millisecond);
		_splitCount = 3;

		Size = size;
		Health = size switch
		{
			1 => 3,
			2 => 6,
			3 => 12,
			_ => 1,
		};

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

			Radius = unit;

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
				Restitution = 1.0f,
			};

			body.Add(fixture);

			// Angular velocity has to be set after fixtures are added, otherwise bugs.
			body.LinearVelocity = initialVelocity;
			body.AngularVelocity = initialAngularVelocity;

			body.OnCollision += OnCollisionHandler;

			Body = body;
		}
	}

	private bool OnCollisionHandler(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB.Body.Tag is Bullet bullet)
		{
			Health -= bullet.GunSettings.Damage;

			if (Health <= 0)
				Explode(bullet.Body.LinearVelocity);
		}

		return true;
	}

	public void Update(GameTime gameTime)
	{
		this.Wrap(_handler.Bounds);
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

	private void Explode(Vector2 bulletDirection)
	{
		_handler.RemoveGameObject(this);

		if (Size <= 1)
			return;

		var direction = Vector2.Normalize(bulletDirection);

		for (int i = 0; i < _splitCount; i++)
		{
			var asteroid = new Asteroid(_handler,
				size: Size - 1,
				initialPosition: Body.Position + direction * Size * 0.3f,
				initialRotation: _generator.NextSingle() * MathHelper.TwoPi,
				initialVelocity: direction * 2 * (4 - (Size - 1)),
				initialAngularVelocity: _generator.NextSingle(-1, 1));

			_handler.AddGameObject(asteroid);

			direction.Rotate(MathHelper.TwoPi / _splitCount);
		}
	}
}