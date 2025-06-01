using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tilteroids.Core.Gameplay;

public interface IGameObject
{
	void Update(GameTime gameTime);
	void Draw(SpriteBatch spriteBatch);
}