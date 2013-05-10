#ifndef _MeshInstance_H_
#define _MeshInstance_H_

//////////////
// INCLUDES //
//////////////
#include<d3d11.h>
#include<D3DX10math.h>
#include <vector>
#include <memory>
#include "DXUT.h"
#include "App.h"
#include "DXUTcamera.h"
#include "SDKMesh.h"
#include "Texture2D.h"
#include "Shader.h"
#include "Buffer.h"
#include <vector>
#include <memory>
using namespace std;

////////////////////////////////////////////////////////////////////////////////
// Class name: ModelClass
////////////////////////////////////////////////////////////////////////////////
#pragma region Cube Vertices
const float cubeVertices[] = {-1.0f,-1.0f,-1.0f,
		-1.0f,-1.0f, 1.0f,
		-1.0f, 1.0f, 1.0f,
		1.0f, 1.0f,-1.0f,
		-1.0f,-1.0f,-1.0f,
		-1.0f, 1.0f,-1.0f,
		1.0f,-1.0f, 1.0f,
		-1.0f,-1.0f,-1.0f,
		1.0f,-1.0f,-1.0f,
		1.0f, 1.0f,-1.0f,
		1.0f,-1.0f,-1.0f,
		-1.0f,-1.0f,-1.0f,
		-1.0f,-1.0f,-1.0f,
		-1.0f, 1.0f, 1.0f,
		-1.0f, 1.0f,-1.0f,
		1.0f,-1.0f, 1.0f,
		-1.0f,-1.0f, 1.0f,
		-1.0f,-1.0f,-1.0f,
		-1.0f, 1.0f, 1.0f,
		-1.0f,-1.0f, 1.0f,
		1.0f,-1.0f, 1.0f,
		1.0f, 1.0f, 1.0f,
		1.0f,-1.0f,-1.0f,
		1.0f, 1.0f,-1.0f,
		1.0f,-1.0f,-1.0f,
		1.0f, 1.0f, 1.0f,
		1.0f,-1.0f, 1.0f,
		1.0f, 1.0f, 1.0f,
		1.0f, 1.0f,-1.0f,
		-1.0f, 1.0f,-1.0f,
		1.0f, 1.0f, 1.0f,
		-1.0f, 1.0f,-1.0f,
		-1.0f, 1.0f, 1.0f,
		1.0f, 1.0f, 1.0f,
		-1.0f, 1.0f, 1.0f,
		1.0f,-1.0f, 1.0f};
#pragma endregion

class MeshInstance
{

public:
	MeshInstance();

	MeshInstance(const MeshInstance&);
	~MeshInstance();

	bool SetBuffers(ID3D11Device* device);
	void Shutdown();
	void Render(ID3D11DeviceContext* deviceContext);
	int GetInstanceCount();
	int GetVertexCount();
	int AddInstance(D3DXMATRIXA16& in);
	void SetPosition(int id, D3DXMATRIXA16& pos);
	bool IsLoaded();

#pragma region Needed for Interface
	void Create(ID3D11Device* device, LPCTSTR szFileName);
	void Destroy();
	void ComputeInFrustumFlags(const D3DXMATRIXA16 &worldViewProj, bool cullNear = true);
#pragma endregion

private:
	bool InitializeBuffers(ID3D11Device* device);
	void ShutdownBuffers();
	void RenderBuffers(ID3D11DeviceContext* deviceContext);
	vector<D3DXMATRIXA16*> positionList;
	ID3D11Buffer* m_instanceBuffer;
	ID3D11Buffer* m_vertexBuffer;

	bool InitializeVertexBuffer(ID3D11Device* device, float const vetices[]);
	bool InitializeIndexBuffer(ID3D11Device* device);

	int m_instanceCount;
	int m_vertexCount;

	struct VertexType
	{
		D3DXMATRIXA16 position;
		//D3DXVECTOR2 texture;
	};

	struct InstanceType
	{
		D3DXMATRIXA16 position;
	};
	#pragma region CubeVertices
	
#pragma endregion
};

#endif