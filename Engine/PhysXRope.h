#pragma once
#include "PhysXGroup.h"
#include "PhysXDynamicCube.h"
#include <list>
#include <vector>

class PhysXRope :
	public PhysXGroup
{

#ifndef CLAMP
	#define CLAMP(value, high, low) ((value > high) ? high : ((value < low) ? low : value))
#endif

public:
	PhysXRope(PxPhysics*, PxVec3, PxVec3, int, PxReal, PxVec3, PxMaterial*);
	PhysXRope(PxPhysics* physics, PxVec3 origin, PxVec3 direction, int numberOfSegments, PxVec3 segmentDimensions, PxReal segmentDensity, PxMaterial* material);

	virtual ~PhysXRope(void);
	
	PhysXObject* GetFirst();
	PhysXObject* GetLast();
	void PinJoint(PxPhysics* physics, PhysXObject* object, PxTransform objJointPose, PhysXObject* ropeSegment);
	void AddTo(std::vector<PhysXObject*>* list);
	void AddTo(std::list<PhysXObject*>* list);

public:
	PxAggregate* aggregate;
	static int const maxAggregateSize = 128;

private:
	PxQuat GetQuaternion(PxVec3 from, PxVec3 to);
	void AddObject(PhysXObject* object);
	void AddGroup(PhysXGroup* group);
	void LinkSegments(PxPhysics* physics, PxRigidDynamic*, PxRigidDynamic*, PxVec3);
};

