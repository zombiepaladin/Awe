////////////////////////////////////////////////////////////////////////////////
// Filename: modelclass.cpp
////////////////////////////////////////////////////////////////////////////////
#include "modelclass.h"
#include "XnbModelData.h"
#include "Shader.h"



ModelClass::ModelClass()
{
	m_vertexBuffer = 0;
	m_indexBuffer = 0;
	m_Texture = 0;
	m_model = 0;
	/*
	std::string msaaSamplesStr;
    {
        std::ostringstream oss;
        oss << mMSAASamples;
        msaaSamplesStr = oss.str();
    }
// Set up macros
    D3D10_SHADER_MACRO defines[] = {
        {"MSAA_SAMPLES", msaaSamplesStr.c_str()},
        {0, 0}
    };*/
}


ModelClass::ModelClass(const ModelClass& other)
{
}


ModelClass::~ModelClass()
{
}

void ModelClass::ComputeInFrustumFlags(const D3DXMATRIXA16 &worldViewProj, bool cullNear){
	//nothing
}


HRESULT ModelClass::Create( ID3D11Device* pDev11, LPCTSTR szFileName, bool bCreateAdjacencyIndices, SDKMESH_CALLBACKS11* pLoaderCallbacks)
{
	//char *s = (char*)malloc(1024 * sizeof(char));
	//strcpy(s, "HKEY_CURRENT_USER\\");
	//strcpy(s, (char*)szFileName);
	//char *p = const_cast<char*>(szFileName);
	
	//TODO:fix hardcoding
	Initialize(pDev11, "..\\media\\cube\\Cats.xnb", L"..\\media\\cube\\seafloor.dds");

	//Initialize(pDev11,(char*)szFileName,NULL);
	//mGBufferPS = new PixelShader(pDev11, L"GBuffer.hlsl", "GBufferPS", defines);
	//TODO return
	HRESULT hr = S_OK;
	return hr;
}


bool ModelClass::Initialize(ID3D11Device* device, char* modelFilename, WCHAR* textureFilename)
{
	bool result;


	// Load in the model data,
	result = LoadModel(modelFilename);
	if(!result)
	{
		return false;
	}

	// Initialize the vertex and index buffers.
	result = InitializeBuffers(device);
	if(!result)
	{
		return false;
	}

	// Load the texture for this model.
	result = LoadTexture(device, textureFilename);
	if(!result)
	{
		return false;
	}
	isLoaded = true;
	return true;
}

bool ModelClass::IsLoaded()
{
	return isLoaded;
}

void ModelClass::Shutdown()
{
	// Release the model texture.
	ReleaseTexture();

	// Shutdown the vertex and index buffers.
	ShutdownBuffers();

	// Release the model data.
	ReleaseModel();

	return;
}


void ModelClass::Render(ID3D11DeviceContext* deviceContext,UINT iDiffuseSlot,
                                            UINT iNormalSlot,
                                            UINT iSpecularSlot)
{/*
	SDKMESH_MESH pMesh;
	pMesh.NumVertexBuffers = 1;
	UINT vertexBuffers = (UINT)m_vertexBuffer;
	pMesh.VertexBuffers[0] = vertexBuffers;
	*/
	
	// Put the vertex and index buffers on the graphics pipeline to prepare them for drawing.
//	CDXUTSDKMesh::RenderMesh();
	RenderBuffers(deviceContext);
	 deviceContext->DrawIndexed( m_indexCount, 0, 0 );
	return;
}


int ModelClass::GetIndexCount()
{
	return m_indexCount;
}


ID3D11ShaderResourceView* ModelClass::GetTexture()
{
	return m_Texture->GetTexture();
}


