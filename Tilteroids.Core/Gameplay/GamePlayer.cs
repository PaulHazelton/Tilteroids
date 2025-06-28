using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Framework.Devices.Sensors;
using nkast.Aether.Physics2D.Diagnostics;
using nkast.Aether.Physics2D.Dynamics;
using SpaceshipArcade.MG.Engine.Cameras;
using SpaceshipArcade.MG.Engine.Debugging;
using SpaceshipArcade.MG.Engine.Extensions;
using SpaceshipArcade.MG.Engine.Framework;
using SpaceshipArcade.MG.Engine.Graphics;
using SpaceshipArcade.MG.Engine.Input;
using SpaceshipArcade.MG.Engine.Input.Sensors;
using SpaceshipArcade.MG.Engine.Utilities;
using Tilteroids.Core.Controllers;
using Tilteroids.Core.Data;
using Tilteroids.Core.Debugging;
using Tilteroids.Core.Gameplay.Entities;

namespace Tilteroids.Core.Gameplay;

public class GamePlayer : IGamePlayer
{
	private readonly GameManager _gameManager;
	private readonly GameObjectCollection _gameObjectCollection;
	private readonly TiltController _tiltController;

	#region Experimental and debugging stuff

	private readonly Accelerometer _accelerometer;
	private Vector3 _aCalibrationVector;
	private readonly Vector3BarDisplay _aBarDisplay;
	private readonly Vector3CircleDisplay _aCircleDisplay;

	private readonly Compass _compass;
	private Vector3 _cCalibrationVector;
	private readonly Vector3BarDisplay _cBarDisplay;
	private readonly Vector3CircleDisplay _cCircleDisplay;

	private readonly OrientationSensor _orientationSensor;
	private readonly OrientationDisplay _orientationDisplay;

	private Matrix _calibrationMatrix = Matrix.Identity;

	private readonly Vector2CircleDisplay _aimDisplay;

	private readonly DebugView _debugView;

	#endregion

	private World World { get; set; }
	private Camera Camera { get; set; }
	private Matrix _projection;
	private Spaceship? _spaceShip;

	// Settings
	private DebugFlags _debugSettings = DebugFlags.Physics | DebugFlags.SensorData | DebugFlags.AimVector;

	// Public Interface Stuff
	public ContentBucket ContentBucket { get; }
	public int ScreenWidth { get; private set; }
	public int ScreenHeight { get; private set; }
	public RectangleF Bounds { get; private set; }
	public Vector2 AimVector => _tiltController.AimVector;


