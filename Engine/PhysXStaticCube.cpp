#include "PhysXStaticCube.h"


PhysXStaticCube::PhysXStaticCube(PxPhysics* physics, PxTransform transform, PxGeometry geometry, PxMaterial* material)
{
	this->actor = this->cube = PxCreateStatic(*physics, transform, geometry, *material);
	this->Update();
}

PhysXStaticCube::PhysXStaticCube(PxPhysics* physics, PxTransform transform, PxVec3 halfExtents, PxMaterial* material)
{
	PxBoxGeometry geometry(halfExtents);

	this->actor = this->cube = PxCreateStatic(*physics, transform, geometry, *material);
	
	this->Update();
}


PhysXStaticCube::~PhysXStaticCube(void)
{
	if(this->cube)
	{
		cube->release();
		cube = NULL;
	}
}
