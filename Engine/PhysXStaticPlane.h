#pragma once
#include "physxobject.h"
class PhysXStaticPlane :
	public PhysXObject
{
public:
	PhysXStaticPlane(PxPhysics*, PxTransform, PxMaterial*);

	virtual ~PhysXStaticPlane(void);
	PxRigidStatic* plane;
};

