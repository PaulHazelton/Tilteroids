using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using SpaceshipArcade.MG.Engine.Utilities;
using Tilteroids.Main.Data;
using Tilteroids.Main.Gameplay.Guns;
using Tilteroids.Main.Graphics;

namespace Tilteroids.Main.Gameplay.Entities;

public class Bullet : IGameObject, IPhysicsObject
{
	// Public
	public Body Body { get; private init; }

	// Private
	private readonly IGameObjectHandler _handler;
	private readonly Gun _gunSettings;

	public Bullet(IGameObjectHandler handler, Vector2 position, float aimAngle, Gun gunSettings)
	{
		_handler = handler;
		_gunSettings = gunSettings;

		Body = CreateBody();

		Body CreateBody()
		{
			var body = new Body()
			{
				Position = position + PMath.PolarToCartesian(_gunSettings.MuzzleOffset, aimAngle),
				Rotation = aimAngle,
				BodyType = BodyType.Dynamic,
				LinearVelocity = PMath.PolarToCartesian(_gunSettings.MuzzleVelocity, aimAngle),
				Tag = this,
			};

			body.CreateRectangle(_gunSettings.Length, _gunSettings.Width, _gunSettings.Density, Vector2.Zero);

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
		Primitives.DrawRectangle(Body.Position * Constants.PixelsPerMeter, new Vector2(_gunSettings.Length, _gunSettings.Width) * Constants.PixelsPerMeter, Body.Rotation, Color.White, layerDepth: 1);
	}
}