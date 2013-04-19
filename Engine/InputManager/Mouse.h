#pragma once

#include "Windows.h"

namespace Engine {
namespace InputManager
{
	enum MouseButton
	{
		LeftClick = 0x01,
		RightClick = 0x02,
		MiddleClick = 0x04,
		X1Click = 0x05,
		X2Click = 0x06,
	};

	struct MouseState
	{
	public:
		MouseState(int, int, bool[]);
		int XPos;
		int YPos;
		bool IsPressed(MouseButton);
		MouseState* Clone();
	private:
		bool buttonStates[6];
	};

	class Mouse
	{
	public:
		Mouse(void);
		~Mouse(void);
		bool Init();
		MouseState* GetState();
	private:
		HHOOK mouseHook;
		HHOOK keyboardHook;
		int XPos;
		int YPos;
		bool buttonStates[6];
		LRESULT CALLBACK handleMouseMessage(int, WPARAM, LPARAM);
		LRESULT CALLBACK handleKeyboardMessage(int, WPARAM, LPARAM);
	};
}
}