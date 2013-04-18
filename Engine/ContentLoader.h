#pragma once

class ContentLoader
{
public:
	static ContentLoader* CreateContentLoader(char* filename);
	Model* LoadModel(char* assetName);
	Texture* LoadTexture(char* assetName);
private:
	ContentLoader(void);
	~ContentLoader(void);
};

class Model
{
public:
	Model(void);
	~Model(void);
};

class Texture
{
public:
	Texture(void);
	~Texture(void);
};

