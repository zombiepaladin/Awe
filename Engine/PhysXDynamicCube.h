#pragma once
#include "physxobject.h"

class PhysXDynamicCube :
	public PhysXObject
{
public:
	PhysXDynamicCube(PxPhysics*, PxTransform, PxBoxGeometry, PxMaterial*, PxReal);
	PhysXDynamicCube(PxPhysics*, PxTransform, PxVec3, PxMaterial*, PxReal);

	virtual ~PhysXDynamicCube(void);
	PxRigidDynamic* cube;
};

