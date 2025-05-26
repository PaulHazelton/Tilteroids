using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tilteroids.Main.Gameplay;

public interface IGameObject
{
	void Update(GameTime gameTime);
	void Draw(SpriteBatch spriteBatch);
}