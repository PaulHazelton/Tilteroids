using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceshipArcade.MG.Engine.Framework;
using SpaceshipArcade.MG.Engine.Input;
using Tilteroids.Main.Graphics;

namespace Tilteroids.Main.Scenes;

public class StartMenu : Scene
{
	public StartMenu(GameManager manager, GraphicsDevice device, IServiceProvider serviceProvider)
		: base(manager, device, serviceProvider)
	{
		
	}

	protected override void UpdateSize()
	{
		// throw new NotImplementedException();
	}

	public override void Update(GameTime gameTime)
	{
		
	}

	public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
	{
		GraphicsDevice.Clear(BackgroundColor);
		Primitives.SetSpriteBatch(spriteBatch);

		spriteBatch.Begin();
		
		Primitives.DrawRectangle(new Vector2(ScreenWidth / 2, ScreenHeight / 2), new Vector2(400, 200), 0, Color.White);

		spriteBatch.End();
	}
}