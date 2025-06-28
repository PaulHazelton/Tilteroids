using Microsoft.Xna.Framework.Input;
using SpaceshipArcade.MG.Engine.Framework;
using SpaceshipArcade.MG.Engine.Graphics;
using SpaceshipArcade.MG.Engine.Input;
using Tilteroids.Core.Data;

namespace Tilteroids.Core.Scenes;

public class StartMenu : Scene
{
	private readonly ContentBucket ContentBucket;

	public StartMenu(GameManager manager, ContentBucket contentBucket)
		: base(manager)
	{
		ContentBucket = contentBucket;
	}

	protected override void UpdateSize()
	{
		// throw new NotImplementedException();
	}

	public override void Update(GameTime gameTime)
	{
		if(InputManager.WasButtonPressed(Keys.Escape))
			GameManager.Exit();
	}

	public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
	{
		GraphicsDevice.Clear(BackgroundColor);
		Primitives.SetSpriteBatch(spriteBatch);

		spriteBatch.Begin();

		var pos = new Vector2(ScreenWidth / 2, ScreenHeight / 2);

		Primitives.DrawRectangle(pos, new Vector2(400, 200), 0, Color.White);
		spriteBatch.DrawString(ContentBucket.Fonts.FallbackFont, "Launch", pos, Color.Red);

		spriteBatch.End();
	}
}