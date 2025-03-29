using System;
using Microsoft.Xna.Framework;

namespace Veedja.MG.Engine.Utilities
{
	public enum Direction
	{
		Right,
		Down,
		Left,
		Up
	}

	public static class DirectionExtensions
	{
		private const float _right = 0f;
		private const float _down = (float)(Math.PI * 0.5d);
		private const float _left = (float)(Math.PI * 1.0d);
		private const float _up = (float)(Math.PI * 1.5d);

		public static Direction Flip(this Direction d) => d switch
		{
			Direction.Right => Direction.Left,
			Direction.Down => Direction.Up,
			Direction.Left => Direction.Right,
			Direction.Up => Direction.Down,
			_ => throw new NotImplementedException()
		};

		public static Direction GetDirection(this Vector2 v)
		{
			float angle = (float)Math.Atan2(v.Y, v.X);
			float br = (float)( 1 * Math.PI / 4);
			float bl = (float)( 3 * Math.PI / 4);
			float tr = (float)(-1 * Math.PI / 4);
			float tl = (float)(-3 * Math.PI / 4);

			if (angle >= br && angle < bl)
				return Direction.Down;
			else if (angle <= tr && angle > tl)
				return Direction.Up;
			else if (angle <= tl || angle >= bl)
				return Direction.Left;
			else
				return Direction.Right;
		}

		public static float Angle(this Direction d) => d switch
		{
			Direction.Right => _right,
			Direction.Down => _down,
			Direction.Left => _left,
			Direction.Up => _up,
			_ => 0f
		};

		public static Vector2 Vector(this Direction d, float magnitude = 1.0f) => d switch
		{
			Direction.Right => new Vector2(magnitude, 0),
			Direction.Down => new Vector2(0, magnitude),
			Direction.Left => new Vector2(-magnitude, 0),
			Direction.Up => new Vector2(0, -magnitude),
			_ => throw new NotImplementedException("Direction." + d + " not implemented"),
		};

		public static Point Point(this Direction d, int magnitude = 1) => d switch
		{
			Direction.Right => new Point(magnitude, 0),
			Direction.Down => new Point(0, magnitude),
			Direction.Left => new Point(-magnitude, 0),
			Direction.Up => new Point(0, -magnitude),
			_ => throw new NotImplementedException("Direction." + d + " not implemented"),
		};

		public static bool IsHorizontal(this Direction d) => (int)d % 2 == 0;
		public static bool IsVertical(this Direction d) => (int)d % 2 == 1;

		/// <summary>
		/// <list>
		/// 	<item>0 = Right</item>
		/// 	<item>1 = Down</item>
		/// 	<item>2 = Left</item>
		/// 	<item>3 = Up</item>
		/// </list>
		/// </summary>
		public static Direction Get(int i)
		{
			i = i % 4;
			switch (i)
			{
				case 0: return Direction.Right;
				case 1: return Direction.Down;
				case 2: return Direction.Left;
				case 3: return Direction.Up;
				default: return Get(4 + i);
			}
		}

		public static Direction RotateCW(this Direction d) => d switch
		{
			Direction.Right => Direction.Down,
			Direction.Down => Direction.Left,
			Direction.Left => Direction.Up,
			Direction.Up => Direction.Right,
			_ => Direction.Right,
		};
		public static Direction RotateCCW(this Direction d) => d switch
		{
			Direction.Right => Direction.Up,
			Direction.Down => Direction.Right,
			Direction.Left => Direction.Down,
			Direction.Up => Direction.Left,
			_ => Direction.Right,
		};
	}
}