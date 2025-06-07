using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Framework.Devices.Sensors;
using nkast.Aether.Physics2D.Diagnostics;
using nkast.Aether.Physics2D.Dynamics;
using SpaceshipArcade.MG.Engine.Cameras;
using SpaceshipArcade.MG.Engine.Extensions;
using SpaceshipArcade.MG.Engine.Framework;
using SpaceshipArcade.MG.Engine.Input;
using SpaceshipArcade.MG.Engine.Utilities;
using Tilteroids.Core.Controllers;
using Tilteroids.Core.Data;
using Tilteroids.Core.Debugging;
using Tilteroids.Core.Gameplay.Entities;
using Tilteroids.Core.Graphics;

namespace Tilteroids.Core.Gameplay;

public class GamePlayer : IGameObjectHandler
{
	private readonly GameManager gameManager;
	private readonly DebugView _debugView;
	private readonly List<IGameObject> GameObjects;
	private readonly List<IGameObject> _gameObjectsToAdd;
	private readonly List<IGameObject> _gameObjectsToRemove;
	private readonly TiltController _tiltController;
	private readonly Vector3Display _accelerometerDisplay;
	private readonly Vector3Display _compassDisplay;

	private readonly Accelerometer _accelerometer;
	private readonly Compass _compass;

	private World World { get; set; }
	private Camera Camera { get; set; }
	private Matrix _projection;
	private Spaceship? _spaceShip;

	// Settings
	private bool DoDebugDraw { get; set; } = false;

	// Public Interface Stuff
	public ContentBucket ContentBucket { get; }
	public int ScreenWidth { get; private set; }
	public int ScreenHeight { get; private set; }
	public RectangleF Bounds { get; private set; }
	public Vector2 AimVector => _tiltController.AimVector;


	// TODO PAUL: Only pass in what's needed.
	// Pass a func for onExit or something
	public GamePlayer(GameManager manager, ContentBucket contentBucket, int screenWidth, int screenHeight, Accelerometer accelerometer, Compass compass)
	{
		_accelerometer = accelerometer;
		_compass = compass;
		gameManager = manager;
		ContentBucket = contentBucket;
		ScreenWidth = screenWidth;
		ScreenHeight = screenHeight;
		_tiltController = new(accelerometer);
		_gameObjectsToAdd = [];
		_gameObjectsToRemove = [];
		GameObjects = [];

		int unit = ScreenWidth / 30;

		_accelerometerDisplay = new(contentBucket,
			barDestinationRectangle: new Rectangle(2 * unit, 2 * unit, 5 * unit, 1 * unit),
			circlePos: new(6 * unit, 9 * unit), circleRadius: 1 * unit,
			textPanelPosition: new(2 * unit, 8 * unit));

		_compassDisplay = new(contentBucket,
			barDestinationRectangle: new Rectangle(10 * unit, 2 * unit, 5 * unit, 1 * unit),
			circlePos: new(14 * unit, 9 * unit), circleRadius: 1 * unit,
			textPanelPosition: new(10 * unit, 8 * unit));

		World = new World(new Vector2(0, 0));
		Camera = new Camera(ScreenWidth, ScreenHeight, Constants.MetersPerPixel);

		Camera.SnapScale(1);

		_debugView = new DebugView(World);
		_debugView.LoadContent(manager.GraphicsDevice, manager.Content);

		AddGameplayObjects();
	}

	private void Reset()
	{
		RemoveAllGameObjects();
		AddGameplayObjects();
	}

