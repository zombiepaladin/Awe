#pragma once

class XnbTexture2dData {
public:
	uint32_t width;
	uint32_t height;
	uint32_t mipCount;
	vector<uint8_t> mip0;

	XnbTexture2dData();
	XnbTexture2dData(char*);
	void loadFromFile(char*);
};