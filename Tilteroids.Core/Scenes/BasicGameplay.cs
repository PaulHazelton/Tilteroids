using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Devices.Sensors;
using SpaceshipArcade.MG.Engine.Framework;
using Tilteroids.Core.Data;
using Tilteroids.Core.Debugging;
using Tilteroids.Core.Gameplay;
using Tilteroids.Core.Graphics;

namespace Tilteroids.Core.Scenes;

public class BasicGameplay : Scene
{
	private readonly GamePlayer gamePlayer;

	private readonly AccelerometerDisplay accelerometerDisplay;

	public BasicGameplay(GameManager manager, ContentBucket contentBucket, Accelerometer accelerometer) : base(manager)
	{
		gamePlayer = new GamePlayer(manager, contentBucket, ScreenWidth, ScreenHeight);

		accelerometerDisplay = new(contentBucket, accelerometer);

		UpdateSize();
	}

	public override void Update(GameTime gameTime)
	{
		gamePlayer.Update(gameTime);
		accelerometerDisplay.Update(gameTime);
	}

	public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
	{
		GraphicsDevice.Clear(BackgroundColor);

		gamePlayer.Draw(spriteBatch);

		accelerometerDisplay.Draw(spriteBatch);

		// Top left corner for debugging
		spriteBatch.Begin();
		Primitives.DrawRectangle(Vector2.Zero, new Vector2(100, 100), 0, Color.Red, 1);
		spriteBatch.End();
	}

	protected override void UpdateSize()
	{
		gamePlayer.UpdateSize(ScreenWidth, ScreenHeight);
	}
}