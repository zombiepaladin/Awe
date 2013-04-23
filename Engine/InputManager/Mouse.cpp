
#include "Windows.h"
#include "Mouse.h"

namespace Engine {
namespace InputManager
{
#pragma region Mouse

	Mouse::Mouse(void)
	{
		for(int i = 0; i < 4; i++)
			buttonStates[i] = false;
		XPos = 0;
		YPos = 0;
	}

	Mouse::~Mouse(void)
	{
		UnhookWindowsHookEx(mouseHook);
		UnhookWindowsHookEx(keyboardHook);
	}

	bool Mouse::Init()
	{
		//mouseHook = SetWindowsHookEx(WH_MOUSE, handleMouseMessage, NULL, 0);
		//keyboardHook = SetWindowsHookEx(WH_KEYBOARD, handleKeyboardMessage, NULL, 0);

		return (mouseHook != NULL) && (keyboardHook != NULL);
	}

	MouseState* Mouse::GetState()
	{
		return new MouseState(XPos, YPos, buttonStates);
	}

	LRESULT CALLBACK Mouse::handleMouseMessage(int nCode, WPARAM wParam, LPARAM lParam)
	{
		if(nCode > 0)
		{
			POINT point = ((MOUSEHOOKSTRUCT*)lParam)->pt;
			XPos = point.x;
			YPos = point.y;
		}

		return CallNextHookEx(mouseHook, nCode, wParam, lParam);
	}

	LRESULT CALLBACK Mouse::handleKeyboardMessage(int nCode, WPARAM wParam, LPARAM lParam)
	{
		if(nCode > 0 && wParam <= 6)
		{
			bool transitionCode = (31 >> lParam) == 1;
			buttonStates[wParam] = transitionCode;
		}

		return CallNextHookEx(keyboardHook, nCode, wParam, lParam);
	}

#pragma endregion

#pragma region MouseState

	MouseState::MouseState(int x, int y, bool buttonStates[])
	{
		XPos = x;
		YPos = y;
		memcpy(MouseState::buttonStates, buttonStates, 6);
	}

	bool MouseState::IsPressed(MouseButton button)
	{
		return buttonStates[(int)button];
	}

	MouseState* MouseState::Clone()
	{
		return new MouseState(XPos, YPos, buttonStates);
	}

#pragma endregion
}
}