#include "PhysXGroup.h"


PhysXGroup::PhysXGroup(void)
{
	objectList = new std::vector<PhysXObject*>;
}


PhysXGroup::~PhysXGroup(void)
{
	if(objectList)
	{
		for(int i = 0; i < objectList->size(); i++)
		{
			if((*objectList)[i])
			{
				delete (*objectList)[i];
				(*objectList)[i] = NULL;
			}
		}

		objectList->clear();
		objectList = NULL;
	}
}


void PhysXGroup::AddObject(PhysXObject* object)
{
	if(object)
	{
		objectList->push_back(object);
	}
}


void PhysXGroup::AddGroup(PhysXGroup* group)
{
	std::vector<PhysXObject*>* groupList = group->GetGroup();

	for(int i = 0; i < groupList->size(); i++)
	{
		if((*groupList)[i])
		{
			objectList->push_back((*groupList)[i]);
		}
	}
}


void PhysXGroup::AddToScene(PxScene* scene)
{
	for(int i = 0; i < objectList->size(); i++)
	{
		scene->addActor(*((*objectList)[i]->actor));
	}
}

void PhysXGroup::RemoveObject(PhysXObject* object)
{
	for(std::vector<PhysXObject*>::iterator it = objectList->begin(); it != objectList->end(); ++it)
	{
		if((*it._Ptr) == object)
		{
			objectList->erase(it);
			break;
		}
	}
}

std::vector<PhysXObject*>* PhysXGroup::GetGroup(void)
{
	return objectList;
}

bool PhysXGroup::Contains(PhysXObject* object)
{
	for(int i = 0; i < objectList->size(); i++)
	{
		if((*objectList)[i] == object)
			return true;
	}

	return false;
}

bool PhysXGroup::ContainsActor(PhysXObject* object)
{
	for(int i = 0; i < objectList->size(); i++)
	{
		if((*objectList)[i]->actor == object->actor)
			return true;
	}

	return false;
}

PhysXObject* PhysXGroup::GetByActor(PxRigidActor* actor)
{
	for(int i = 0; i < objectList->size(); i++)
	{
		if((*objectList)[i]->actor->isRigidActor() == actor)
		{
			return (*objectList)[i];
		}
	}

	return NULL;
}
