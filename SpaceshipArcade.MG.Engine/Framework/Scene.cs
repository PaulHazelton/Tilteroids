using Microsoft.Xna.Framework.Content;

namespace SpaceshipArcade.MG.Engine.Framework;

public abstract class Scene : IDisposable
{
	// Parent
	protected GameManager GameManager { get; private set; }

	// Services
	public IServiceProvider ServiceProvider => GameManager.Services;

	// Graphics Data
	public ContentManager ContentManager { get; private set; }
	public GraphicsDevice GraphicsDevice => GameManager.GraphicsDevice;
	protected int ScreenWidth { get; private set; }
	protected int ScreenHeight { get; private set; }
	protected Color BackgroundColor { get; set; }

	// Constructors
	protected Scene(GameManager manager)
	{
		GameManager = manager;

		ContentManager = new ContentManager(ServiceProvider, "Content");
		ScreenWidth = GraphicsDevice.Viewport.Width;
		ScreenHeight = GraphicsDevice.Viewport.Height;
	}
	public virtual void Dispose()
	{
		GC.SuppressFinalize(this);
		ContentManager.Dispose();
	}

	internal void WindowSizeChanged(int w, int h)
	{
		ScreenWidth = w;
		ScreenHeight = h;
		UpdateSize();
	}
	protected abstract void UpdateSize();

	// Updating and drawing
	public abstract void Update(GameTime gameTime);
	public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
}