using Apos.Shapes;
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

	public override void Draw(SpriteBatch spriteBatch, ShapeBatch shapeBatch, GameTime gameTime)
	{
		GraphicsDevice.Clear(BackgroundColor);

		gamePlayer.Draw(spriteBatch);

		shapeBatch.Begin();
		shapeBatch.BorderLine(new Vector2(100, 20), new Vector2(450, -15), 20, Color.White, 2f);

		shapeBatch.DrawCircle(new Vector2(120, 120), 75, new Color(96, 165, 250), new Color(191, 219, 254), 4f);
		shapeBatch.DrawCircle(new Vector2(120, 120), 30, Color.White, Color.Black, 20f);

		shapeBatch.DrawCircle(new Vector2(370, 120), 100, new Color(96, 165, 250), new Color(191, 219, 254), 4f);
		shapeBatch.DrawCircle(new Vector2(370, 120), 40, Color.White, Color.Black, 20f);

		shapeBatch.DrawCircle(new Vector2(190, 270), 10, Color.Black, Color.White, 2f);
		shapeBatch.DrawCircle(new Vector2(220, 270), 10, Color.Black, Color.White, 2f);

		shapeBatch.FillCircle(new Vector2(235, 400), 30, new Color(220, 38, 38));
		shapeBatch.FillRectangle(new Vector2(235, 370), new Vector2(135, 60), new Color(220, 38, 38));
		shapeBatch.FillCircle(new Vector2(235, 400), 20, Color.White);
		shapeBatch.FillRectangle(new Vector2(235, 380), new Vector2(125, 40), Color.White);
		shapeBatch.End();
	}

	protected override void UpdateSize()
	{
		gamePlayer.UpdateSize(ScreenWidth, ScreenHeight);
	}
}