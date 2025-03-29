using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Veedja.MG.Engine.Utilities
{
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
		private readonly AnchorCorner _anchorCorner;

		private Color _textColor;
		private Vector2 _position;

		/// <param name="position">The top left corner of the panel</param>
		public TextPanel(SpriteFont font, Vector2 position, AnchorCorner anchorCorner)
		{
			_font = font;
			_lines = new List<string>();

			_textColor = Color.White;
			_position = position;
			_anchorCorner = anchorCorner;
		}

		public void AddLine(string line) => _lines.Add(line);
		public void ClearLines() => _lines.Clear();

		public void Draw(SpriteBatch spriteBatch)
		{
			int i = 0;
			Vector2 linePosition = Vector2.Zero;

			foreach (string line in _lines)
			{
				switch (_anchorCorner)
				{
					case AnchorCorner.TopLeft:
						linePosition = _position + new Vector2(0, i * _font.LineSpacing);
						break;
					case AnchorCorner.TopRight:
						linePosition = _position + new Vector2(-_font.MeasureString(line).X, i * _font.LineSpacing);
						break;
					case AnchorCorner.BottomLeft:
						linePosition = _position + new Vector2(0, (-_lines.Count + i) * _font.LineSpacing);
						break;
					case AnchorCorner.BottomRight:
						linePosition = _position + new Vector2(-_font.MeasureString(line).X, (-_lines.Count + i) * _font.LineSpacing);
						break;
				}

				spriteBatch.DrawString(_font, line, linePosition, _textColor);
				i++;
			}
		}
	}
}