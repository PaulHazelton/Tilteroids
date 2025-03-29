using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceshipArcade.MG.Engine.Animations;

public class Animation
{
	// Loaded from files
	private AnimationAsset _asset;

	// State
	private float _elapsedTime;
	private readonly int _frameCount;
	private readonly float _duration;
	// Tools
	private Rectangle _sourceRectangle;

	// Properties
	public bool IsCompleted { get; private set; }
	public Action? OnCompleted { get; set; }
	public int Width { get; private set; }
	public int Height { get; private set; }

	// Constructor
	public Animation(string path, ContentManager contentManager, Func<string, AnimationSpec> fileLoader)
		: this(new AnimationAsset(path, contentManager, fileLoader)) { }
	public Animation(AnimationAsset asset)
	{
		_asset = asset;

		_frameCount = _asset.Texture.Width / _asset.Spec.FrameWidth;
		_duration = (float)_frameCount / asset.Spec.Fps;

		Width = _asset.Spec.FrameWidth;
		Height = _asset.Spec.FrameHeight;

		IsCompleted = false;

		if (asset.Spec.RandomTimeOffset == true)
		{
			var generator = new Random();
			_elapsedTime = MathHelper.Lerp(0, _duration, generator.NextSingle());
		}
		else
			_elapsedTime = 0;

		UpdateSourceRectangle();
	}

	public void Update(GameTime deltaTime)
	{
		// Update Time
		_elapsedTime += (float)deltaTime.ElapsedGameTime.TotalSeconds;
		if (_elapsedTime >= _duration)
		{
			OnCompleted?.Invoke();

			if (_asset.Spec.IsLooping)
				_elapsedTime -= _duration;
		}

		// Update Source Rectangle
		UpdateSourceRectangle();
	}

	public void Draw(
		SpriteBatch spriteBatch,
		Vector2 position,
		float rotation = 0,
		float scale = 1,
		SpriteEffects effects = SpriteEffects.None,
		float layerDepth = 1)
	{
		spriteBatch.Draw(
			texture: _asset.Texture,
			position: position,
			sourceRectangle: _sourceRectangle,
			color: Color.White,
			rotation: rotation,
			origin: _asset.Spec.Origin,
			scale: scale,
			effects: effects,
			layerDepth: layerDepth);
	}

	public void Reset()
	{
		_elapsedTime = 0;
		IsCompleted = false;
		UpdateSourceRectangle();
	}

	private void UpdateSourceRectangle()
	{
		int frame = MathHelper.Clamp((int)((_elapsedTime / _duration) * _frameCount), 0, _frameCount - 1);
		_sourceRectangle = new Rectangle(frame * _asset.Spec.FrameWidth, 0, _asset.Spec.FrameWidth, _asset.Spec.FrameHeight);
	}
}