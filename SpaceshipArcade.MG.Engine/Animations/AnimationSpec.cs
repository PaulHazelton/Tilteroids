namespace SpaceshipArcade.MG.Engine.Animations;

public class AnimationSpec
{
	/// <summary>
	/// Duration of the animation in seconds
	/// </summary>
	public int Fps { get; set; }
	public bool IsLooping { get; set; }
	public int FrameWidth { get; set; }
	public int FrameHeight { get; set; }
	public Vector2 Origin { get; set; }
	public bool? RandomTimeOffset { get; set; }
}