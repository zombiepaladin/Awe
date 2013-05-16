#include "PhysXDynamicCube.h"


PhysXDynamicCube::PhysXDynamicCube(PxPhysics* physics, PxTransform transform, PxBoxGeometry geometry, 
						 PxMaterial* material, PxReal density)
{
	this->actor = this->cube = PxCreateDynamic(*physics, transform, geometry, *material, density);

	this->Update();
}

PhysXDynamicCube::PhysXDynamicCube(PxPhysics* physics, PxTransform transform, PxVec3 halfExtents, 
						 PxMaterial* material, PxReal density)
{
	PxBoxGeometry geometry(halfExtents);

	this->actor = this->cube = PxCreateDynamic(*physics, transform, geometry, *material, density);

	this->Update();
}


PhysXDynamicCube::~PhysXDynamicCube(void)
{
	if(this->cube)
	{
		this->cube->release();
		this->cube = NULL;
	}
}
