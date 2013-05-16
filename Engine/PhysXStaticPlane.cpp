#include "PhysXStaticPlane.h"


PhysXStaticPlane::PhysXStaticPlane(PxPhysics* physics, PxTransform transform, PxMaterial* material)
{
	this->actor = this->plane = physics->createRigidStatic(transform);

	this->plane->createShape(PxPlaneGeometry(), *material);
	
	//Just need a really large scale for a cube if we're going to render
	this->UpdateScale(PxVec3(100,100,100));

	this->Update();
}


PhysXStaticPlane::~PhysXStaticPlane(void)
{
	if(this->plane)
	{
		this->plane->release();
		this->plane = NULL;
	}
}
