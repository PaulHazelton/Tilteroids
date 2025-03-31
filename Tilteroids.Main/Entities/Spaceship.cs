using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using SpaceshipArcade.MG.Engine.Input;
using SpaceshipArcade.MG.Engine.Utilities;
using Tilteroids.Main.Data;
using Tilteroids.Main.Graphics;

namespace Tilteroids.Main.Entities;

public class Spaceship
{
	public Body Body { get; private init; }

	public TextPanel DebugPanel;

	private readonly Texture2D shipTexture;

	private Vector2 origin;
	private float scale;

	public Spaceship(ContentBucket contentBucket, Vector2 startingPos)
	{
		DebugPanel = new TextPanel(contentBucket.Fonts.FallbackFont, new Vector2(10, 10), TextPanel.AnchorCorner.TopLeft);

		shipTexture = contentBucket.Textures.Ship;
		origin = new Vector2(shipTexture.Width / 2, shipTexture.Height / 2);

		scale = (float)Constants.PixelsPerMeter / shipTexture.Width;

		Body = new()
		{
			Position = startingPos,
			BodyType = BodyType.Dynamic,
		};

		Body.Add(CreateShipFixture());

		// Body.AngularDamping = 20;

		static Fixture CreateShipFixture()
		{
			// var shipVertices = new Vertices([
			// 	new(8, 1),
			// 	new(15, 15),
			// 	// new(8, 12),
			// 	new(1, 15)
			// ]);
			var shipVertices = new Vertices([
				new(0, -7),
				new(7, 7),
				// new(8, 12),
				new(-7, 7)
			]);
			shipVertices.Scale(new(1/16f));

			PolygonShape shipShape = new(shipVertices, 1);

			return new Fixture(shipShape)
			{
				Restitution = 0.5f
			};
		}
	}

	private Vector2 _aimVector;

	public void Update(Vector2 aimVector)
	{
		DebugPanel.ClearLines();

		_aimVector = aimVector;

		// Aim
		aimVector.Normalize();
		float aimAngle = (float)(Math.Atan2(aimVector.Y, aimVector.X) + (Math.PI / 2));

		// Body rotation vector
		float angleDifference = -(float)Math.Atan2(Math.Sin(Body.Rotation - aimAngle), Math.Cos(Body.Rotation - aimAngle));
		// float angleDiffAbs = Math.Abs(angleDifference);
		
		// float torque = 5 * (angleDiffAbs < 1
		// 	? angleDifference
		// 	: angleDifference * angleDiffAbs * 2);

		float torque = ComputeTorque(Body.Rotation, Body.AngularVelocity, aimAngle);

		DebugPanel.AddLine($"Angle Difference: {angleDifference}");

		Body.ApplyTorque(torque);

		// Thrust
		var forceVector = PMath.PolarToCartesian(10, (float)(Body.Rotation - (Math.PI / 2)));

		if (InputManager.IsButtonHeld(MouseButton.Right))
			Body.ApplyForce(forceVector, Body.WorldCenter);
	}

	private const float proportionalGain = 400.0f;
	private const float derivativeGain = 30.0f;

	private float ComputeTorque(float currentAngle, float currentAngularVelocity, float targetAngle)
	{
		float angleDiff = -(float)Math.Atan2(Math.Sin(currentAngle - targetAngle), Math.Cos(currentAngle - targetAngle));

		float targetAcceleration = proportionalGain * angleDiff - derivativeGain * currentAngularVelocity;

		float torque = Body.Inertia * targetAcceleration;

		return MathHelper.Clamp(torque, -20, 20);
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

		// Vector2 diagramCenter = Body.Position * Constants.PixelsPerMeter;

		// Vector2 bodyRotation = PMath.PolarToCartesian(100, (float)(Body.Rotation - (Math.PI / 2)));

		// Primitives.DrawLine(diagramCenter, diagramCenter + bodyRotation, 1, Color.White);
		// Primitives.DrawLine(diagramCenter, diagramCenter + _aimVector, 1, Color.Lime);

		// DebugPanel.Draw(spriteBatch);
	}
}