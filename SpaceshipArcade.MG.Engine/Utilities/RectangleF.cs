namespace SpaceshipArcade.MG.Engine.Utilities;

public struct RectangleF
{
	public float X;
	public float Y;
	public float Width;
	public float Height;

	public Vector2 Location
	{
		readonly get => new(X, Y);
		set
		{
			X = value.X;
			Y = value.Y;
		}
	}

	public Vector2 Size
	{
		readonly get => new(Width, Height);
		set
		{
			Width = value.X;
			Height = value.Y;
		}
	}

	/// <summary>
	/// Returns the x coordinate of the left edge.
	/// </summary>
	public readonly float Left => X;

	/// <summary>
	/// Returns the x coordinate of the right edge.
	/// </summary>
	public readonly float Right => X + Width;

	/// <summary>
	/// Returns the y coordinate of the top edge.
	/// </summary>
	public readonly float Top => Y;

	/// <summary>
	/// Returns the y coordinate of the bottom edge.
	/// </summary>
	public readonly float Bottom => Y + Height;

	public RectangleF(float x, float y, float width, float height)
	{
		X = x;
		Y = y;
		Width = width;
		Height = height;
	}
	public RectangleF(Vector2 position, Vector2 size)
	{
		X = position.X;
		Y = position.Y;
		Width = size.X;
		Height = size.Y;
	}

	public readonly bool Contains(Vector2 point)
	{
		return
			X <= point.X && point.X < X + Width &&
			Y <= point.Y && point.Y < Y + Height;
	}
}
