using Tilteroids.Main.Data;
using Tilteroids.Main.Entities;

namespace Tilteroids.Main.Gameplay;

public interface IGameObjectHandler
{
	ContentBucket ContentBucket { get; }
	int ScreenWidth { get; }
	int ScreenHeight { get; }
	void AddGameObject(IGameObject gameObject);
	void RemoveGameObject(IGameObject gameObject);
}