#pragma once

#ifndef PHYSXOBJECT_4182013503
#define PHYSXOBJECT_4182013503

#include <PxPhysicsAPI.h>

class PhysXObject
{
public:
	int id;
	int x,y,z;
	float sx,sy,sz;
	physx::PxRigidActor* actor;
public:
	PhysXObject();
	~PhysXObject(void);
};

#endif