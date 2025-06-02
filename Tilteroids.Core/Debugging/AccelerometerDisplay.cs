using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Devices.Sensors;
using SpaceshipArcade.MG.Engine.Utilities;
using Tilteroids.Core.Data;
using Tilteroids.Core.Gameplay;
using Tilteroids.Core.Graphics;

namespace Tilteroids.Core.Debugging;

public class AccelerometerDisplay(ContentBucket contentBucket, Accelerometer accelerometer) : IGameObject
{
	private readonly Accelerometer _accelerometer = accelerometer;

	private readonly TextPanel _textPanel = new(contentBucket.Fonts.FallbackFont, position: new Vector2(200, 800), TextPanel.AnchorCorner.TopLeft);

	private float X = 0;
	private float Y = 0;
	private float Z = 0;

	private Rectangle recX = new();// = new(450, 210, 0, 80);
	private Rectangle recY = new();// = new(450, 410, 0, 80);
	private Rectangle recZ = new();// = new(450, 610, 0, 80);

	public void Update(GameTime gameTime)
	{
		// Graphics
		X = _accelerometer.CurrentValue.Acceleration.X;
		Y = _accelerometer.CurrentValue.Acceleration.Y;
		Z = _accelerometer.CurrentValue.Acceleration.Z;

		int xWidth = (int)MathHelper.Clamp(PMath.Map(0, 1.1f, 0, 240, Math.Abs(X)), 0, 240);
		recX = new(x: X > 0 ? 450 : 450 - xWidth, y: 210, width: xWidth, height: 80);

		int yWidth = (int)MathHelper.Clamp(PMath.Map(0, 1.1f, 0, 240, Math.Abs(Y)), 0, 240);
		recY = new(x: Y > 0 ? 450 : 450 - yWidth, y: 410, width: yWidth, height: 80);

		int zWidth = (int)MathHelper.Clamp(PMath.Map(0, 1.1f, 0, 240, Math.Abs(Z)), 0, 240);
		recZ = new(x: Z > 0 ? 450 : 450 - zWidth, y: 610, width: zWidth, height: 80);

		// Text
		_textPanel.ClearLines();
		_textPanel.AddLine($"Valid: {_accelerometer.IsDataValid}");
		_textPanel.AddLine($"X: {_accelerometer.CurrentValue.Acceleration.X}");
		_textPanel.AddLine($"Y: {_accelerometer.CurrentValue.Acceleration.Y}");
		_textPanel.AddLine($"Z: {_accelerometer.CurrentValue.Acceleration.Z}");
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Begin();

		Primitives.DrawRectangleOutline(new Rectangle(200, 200, 500, 100), Color.Magenta, 2.0f, 1.0f);
		Primitives.DrawRectangle(recX, Color.Magenta);
		Primitives.DrawRectangleOutline(new Rectangle(200, 400, 500, 100), Color.Lime, 2.0f, 1.0f);
		Primitives.DrawRectangle(recY, Color.Lime);
		Primitives.DrawRectangleOutline(new Rectangle(200, 600, 500, 100), Color.Cyan, 2.0f, 1.0f);
		Primitives.DrawRectangle(recZ, Color.Cyan);
		_textPanel.Draw(spriteBatch);

		spriteBatch.End();
	}
}