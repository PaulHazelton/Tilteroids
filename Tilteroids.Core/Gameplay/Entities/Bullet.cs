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
	private readonly float _radius;
	private TimeSpan _lifeTime;

	// Public
	public Body Body { get; private init; }
	public readonly Gun GunSettings;

	public Bullet(IGamePlayer handler, Vector2 position, float aimAngle, Gun gunSettings)
	{
		_handler = handler;
		GunSettings = gunSettings;

		_radius = MathHelper.Max(GunSettings.Length, GunSettings.Width);

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
		_lifeTime += gameTime.ElapsedGameTime;

		// Calculate distance travelled
		var distance = (float)(GunSettings.MuzzleVelocity * _lifeTime.TotalSeconds);

		if (distance > _handler.Bounds.Height)
			_handler.RemoveGameObject(this);

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
		Primitives.DrawRectangle(Body.Position, new Vector2(GunSettings.Length, GunSettings.Width), Body.Rotation, Color.White, layerDepth: 1);
	}
}