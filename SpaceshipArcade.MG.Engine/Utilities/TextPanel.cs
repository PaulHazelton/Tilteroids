namespace SpaceshipArcade.MG.Engine.Utilities;

/// <summary>
/// Holds a list of strings to draw to the screen
/// Useful for debugging
/// </summary>
public class TextPanel
{
	public enum AnchorCorner
	{
		TopLeft, TopRight, BottomLeft, BottomRight
	}

	private readonly SpriteFont _font;
	private readonly List<string> _lines;

	// Required Parameters
	public Vector2 Position { get; set; }

	// Optional Parameters
	public AnchorCorner Anchor { get; set; } = AnchorCorner.TopLeft;
	public Color TextColor { get; set; } = Color.White;
	public float Scale { get; set; } = 1.0f;
	public float LayerDepth { get; set; } = 1.0f;

	/// <param name="position">The top left corner of the panel</param>
	public TextPanel(SpriteFont font, Vector2 position)
	{
		_font = font;
		_lines = [];

		Position = position;
	}

	public void AddLine(string line) => _lines.Add(line);
	public void ClearLines() => _lines.Clear();

	public void Draw(SpriteBatch spriteBatch)
	{
		int i = 0;
		Vector2 linePosition = Vector2.Zero;

		foreach (string line in _lines)
		{
			switch (Anchor)
			{
				case AnchorCorner.TopLeft:
					linePosition = Position + new Vector2(0, i * _font.LineSpacing * Scale);
					break;
				case AnchorCorner.TopRight:
					linePosition = Position + new Vector2(-_font.MeasureString(line).X, i * _font.LineSpacing * Scale);
					break;
				case AnchorCorner.BottomLeft:
					linePosition = Position + new Vector2(0, (-_lines.Count + i) * _font.LineSpacing * Scale);
					break;
				case AnchorCorner.BottomRight:
					linePosition = Position + new Vector2(-_font.MeasureString(line).X, (-_lines.Count + i) * _font.LineSpacing * Scale);
					break;
			}

			spriteBatch.DrawString(
				spriteFont: _font,
				text: line,
				position: linePosition,
				color: TextColor,
				rotation: 0,
				origin: default,
				scale: Scale,
				effects: default,
				layerDepth: LayerDepth);

			i++;
		}
	}
}