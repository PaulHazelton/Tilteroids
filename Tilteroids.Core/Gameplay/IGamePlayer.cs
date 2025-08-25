using nkast.Aether.Physics2D.Dynamics;
using SpaceshipArcade.MG.Engine.Utilities;
using Tilteroids.Core.Data;
using Tilteroids.Core.Debugging;

namespace Tilteroids.Core.Gameplay;

public interface IGamePlayer
{
	GameState GameState { get; }
	DebugFlags DebugSettings { get; }
	ContentBucket ContentBucket { get; }
	int ScreenWidth { get; }
	int ScreenHeight { get; }
	RectangleF Bounds { get; }
	void AddGameObject(IGameObject gameObject);
	void RemoveGameObject(IGameObject gameObject);

	World World { get; }
}