#include "SceneGraph.h"
using std::tr1::shared_ptr;

// NOTE: Must match layout of shader constant buffers
using namespace std;
__declspec(align(16))

struct PerFrameConstants
{
    D3DXMATRIX mCameraWorldViewProj;
    D3DXMATRIX mCameraWorldView;
    D3DXMATRIX mCameraViewProj;
    D3DXMATRIX mCameraProj;
    D3DXVECTOR4 mCameraNearFar;

    unsigned int mFramebufferDimensionsX;
    unsigned int mFramebufferDimensionsY;
    unsigned int mFramebufferDimensionsZ;
    unsigned int mFramebufferDimensionsW;

    UIConstants mUI;
};

SceneGraph::SceneGraph()
{
}

SceneGraph::~SceneGraph()
{
}

bool SceneGraph::IsLoaded()
{
	if(!instanceList.empty())
	{
		return true;
	}

	if(meshList.empty())
	{
		return false;
	}

	for(int i=0;i<meshList.size();i++)
	{
		if(!meshList[i]->IsLoaded())
		{
			return false;
		}
	}
	return true;
}

void SceneGraph::StartScene(D3DXMATRIXA16& worldMatrix, float sceneScaling)
{
	_worldMatrix = worldMatrix;
	_sceneScaling=sceneScaling;
}

int SceneGraph::Add(ID3D11Device* device, LPCTSTR szFileName, int x, int y, int z, float xScale, float yScale, float zScale)
{
	D3DXMATRIXA16 t,s, final;
	D3DXMatrixScaling(&s,xScale,yScale,zScale);
	D3DXMatrixTranslation(&t,x,y,z);
	final=s*t;
	return Add(device,szFileName,final);
}

int SceneGraph::Add(ID3D11Device* device, LPCTSTR szFileName, int x, int y, int z, float scale)
{
	return Add(device,szFileName,x,y,z,scale,scale,scale);
}

int SceneGraph::Add(ID3D11Device* device, LPCTSTR szFileName)
{
	D3DXMATRIXA16 t;
	D3DXMatrixTranslation(&t,0,0,0);
	return Add(device,szFileName,t);
}

int SceneGraph::Add(ID3D11Device* device, LPCTSTR szFileName, D3DXMATRIXA16& position)
{
	CDXUTSDKMesh* newMesh = new CDXUTSDKMesh();
	D3DXMATRIXA16* newPosition = new D3DXMATRIXA16((_worldMatrix*position));
	newMesh->Create(device, szFileName);
	meshList.push_back(newMesh);
	unsigned int x = meshList.size();
	positionList.push_back(newPosition);
	return meshList.size();
}

void SceneGraph::TranslateMesh(int id, D3DXMATRIXA16& translationMatrix)
{
	if(id>=positionList.size())
	{
		invalid_argument ia("In: TranslateMesh(int id,D3DXMATRIXA16& translationMatrix): ID is not in the SceneGraph");
		throw ia;
	}
	D3DXMATRIXA16 target = *positionList[id];
	target = translationMatrix * target;
	positionList[id]= &target;
}

void SceneGraph::SetMeshPosition(int instanceId, int id, int x,int y,int z)
{
	if(instanceId>0)
	{
		SetInstancePosition(instanceId, id,x,y,z);
	}
	else
	{
		D3DXMATRIXA16 trans,target;
		D3DXMatrixTranslation(&trans,x,y,z);
		target = _worldMatrix*trans;
		SetMeshPosition(id,target);
	}
}

void SceneGraph::SetMeshPosition(int id, D3DXMATRIXA16& newPositionMatrix)
{
	if(id>positionList.size())
	{
		invalid_argument ia("In: SetMeshPosition(int id,D3DXMATRIXA16& translationMatrix): ID is not in the SceneGraph");
		throw ia;
	}
	positionList[id - 1]= new D3DXMATRIXA16(newPositionMatrix);
}

void SceneGraph::ComputeInFrustumFlags(const D3DXMATRIXA16 &cameraViewProj)
{
	for(int i =0;i<meshList.size();i++)
	{
		meshList[i]->ComputeInFrustumFlags((*positionList[i])*cameraViewProj);
	}
}

void SceneGraph::Render(ID3D11DeviceContext* deviceContext,ID3D11Buffer* mPerFrameConstants,D3DXMATRIXA16& cameraView, D3DXMATRIXA16& cameraProj)
{
	D3D11_MAPPED_SUBRESOURCE mappedResource;
	D3DXMATRIXA16 cameraViewProj = cameraView * cameraProj;
	D3DXMATRIXA16 origin; 
	D3DXMatrixTranslation(&origin,0,0,0);
	PerFrameConstants *constants;

	for(int i=0;i<meshList.size();i++)
	{
		deviceContext->Map(mPerFrameConstants, 0, D3D11_MAP_WRITE_DISCARD, 0, &mappedResource);
		constants = static_cast<PerFrameConstants *>(mappedResource.pData);
		constants->mCameraWorldViewProj=(*positionList[i])*cameraViewProj;
		constants->mCameraWorldView=(*positionList[i])*cameraView;
		deviceContext->Unmap(mPerFrameConstants, 0);
		meshList[i]->ComputeInFrustumFlags((*positionList[i])*cameraViewProj,0);
		meshList[i]->Render(deviceContext,0);
	}

	for(int i=0; i<instanceList.size();i++)
	{
		instanceList[i]->Render(deviceContext,_worldMatrix,cameraView,cameraProj);
	}
}

void SceneGraph::Destroy()
{
	if(!meshList.empty())
	{
		for(int i=meshList.size()-1; i>=0; i--)
		{
			SAFE_DELETE(meshList[i]);
		}
		meshList.clear();
	}
	if(!positionList.empty())
	{
		for(int i=positionList.size()-1; i>=0; i--)
		{
			SAFE_DELETE(positionList[i]);
		}
		positionList.clear();
	}

	if(!instanceList.empty())
	{
		for(int i=instanceList.size()-1; i>=0; i--)
		{
			SAFE_DELETE(instanceList[i]);
		}
		instanceList.clear();
	}
}

#pragma region MeshInstance Stuff
int SceneGraph::AddMeshInstance(ID3D11Device* device, LPCTSTR szFileName)
{
	MeshInstance* newInstance = new MeshInstance();
	newInstance->Create(device, szFileName);
	instanceList.push_back(newInstance);
	return instanceList.size();
}

int SceneGraph::AddInstance(int meshId, int x, int y, int z, float xScale, float yScale, float zScale)
{
	return -instanceList[meshId-1]->AddInstance(x,y,z);
}

int SceneGraph::AddInstance(int meshId,int x, int y, int z, float scale)
{
	return AddInstance(meshId, x,y,z,scale,scale,scale);
}

int SceneGraph::AddInstance(int meshId)
{
	return AddInstance(meshId, 0,0,0,1,1,1);
}



void SceneGraph::SetInstancePosition(int meshId, int instanceId, int x,int y,int z)
{
	D3DXMATRIXA16 t;
	D3DXMatrixTranslation(&t,x,y,z);
	instanceList[meshId-1]->SetPosition(-instanceId,x,y,z);
}


#pragma endregion
