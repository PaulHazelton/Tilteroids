using nkast.Aether.Physics2D.Dynamics;

namespace Tilteroids.Core.Gameplay;

public class GameObjectCollection
{
	public List<IGameObject> GameObjects { get; private set; } = [];
	
	private readonly List<IGameObject> _gameObjectsToAdd = [];
	private readonly List<IGameObject> _gameObjectsToRemove = [];

	private readonly World _world;

	public GameObjectCollection(World world)
	{
		_world = world;
	}

	public void Add(IGameObject gameObject) => _gameObjectsToAdd.Add(gameObject);
	public void Remove(IGameObject gameObject) => _gameObjectsToRemove.Add(gameObject);

	public void Update(GameTime gameTime)
	{
		// Handle queued objects
		foreach (var gameObject in _gameObjectsToAdd)
			HandleAdd(gameObject);
		foreach (var gameObject in _gameObjectsToRemove)
			HandleRemove(gameObject);

		_gameObjectsToAdd.Clear();
		_gameObjectsToRemove.Clear();

		// Update objects
		foreach (IGameObject gameObject in GameObjects)
			gameObject.Update(gameTime);
	}
	public void Clear()
	{
		_gameObjectsToAdd.Clear();
		_gameObjectsToRemove.Clear();
		GameObjects.Clear();
		_world.Clear();
	}

	private void HandleAdd(IGameObject gameObject)
	{
		GameObjects.Add(gameObject);

		if (gameObject is IPhysicsObject po)
			_world.Add(po.Body);
	}
	private void HandleRemove(IGameObject gameObject)
	{
		if (GameObjects.Remove(gameObject))
		{
			if (gameObject is IPhysicsObject po && _world.BodyList.Contains(po.Body))
				_world.RemoveAsync(po.Body);
		}
	}
}