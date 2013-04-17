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
	
	vector<CDXUTSDKMesh*>::iterator i;
	for(i=meshList.begin();i!=meshList.end();i++)
	{
		if(!(*i)->IsLoaded())
		{
			return false;
		}
	}
	return true;
}

void SceneGraph::StartScene(D3DXMATRIXA16& worldMatrix, float sceneScaling)
{
	if(!meshList.empty())
	{
		EmptyList(meshList);
	}

	if(!positionList.empty())
	{
		EmptyList(positionList);
	}

	_worldMatrix = worldMatrix;
	_sceneScaling=sceneScaling;
}

void SceneGraph::Add(ID3D11Device* device, LPCTSTR szFileName)
{
	Add(device,szFileName,_worldMatrix);
}

void SceneGraph::Add(ID3D11Device* device, LPCTSTR szFileName, D3DXMATRIXA16& position)
{
	CDXUTSDKMesh newMesh;
	newMesh.Create(device, szFileName);
	meshList.push_back(&newMesh);
	positionList.push_back(&position);
	//maybe return size to use as an ID
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

void SceneGraph::SetMeshPosition(int id, D3DXMATRIXA16& newPositionMatrix)
{
	if(id>=positionList.size())
	{
		invalid_argument ia("In: SetMeshPosition(int id,D3DXMATRIXA16& translationMatrix): ID is not in the SceneGraph");
		throw ia;
	}
	positionList[id]=&newPositionMatrix;
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
	for(int i=0;i<meshList.size();i++)
	{
		deviceContext->Map(mPerFrameConstants, 0, D3D11_MAP_WRITE_DISCARD, 0, &mappedResource);
		PerFrameConstants* constants = static_cast<PerFrameConstants *>(mappedResource.pData);
		constants->mCameraWorldViewProj=(*positionList[i])*cameraViewProj;
		constants->mCameraWorldView=(*positionList[i])*cameraView;
		deviceContext->Unmap(mPerFrameConstants, 0);
		meshList[i]->Render(deviceContext,0);
	}

}

void SceneGraph::EmptyList(vector<CDXUTSDKMesh*> list)
{
	if(!list.empty())
	{
		list.erase(list.begin());
		//unnecessary?
		/*
		vector<CDXUTSDKMesh*>::iterator i;
		for(i=list.begin(); i!=list.end();i++)
		{
			SAFE_DELETE(*i);
		}*/
	}
}

void SceneGraph::EmptyList(vector<D3DXMATRIXA16*> list)
{
	if(!list.empty())
	{
		list.erase(list.begin());
		//unnecessary?
		/*
		vector<D3DXMATRIXA16*>::iterator i;
		for(i=list.begin(); i!=list.end();i++)
		{
			SAFE_DELETE(*i);
		}*/
	}
}

void SceneGraph::Destroy()
{
	EmptyList(meshList);
	EmptyList(positionList);
}
