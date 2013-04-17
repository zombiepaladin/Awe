#include "SceneGraph.h"
using std::tr1::shared_ptr;

// NOTE: Must match layout of shader constant buffers
using namespace std;
__declspec(align(16))

SceneGraph::SceneGraph()
{
}

SceneGraph::~SceneGraph()
{
}

bool SceneGraph::IsLoaded()
{
	return false;
}

void SceneGraph::Add(ID3D11Device* device, LPCTSTR szFileName)
{
}

void SceneGraph::ComputeInFrustumFlags(const D3DXMATRIXA16 &worldViewProj)
{
}

void SceneGraph::Render(ID3D11DeviceContext* deviceContext)
{
}

void SceneGraph::Destroy()
{
}
