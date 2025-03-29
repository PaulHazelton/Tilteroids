using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceshipArcade.MG.Engine.Framework;
using SpaceshipArcade.MG.Engine.Input;
using Tilteroids.Main.Data;
using Tilteroids.Main.Graphics;

namespace Tilteroids.Main.Scenes;

public class BasicGameplay : Scene
{
	private readonly ContentBucket ContentBucket;

	private Vector2 _center;
	private Vector2 _origin;

	public BasicGameplay(GameManager manager, ContentBucket contentBucket) : base(manager)
	{
		ContentBucket = contentBucket;

		_origin = new Vector2(ContentBucket.Textures.Ship.Width / 2, ContentBucket.Textures.Ship.Height / 2);

		UpdateSize();
	}

	protected override void UpdateSize()
	{
		_center = new Vector2(ScreenWidth / 2, ScreenHeight / 2);
	}

	public override void Update(GameTime gameTime)
	{
		if (InputManager.WasButtonPressed(Keys.Escape))
			GameManager.Exit();
	}

	public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
	{
		GraphicsDevice.Clear(BackgroundColor);
		Primitives.SetSpriteBatch(spriteBatch);

		spriteBatch.Begin();

		spriteBatch.Draw(
			texture: ContentBucket.Textures.Ship,
			position: _center,
			sourceRectangle: null,
			color: Color.White,
			rotation: 0,
			origin: _origin,
			scale: 1,
			effects: SpriteEffects.None,
			layerDepth: 1);

		spriteBatch.End();
	}
}