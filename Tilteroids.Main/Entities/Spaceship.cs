using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using SpaceshipArcade.MG.Engine.Extensions;
using SpaceshipArcade.MG.Engine.Input;
using SpaceshipArcade.MG.Engine.Utilities;
using Tilteroids.Main.Controllers;
using Tilteroids.Main.Data;
using Tilteroids.Main.Gameplay;

namespace Tilteroids.Main.Entities;

public class Spaceship : IGameObject, IPhysicsObject
{
	// Public
	public Body Body { get; private init; }

	// Private
	private readonly IGameObjectHandler _handler;
	private readonly Texture2D _shipTexture;
	private readonly Vector2 _origin;
	private readonly float _scale;
	private readonly TorqueController _torqueController;

	public Spaceship(IGameObjectHandler handler, Vector2 startingPos)
	{
		Body = CreateBody();

		_handler = handler;

		_shipTexture = handler.ContentBucket.Textures.Ship;

		_origin = new Vector2(_shipTexture.Width / 2, _shipTexture.Height / 2);
		
		_scale = (float)Constants.PixelsPerMeter / _shipTexture.Width;

		_torqueController = new(inertia: Body.Inertia);

		Body CreateBody()
		{
			var shipVertices = new Vertices([
				new(-7, -7),
				new(7, 0),
				new(-7, 7)
			]);
			shipVertices.Scale(new(1/16f));

			PolygonShape shipShape = new(shipVertices, 1);

			var fixture = new Fixture(shipShape)
			{
				Restitution = 0.5f
			};

			var body = new Body()
			{
				Position = startingPos,
				BodyType = BodyType.Dynamic,
			};

			body.Add(fixture);

			return body;
		}
	}

	public void Update(GameTime gameTime)
	{
		// Aim
		float aimAngle = GetAimAngle();
		float torque = _torqueController.ComputeTorque(Body.Rotation, Body.AngularVelocity, aimAngle);
		Body.ApplyTorque(torque);

		// Thrust
		var forceVector = PMath.PolarToCartesian(10, Body.Rotation);

		if (InputManager.IsButtonHeld(MouseButton.Right))
			Body.ApplyForce(forceVector, Body.WorldCenter);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(
			texture: _shipTexture,
			position: Body.Position * Constants.PixelsPerMeter,
			sourceRectangle: null,
			color: Color.White,
			rotation: Body.Rotation,
			origin: _origin,
			scale: _scale,
			effects: SpriteEffects.None,
			layerDepth: 0.1f);
	}

	private float GetAimAngle()
	{
		var aimVector = InputManager.MouseState.Position.ToVector2() - new Vector2(_handler.ScreenWidth / 2, _handler.ScreenHeight / 2);
		return aimVector.Angle();
	}
}