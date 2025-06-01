using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tilteroids.Core.Data;

public class ContentBucket(ContentManager cm)
{
	public readonly Fonts_ Fonts = new(cm);
	public readonly Textures_ Textures = new(cm);

	public class Fonts_(ContentManager cm)
	{
		public readonly SpriteFont FallbackFont = cm.Load<SpriteFont>("Font");
		// public readonly SpriteFont DebugFont = cm.Load<SpriteFont>("Fonts/Debug-Font");
	}

	public class Textures_(ContentManager cm)
	{
		public readonly Texture2D Ship = cm.Load<Texture2D>("Textures/Ship");
	}
}