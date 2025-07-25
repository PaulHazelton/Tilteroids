using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Framework.Devices.Sensors;
using MonoGame.Framework.Utilities;
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

	private readonly DebugView _debugView;
	private readonly SensorDebugSuite _sensorDebugSuite;
	private readonly Vector2CircleDisplay _aimDisplay;

	public World World { get; private set; }
	private Camera Camera { get; set; }
	private Matrix _projection;
	private Spaceship? _spaceShip;

	// Settings
	private DebugFlags _debugSettings = DebugFlags.None;

	// Public Interface Stuff
	public ContentBucket ContentBucket { get; }
	public int ScreenWidth { get; private set; }
	public int ScreenHeight { get; private set; }
	public RectangleF Bounds { get; private set; }


	// TODO PAUL: Only pass in what's needed.
	// Pass a func for onExit or something
	public GamePlayer(GameManager manager, ContentBucket contentBucket, int screenWidth, int screenHeight, Accelerometer accelerometer, Compass compass, OrientationSensor orientationSensor)
	{
		_gameManager = manager;
		_tiltController = new(orientationSensor);
		ContentBucket = contentBucket;
		ScreenWidth = screenWidth;
		ScreenHeight = screenHeight;

		int unit = screenWidth / 30;
		_sensorDebugSuite = new(accelerometer, compass, orientationSensor, unit);
		_aimDisplay = new(position: new(18 * unit, 9 * unit), radius: 1 * unit);

		World = new World(Vector2.Zero);
		Camera = new Camera(ScreenWidth, ScreenHeight, Constants.MetersPerPixel);
		Camera.SnapScale(Constants.PixelsPerMeter);

		_gameObjectCollection = new(World);

		_debugView = new DebugView(World)
		{
			Flags = DebugViewFlags.Shape | DebugViewFlags.ContactPoints | DebugViewFlags.ContactNormals
		};
		_debugView.LoadContent(manager.GraphicsDevice, manager.Content);

		AddGameplayObjects();
	}

	public void UpdateSize(int screenWidth, int screenHeight)
	{
		ScreenWidth = screenWidth;
		ScreenHeight = screenHeight;

		Camera.UpdateScreenSize(ScreenWidth, ScreenHeight);

		float halfWidth = (ScreenWidth / 2);
		float halfHeight = (ScreenHeight / 2);
		_projection = Matrix.CreateOrthographicOffCenter(-halfWidth, halfWidth, halfHeight, -halfHeight, -1, 1);
	}

	public void Update(GameTime gameTime)
	{
		ProcessInput();

		// Update World
		World.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

		// Update all game objects
		_gameObjectCollection.Update(gameTime);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		Primitives.SetSpriteBatch(spriteBatch);

		#region World Space

		spriteBatch.Begin(
			transformMatrix: Camera.View
		);

		// Actual Game Objects
		if (!_debugSettings.HasFlag(DebugFlags.Physics))
		{
			foreach (var gameObject in _gameObjectCollection.GameObjects)
				gameObject.Draw(spriteBatch);
		}

		// World Border
		// Primitives.DrawRectangleOutline(Bounds, Color.Blue, 2.0f / Constants.PixelsPerMeter, 0);

		WorldSpaceDebugDraw();

		spriteBatch.End();

		#endregion

		#region Screen Space

		spriteBatch.Begin();

		ScreenSpaceDebugDraw();

		spriteBatch.End();

		#endregion

		#region Local Functions

		void WorldSpaceDebugDraw(float alpha = 1.0f)
		{
			if (_debugSettings.HasFlag(DebugFlags.Physics))
				_debugView.RenderDebugData(_projection, Camera.SimView, blendState: BlendState.Opaque, alpha: alpha);
		}
		void ScreenSpaceDebugDraw()
		{
			if (_debugSettings.HasFlag(DebugFlags.SensorData))
				_sensorDebugSuite.Draw(spriteBatch);

			if (_debugSettings.HasFlag(DebugFlags.AimVector))
				_aimDisplay.Draw(_tiltController.AimVector);
		}

		#endregion
	}

	public void AddGameObject(IGameObject gameObject) => _gameObjectCollection.Add(gameObject);
	public void RemoveGameObject(IGameObject gameObject) => _gameObjectCollection.Remove(gameObject);

	#region Private Functions

	private void ProcessInput()
	{
		switch (PlatformInfo.MonoGamePlatform)
		{
			case MonoGamePlatform.DesktopGL:
				KeyboardInput();
				MouseInput();
				break;

			case MonoGamePlatform.Android:
			case MonoGamePlatform.iOS:
				OrientationInput();
				TouchInput();
				break;
		}

		void KeyboardInput()
		{
			if (InputManager.WasButtonPressed(Keys.Escape))
				_gameManager.Exit();

			if (InputManager.WasButtonPressed(Keys.R))
				Reset();

			if (InputManager.WasButtonPressed(Keys.F1))
				_debugSettings ^= DebugFlags.Physics;
			if (InputManager.WasButtonPressed(Keys.F2))
				_debugSettings ^= DebugFlags.SensorData;
			if (InputManager.WasButtonPressed(Keys.F3))
				_debugSettings ^= DebugFlags.AimVector;
		}

		void MouseInput()
		{
			if (_spaceShip is null)
				return;

			var diff = Camera.GetMouseWorld() - _spaceShip.Body.WorldCenter;
			_spaceShip.Aim(diff);

			if (InputManager.IsButtonHeld(MouseButton.Right))
				_spaceShip.Thrust();
			if (InputManager.IsButtonHeld(MouseButton.Left))
				_spaceShip.Fire();
		}

		void OrientationInput()
		{
			_tiltController.Update();
			_spaceShip?.Aim(_tiltController.AimVector);
		}

		void TouchInput()
		{
			TouchCollection touchCollection = TouchPanel.GetState();

			if (touchCollection.Count > 0)
			{
				TouchLocation touch = touchCollection[0];
				if (touch.State == TouchLocationState.Pressed || touch.State == TouchLocationState.Moved)
				{
					// Top Left => Calibrate
					if (touch.Position.X < ScreenWidth / 2 && touch.Position.Y < ScreenHeight / 2)
						Calibrate();

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

		for (int i = 0; i < 5; i++)
		{
			int size = 3;//generator.NextInt(1, 4);

			var asteroid = new Asteroid(this,
				size: size,
				initialPosition: new Vector2(generator.NextSingle(-maxX, maxX), generator.NextSingle(-maxY, maxY)),
				initialRotation: generator.NextSingle() * MathHelper.TwoPi,
				initialVelocity: generator.NextVector(1f, 4f) * (4 - size),
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

		// World.Add(body);

		Bounds = new(v1, size);
	}

	private void Calibrate()
	{
		_tiltController.Calibrate();
		_sensorDebugSuite.Calibrate();
	}

	#endregion
}