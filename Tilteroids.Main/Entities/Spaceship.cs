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

namespace Tilteroids.Main.Entities;

public class Spaceship
{
	// Public
	public Body Body { get; private init; }

	// Private
	private readonly Vector2 origin;
	private readonly float scale;
	private readonly TorqueController torqueController;
	private readonly Texture2D shipTexture;

	public Spaceship(ContentBucket contentBucket, Vector2 startingPos)
	{
		shipTexture = contentBucket.Textures.Ship;

		origin = new Vector2(shipTexture.Width / 2, shipTexture.Height / 2);
		scale = (float)Constants.PixelsPerMeter / shipTexture.Width;

		Body = CreateBody();

		torqueController = new(inertia: Body.Inertia);

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

	public void Update(Vector2 aimVector)
	{
		// Aim
		float aimAngle = aimVector.Angle();
		float torque = torqueController.ComputeTorque(Body.Rotation, Body.AngularVelocity, aimAngle);
		Body.ApplyTorque(torque);

		// Thrust
		var forceVector = PMath.PolarToCartesian(10, Body.Rotation);

		if (InputManager.IsButtonHeld(MouseButton.Right))
			Body.ApplyForce(forceVector, Body.WorldCenter);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(
			texture: shipTexture,
			position: Body.Position * Constants.PixelsPerMeter,
			sourceRectangle: null,
			color: Color.White,
			rotation: Body.Rotation,
			origin: origin,
			scale: scale,
			effects: SpriteEffects.None,
			layerDepth: 0.1f);
	}
}