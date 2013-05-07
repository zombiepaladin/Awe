////////////////////////////////////////////////////////////////////////////////
// Filename: modelclass.h
////////////////////////////////////////////////////////////////////////////////
//#pragma once


#ifndef _MODELCLASS_H_
#define _MODELCLASS_H_


//////////////
// INCLUDES //
//////////////
#include "DXUT.h"
#include "DXUTcamera.h"
#include "SDKMesh.h"

#include <d3d11.h>
#include <d3dx10math.h>
#include <fstream>




///////////////////////
// MY CLASS INCLUDES //
///////////////////////


#include "textureclass.h"
#include "stdafx.h"

using namespace std;
////////////////////////////////////////////////////////////////////////////////
// Class name: ModelClass
////////////////////////////////////////////////////////////////////////////////
class ModelClass : public CDXUTSDKMesh
{
private:
	struct VertexType
	{
		D3DXVECTOR3 position;
	    D3DXVECTOR2 texture;
		D3DXVECTOR3 normal;
	};

	struct ModelType
	{
		float x, y, z;
		float tu, tv;
		float nx, ny, nz;
	};

public:
	ModelClass();
	ModelClass(const ModelClass&);
	~ModelClass();

	bool Initialize(ID3D11Device*, char*, WCHAR*);
	void Shutdown();
	//void Render(ID3D11DeviceContext*);
	void Render(ID3D11DeviceContext*, UINT iDiffuseSlot,
                                            UINT iNormalSlot,
                                            UINT iSpecularSlot);

	//TODO implement
	HRESULT Create( ID3D11Device* pDev11, LPCTSTR szFileName, bool bCreateAdjacencyIndices=
                                            false, SDKMESH_CALLBACKS11* pLoaderCallbacks=NULL );

	void ComputeInFrustumFlags(const D3DXMATRIXA16 &worldViewProj,
                               bool cullNear = true);

	bool IsLoaded();

	int GetIndexCount();
	ID3D11ShaderResourceView* GetTexture();


private:
	bool InitializeBuffers(ID3D11Device*);
	void ShutdownBuffers();
	void RenderBuffers(ID3D11DeviceContext*);

	bool LoadTexture(ID3D11Device*, WCHAR*);
	void ReleaseTexture();

	bool LoadModel(char*);
	void ReleaseModel();

private:
	ID3D11Buffer *m_vertexBuffer, *m_indexBuffer;
	int m_vertexCount, m_indexCount;
	TextureClass* m_Texture;
	ModelType* m_model;
	vector<uint32_t> m_indices;
	string m_textureReference;
	bool isLoaded;
	//PixelShader* mGBufferPS;
};

#endif