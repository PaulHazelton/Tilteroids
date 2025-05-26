using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SpaceshipArcade.MG.Engine.Input;

public static class InputManager
{
	public static KeyboardState KeyState { get; private set; }
	public static GamePadState GamePadState { get; private set; }
	public static MouseState MouseState { get; private set; }

	public static KeyboardState OldKeyState { get; private set; }
	public static GamePadState OldGamePadState { get; private set; }
	public static MouseState OldMouseState { get; private set; }


	public static void BeginUpdate()
	{
		KeyState = Keyboard.GetState();
		MouseState = Mouse.GetState();
		GamePadState = GamePad.GetState(PlayerIndex.One);
	}

	public static void EndUpdate()
	{
		OldKeyState = KeyState;
		OldMouseState = MouseState;
		OldGamePadState = GamePadState;
	}

	public static bool IsButtonHeld(Keys key) => KeyState.IsKeyDown(key);
	public static bool WasButtonPressed(Keys key) => KeyState.IsKeyDown(key) && OldKeyState.IsKeyUp(key);
	public static bool WasButtonReleased(Keys key) => KeyState.IsKeyUp(key) && OldKeyState.IsKeyDown(key);

	public static bool IsButtonHeld(Buttons button) => GamePadState.IsButtonDown(button);
	public static bool WasButtonPressed(Buttons button) => GamePadState.IsButtonDown(button) && OldGamePadState.IsButtonUp(button);
	public static bool WasButtonReleased(Buttons button) => GamePadState.IsButtonUp(button) && OldGamePadState.IsButtonDown(button);

	public static bool IsButtonHeld(MouseButton button) => button switch
	{
		MouseButton.Left => MouseState.LeftButton == ButtonState.Pressed,
		MouseButton.Middle => MouseState.MiddleButton == ButtonState.Pressed,
		MouseButton.Right => MouseState.RightButton == ButtonState.Pressed,
		MouseButton.X1 => MouseState.XButton1 == ButtonState.Pressed,
		MouseButton.X2 => MouseState.XButton2 == ButtonState.Pressed,
		_ => throw new NotImplementedException($"Mouse Button {button} not implemented")
	};
	public static bool WasButtonPressed(MouseButton button) => button switch
	{
		MouseButton.Left => MouseState.LeftButton == ButtonState.Pressed && OldMouseState.LeftButton == ButtonState.Released,
		MouseButton.Middle => MouseState.MiddleButton == ButtonState.Pressed && OldMouseState.MiddleButton == ButtonState.Released,
		MouseButton.Right => MouseState.RightButton == ButtonState.Pressed && OldMouseState.RightButton == ButtonState.Released,
		MouseButton.X1 => MouseState.XButton1 == ButtonState.Pressed && OldMouseState.XButton1 == ButtonState.Released,
		MouseButton.X2 => MouseState.XButton2 == ButtonState.Pressed && OldMouseState.XButton2 == ButtonState.Released,
		_ => throw new NotImplementedException($"Mouse Button {button} not implemented")
	};
	public static bool WasButtonReleased(MouseButton button) => button switch
	{
		MouseButton.Left => MouseState.LeftButton == ButtonState.Released && OldMouseState.LeftButton == ButtonState.Pressed,
		MouseButton.Middle => MouseState.MiddleButton == ButtonState.Released && OldMouseState.MiddleButton == ButtonState.Pressed,
		MouseButton.Right => MouseState.RightButton == ButtonState.Released && OldMouseState.RightButton == ButtonState.Pressed,
		MouseButton.X1 => MouseState.XButton1 == ButtonState.Released && OldMouseState.XButton1 == ButtonState.Pressed,
		MouseButton.X2 => MouseState.XButton2 == ButtonState.Released && OldMouseState.XButton2 == ButtonState.Pressed,
		_ => throw new NotImplementedException($"Mouse Button {button} not implemented")
	};

	public static int ScrollDifference() => MouseState.ScrollWheelValue - OldMouseState.ScrollWheelValue;
	public static int HorizontalScrollDifference() => MouseState.HorizontalScrollWheelValue - OldMouseState.HorizontalScrollWheelValue;
}