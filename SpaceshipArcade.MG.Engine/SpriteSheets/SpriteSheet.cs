namespace SpaceshipArcade.MG.Engine.SpriteSheets;

public class SpriteSheet
{
	public Texture2D Texture { get; private init; }
	public Dictionary<string, Rectangle> SourceRectangles { get; private init; }

	public SpriteSheet(Texture2D texture, SpriteSheetSpec spec)
	{
		Texture = texture;

		SourceRectangles = spec.SourceRectangles.ToDictionary(kvp => kvp.Key, kvp => ParseRectangle(kvp.Value, spec.Scale));
	}

	public Rectangle this[string key] => SourceRectangles[key];

	private static Rectangle ParseRectangle(string input, int scale)
	{
		string[] xywh = input.Split(',');

		return new(
			int.Parse(xywh[0]) * scale,
			int.Parse(xywh[1]) * scale,
			int.Parse(xywh[2]) * scale,
			int.Parse(xywh[3]) * scale
		);

		// int[] parsedInts = [..
		// 	input
		// 	.Split(',')
		// 	.Select(s => int.Parse(s.Trim()))
		// ];

		// return new(
		// 	parsedInts[0] * scale,
		// 	parsedInts[1] * scale,
		// 	parsedInts[2] * scale,
		// 	parsedInts[3] * scale
		// );
	}
}