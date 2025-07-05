using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using SpaceshipArcade.MG.Engine.Extensions;
using SpaceshipArcade.MG.Engine.Utilities;
using Tilteroids.Core.Gameplay.Guns;
using Tilteroids.Core.Controllers;
using Microsoft.Xna.Framework.Audio;
using SpaceshipArcade.MG.Engine.Graphics;
using Tilteroids.Core.Data;

namespace Tilteroids.Core.Gameplay.Entities;

public class Spaceship : IGameObject, IPhysicsObject
{
	// Private
	private const float _radius = 7.0f / 16.0f;

	private readonly IGamePlayer _handler;
	private readonly Texture2D _shipTexture;
	private readonly Vector2 _origin;
	private readonly float _scale;
	private readonly TorqueController _torqueController;
	private readonly Gun _gunSelection;
	private readonly Vertices _vertices;

	private readonly SoundEffect _gunShotSound;
	private readonly Random _random;

	// Public
	public Body Body { get; private init; }

	public Spaceship(IGamePlayer handler, Vector2 startingPos)
	{
		_gunShotSound = handler.ContentBucket.SoundEffects.Gun;
		_random = new();

		_handler = handler;

		_shipTexture = handler.ContentBucket.Textures.Ship;

		_origin = new Vector2(_shipTexture.Width / 2, _shipTexture.Height / 2);

		_scale = 1.0f / _shipTexture.Width;

		_gunSelection = new Clipper();

		{
			_vertices = new Vertices([
				new(-7, -7),
				new(7, 0),
				new(-7, 7)
			]);
			_vertices.Scale(new(1 / 16f));

			PolygonShape shipShape = new(_vertices, 1);

			var fixture = new Fixture(shipShape)
			{
				Restitution = 0.5f,
			};

			var body = new Body()
			{
				Position = startingPos,
				BodyType = BodyType.Dynamic,
			};

			body.Add(fixture);

			body.FixtureList[0].CollisionCategories = Category.Cat2;

			Body = body;
		}

		_torqueController = new(inertia: Body.Inertia);
	}

	public void Aim(Vector2 aimVector)
	{
		if (aimVector == Vector2.Zero || float.IsNaN(aimVector.X) || float.IsNaN(aimVector.Y) || aimVector.LengthSquared() < 0.0025f)
		{
			float torque = _torqueController.ComputeTorque(Body.Rotation, Body.AngularVelocity, Body.Rotation);
			Body.ApplyTorque(torque);
		}
		else
		{
			float aimAngle = aimVector.Angle();
			float torque = _torqueController.ComputeTorque(Body.Rotation, Body.AngularVelocity, aimAngle);
			Body.ApplyTorque(torque);
		}
	}

	public void Thrust()
	{
		var forceVector = PMath.PolarToCartesian(10, Body.Rotation);
		Body.ApplyForce(forceVector, Body.WorldCenter);
	}

	public void Fire() => TryShoot(Body.Rotation, _gunSelection);

	public void Update(GameTime gameTime)
	{
		// Gun cooldowns
		_gunSelection.Update(gameTime);

		if (Body.Position.X - _radius > _handler.Bounds.Right)
			Body.Position = new(_handler.Bounds.Left - _radius, Body.Position.Y);
		if (Body.Position.X + _radius < _handler.Bounds.Left)
			Body.Position = new(_handler.Bounds.Right + _radius, Body.Position.Y);

		if (Body.Position.Y - _radius > _handler.Bounds.Bottom)
			Body.Position = new(Body.Position.X, _handler.Bounds.Top - _radius);
		if (Body.Position.Y + _radius < _handler.Bounds.Top)
			Body.Position = new(Body.Position.X, _handler.Bounds.Bottom + _radius);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		// Draw Triangle
		var tf = Body.GetTransform();

		for (int i = 0; i < _vertices.Count - 1; i++)
		{
			Primitives.DrawLine(
				Transform.Multiply(_vertices[i], ref tf),
				Transform.Multiply(_vertices[i + 1], ref tf),
				thickness: 2.0f / Constants.PixelsPerMeter, Color.White, 0.1f);
		}
		Primitives.DrawLine(
			Transform.Multiply(_vertices[^1], ref tf),
			Transform.Multiply(_vertices[0], ref tf),
			thickness: 2.0f / Constants.PixelsPerMeter, Color.White, 0.1f);

		// Draw Image
		// spriteBatch.Draw(
		// 	texture: _shipTexture,
		// 	position: Body.Position,
		// 	sourceRectangle: null,
		// 	color: Color.White,
		// 	rotation: Body.Rotation,
		// 	origin: _origin,
		// 	scale: _scale,
		// 	effects: SpriteEffects.None,
		// 	layerDepth: 0.1f);
	}

	private void TryShoot(float aimAngle, Gun gunSettings)
	{
		// Don't fire if bullet will be outside bounds
		if (!_handler.Bounds.Contains(Body.Position + PMath.PolarToCartesian(gunSettings.MuzzleOffset, aimAngle)))
			return;

		// Check cooldown
		if (!gunSettings.ReadyToFire)
			return;

		// Add bullet object
		_handler.AddGameObject(new Bullet(_handler, Body.Position, aimAngle, gunSettings));

		// Apply recoil
		Body.ApplyLinearImpulse(PMath.PolarToCartesian(gunSettings.RecoilMagnitude, aimAngle - MathHelper.Pi));

		// Limit fire rate
		gunSettings.ResetCooldown();

		// Play Sound
		_gunShotSound.Play(0.5f, _random.NextSingle(-0.25f, 0.25f), 0);
	}
}