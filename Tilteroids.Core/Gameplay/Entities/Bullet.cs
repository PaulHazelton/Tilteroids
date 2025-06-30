using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using SpaceshipArcade.MG.Engine.Graphics;
using SpaceshipArcade.MG.Engine.Utilities;
using Tilteroids.Core.Gameplay.Guns;

namespace Tilteroids.Core.Gameplay.Entities;

public class Bullet : IGameObject, IPhysicsObject
{
	// Private
	private readonly IGamePlayer _handler;

	// Public
	public Body Body { get; private init; }
	public readonly Gun GunSettings;

	public Bullet(IGamePlayer handler, Vector2 position, float aimAngle, Gun gunSettings)
	{
		_handler = handler;
		GunSettings = gunSettings;

		Body = CreateBody();

		Body CreateBody()
		{
			var body = new Body()
			{
				Tag = this,
				BodyType = BodyType.Dynamic,
				Position = position + PMath.PolarToCartesian(GunSettings.MuzzleOffset, aimAngle),
				Rotation = aimAngle,
				LinearVelocity = PMath.PolarToCartesian(GunSettings.MuzzleVelocity, aimAngle),
			};

			body.CreateRectangle(GunSettings.Length, GunSettings.Width, GunSettings.Density, Vector2.Zero);

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
		Primitives.DrawRectangle(Body.Position, new Vector2(GunSettings.Length, GunSettings.Width), Body.Rotation, Color.White, layerDepth: 1);
	}
}