#pragma once

#include <list>
#include "DXUT.h"
#include "DXUTcamera.h"
#include "SDKMesh.h"
#include "Texture2D.h"
#include "Shader.h"
#include "Buffer.h"
#include <vector>
#include <memory>

using namespace std;
class SceneGraph
{
public:
	SceneGraph();
	~SceneGraph();
	void Destroy();
	void Render(ID3D11DeviceContext* deviceContext);
	bool IsLoaded();
	void ComputeInFrustumFlags(const D3DXMATRIXA16 &worldViewProj);
	void Add(ID3D11Device* device, LPCTSTR szFileName);
private:
	list<CDXUTSDKMesh> List;
	float sceneScale;
	D3DXMATRIXA16 worldMatrix;
};