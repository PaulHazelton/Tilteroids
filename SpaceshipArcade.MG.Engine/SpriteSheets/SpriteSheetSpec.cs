using System.Text.Json.Serialization;

namespace SpaceshipArcade.MG.Engine.SpriteSheets;

public class SpriteSheetSpec
{
	public int Scale { get; private init; }
	public Dictionary<string, string> SourceRectangles { get; private init; }

	[JsonConstructor]
	public SpriteSheetSpec(int scale, Dictionary<string, string> sourceRectangles)
	{
		Scale = scale;
		SourceRectangles = sourceRectangles;
	}
}