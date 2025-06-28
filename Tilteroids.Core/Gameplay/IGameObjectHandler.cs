using SpaceshipArcade.MG.Engine.Utilities;
using Tilteroids.Core.Data;

namespace Tilteroids.Core.Gameplay;

public interface IGameObjectHandler
{
	ContentBucket ContentBucket { get; }
	int ScreenWidth { get; }
	int ScreenHeight { get; }
	RectangleF Bounds { get; }
	void AddGameObject(IGameObject gameObject);
	void RemoveGameObject(IGameObject gameObject);

	Vector2 AimVector { get; }
}