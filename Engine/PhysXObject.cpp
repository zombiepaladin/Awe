#include "PhysXObject.h"


PhysXObject::PhysXObject()
{
	this->id = -1;
	this->x = 0;
	this->y = 0;
	this->z = 0;
	this->sx = 1;
	this->sy = 1;
	this->sz = 1;
	this->actor = NULL;
}


PhysXObject::~PhysXObject(void)
{
}
