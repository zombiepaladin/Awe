#pragma once

//////////////
// INCLUDES //
//////////////
#include <d3d11.h>
#include <d3dx10math.h>
#include <d3dx11async.h>
#include <fstream>
#include <vector>
#include "textureclass.h"

#include "DXUT.h"
using namespace std;

#pragma region Icosahedron vertices;
#define SCALE 0.5f
#define PHI SCALE*(1.6180f)
#define IV1 0.0f, SCALE, PHI
#define IV2 0.0f, -SCALE, -PHI
#define IV3 -SCALE, -PHI, 0.0f
#define IV4 0.0f, -SCALE, PHI
#define IV5 SCALE, -PHI, 0.0f
#define IV6 PHI, 0.0f, SCALE
#define IV7 PHI, 0.0f, -SCALE
#define IV8 SCALE, PHI, 0.0f
#define IV9 0.0f, SCALE, -PHI
#define IV10 -SCALE, PHI, 0.0f
#define IV11 -PHI, 0.0f, -SCALE
#define IV12 -PHI, 0.0f, SCALE
#pragma endregion

////////////////////////////////////////////////////////////////////////////////
// Class name: MeshInstance
////////////////////////////////////////////////////////////////////////////////
class MeshInstance
{
private:
	struct MatrixBufferType
	{
		D3DXMATRIX world;
		D3DXMATRIX view;
		D3DXMATRIX projection;
	};

#pragma region Texture Shader
public:
	MeshInstance();
	MeshInstance(const MeshInstance&);
	~MeshInstance();

	bool InitializeTextureShader(ID3D11Device*, HWND);
	void ShutdownTexturesShader();
	bool RenderTextureShader(ID3D11DeviceContext*, int, int, D3DXMATRIX, D3DXMATRIX, D3DXMATRIX, ID3D11ShaderResourceView*);

private:
	bool InitializeShader(ID3D11Device*, HWND, WCHAR*, WCHAR*);
	void ShutdownShader();
	void OutputShaderErrorMessage(ID3D10Blob*, HWND, WCHAR*);

	bool SetShaderParameters(ID3D11DeviceContext*, D3DXMATRIX, D3DXMATRIX, D3DXMATRIX, ID3D11ShaderResourceView*);
	void RenderShader(ID3D11DeviceContext*, int, int);

private:
	ID3D11VertexShader* m_vertexShader;
	ID3D11PixelShader* m_pixelShader;
	ID3D11InputLayout* m_layout;
	ID3D11Buffer* m_matrixBuffer;
	ID3D11SamplerState* m_sampleState;
#pragma endregion

#pragma region ModelClass
private:
	struct VertexType
	{
		D3DXVECTOR3 position;
	    D3DXVECTOR2 texture;
	};

	struct InstanceType
	{
		D3DXVECTOR3 position;
	};

public:
	bool InitializeMeshInstance(ID3D11Device*, WCHAR*);
	void ShutdownMeshInstance();
	void RenderMeshInstance(ID3D11DeviceContext*);

	int GetVertexCount();
	int GetInstanceCount();
	ID3D11ShaderResourceView* GetTexture();
	
private:
	bool InitializeBuffers(ID3D11Device*);
	void ShutdownBuffers();
	void RenderBuffers(ID3D11DeviceContext*);

	bool LoadTexture(ID3D11Device*, WCHAR*);
	void ReleaseTexture();

private:
	ID3D11Buffer* m_vertexBuffer;
	ID3D11Buffer* m_instanceBuffer;
	int m_vertexCount;
	int m_instanceCount;
	TextureClass* m_Texture;

#pragma endregion

public: 
	void SetPosition(int instanceId, float x, float y, float z);
	void Render(ID3D11DeviceContext* deviceContext,D3DXMATRIXA16& worldMatrix,
		D3DXMATRIXA16& viewMatrix, D3DXMATRIXA16& projectionMatrix);
	void Create(ID3D11Device* device, LPCTSTR szFileName);
	int AddInstance(float x, float y, float z);
	void Destroy();

	bool IsLoaded();

private:
	vector<D3DXVECTOR3*> positionList;
	void ReleasePositionList();

	bool InitializeVertexBuffer(ID3D11Device* device);
	bool InitializeInstanceBuffer(ID3D11Device* device);
	ID3D11Device* _device;
};

const float mesh[] = {IV5,IV3,IV2
	,IV5,IV4,IV3
	,IV6,IV4,IV5
	,IV1,IV4,IV6
	,IV7,IV5,IV2
	,IV6,IV5,IV7
	,IV8,IV6,IV7
	,IV1,IV6,IV8
	,IV9,IV7,IV2
	,IV8,IV7,IV9
	,IV10,IV8,IV9
	,IV1,IV8,IV10
	,IV11,IV9,IV2
	,IV10,IV9,IV11
	,IV12,IV10,IV11
	,IV1,IV10,IV12
	,IV3,IV11,IV2
	,IV12,IV11,IV3
	,IV4,IV12,IV3
	,IV1,IV12,IV4};
const float mesh1[] = {IV2,IV3,IV5,IV4,IV6,IV1,
	IV2,IV5,IV7,IV6,IV8,IV1,
	IV2,IV7,IV9,IV8,IV10,IV1,
	IV2,IV9,IV11,IV10,IV12,IV1,
	IV2,IV11,IV3,IV12,IV4,IV1};

const float cube[] = {-1.0f,-1.0f,-1.0f,
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