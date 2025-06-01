using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceshipArcade.MG.Engine.Framework;
using Tilteroids.Core.Data;
using Tilteroids.Core.Gameplay;

namespace Tilteroids.Core.Scenes;

public class BasicGameplay : Scene
{
	private readonly GamePlayer gamePlayer;

	public BasicGameplay(GameManager manager, ContentBucket contentBucket) : base(manager)
	{
		gamePlayer = new GamePlayer(manager, contentBucket, ScreenWidth, ScreenHeight);

		UpdateSize();
	}

	public override void Update(GameTime gameTime)
	{
		gamePlayer.Update(gameTime);
	}

	public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
	{
		GraphicsDevice.Clear(BackgroundColor);

		gamePlayer.Draw(spriteBatch);
	}

	protected override void UpdateSize()
	{
		gamePlayer.UpdateSize(ScreenWidth, ScreenHeight);
	}
}