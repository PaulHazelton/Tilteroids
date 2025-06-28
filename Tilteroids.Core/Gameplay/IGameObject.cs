namespace Tilteroids.Core.Gameplay;

public interface IGameObject
{
	void Update(GameTime gameTime);
	void Draw(SpriteBatch spriteBatch);
}