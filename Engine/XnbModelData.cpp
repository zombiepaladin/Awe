#include "stdafx.h"
#include "XnbParser.h"
#include "ContentReader.h"
#include "XnbModelData.h"

void XnbModelData::loadFromFile(char* fileName) {
	XnbParser parser;
	ContentReader reader = parser.parse(fileName);

	//retrieve data from reader here
	this->vertexData = reader.modelVertexData;
	this->indexData = reader.modelIndexData;
	this->textureReference = reader.modelTextureReference;
	this->vertexDataSize = reader.modelVertexDataSize;
}

XnbModelData::XnbModelData() {

}

XnbModelData::XnbModelData(char* fileName) {
	this->loadFromFile(fileName);
}