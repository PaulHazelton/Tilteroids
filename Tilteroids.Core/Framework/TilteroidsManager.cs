using MonoGame.Framework.Devices.Sensors;
using MonoGame.Framework.Utilities;
using SpaceshipArcade.MG.Engine.Framework;
using Tilteroids.Core.Data;
using Tilteroids.Core.Scenes;
using Tilteroids.Core.Services.Interfaces;
using Tilteroids.Core.Services.Implementations;
using SpaceshipArcade.MG.Engine.Input.Sensors;
using SpaceshipArcade.MG.Engine.Graphics;

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
			case MonoGamePlatform.DesktopGL: InitializeDesktopGl(fullScreen: false, 143.91d); break;
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
		Window.IsBorderless = false;

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

		void startGame() => ChangeScene(() => new BasicGameplay(this, contentBucket, Accelerometer, Compass, OrientationSensor, startMainMenu));
		void startMainMenu() => ChangeScene(() => new StartMenu(this, contentBucket, startGame));

		startMainMenu();

		base.LoadContent();
	}

	// private class LoadedManager(SpriteBatch sb, ContentBucket cb)
	// {
	// 	private readonly SpriteBatch _spriteBatch = sb;
	// 	private readonly ContentBucket _contentBucket = cb;

	// 	public void StartGame()
	// 	{
	// 		ChangeScene((gm) => new BasicGameplay(gm, _contentBucket, Accelerometer, Compass, OrientationSensor));
	// 	}
	// }

	protected override void Update(GameTime gameTime)
	{
		OrientationSensor.Sample();
		base.Update(gameTime);
	}
}
