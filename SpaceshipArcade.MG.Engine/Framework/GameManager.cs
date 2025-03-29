using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceshipArcade.MG.Engine.Input;

namespace SpaceshipArcade.MG.Engine.Framework
{
	public abstract class GameManager : Game
	{
		// Low level framework stuff
		protected readonly GraphicsDeviceManager _graphics;
		protected SpriteBatch? _spriteBatch;

		// Manager stuff
		private Scene? _scene;

		// FPS
		private int _frameCount = 0;
		private double _secondsPassed = 0;
		public int CurrentFPS { get; private set; }


		public GameManager(bool fullScreen, int targetFps)
		{
			Content.RootDirectory = "Content";
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8,
				HardwareModeSwitch = fullScreen,
				IsFullScreen = fullScreen,
			};
			TargetElapsedTime = TimeSpan.FromSeconds(1.0d / targetFps);

			Window.ClientSizeChanged += (sender, e) => WindowSizeChanged();
		}

		public void ChangeScene(Func<GameManager, Scene> createScene)
		{
			_scene?.Dispose();
			_scene = createScene(this);
			WindowSizeChanged();
		}

		protected void WindowSizeChanged() => _scene?.WindowSizeChanged(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

		protected override void Update(GameTime gameTime)
		{
			InputManager.BeginUpdate();

			// _scene?.Update(new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime));
			_scene?.Update(gameTime);
			base.Update(gameTime);	// TODO Look into removing this

			InputManager.EndUpdate();
		}
		protected override void Draw(GameTime gameTime)
		{
			// Calculate fps
			_frameCount++;
			_secondsPassed += gameTime.ElapsedGameTime.TotalSeconds;
			if (_secondsPassed >= 1)
			{
				CurrentFPS = _frameCount;
				_secondsPassed -= 1;
				_frameCount = 0;
			}

			_scene?.Draw(_spriteBatch!, gameTime);
			base.Draw(gameTime);
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_graphics.Dispose();
				_spriteBatch?.Dispose();
				_scene?.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}