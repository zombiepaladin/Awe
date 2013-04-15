#include "stdafx.h"
#include "XnbParser.h"
#include "ContentReader.h"
#include "XnbTexture2dData.h"

void XnbTexture2dData::loadFromFile(char* fileName) {
	XnbParser parser;
	ContentReader reader = parser.parse(fileName);

	//retrieve data from reader here
	this->height = reader.texture2dHeight;
	this->width = reader.texture2dWidth;
	this->mipCount = reader.texture2dMipCount;
	this->mip0 = reader.texture2dMip0;
}

XnbTexture2dData::XnbTexture2dData() {

}

XnbTexture2dData::XnbTexture2dData(char* fileName) {
	this->loadFromFile(fileName);
}