	// TODO PAUL: Only pass in what's needed.
	// Pass a func for onExit or something
	public GamePlayer(GameManager manager, ContentBucket contentBucket, int screenWidth, int screenHeight, Accelerometer accelerometer, Compass compass, OrientationSensor orientationSensor)
	{
		_gameManager = manager;

		_tiltController = new(orientationSensor);

		ContentBucket = contentBucket;
		ScreenWidth = screenWidth;
		ScreenHeight = screenHeight;

		World = new World(new Vector2(0, 0));
		Camera = new Camera(ScreenWidth, ScreenHeight, Constants.MetersPerPixel);

		Camera.SnapScale(1);

		_gameObjectCollection = new(World);

		_debugView = new DebugView(World)
		{
			Flags = DebugViewFlags.Shape | DebugViewFlags.ContactPoints | DebugViewFlags.ContactNormals
		};
		_debugView.LoadContent(manager.GraphicsDevice, manager.Content);

		AddGameplayObjects();

		#region Sensor debug stuff

		int unit = screenWidth / 30;

		_accelerometer = accelerometer;
		_aBarDisplay = new(new Rectangle(2 * unit, 2 * unit, 5 * unit, 1 * unit));
		_aCircleDisplay = new(position: new(6 * unit, 9 * unit), radius: 1 * unit);

		_compass = compass;
		_cBarDisplay = new(new Rectangle(10 * unit, 2 * unit, 5 * unit, 1 * unit));
		_cCircleDisplay = new(position: new(14 * unit, 9 * unit), radius: 1 * unit);

		_orientationSensor = orientationSensor;
		_orientationDisplay = new(position: new(18 * unit, 4.5f * unit), radius: 2 * unit);

		_aimDisplay = new(position: new(18 * unit, 9 * unit), radius: 1 * unit);

		#endregion
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
		ProcessInput();

		// Update all game objects
		_gameObjectCollection.Update(gameTime);

		// Update World
		World.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		Primitives.SetSpriteBatch(spriteBatch);

		#region World Space

		spriteBatch.Begin(
			transformMatrix: Camera.View
		);

		// Actual Game Objects
		foreach (var gameObject in _gameObjectCollection.GameObjects)
			gameObject.Draw(spriteBatch);

		// World Border
		Primitives.DrawRectangleOutline(Scale(Bounds, Constants.PixelsPerMeter), Color.Blue, 2.0f, 0);
		static Rectangle Scale(RectangleF rec, float scale) => new((int)(rec.X * scale), (int)(rec.Y * scale), (int)(rec.Width * scale), (int)(rec.Height * scale));

		WorldSpaceDebugDraw();

		spriteBatch.End();

		#endregion

		#region  Screen Space

		spriteBatch.Begin();

		ScreenSpaceDebugDraw();

		spriteBatch.End();

		#endregion

		#region Local Functions

		void WorldSpaceDebugDraw(float alpha = 1.0f)
		{
			if (_debugSettings.HasFlag(DebugFlags.Physics))
				_debugView.RenderDebugData(_projection, Camera.SimView, blendState: BlendState.Additive, alpha: alpha);
		}
		void ScreenSpaceDebugDraw()
		{
			if (_debugSettings.HasFlag(DebugFlags.SensorData))
			{
				_aBarDisplay.Draw(_accelerometer.CurrentValue.Acceleration, _aCalibrationVector);
				_aCircleDisplay.Draw(_accelerometer.CurrentValue.Acceleration, _aCalibrationVector);
				_cBarDisplay.Draw(_compass.CurrentValue.MagnetometerReading, _cCalibrationVector);
				_cCircleDisplay.Draw(_compass.CurrentValue.MagnetometerReading, _cCalibrationVector);
				_orientationDisplay.Draw(_orientationSensor.CurrentValue, _calibrationMatrix);
			}

			if (_debugSettings.HasFlag(DebugFlags.AimVector))
				_aimDisplay.Draw(_tiltController.AimVector);
		}

		#endregion
	}

	#region Game Objects

	public void AddGameObject(IGameObject gameObject) => _gameObjectCollection.Add(gameObject);
	public void RemoveGameObject(IGameObject gameObject) => _gameObjectCollection.Remove(gameObject);

	#endregion

	#region Private Functions

	private void ProcessInput()
	{
		if (InputManager.WasButtonPressed(Keys.Escape))
			_gameManager.Exit();

		if (InputManager.WasButtonPressed(Keys.F1))
			_debugSettings ^= DebugFlags.Physics;
		if (InputManager.WasButtonPressed(Keys.F2))
			_debugSettings ^= DebugFlags.SensorData;
		if (InputManager.WasButtonPressed(Keys.F3))
			_debugSettings ^= DebugFlags.AimVector;

		if (InputManager.WasButtonPressed(Keys.R))
			Reset();

		// Update Input
		_tiltController.Update();

		TouchCollection touchCollection = TouchPanel.GetState();

		if (touchCollection.Count > 0)
		{
			TouchLocation touch = touchCollection[0];
			if (touch.State == TouchLocationState.Pressed || touch.State == TouchLocationState.Moved)
			{
				// Top Left => Calibrate
				if (touch.Position.X < ScreenWidth / 2 && touch.Position.Y < ScreenHeight / 2)
				{
					Calibrate();
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
	}

	private void Reset()
	{
		_gameObjectCollection.Clear();
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

	private void Calibrate()
	{
		_tiltController.Calibrate();
		_aCalibrationVector = _accelerometer.CurrentValue.Acceleration;
		_cCalibrationVector = _compass.CurrentValue.MagnetometerReading;
		_calibrationMatrix = _orientationSensor.CurrentValue;
	}

	#endregion
}