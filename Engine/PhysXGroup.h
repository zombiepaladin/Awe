#pragma once
#include "PhysXObject.h"
#include <vector>

class PhysXGroup
{
public:
	std::vector<PhysXObject*> * objectList;
public:
	PhysXGroup(void);
	~PhysXGroup(void);
	virtual void AddObject(PhysXObject* object);
	virtual void AddGroup(PhysXGroup* group);
	virtual void AddToScene(PxScene* scene);
	virtual void RemoveObject(PhysXObject* object);
	virtual bool Contains(PhysXObject* object);
	virtual bool ContainsActor(PhysXObject* object);
	virtual PhysXObject* GetByActor(PxRigidActor* object);
	virtual std::vector<PhysXObject*>* GetGroup(void);
};

