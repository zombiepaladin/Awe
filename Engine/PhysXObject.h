#pragma once

#ifndef PHYSXOBJECT_4182013503
#define PHYSXOBJECT_4182013503

#include <PxPhysicsAPI.h>

using namespace physx;

typedef enum
{
	Red,
	Green,
	Blue,
	Yellow,
	None
} ObjectGrouping;

class PhysXObject
{
public:
	int id;
	int x,y,z;
	float sx,sy,sz;
	bool isGravityGenerator;
	int forceOfGravity;
	bool isFrozen;
	ObjectGrouping group;

	physx::PxRigidActor* actor;
public:
	PhysXObject();
	~PhysXObject(void);
	void GetMat44(bool colMajor, float out_matrix[16]);
	void GetMat33(bool colMajor, float out_matrix[9]);
	void Update(void);
	virtual void AddToScene(PxScene* scene);
	void UpdateScale(PxVec3 scaleVector);
	void UpdatePosition(PxVec3 position);
};

#endif