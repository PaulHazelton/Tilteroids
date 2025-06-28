namespace SpaceshipArcade.MG.Engine.Graphics;

public class Frames
{
	private readonly Rectangle[] _rectangles;

	public Frames(int frameWidth, int frameHeight, int count, bool horizontal = true)
	{
		_rectangles = new Rectangle[count];

		if (horizontal)
			for (int i = 0; i < count; i++)
				_rectangles[i] = new Rectangle(i * frameWidth, 0, frameWidth, frameHeight);
		else
			for (int i = 0; i < count; i++)
				_rectangles[i] = new Rectangle(0, i * frameHeight, frameWidth, frameHeight);
	}

	public Rectangle this[int index] => _rectangles[index];
}