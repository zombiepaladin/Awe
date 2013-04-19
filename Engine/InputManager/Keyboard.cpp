#include <stdlib.h>
#include <string.h>

#include "Windows.h"
#include "Keyboard.h"

namespace Engine {
namespace InputManager
{
#pragma region Keyboard

	Keyboard::Keyboard(void)
	{
		for(int i = 0; i < 254; i++)
			keyStates[i] = false;
	}

	Keyboard::~Keyboard(void)
	{
		UnhookWindowsHookEx(hookHandle);
	}

	bool Keyboard::Init()
	{
		//hookHandle = SetWindowsHookEx(WH_KEYBOARD, handleKeyboardMessage, NULL, 0);
		
		return hookHandle != NULL;
	}

	KeyBoardState* Keyboard::GetState()
	{
		return new KeyBoardState(keyStates);
	}

	LRESULT CALLBACK Keyboard::handleKeyboardMessage(int nCode, WPARAM wParam, LPARAM lParam)
	{
		if(nCode > 0 && wParam >= KEY_OFFSET)
		{
			bool transitionCode = (31 >> lParam) == 1;
			keyStates[wParam - KEY_OFFSET] = transitionCode;
		}

		return CallNextHookEx(hookHandle, nCode, wParam, lParam);
	}

#pragma endregion 

#pragma region KeyBoardState

	KeyBoardState::KeyBoardState(bool keyStates[])
	{
		memcpy(KeyBoardState::keyStates, keyStates, 254);
	}

	bool KeyBoardState::IsPressed(Keys key)
	{
		return keyStates[((int)key) - KEY_OFFSET];
	}

	KeyBoardState* KeyBoardState::Clone()
	{
		return new KeyBoardState(keyStates);
	}

#pragma endregion 
}
}