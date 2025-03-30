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
using Tilteroids.Main.Graphics;

namespace Tilteroids.Main.Scenes;

public class BasicGameplay : Scene
{
	private readonly ContentBucket ContentBucket;

	// Debug Draw for Physics
	private readonly DebugView _debugView;
	private Matrix _projection;

	public World World { get; set; }
	public Camera Camera { get; set; }

	private readonly Spaceship Spaceship;

	public BasicGameplay(GameManager manager, ContentBucket contentBucket) : base(manager)
	{
		ContentBucket = contentBucket;

		World = new World(new Vector2(0, 9.82f));
		Camera = new Camera(ScreenWidth, ScreenHeight, Constants.MetersPerPixel);

		Camera.SnapScale(1);

		_debugView = new DebugView(World);
		_debugView.LoadContent(manager.GraphicsDevice, manager.Content);

		AddWorldBorder(size: new(24, 13.5f));
		Spaceship = new(ContentBucket, new Vector2(0, 0));
		World.Add(Spaceship.Body);

		UpdateSize();
	}

	private void AddWorldBorder(Vector2 size, Vector2 center = default)
	{
		var body = new Body
		{
			BodyType = BodyType.Static
		};

		var v1 = center + new Vector2(-size.X / 2, -size.Y / 2);
		var v2 = center + new Vector2( size.X / 2, -size.Y / 2);
		var v3 = center + new Vector2( size.X / 2,  size.Y / 2);
		var v4 = center + new Vector2(-size.X / 2,  size.Y / 2);

		body.CreateEdge(v1, v2);
		body.CreateEdge(v2, v3);
		body.CreateEdge(v3, v4);
		body.CreateEdge(v4, v1);

		World.Add(body);
	}

	protected override void UpdateSize()
	{
		Camera.UpdateScreenSize(ScreenWidth, ScreenHeight);

		float halfWidthMeters = Constants.MetersPerPixel * (ScreenWidth / 2);
		float halfHeightMeters = Constants.MetersPerPixel * (ScreenHeight / 2);
		_projection = Matrix.CreateOrthographicOffCenter(-halfWidthMeters, halfWidthMeters, halfHeightMeters, -halfHeightMeters, -1, 1);
	}

	public override void Update(GameTime gameTime)
	{
		if (InputManager.WasButtonPressed(Keys.Escape))
			GameManager.Exit();

		World.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

		Camera.SetPosition(Vector2.Zero);
		Camera.SetRotation(0);
	}

	public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
	{
		GraphicsDevice.Clear(BackgroundColor);
		Primitives.SetSpriteBatch(spriteBatch);

		spriteBatch.Begin(
			transformMatrix: Camera.View
		);

		Spaceship.Draw(spriteBatch);

		DebugDraw();

		spriteBatch.End();
	}

	protected void DebugDraw(float alpha = 1)
	{
		_debugView.AppendFlags(DebugViewFlags.ContactNormals);
		_debugView.AppendFlags(DebugViewFlags.ContactPoints);
		_debugView.AppendFlags(DebugViewFlags.DebugPanel);
		_debugView.RenderDebugData(_projection, Camera.SimView, blendState: BlendState.Additive, alpha: alpha);
	}
}