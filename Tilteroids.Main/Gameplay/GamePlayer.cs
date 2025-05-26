using Tilteroids.Main.Data;
using Tilteroids.Main.Entities;

namespace Tilteroids.Main.Gameplay;

public class GamePlayer : IGameObjectHandler
{
	public ContentBucket ContentBucket { get; }
	public int ScreenWidth { get; }
	public int ScreenHeight { get; }

	public GamePlayer(ContentBucket contentBucket, int screenWidth, int screenHeight)
	{
		ContentBucket = contentBucket;
		ScreenWidth = screenWidth;
		ScreenHeight = screenHeight;
	}

	public void AddGameObject(IGameObject gameObject)
	{
		throw new System.NotImplementedException();
	}

	public void RemoveGameObject(IGameObject gameObject)
	{
		throw new System.NotImplementedException();
	}
}