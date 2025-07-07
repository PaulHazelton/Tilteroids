using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using SpaceshipArcade.MG.Engine.Extensions;
using SpaceshipArcade.MG.Engine.Graphics;
using SpaceshipArcade.MG.Engine.Utilities;
using Tilteroids.Core.Data;
using Tilteroids.Core.Gameplay.Torus;

namespace Tilteroids.Core.Gameplay.Entities;

public class Asteroid : IGameObject, IPhysicsObject, IWrappable, IDamageColider
{
	private const float Density = 1.0f;

	private readonly IGamePlayer _handler;
	private readonly Random _generator;
	private readonly Vertices _vertices;
	private readonly int _splitCount;

	private readonly TextPanel _debugPanel;

	private Vector2 _previousLinearVelocity;
	private float _previousAngularVelocity;
	private Vector2 _pastColiderVelocity;

	public Body Body { get; private init; }
	public int Size { get; private init; }
	public int InitialHealth { get; private init; }
	public int Health { get; private set; }

	public int DamageMass => Size;
	public float Radius { get; private set; }
	public Vector2 WorldCenter
	{
		get => Body.Position;
		set => Body.Position = value;
	}

	// For now, Asteroids will just be squares
	// Eventually, Asteroids will be polygons
	public Asteroid(IGamePlayer gamePlayer, int size, Vector2 initialPosition, float initialRotation, Vector2 initialVelocity, float initialAngularVelocity)
	{
		_previousLinearVelocity = initialVelocity;
		_previousAngularVelocity = initialAngularVelocity;
		_handler = gamePlayer;
		_generator = new(DateTime.Now.Millisecond);
		_splitCount = 3;
		_debugPanel = new(_handler.ContentBucket.Fonts.FallbackFont, initialPosition)
		{
			Scale = Constants.MetersPerPixel,
		};

		Size = size;
		InitialHealth = size switch
		{
			1 => 3,
			2 => 6,
			3 => 12,
			_ => 1,
		};
		Health = InitialHealth;

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

			Radius = unit * 0.8f;

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
			body.OnSeparation += OnSeparationHandler;

			Body = body;
		}
	}

	private bool OnCollisionHandler(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		_pastColiderVelocity = fixtureB.Body.LinearVelocity;

		if (fixtureB.Body.Tag is Bullet bullet)
		{
			Health -= bullet.GunSettings.Damage;
		}

		else if (fixtureB.Body.Tag is IDamageColider)
		{
			_previousLinearVelocity = Body.LinearVelocity;
			_previousAngularVelocity = Body.AngularVelocity;

			// // Subtract damamge based on mass of other asteroid and relative velocity
			// int otherMass = colider.DamageMass;
			// var relativeVelocitySquared = (colider.Body.LinearVelocity - Body.LinearVelocity).LengthSquared();

			// int damage = (int)(otherMass * relativeVelocitySquared * 0.05f);

			// // _debugPanel.AddLine($"Contact rel vel: {relativeVelocitySquared:F4}");
			// _debugPanel.AddLine($"Damage: {damage}");
		}

		return true;
	}

	private void OnSeparationHandler(Fixture sender, Fixture other, Contact contact)
	{
		if (Size == 1)
			return;

		if (other.Body.Tag is IDamageColider)
		{
			// Determine change in kinetic energy
			var linearDiff = _previousLinearVelocity - Body.LinearVelocity;
			var angularDiff = _previousAngularVelocity - Body.AngularVelocity;

			float linearKEChange = 0.5f * Body.Mass * (linearDiff).LengthSquared();
			float angularKEChange = 0.5f * Body.Inertia * (angularDiff * angularDiff);

			float KEChange = linearKEChange + angularKEChange;

			float damage = KEChange * 0.3f;

			if (other.Body.Tag is Asteroid asteroid)
			{
				int sizeDiff = this.Size - asteroid.Size;
				float damageScale = PMath.Map(sizeDiff, -2, 2, 0, 2);
				damage *= damageScale;
			}

			Health -= (int)damage;

			// _debugPanel.AddLine($"KE: {KEChange:F2} | Damage: {damage}");
		}
	}

	public void Update(GameTime gameTime)
	{
		if (Health <= 0)
			Explode(_pastColiderVelocity);

		_debugPanel.Position = Body.WorldCenter;
		_debugPanel.ClearLines();
		_debugPanel.AddLine($"{Health}/{InitialHealth}");

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

		// Primitives.DrawCircleOutline(WorldCenter, Radius, Color.Red, 1.0f);

		_debugPanel.Draw(spriteBatch);
	}

	private void Explode(Vector2 direction)
	{
		_handler.RemoveGameObject(this);

		if (Size <= 1)
			return;

		if (direction == Vector2.Zero)
			direction = Vector2.UnitX;
		else
			direction.Normalize();

		for (int i = 0; i < _splitCount; i++)
		{
			var asteroid = new Asteroid(_handler,
				size: Size - 1,
				initialPosition: Body.Position + direction * Size * 0.5f,
				initialRotation: _generator.NextSingle() * MathHelper.TwoPi,
				initialVelocity: direction * 2.5f * (4 - Size),
				initialAngularVelocity: _generator.NextSingle(-1, 1));

			_handler.AddGameObject(asteroid);

			direction.Rotate(MathHelper.TwoPi / _splitCount);
		}
	}
}