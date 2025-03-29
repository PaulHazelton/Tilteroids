using Microsoft.Xna.Framework.Graphics;
using SpaceshipArcade.MG.Engine.Framework;
using Tilteroids.Main.Graphics;
using Tilteroids.Main.Scenes;
using Tilteroids.Main.Services.Implementations;
using Tilteroids.Main.Services.Interfaces;

namespace Tilteroids.Main.Framework;

public sealed class TilteroidsManager : GameManager
{
	public TilteroidsManager() : base(false, 144) { }

	protected override void Initialize()
	{
		// Initialize graphics stuff
		_graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
		_graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
		_graphics.IsFullScreen = true;
		_graphics.ApplyChanges();

		Window.AllowUserResizing = true;
		Window.IsBorderless = true;

		Window.Title = "Tilteroids - Dev";
		IsMouseVisible = true;

		base.Initialize();
	}

	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);

		// Load all content
		Primitives.LoadContent(GraphicsDevice);
		Services.AddService<IUserSettingsService>(new UserSettingsService());

		ChangeScene((gm, gd, sc) => new StartMenu(gm, gd, sc));

		base.LoadContent();
	}
}
