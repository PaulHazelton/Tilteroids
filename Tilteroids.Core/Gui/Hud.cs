using SpaceshipArcade.MG.Engine.SpriteSheets;
using Tilteroids.Core.Data;

namespace Tilteroids.Core.Gui;

public class Hud
{
	private readonly SpriteSheet _spriteSheet;

	// private readonly Vector2 _origin;
	private readonly float _scale;

	public Hud(ContentBucket contentBucket)
	{
		_spriteSheet = contentBucket.SpriteSheets.MainSpriteSheet;
		_scale = 1;
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(
			texture: _spriteSheet.Texture,
			position: Vector2.Zero,
			sourceRectangle: _spriteSheet[SpriteIdentifiers.SafeZone],
			color: Color.White,
			rotation: 0,
			origin: Vector2.Zero,
			scale: _scale,
			effects: SpriteEffects.None,
			layerDepth: 0.1f);
	}
}