using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nkast.Aether.Physics2D.Diagnostics;
using nkast.Aether.Physics2D.Dynamics;
using SpaceshipArcade.MG.Engine.Cameras;
using SpaceshipArcade.MG.Engine.Framework;
using SpaceshipArcade.MG.Engine.Input;
using Tilteroids.Main.Data;
using Tilteroids.Main.Entities;
using Tilteroids.Main.Gameplay;
using Tilteroids.Main.Graphics;

namespace Tilteroids.Main.Scenes;

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

		gamePlayer.Draw(spriteBatch, gameTime);
	}

	protected override void UpdateSize()
	{
		gamePlayer.UpdateSize(ScreenWidth, ScreenHeight);
	}
}