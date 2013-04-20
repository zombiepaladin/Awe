#pragma once

#include <string.h>

class MediaPlayer
{
public:
	MediaPlayer(void);
	~MediaPlayer(void);
	bool Init();
	void PlaySong(void);
	void PlaySound(void);
	void Pause(void);
	void Stop(void);
};

