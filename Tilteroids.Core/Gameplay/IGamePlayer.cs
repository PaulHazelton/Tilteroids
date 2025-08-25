using SpaceshipArcade.MG.Engine.Utilities;
using Tilteroids.Core.Data;
using Tilteroids.Core.Debugging;

namespace Tilteroids.Core.Gameplay;

public interface IGamePlayer
{
	DebugFlags DebugSettings { get; }

	ContentBucket ContentBucket { get; }

	GameState GameState { get; }

	RectangleF Bounds { get; }

	void AddGameObject(IGameObject gameObject);
	void RemoveGameObject(IGameObject gameObject);

	void LoseLife();
	void GameOver();
}