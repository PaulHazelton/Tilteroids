using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using SpaceshipArcade.MG.Engine.SpriteSheets;
using SpaceshipArcade.MG.Engine.Utilities;

namespace Tilteroids.Core.Data;

public class ContentBucket(ContentManager cm)
{
	public readonly Fonts_ Fonts = new(cm);
	public readonly Textures_ Textures = new(cm);
	public readonly SoundEffects_ SoundEffects = new(cm);
	public readonly SpriteSheets_ SpriteSheets = new(cm);

	public class Fonts_(ContentManager cm)
	{
		public readonly SpriteFont FallbackFont = cm.Load<SpriteFont>("Font");
		// public readonly SpriteFont DebugFont = cm.Load<SpriteFont>("Fonts/Debug-Font");
	}

	public class Textures_(ContentManager cm)
	{
		public readonly Texture2D Ship = cm.Load<Texture2D>("Textures/Ship");
		// public readonly Texture2D Atlas = cm.Load<Texture2D>("sprite-atlas-1");
	}

	public class SoundEffects_(ContentManager cm)
	{
		public readonly SoundEffect Gun = cm.Load<SoundEffect>("SoundEffects/Gun/Recoil-Gun");
	}

	public class SpriteSheets_
	{
		public readonly SpriteSheet MainSpriteSheet;

		public SpriteSheets_(ContentManager cm)
		{
			// Local function aliases
			Texture2D Texture(string name) => cm.Load<Texture2D>(name);
			SpriteSheetSpec Spec(string name) => FileManager.LoadJson<SpriteSheetSpec>(RootPath.Content, name);
			
			MainSpriteSheet = new(Texture("Textures/sprite-atlas-1"), Spec("Textures/sprite-atlas-1.json"));
		}

	}
}