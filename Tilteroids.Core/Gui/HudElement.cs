using SpaceshipArcade.MG.Engine.SpriteSheets;
using Tilteroids.Core.Data;

namespace Tilteroids.Core.Gui;

public class HudElement
{
	// Immutable params
	private readonly SpriteSheet _spriteSheet;
	private readonly string _identifier;

	// Mutable params
	public Vector2 Position { get; set; }


	public HudElement(SpriteSheet spriteSheet, string identifier, Vector2 position)
	{
		_spriteSheet = spriteSheet;
		_identifier = identifier;
		Position = position;
	}

	public void DrawFrame(SpriteBatch spriteBatch, bool active)
	{
		spriteBatch.Draw(
			texture: _spriteSheet.Texture,
			position: Position,
			sourceRectangle: _spriteSheet[active ? SpriteIdentifiers.FrameClosed : SpriteIdentifiers.FrameOpen],
			color: Color.White,
			rotation: 0,
			origin: Vector2.Zero,
			scale: 1,
			effects: SpriteEffects.None,
			layerDepth: 0.1f);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(
			texture: _spriteSheet.Texture,
			position: Position,
			sourceRectangle: _spriteSheet[_identifier],
			color: Color.White,
			rotation: 0,
			origin: Vector2.Zero,
			scale: 1,
			effects: SpriteEffects.None,
			layerDepth: 0.1f);
	}
}