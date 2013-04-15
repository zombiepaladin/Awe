#pragma once

class XnbModelData {
public:
	vector<uint8_t> vertexData;

	XnbModelData();
	XnbModelData(char*);
	void loadFromFile(char*);
};