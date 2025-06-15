using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceshipArcade.MG.Engine.Utilities;

namespace SpaceshipArcade.MG.Engine.Graphics;

public static class Primitives
{
	private const int CircleRadius = 256;
	private const int CircleThickness = 32;

	private static bool _isContentLoaded = false;

	// public static DrawMode DrawMode = DrawMode.RAW;
	private static SpriteBatch? _spriteBatch;
	private static Texture2D? _rectangleSprite;
	private static Texture2D? _circleSprite;
	private static Texture2D? _circleOutline;
	private static Vector2 _rectangleOrigin;
	private static Vector2 _circleOrigin;

	public static void LoadContent(GraphicsDevice graphicsDevice)
	{
		// Make a 1 x 1 pixel to draw rectangles and lines
		_rectangleSprite = new Texture2D(graphicsDevice, 1, 1);
		Color[] data = [Color.White];
		_rectangleSprite.SetData(data);
		_rectangleOrigin = new Vector2(_rectangleSprite.Width / 2f, _rectangleSprite.Height / 2f);

		// Make a default circle for drawing without a special sprite
		_circleSprite = CreateCircleTexture(graphicsDevice, CircleRadius, Color.White);
		_circleOutline = CreateCircleOutlineTexture(graphicsDevice, CircleRadius, CircleThickness, Color.White);
		_circleOrigin = new Vector2(_circleSprite.Width / 2f, _circleSprite.Height / 2f);

		_isContentLoaded = true;
	}

	public static void SetSpriteBatch(SpriteBatch spriteBatch)
	{
		_spriteBatch = spriteBatch;
	}

