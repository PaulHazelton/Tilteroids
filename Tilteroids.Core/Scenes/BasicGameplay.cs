using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Devices.Sensors;
using SpaceshipArcade.MG.Engine.Framework;
using SpaceshipArcade.MG.Engine.Input.Sensors;
using Tilteroids.Core.Data;
using Tilteroids.Core.Gameplay;

namespace Tilteroids.Core.Scenes;

public class BasicGameplay : Scene
{
	private readonly GamePlayer gamePlayer;

	public BasicGameplay(GameManager manager, ContentBucket contentBucket, Accelerometer accelerometer, Compass compass, OrientationSensor orientationSensor) : base(manager)
	{
		gamePlayer = new GamePlayer(manager, contentBucket, ScreenWidth, ScreenHeight, accelerometer, compass, orientationSensor);

		UpdateSize();
	}

	public override void Update(GameTime gameTime)
	{
		gamePlayer.Update(gameTime);
	}

	public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
	{
		GraphicsDevice.Clear(BackgroundColor);

		gamePlayer.Draw(spriteBatch);

		// Top left corner for debugging
		// spriteBatch.Begin();
		// Primitives.DrawRectangle(Vector2.Zero, new Vector2(100, 100), 0, Color.Red, 1);
		// spriteBatch.End();
	}

	protected override void UpdateSize()
	{
		gamePlayer.UpdateSize(ScreenWidth, ScreenHeight);
	}
}