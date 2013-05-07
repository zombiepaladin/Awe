#pragma once

#include "stdafx.h"

struct XnbBone {
	string name;
	float transform[4][4];
};

//Bone hierarchy struct?

struct XnbMeshPart {
	int vertexOffset;
	int numVertices;
	int startIndex;
	int primitiveCount;
	int vertexBufferResourceNumber;
	int indexBufferResourceNumber;
	int effectResourceNumber;
};

struct XnbMesh {
	string name;
	int parentBoneNumber;
	int modelRootBoneNumber;
	float boundCenter[3];
	float boundRadius;
	int meshPartCount;
	vector<XnbMeshPart> meshParts;
};

struct XnbVertexDeclarationElement {
	int offset;
	string format;
	string usage;
	int usageIndex;
};

struct XnbVertexDeclaration {
	int stride;
	int elementCount;
	vector<XnbVertexDeclarationElement> elements;
};

struct XnbVertexBuffer {
	XnbVertexDeclaration vertexDeclaration;
	int vertexCount;
	int byteCount;
	vector<float> vertexData;
};

struct XnbIndexBuffer {
	bool is16Bit; //if false, it's 32 bit
	int byteCount;
	vector<uint32_t> indexData;
};

class XnbModelData {
public:
	vector<float> vertexData;
	int vertexDataSize;
	vector<uint32_t> indexData;
	string textureReference;

	XnbModelData();
	XnbModelData(char*);
	void loadFromFile(char*);
};