bool ModelClass::InitializeBuffers(ID3D11Device* device)
{
	VertexType* vertices;
	unsigned long* indices;
	D3D11_BUFFER_DESC vertexBufferDesc, indexBufferDesc;
    D3D11_SUBRESOURCE_DATA vertexData, indexData;
	HRESULT result;
	int i;


	// Create the vertex array.
	vertices = new VertexType[m_vertexCount];
	if(!vertices)
	{
		return false;
	}

	// Create the index array.
	indices = new unsigned long[m_indexCount];
	if(!indices)
	{
		return false;
	}

	// Load the vertex array and index array with data.
	for(i=0; i<m_vertexCount; i++)
	{
		vertices[i].position = D3DXVECTOR3(m_model[i].x, m_model[i].y, m_model[i].z);
		vertices[i].texture = D3DXVECTOR2(m_model[i].tu, m_model[i].tv);
		vertices[i].normal = D3DXVECTOR3(m_model[i].nx, m_model[i].ny, m_model[i].nz);
		//indices[i] = i;
	}

	for(i=0; i<m_indexCount; i++)
		indices[i] = m_indices[i];

	// Set up the description of the static vertex buffer.
    vertexBufferDesc.Usage = D3D11_USAGE_DEFAULT;
    vertexBufferDesc.ByteWidth = sizeof(VertexType) * m_vertexCount;
    vertexBufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    vertexBufferDesc.CPUAccessFlags = 0;
    vertexBufferDesc.MiscFlags = 0;
	vertexBufferDesc.StructureByteStride = 0;

	// Give the subresource structure a pointer to the vertex data.
    vertexData.pSysMem = vertices;
	vertexData.SysMemPitch = 0;
	vertexData.SysMemSlicePitch = 0;

	// Now create the vertex buffer.
    result = device->CreateBuffer(&vertexBufferDesc, &vertexData, &m_vertexBuffer);
	if(FAILED(result))
	{
		return false;
	}

	// Set up the description of the static index buffer.
    indexBufferDesc.Usage = D3D11_USAGE_DEFAULT;
    indexBufferDesc.ByteWidth = sizeof(int32_t) * m_indexCount;
    indexBufferDesc.BindFlags = D3D11_BIND_INDEX_BUFFER;
    indexBufferDesc.CPUAccessFlags = 0;
    indexBufferDesc.MiscFlags = 0;
	indexBufferDesc.StructureByteStride = 0;

	// Give the subresource structure a pointer to the index data.
    indexData.pSysMem = indices;
	indexData.SysMemPitch = 0;
	indexData.SysMemSlicePitch = 0;

	// Create the index buffer.
	result = device->CreateBuffer(&indexBufferDesc, &indexData, &m_indexBuffer);
	if(FAILED(result))
	{
		return false;
	}

	// Release the arrays now that the vertex and index buffers have been created and loaded.
	delete [] vertices;
	vertices = 0;

	delete [] indices;
	indices = 0;

	return true;
}


void ModelClass::ShutdownBuffers()
{
	// Release the index buffer.
	if(m_indexBuffer)
	{
		m_indexBuffer->Release();
		m_indexBuffer = 0;
	}

	// Release the vertex buffer.
	if(m_vertexBuffer)
	{
		m_vertexBuffer->Release();
		m_vertexBuffer = 0;
	}

	return;
}


void ModelClass::RenderBuffers(ID3D11DeviceContext* deviceContext)
{
	unsigned int stride;
	unsigned int offset;


	// Set vertex buffer stride and offset.
	stride = sizeof(VertexType); 
	offset = 0;
    
	// Set the vertex buffer to active in the input assembler so it can be rendered.
	deviceContext->IASetVertexBuffers(0, 1, &m_vertexBuffer, &stride, &offset);

    // Set the index buffer to active in the input assembler so it can be rendered.
	deviceContext->IASetIndexBuffer(m_indexBuffer, DXGI_FORMAT_R32_UINT, 0);

    // Set the type of primitive that should be rendered from this vertex buffer, in this case triangles.
	deviceContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
	ID3D11ShaderResourceView *BloodyShader=0;
	deviceContext->PSGetShaderResources(0,1,&BloodyShader);
	deviceContext->PSSetShaderResources( 0, 1,&BloodyShader);
	
	return;
}


bool ModelClass::LoadTexture(ID3D11Device* device, WCHAR* filename)
{
	bool result;

	// Create the texture object.
	m_Texture = new TextureClass;
	if(!m_Texture)
	{
		return false;
	}

	// Initialize the texture object.
	result = m_Texture->Initialize(device, filename);
	if(!result)
	{
		return false;
	}

	return true;
}


void ModelClass::ReleaseTexture()
{
	// Release the texture object.
	if(m_Texture)
	{
		m_Texture->Shutdown();
		delete m_Texture;
		m_Texture = 0;
	}

	return;
}


bool ModelClass::LoadModel(char* filename)
{
	int i;
	int j;
	vector<float> vertexData;
	XnbModelData data(filename);

	// Read in the vertex count.
	m_vertexCount = data.vertexData.size() / data.vertexDataSize; //vertexDataSize is the number of numbers that make up each vertex definition
	vertexData = data.vertexData;

	// Set the number of indices
	m_indexCount = data.indexData.size();
	m_indices = data.indexData;

	// Create the model using the vertex count that was read in.
	m_model = new ModelType[m_vertexCount];
	if(!m_model)
	{
		return false;
	}

	j=0;
	ModelType* currentModel;
	for(i=0; i < m_vertexCount; i++) //need to read xnb vertices backwards to make them compatible
	{
		currentModel = &m_model[i];

		//pos
		currentModel->z = vertexData[j++];
		currentModel->y = vertexData[j++];
		currentModel->x = vertexData[j++];

		//norm
		currentModel->nz = vertexData[j++];
		currentModel->ny = vertexData[j++];
		currentModel->nx = vertexData[j++];
	
		if(data.vertexDataSize > 6)
		{
			//texture
			currentModel->tu = vertexData[j++];
			currentModel->tv = vertexData[j++];
		}

		for(int a = data.vertexDataSize; a > 8; a--) //skip past the rest of the values
			j++;
		
	}

	return true;
}


void ModelClass::ReleaseModel()
{
	if(m_model)
	{
		delete [] m_model;
		m_model = 0;
	}

	return;
}