	public static Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int r, Color color, Color? middleLine = null)
	{
		// Variables we need
		var d = r * 2;  // diameter
		var rOuter2 = r * r; // circle radius squared
		float center = r - 0.5f; // Offset by half a pixel, don't remember why
		var texture = new Texture2D(graphicsDevice, d, d);
		var data = new Color[d * d];
		for (int x = 0; x < d; x++)
		{
			for (int y = 0; y < d; y++)
			{
				// Circle bit
				data[y * d + x] = (x - center) * (x - center) + (y - center) * (y - center) < rOuter2
					? color
					: Color.Transparent;

				// Line in the middle
				if (middleLine is not null && y == r && x >= r)
					data[y * d + x] = middleLine.Value;
			}
		}
		texture.SetData(data);
		return texture;
	}
	public static Texture2D CreateCircleOutlineTexture(GraphicsDevice graphicsDevice, int r, int thickness, Color color)
	{
		// Variables we need
		var d = r * 2;  // diameter
		var rOuter2 = r * r; // radius squared
		var rInner2 = (r - thickness) * (r - thickness);
		float center = r - 0.5f; // Offset by half a pixel, don't remember why
		var texture = new Texture2D(graphicsDevice, d, d);
		var data = new Color[d * d];
		for (int x = 0; x < d; x++)
		{
			for (int y = 0; y < d; y++)
			{
				// Circle bit
				float pixelR2 = (x - center) * (x - center) + (y - center) * (y - center);
				data[y * d + x] = pixelR2 < rOuter2 && pixelR2 >= rInner2
					? color
					: Color.Transparent;
			}
		}
		texture.SetData(data);
		return texture;
	}

	[MemberNotNull(nameof(_rectangleSprite), nameof(_circleSprite))]
	private static void AssertLoaded()
	{
		if (!_isContentLoaded || _rectangleSprite == null || _circleSprite == null)
			throw new InvalidOperationException("Draw function called before " + nameof(LoadContent));
		AssertSpriteBatch();
	}
	private static void AssertSpriteBatch()
	{
		_ = _spriteBatch ?? throw new InvalidOperationException("Draw function called before " + nameof(SetSpriteBatch));
	}

	#region Draw Functions
	public static void DrawRectangle(Vector2 position, Vector2 size, float angle, Color color, float layerDepth = 0)
	{
		AssertLoaded();

		// switch (DrawMode)
		// {
		// 	case DrawMode.RAW:
		_spriteBatch!.Draw(_rectangleSprite, position, null, color, angle, _rectangleOrigin, size, SpriteEffects.None, layerDepth);
		// break;
		// case DrawMode.CONVERT_TO_DISPLAY:
		// 	_spriteBatch!.Draw(_rectangleSprite, ConvertUnits.ToDisplayUnits(position), null, color, angle, _rectangleOrigin, ConvertUnits.ToDisplayUnits(size), SpriteEffects.None, layerDepth);
		// 	break;
		// }
	}

	public static void DrawRectangle(Rectangle rectangle, Color color)
	{
		AssertSpriteBatch();
		_spriteBatch!.Draw(_rectangleSprite, rectangle, null, color, 0, Vector2.Zero, SpriteEffects.None, 0);
	}
	public static void DrawRectangleWithOutline(Rectangle rectangle, Color fill, int strokeWidth, Color strokeColor)
	{
		if (strokeWidth < 1)
		{
			DrawRectangle(rectangle, fill);
			return;
		}
		DrawRectangle(rectangle, strokeColor);
		rectangle.X += strokeWidth;
		rectangle.Y += strokeWidth;
		rectangle.Width -= strokeWidth * 2;
		rectangle.Height -= strokeWidth * 2;
		DrawRectangle(rectangle, fill);
	}
	public static void DrawRectangleOutline(Rectangle rectangle, Color color, float thickness, float inset)
	{
		if (thickness == 0) return;
		// Adjust alpha for very thin lines
		if (thickness < 1)
		{
			color.A = (byte)MathHelper.Lerp(0, color.A, thickness);
			thickness = 1;
		}

		var tl = rectangle.Location.ToVector2();
		var tr = tl + new Vector2(rectangle.Width, 0);
		var bl = tl + new Vector2(0, rectangle.Height);
		var br = tl + rectangle.Size.ToVector2();

		// Inset
		tl += new Vector2(inset, inset);
		tr += new Vector2(-inset, inset);
		bl += new Vector2(inset, -inset);
		br += new Vector2(-inset, -inset);

		DrawLine(tl - new Vector2(thickness / 2f, 0), tr + new Vector2(thickness / 2f, 0), thickness, color);
		DrawLine(tr, br, thickness, color);
		DrawLine(br + new Vector2(thickness / 2f, 0), bl - new Vector2(thickness / 2f, 0), thickness, color);
		DrawLine(bl, tl, thickness, color);
	}

	public static void DrawCircle(Vector2 position, float radius, Color color, float layerDepth = 1.0f)
	{
		DrawCircle(position, radius, 0, color, layerDepth);
	}
	public static void DrawCircle(Vector2 position, float radius, float angle, Color color, float layerDepth = 1.0f)
	{
		AssertLoaded();

		float d = radius * 2f;    // Diameter

		// switch (DrawMode)
		// {
		// 	case DrawMode.RAW:
		_spriteBatch!.Draw(_circleSprite, position, null, color, angle, _circleOrigin, d / _circleSprite!.Width, SpriteEffects.None, layerDepth);
		// 		break;
		// 	case DrawMode.CONVERT_TO_DISPLAY:
		// 		d = ConvertUnits.ToDisplayUnits(d);
		// 		_spriteBatch!.Draw(_circleSprite, ConvertUnits.ToDisplayUnits(position), null, color, angle, _circleOrigin, d / _circleSprite!.Width, SpriteEffects.None, 1f);
		// 		break;
		// }
	}

	public static void DrawCircleOutline(Vector2 position, float radius, Color color, float layerDepth = 1.0f)
	{
		DrawCircleOutline(position, radius, 0, color, layerDepth);
	}
	public static void DrawCircleOutline(Vector2 position, float radius, float angle, Color color, float layerDepth = 1.0f)
	{
		AssertLoaded();

		float d = radius * 2f;    // Diameter

		_spriteBatch!.Draw(_circleOutline, position, null, color, angle, _circleOrigin, d / _circleSprite!.Width, SpriteEffects.None, layerDepth);
	}

	public static void DrawLine(Vector2 p1, Vector2 p2, float thickness, Color color, float layerDepth = 0)
	{
		Vector2 midpoint = PMath.Midpoint(p1, p2);
		Vector2 size = new((p1 - p2).Length(), thickness);
		float angle = (float)Math.Atan2((p2 - p1).Y, (p2 - p1).X);

		DrawRectangle(midpoint, size, angle, color, layerDepth);
	}
	public static void DrawHorizontalLine(Point position, int length, int thickness, Color color)
	{
		DrawRectangle(new Rectangle(position, new Point(length, thickness)), color);
	}
	public static void DrawVerticalLine(Point position, int length, int thickness, Color color)
	{
		DrawRectangle(new Rectangle(position, new Point(thickness, length)), color);
	}

	#endregion
}