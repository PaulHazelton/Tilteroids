using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceshipArcade.MG.Engine.Framework;

namespace Tilteroids.Main.Scenes;

public class StartMenu : GameEnvironment
{
	public StartMenu(GameManager manager, GraphicsDevice device, IServiceProvider serviceProvider)
		: base(manager, device, serviceProvider)
	{
		
	}

	protected override void UpdateSize()
	{
		throw new NotImplementedException();
	}

	public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
	{
		throw new NotImplementedException();
	}

	public override void Update(GameTime gameTime)
	{
		throw new NotImplementedException();
	}
}