	private void AddGameplayObjects()
	{
		Vector2 worldSize = new(ScreenWidth * Constants.MetersPerPixel, ScreenHeight * Constants.MetersPerPixel);

		// World border
		AddWorldBorder(worldSize);

		// Spaceship
		_spaceShip = new Spaceship(this, new Vector2(0, 0));
		AddGameObject(_spaceShip);

		// Asteroids
		var generator = new Random();

		float maxX = worldSize.X / 2 - 2.0f;
		float maxY = worldSize.Y / 2 - 2.0f;

		for (int i = 0; i < 10; i++)
		{
			int size = generator.NextInt(1, 4);

			var asteroid = new Asteroid(this,
				size: size,
				initialPosition: new Vector2(generator.NextSingle(-maxX, maxX), generator.NextSingle(-maxY, maxY)),
				initialRotation: generator.NextSingle() * MathHelper.TwoPi,
				initialVelocity: generator.NextVector(0.3f, 2f) * (4 - size),
				initialAngularVelocity: generator.NextSingle(-1, 1));

			AddGameObject(asteroid);
		}
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

		if (InputManager.WasButtonPressed(Keys.F1))
			DoDebugDraw = !DoDebugDraw;

		if (InputManager.WasButtonPressed(Keys.R))
			Reset();

		// Update Input
		_tiltController.Update();
		_accelerometerDisplay.Update(_accelerometer.CurrentValue.Acceleration);
		_compassDisplay.Update(Vector3.Normalize(_compass.CurrentValue.MagnetometerReading));

		TouchCollection touchCollection = TouchPanel.GetState();

		if (touchCollection.Count > 0)
		{
			TouchLocation touch = touchCollection[0];
			if (touch.State == TouchLocationState.Pressed || touch.State == TouchLocationState.Moved)
			{
				// Top Left => Calibrate
				if (touch.Position.X < ScreenWidth / 2 && touch.Position.Y < ScreenHeight / 2)
				{
					_tiltController.Calibrate();
					_accelerometerDisplay.Calibrate();
					_compassDisplay.Calibrate();
				}

				if (_spaceShip is not null)
					{
						// Bottom Left => Thrust
						if (touch.Position.X < ScreenWidth / 2 && touch.Position.Y > ScreenHeight / 2)
							_spaceShip.Thrust();

						// Bottom Right => Fire
						if (touch.Position.X > ScreenWidth / 2 && touch.Position.Y > ScreenHeight / 2)
							_spaceShip.Fire();
					}
			}
		}

		// Update all game objects
		UpdateGameObjects(gameTime);

		// Update World
		World.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

		// Update Camera
		Camera.SetPosition(Vector2.Zero);
		Camera.SetRotation(0);
	}

	#region Game Objects
	public void AddGameObject(IGameObject gameObject) => _gameObjectsToAdd.Add(gameObject);
	public void RemoveGameObject(IGameObject gameObject) => _gameObjectsToRemove.Add(gameObject);
	private void Add(IGameObject gameObject)
	{
		GameObjects.Add(gameObject);

		if (gameObject is IPhysicsObject po)
			World.Add(po.Body);
	}
	private void Remove(IGameObject gameObject)
	{
		if (GameObjects.Remove(gameObject))
		{
			// gameObject.Dispose();
			if (gameObject is IPhysicsObject po && World.BodyList.Contains(po.Body))
				World.RemoveAsync(po.Body);
		}
	}
	private void UpdateGameObjects(GameTime gameTime)
	{
		// Update objects
		foreach (IGameObject gameObject in GameObjects)
			gameObject.Update(gameTime);

		// Handle queued objects
		foreach (var gameObject in _gameObjectsToAdd)
			Add(gameObject);
		foreach (var gameObject in _gameObjectsToRemove)
			Remove(gameObject);

		_gameObjectsToAdd.Clear();
		_gameObjectsToRemove.Clear();
	}
	private void RemoveAllGameObjects()
	{
		_gameObjectsToAdd.Clear();
		_gameObjectsToRemove.Clear();

		GameObjects.Clear();
		World.Clear();
	}
	#endregion

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

		foreach (var fixture in body.FixtureList)
			fixture.Restitution = 1.0f;

		World.Add(body);

		Bounds = new(v1, size);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		Primitives.SetSpriteBatch(spriteBatch);

		spriteBatch.Begin(
			transformMatrix: Camera.View
		);

		// Actual Game Objects
		foreach (var gameObject in GameObjects)
			gameObject.Draw(spriteBatch);

		// Physics Debug
		if (DoDebugDraw)
			DebugDraw();

		// World Border
		Primitives.DrawRectangleOutline(Scale(Bounds, Constants.PixelsPerMeter), Color.Blue, 2.0f, 0);
		static Rectangle Scale(RectangleF rec, float scale) => new((int)(rec.X * scale), (int)(rec.Y * scale), (int)(rec.Width * scale), (int)(rec.Height * scale));

		spriteBatch.End();

		spriteBatch.Begin();

		// Sensor Debugging
		_accelerometerDisplay.Draw(spriteBatch);
		_compassDisplay.Draw(spriteBatch);

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