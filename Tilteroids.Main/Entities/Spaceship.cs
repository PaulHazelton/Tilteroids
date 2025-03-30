using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;
using Tilteroids.Main.Data;

namespace Tilteroids.Main.Entities;

public class Spaceship
{
	public Body Body { get; private init; }

	private readonly Texture2D shipTexture;

	private Vector2 origin;
	private float scale;

	public Spaceship(ContentBucket contentBucket, Vector2 startingPos)
	{
		shipTexture = contentBucket.Textures.Ship;
		origin = new Vector2(0.5f, 0.5f);

		scale = (float)Constants.PixelsPerMeter / shipTexture.Width;

		Body = new()
		{
			Position = startingPos,
			BodyType = BodyType.Dynamic,
		};

		Body.Add(CreateShipFixture());

		static Fixture CreateShipFixture()
		{
			var shipVertices = new Vertices([
				new(8, 1),
				new(15, 15),
				new(8, 12),
				new(1, 15)
			]);

			shipVertices.Scale(new(1/16f));

			PolygonShape shipShape = new(shipVertices, 1);

			return new Fixture(shipShape);
		}
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(
			texture: shipTexture,
			position: Body.Position * Constants.PixelsPerMeter,
			sourceRectangle: null,
			color: Color.White,
			rotation: Body.Rotation,
			origin: origin,
			scale: scale,
			effects: SpriteEffects.None,
			layerDepth: 0.1f);
	}
}