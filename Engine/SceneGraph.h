#pragma once

#include "DXUT.h"
#include "App.h"
#include "DXUTcamera.h"
#include "SDKMesh.h"
#include "Texture2D.h"
#include "Shader.h"
#include "Buffer.h"
#include "MeshInstance.h"
#include <vector>
#include <memory>
extern class MeshInstance;
using namespace std;
class SceneGraph
{
public:
	SceneGraph();
	~SceneGraph();
	void Destroy();
	void Render(ID3D11DeviceContext* deviceContext,ID3D11Buffer* mPerFrameConstants, D3DXMATRIXA16& cameraView, D3DXMATRIXA16& cameraProj);
	bool IsLoaded();
	void ComputeInFrustumFlags(const D3DXMATRIXA16 &cameraViewProj);
	int Add(ID3D11Device* device, LPCTSTR szFileName,D3DXMATRIXA16& position);
	int Add(ID3D11Device* device, LPCTSTR szFileName);
	int Add(ID3D11Device* device, LPCTSTR szFileName, int x, int y, int z, float scale);
	int Add(ID3D11Device* device, LPCTSTR szFileName, int x, int y, int z, float xScale, float yScale, float zScale);
	void TranslateMesh(int id, D3DXMATRIXA16& TranslationMatrix);
	void SetMeshPosition(int id, D3DXMATRIXA16& newPositionMatrix);
	void SetMeshPosition(int id, int x,int y,int z);
	void StartScene(D3DXMATRIXA16& worldMatrix,float sceneScaling);

#pragma region MeshInstance Stuff
	int AddMeshInstance(ID3D11Device* device, LPCTSTR szFileName);
	int AddInstance(int meshId, int x, int y, int z, float xScale, float yScale, float zScale);
	int AddInstance(int meshId, int x, int y, int z, float scale);
	int AddInstance(int meshId, D3DXMATRIXA16& position);
	int AddInstance(int meshId);

	void SetInstancePosition(int meshId, int instanceId, int x,int y,int z);
	void SetInstancePosition(int meshId, int instanceId, D3DXMATRIXA16& newPositionMatrix);
#pragma endregion
private:
	vector<CDXUTSDKMesh*> meshList;
	vector<D3DXMATRIXA16*> positionList;
	vector<MeshInstance*> instanceList;
	float _sceneScaling;
	D3DXMATRIXA16 _worldMatrix;
};