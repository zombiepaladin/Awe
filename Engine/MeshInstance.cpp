#include "MeshInstance.h"
using std::tr1::shared_ptr;

// NOTE: Must match layout of shader constant buffers
using namespace std;
__declspec(align(16));

MeshInstance::MeshInstance()
{
	m_instanceBuffer = 0;
}


MeshInstance::MeshInstance(const MeshInstance& other)
{
	
}


MeshInstance::~MeshInstance()
{
	if(!positionList.empty())
	{
		for(int i=positionList.size()-1; i>=0; i--)
		{
			SAFE_DELETE(positionList[i]);
		}
	}
}


bool MeshInstance::SetBuffers(ID3D11Device* device)
{
	bool result;


	// Initialize the vertex and instance buffers.
	result = InitializeBuffers(device);
	if(!result)
	{
		return false;
	}

	// Load the texture for this model.

	return true;
}


void MeshInstance::Shutdown()
{

	// Shutdown the vertex and instance buffers.
	ShutdownBuffers();

	return;
}


void MeshInstance::Render(ID3D11DeviceContext* deviceContext)
{
	// Put the vertex and instance buffers on the graphics pipeline to prepare them for drawing.
	RenderBuffers(deviceContext);

	return;
}


int MeshInstance::GetInstanceCount()
{
	return m_instanceCount;
}




bool MeshInstance::InitializeBuffers(ID3D11Device* device)
{
	
	D3D11_BUFFER_DESC vertexBufferDesc,instanceBufferDesc;
    D3D11_SUBRESOURCE_DATA vertexData, instanceData;
	HRESULT result;



	// Set up the description of the instance buffer.
    instanceBufferDesc.Usage = D3D11_USAGE_DEFAULT;
    instanceBufferDesc.ByteWidth = sizeof(positionList) * m_instanceCount;
    instanceBufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    instanceBufferDesc.CPUAccessFlags = 0;
    instanceBufferDesc.MiscFlags = 0;
	instanceBufferDesc.StructureByteStride = 0;

	// Give the subresource structure a pointer to the instance data.
    instanceData.pSysMem = &positionList;
	instanceData.SysMemPitch = 0;
	instanceData.SysMemSlicePitch = 0;

	// Create the instance buffer.
	result = device->CreateBuffer(&instanceBufferDesc, &instanceData, &m_instanceBuffer);
	if(FAILED(result))
	{
		return false;
	}

	// Release the instance array now that the instance buffer has been created and loaded.

	return true;
}


void MeshInstance::ShutdownBuffers()
{
	// Release the instance buffer.
	if(m_instanceBuffer)
	{
		m_instanceBuffer->Release();
		m_instanceBuffer = 0;
	}

	// Release the vertex buffer.
	return;
}


void MeshInstance::RenderBuffers(ID3D11DeviceContext* deviceContext)
{

	unsigned int strides[2];
	unsigned int offsets[2];
	ID3D11Buffer* bufferPointers[2];


	// Set the buffer strides.
	strides[0] = sizeof(positionList); 
	strides[1] = 0; 

	// Set the buffer offsets.
	offsets[0] = 0;
	offsets[1] = 0;
    
	// Set the array of pointers to the vertex and instance buffers.
	bufferPointers[0] = 0;	
	bufferPointers[1] = m_instanceBuffer;

	// Set the vertex buffer to active in the input assembler so it can be rendered.
	deviceContext->IASetVertexBuffers(0, 2, bufferPointers, strides, offsets);

    // Set the type of primitive that should be rendered from this vertex buffer, in this case triangles.
	deviceContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
	return;
}

int MeshInstance::AddInstance(D3DXMATRIXA16& in){

	positionList.push_back(new D3DXMATRIXA16(in));
	m_instanceCount = positionList.size();
	return positionList.size();
}

void MeshInstance::SetPosition(int id, D3DXMATRIXA16& pos)
{
	int x = positionList.size();
	positionList[id-1] = new D3DXMATRIXA16(pos);
}

void MeshInstance::Create( ID3D11Device* pDev11, LPCTSTR szFileName)
{

}

void MeshInstance::Destroy()
{

}

bool MeshInstance::IsLoaded()
{
	return true;
}

void MeshInstance::ComputeInFrustumFlags(const D3DXMATRIXA16 &worldViewProj, bool cullNear)
{
}