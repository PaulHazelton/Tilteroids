using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Devices.Sensors;
using MonoGame.Framework.Utilities;
using SpaceshipArcade.MG.Engine.Framework;
using Tilteroids.Core.Data;
using Tilteroids.Core.Graphics;
using Tilteroids.Core.Scenes;
using Tilteroids.Core.Services.Interfaces;
using Tilteroids.Core.Services.Implementations;
using SpaceshipArcade.MG.Engine.Input.Sensors;
using Microsoft.Xna.Framework;

namespace Tilteroids.Core.Framework;

public sealed class TilteroidsManager : GameManager
{
	public Accelerometer Accelerometer { get; private set; } = new();
	public Compass Compass { get; private set; } = new();
	public OrientationSensor OrientationSensor { get; private set; } = new(compassRollingAvgCount: 6);

	protected override void Initialize()
	{
		switch (PlatformInfo.MonoGamePlatform)
		{
			case MonoGamePlatform.DesktopGL: InitializeDesktopGl(true, 143.91d); break;
			case MonoGamePlatform.Android: InitializeAndroid(120); break;
			default: throw new NotSupportedException($"Platform {PlatformInfo.MonoGamePlatform} not supported.");
		}

		base.Initialize();
	}

	private void InitializeDesktopGl(bool fullScreen, double targetFps, bool hardwareModeSwitch = false)
	{
		// Initialize graphics stuff
		_graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
		_graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

		// _graphics.SynchronizeWithVerticalRetrace = true;

		_graphics.HardwareModeSwitch = hardwareModeSwitch;
		_graphics.IsFullScreen = fullScreen;

		_graphics.ApplyChanges();

		// Window stuff
		Window.AllowUserResizing = true;
		Window.IsBorderless = true;

		Window.Title = "Tilteroids - Dev";

		// Other
		TargetElapsedTime = TimeSpan.FromSeconds(1.0d / targetFps);

		IsMouseVisible = true;
	}

	private void InitializeAndroid(double targetFps = 120)
	{
		// Sensors
		Accelerometer.Start();
		Compass.Start();
		OrientationSensor.Start();

		// Other
		TargetElapsedTime = TimeSpan.FromSeconds(1.0d / targetFps);
	}

	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);

		// Load all content
		var contentBucket = new ContentBucket(Content);
		Primitives.LoadContent(GraphicsDevice);
		Services.AddService<IUserSettingsService>(new UserSettingsService());

		// ChangeScene((gm) => new StartMenu(gm, contentBucket));
		ChangeScene((gm) => new BasicGameplay(gm, contentBucket, Accelerometer, Compass, OrientationSensor));

		base.LoadContent();
	}

	protected override void Update(GameTime gameTime)
	{
		OrientationSensor.Sample();
		base.Update(gameTime);
	}
}
