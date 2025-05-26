using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using SpaceshipArcade.MG.Engine.Utilities;
using Tilteroids.Main.Data;
using Tilteroids.Main.Graphics;

namespace Tilteroids.Main.Gameplay.Entities;

public class Bullet : IGameObject, IPhysicsObject
{
	// Public
	public Body Body { get; private init; }

	// Private
	private readonly IGameObjectHandler _handler;

	// Bullet settings
	public const float MuzzleOffset = 0.4f;
	public const float MuzzleVelocity = 20.0f;
	public const float Width = 0.05f;
	public const float Length = 0.4f;
	public const float Density = 1.0f;

	public Bullet(IGameObjectHandler handler, Vector2 position, float aimAngle)
	{
		_handler = handler;

		Body = CreateBody();

		Body CreateBody()
		{
			var body = new Body()
			{
				Position = position + PMath.PolarToCartesian(MuzzleOffset, aimAngle),
				Rotation = aimAngle,
				BodyType = BodyType.Dynamic,
				LinearVelocity = PMath.PolarToCartesian(MuzzleVelocity, aimAngle),
				Tag = this,
			};

			body.CreateRectangle(Length, Width, Density, Vector2.Zero);

			body.OnCollision += OnCollisionHandler;

			body.FixtureList[0].CollidesWith = Category.Cat1;

			return body;
		}
	}

	private bool OnCollisionHandler(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		_handler.RemoveGameObject(this);

		return true;
	}

	public void Update(GameTime gameTime)
	{
		if (!_handler.Bounds.Contains(Body.Position))
			_handler.RemoveGameObject(this);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		Primitives.DrawRectangle(Body.Position * Constants.PixelsPerMeter, new Vector2(Length, Width) * Constants.PixelsPerMeter, Body.Rotation, Color.White, layerDepth: 1);
	}
}