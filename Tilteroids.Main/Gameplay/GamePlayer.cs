using System.Collections.Generic;
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

namespace Tilteroids.Main.Gameplay;

public class GamePlayer : IGameObjectHandler
{
	// Parent
	private readonly GameManager gameManager;

	// Dependencies
	public ContentBucket ContentBucket { get; }

	// External data
	public int ScreenWidth { get; private set; }
	public int ScreenHeight { get; private set; }

	// Internal data
	
	// Debug Draw for Physics
	private readonly DebugView _debugView;
	private Matrix _projection;

	private World World { get; set; }
	private Camera Camera { get; set; }
	private List<IGameObject> GameObjects { get; set; }

	// TODO PAUL: Only pass in what's needed.
	// Pass a func for onExit or something
	public GamePlayer(GameManager manager, ContentBucket contentBucket, int screenWidth, int screenHeight)
	{
		gameManager = manager;
		ContentBucket = contentBucket;
		ScreenWidth = screenWidth;
		ScreenHeight = screenHeight;
		GameObjects = [];

		World = new World(new Vector2(0, 9.82f));
		World = new World(new Vector2(0, 0));
		Camera = new Camera(ScreenWidth, ScreenHeight, Constants.MetersPerPixel);

		Camera.SnapScale(1);

		_debugView = new DebugView(World);
		_debugView.LoadContent(manager.GraphicsDevice, manager.Content);

		var spaceship = new Spaceship(this, new Vector2(0, 0));
		AddGameObject(spaceship);

		AddWorldBorder(size: new(24, 13.5f));
	}

	public void UpdateSize(int screenWidth, int screenHeight)
	{
		ScreenWidth = screenWidth;
		ScreenHeight = screenHeight;

		Camera.UpdateScreenSize(ScreenWidth, ScreenHeight);

		float halfWidthMeters = Constants.MetersPerPixel * (ScreenWidth / 2);
		float halfHeightMeters = Constants.MetersPerPixel * (ScreenHeight / 2);
		_projection = Matrix.CreateOrthographicOffCenter(-halfWidthMeters, halfWidthMeters, halfHeightMeters, -halfHeightMeters, -1, 1);
	}

	public void Update(GameTime gameTime)
	{
		if (InputManager.WasButtonPressed(Keys.Escape))
			gameManager.Exit();

		// Update all game objects
		foreach (var gameObject in GameObjects)
			gameObject.Update(gameTime);

		World.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

		Camera.SetPosition(Vector2.Zero);
		Camera.SetRotation(0);
	}

	public void AddGameObject(IGameObject gameObject)
	{
		GameObjects.Add(gameObject);

		if (gameObject is IPhysicsObject po)
			World.Add(po.Body);
	}

	public void RemoveGameObject(IGameObject gameObject)
	{
		throw new System.NotImplementedException();
	}

	private void AddWorldBorder(Vector2 size, Vector2 center = default)
	{
		var body = new Body
		{
			BodyType = BodyType.Static
		};

		var v1 = center + new Vector2(-size.X / 2, -size.Y / 2);
		var v2 = center + new Vector2(size.X / 2, -size.Y / 2);
		var v3 = center + new Vector2(size.X / 2, size.Y / 2);
		var v4 = center + new Vector2(-size.X / 2, size.Y / 2);

		body.CreateEdge(v1, v2);
		body.CreateEdge(v2, v3);
		body.CreateEdge(v3, v4);
		body.CreateEdge(v4, v1);

		World.Add(body);
	}

	public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
	{
		Primitives.SetSpriteBatch(spriteBatch);

		spriteBatch.Begin(
			transformMatrix: Camera.View
		);

		foreach (var gameObject in GameObjects)
			gameObject.Draw(spriteBatch);

		DebugDraw();

		spriteBatch.End();
	}

	private void DebugDraw(float alpha = 1)
	{
		_debugView.AppendFlags(DebugViewFlags.ContactNormals);
		_debugView.AppendFlags(DebugViewFlags.ContactPoints);
		// _debugView.AppendFlags(DebugViewFlags.DebugPanel);
		// _debugView.AppendFlags(DebugViewFlags.CenterOfMass);
		_debugView.RenderDebugData(_projection, Camera.SimView, blendState: BlendState.Additive, alpha: alpha);
	}
}