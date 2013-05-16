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
	this->isFrozen = false;
	this->isGravityGenerator = false;
	this->group = ObjectGrouping::None;
	this->forceOfGravity = 0;
}

PhysXObject::~PhysXObject(void)
{
	if(this->actor)
	{
		actor->release();
		actor = NULL;
	}
}

void PhysXObject::AddToScene(PxScene* scene)
{
	scene->addActor(*(this->actor));
}

void PhysXObject::Update(void)
{
	PxTransform actorTrans = this->actor->getGlobalPose();

	this->UpdatePosition(actorTrans.p);
}

void PhysXObject::UpdateScale(PxVec3 scaleVector)
{
	this->sx = scaleVector.x;
	this->sy = scaleVector.y;
	this->sz = scaleVector.z;
}

void PhysXObject::GetMat44(bool colMajor, float *out_matrix)
{
	PxTransform actorTrans = actor->getGlobalPose();
	PxMat33 m = PxMat33(actorTrans.q);

	if(colMajor)
	{
		out_matrix[0] = m.column0[0];
		out_matrix[1] = m.column0[1];
		out_matrix[2] = m.column0[2];
		out_matrix[3] = 0;
		
		out_matrix[4] = m.column1[0];
		out_matrix[5] = m.column1[1];
		out_matrix[6] = m.column1[2];
		out_matrix[7] = 0;
		
		out_matrix[8] = m.column2[0];
		out_matrix[9] = m.column2[1];
		out_matrix[10] = m.column2[2];
		out_matrix[11] = 0;
		
		out_matrix[12] = actorTrans.p[0];
		out_matrix[13] = actorTrans.p[1];
		out_matrix[14] = actorTrans.p[2];
		out_matrix[15] = 1;
	}
	else
	{
		out_matrix[0] = m.column0[0];
		out_matrix[1] = m.column1[0];
		out_matrix[2] = m.column2[0];
		out_matrix[3] = actorTrans.p[0];
		
		out_matrix[4] = m.column0[1];
		out_matrix[5] = m.column1[1];
		out_matrix[6] = m.column2[1];
		out_matrix[7] = actorTrans.p[1];
		
		out_matrix[8] = m.column0[2];
		out_matrix[9] = m.column1[2];
		out_matrix[10] = m.column2[2];
		out_matrix[11] = actorTrans.p[2];
		
		out_matrix[12] = 0;
		out_matrix[13] = 0;
		out_matrix[14] = 0;
		out_matrix[15] = 1;
	}
}

void PhysXObject::GetMat33(bool colMajor, float *out_matrix)
{
	PxMat33 m = PxMat33(actor->getGlobalPose().q);

	if(colMajor)
	{
		out_matrix[0] = m.column0[0];
		out_matrix[1] = m.column0[1];
		out_matrix[2] = m.column0[2];
		
		out_matrix[3] = m.column1[0];
		out_matrix[4] = m.column1[0];
		out_matrix[5] = m.column1[2];
		
		out_matrix[6] = m.column2[0];
		out_matrix[7] = m.column2[1];
		out_matrix[8] = m.column2[2];
		
	}
	else
	{
		out_matrix[0] = m.column0[0];
		out_matrix[1] = m.column1[0];
		out_matrix[2] = m.column2[0];
		
		out_matrix[3] = m.column0[1];
		out_matrix[4] = m.column1[1];
		out_matrix[5] = m.column2[1];
		
		out_matrix[6] = m.column0[2];
		out_matrix[7] = m.column1[2];
		out_matrix[8] = m.column2[2];
	}
}

void PhysXObject::UpdatePosition(PxVec3 position)
{
	this->x = position.x;
	this->y = position.y;
	this->z = position.z;
}