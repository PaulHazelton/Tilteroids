namespace SpaceshipArcade.MG.Engine.Utilities;

public struct RectangleF
{
	public float X;
	public float Y;
	public float Width;
	public float Height;

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
