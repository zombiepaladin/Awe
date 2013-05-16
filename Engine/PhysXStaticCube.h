#pragma once
#include "physxobject.h"
#include <list>

class PhysXStaticCube :
	public PhysXObject
{
public:
	PhysXStaticCube(PxPhysics*, PxTransform, PxGeometry, PxMaterial*);
	PhysXStaticCube(PxPhysics*, PxTransform, PxVec3, PxMaterial*);

	virtual ~PhysXStaticCube(void);

public:
	PxRigidStatic* cube;
private:
	PxTransform GetTransform(PxVec3 from, PxVec3 to);
};

