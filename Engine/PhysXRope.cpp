#include "PhysXRope.h"


//Makes a rope that spans between two points with a given number of segments, good
//when the number of objects needs to be restricted or more dynamic
PhysXRope::PhysXRope(PxPhysics* physics, PxVec3 from, PxVec3 to, int numberOfSegments, PxReal segmentDensity, 
					 PxVec3 segmentDimensions, PxMaterial* material)
{
	//Getting the direction that the links will be made in
	PxVec3 dir = (to - from);
	PxReal distance = dir.magnitude();
	dir.normalize();

	//Clamping the max number of segments
	numberOfSegments = CLAMP(numberOfSegments, maxAggregateSize, 0);

	//Scaling the x dimension to fit the given number of segments
	segmentDimensions.x = distance / numberOfSegments;

	//This will define the geometry of a block, or rope segment
	PxBoxGeometry segmentGeometry = PxBoxGeometry(segmentDimensions);

	//Getting what we need for the transformation
	PxQuat quaternion = this->GetQuaternion(from, to);
	PxVec3 position;

	//Setting up our aggregate and prepping for the loop
	this->aggregate = physics->createAggregate(numberOfSegments, true);
	PhysXDynamicCube* previousSeg = NULL;
	PhysXDynamicCube* segment = NULL;

	for(int i = 0; i < numberOfSegments; i++)
	{
		//Laying out each segment
		position = from + dir * i * segmentDimensions.x;

		//Lets create our segment
		segment = new PhysXDynamicCube(physics,PxTransform(position, quaternion), 
			segmentGeometry, material, segmentDensity);

		//And initialize the scale
		segment->UpdateScale(segmentDimensions);

		//Set some default parameters
		segment->cube->setLinearDamping(0.75);
		segment->cube->setLinearVelocity(PxVec3(0,0,0));

		if(previousSeg != NULL)
		{
			this->LinkSegments(physics, previousSeg->cube, segment->cube, segmentDimensions);
		}

		this->AddObject(segment);

		previousSeg = segment;

		segment->Update();
	}

}

PhysXRope::PhysXRope(PxPhysics* physics, PxVec3 origin, PxVec3 direction, int numberOfSegments, PxVec3 segmentDimensions, PxReal segmentDensity, PxMaterial* material)
{
	//Getting the direction that the links will be made in
	PxVec3 dir = direction.getNormalized();

	//Clamping the max number of segments
	numberOfSegments = CLAMP(numberOfSegments, maxAggregateSize, 0);

	//This will define the geometry of a block, or rope segment
	PxBoxGeometry segmentGeometry = PxBoxGeometry(segmentDimensions);

	//Getting what we need for the transformation
	PxQuat quaternion = this->GetQuaternion(origin, origin + dir);
	PxVec3 position;

	//Setting up our aggregate and prepping for the loop
	this->aggregate = physics->createAggregate(numberOfSegments, true);
	PhysXDynamicCube* previousSeg = NULL;
	PhysXDynamicCube* segment = NULL;

	for(int i = 0; i < numberOfSegments; i++)
	{
		//Laying out each segment
		position = origin + dir * i * segmentDimensions.x;

		//Lets create our segment
		segment = new PhysXDynamicCube(physics,PxTransform(position, quaternion), 
			segmentGeometry, material, segmentDensity);

		//And initialize the scale
		segment->UpdateScale(segmentDimensions);

		//Set some default parameters
		segment->cube->setLinearDamping(0.75);
		segment->cube->setLinearVelocity(PxVec3(0,0,0));

		if(previousSeg != NULL)
		{
			this->LinkSegments(physics, previousSeg->cube, segment->cube, segmentDimensions);
		}

		this->AddObject(segment);

		previousSeg = segment;

		segment->Update();
	}
}

PhysXRope::~PhysXRope(void)
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

	if(this->aggregate)
	{
		aggregate->release();
		aggregate = NULL;
	}
}


void PhysXRope::AddObject(PhysXObject* object)
{
	if(object)
	{
		this->objectList->push_back(object);
		this->aggregate->addActor(*(object->actor));
	}
}


void PhysXRope::AddGroup(PhysXGroup* group)
{
	std::vector<PhysXObject*>* groupList = group->GetGroup();

	for(int i = 0; i < groupList->size(); i++)
	{
		if((*groupList)[i])
		{
			objectList->push_back((*groupList)[i]);
			this->aggregate->addActor(*((*groupList)[i]->actor));
		}
	}
}


void PhysXRope::AddTo(std::vector<PhysXObject*>* list)
{
	for(int i = 0; i < this->objectList->size(); i++)
	{
		list->push_back((*objectList)[i]);
	}
}


void PhysXRope::AddTo(std::list<PhysXObject*>* list)
{
	for(int i = 0; i < this->objectList->size(); i++)
	{
		list->push_back((*objectList)[i]);
	}
}


PxQuat PhysXRope::GetQuaternion(PxVec3 from, PxVec3 to)
{
	PxVec3 dir = (to - from);
	PxReal distance = dir.magnitude();
	dir.normalize();

	PxVec3 objectAxis(1,0,0);
	
	PxVec3 axisOfRotation = objectAxis.cross(dir).getNormalized();
	PxReal angle = PxAcos(objectAxis.dot(dir));
	
	return PxQuat(angle, axisOfRotation);
}


void PhysXRope::LinkSegments(PxPhysics* physics, PxRigidDynamic* parent, PxRigidDynamic* child, PxVec3 segmentDimensions)
{
	if(parent == NULL || child == NULL)
		return;

	//First we'll make our transforms, these will offset the joint position on each actor
	PxTransform childTransform = PxTransform(PxVec3(segmentDimensions.x - 0.1,0,0), PxQuat::createIdentity());
	PxTransform parentTransform = PxTransform(PxVec3(-segmentDimensions.x + 0.1,0,0), PxQuat::createIdentity());

	////Create the Joint
	//PxSphericalJoint* socket = PxSphericalJointCreate(*physics, 
	//	parent, parentTransform,
	//	child, childTransform);

	////Now we'll define our Constraints starting with the limit cone
	//socket->setLimitCone(PxJointLimitCone(PxHalfPi, PxHalfPi, 0.1f));
	//socket->setSphericalJointFlag(PxSphericalJointFlag::eLIMIT_ENABLED, true);

	////Next is setting the projection, while it's a bit expensive
	////projection forces constraints and makes the rope appear much more realistic
	////otherwise segments will 'jitter'
	//socket->setProjectionLinearTolerance(0.1f);
	//socket->setConstraintFlag(PxConstraintFlag::ePROJECTION, true);

	PxD6Joint* d6Joint = PxD6JointCreate(*physics, parent, parentTransform, child, childTransform);

	//And define its parameters
	d6Joint->setMotion(PxD6Axis::eX, PxD6Motion::eLOCKED);
	d6Joint->setMotion(PxD6Axis::eY, PxD6Motion::eLOCKED);
	d6Joint->setMotion(PxD6Axis::eZ, PxD6Motion::eLOCKED);
	d6Joint->setMotion(PxD6Axis::eTWIST, PxD6Motion::eLOCKED);
	d6Joint->setMotion(PxD6Axis::eSWING1, PxD6Motion::eLOCKED);
	d6Joint->setMotion(PxD6Axis::eSWING2, PxD6Motion::eLOCKED);

	//1.2
	d6Joint->setProjectionLinearTolerance(1.2f);
	d6Joint->setConstraintFlag(PxConstraintFlag::ePROJECTION, true);
	
}


PhysXObject* PhysXRope::GetFirst()
{
	return (*(this->objectList))[0];
}


PhysXObject* PhysXRope::GetLast()
{
	return (*(this->objectList))[this->objectList->size() - 1];
}


void PhysXRope::PinJoint(PxPhysics* physics, PhysXObject* object, PxTransform objJointPose, PhysXObject* ropeSegment)
{
	//Just gonna quickly get our Transform for the segment set up,
	//we want to be at the back of it. We'll also default to a downward position
	//since this will be suitable for most occaisions
	PxTransform segTransform = PxTransform(PxVec3(ropeSegment->sx,0,0), PxQuat(PxHalfPi, PxVec3(0,0,1)));
	PxRigidActor* objectActor = (object) ? object->actor : NULL;

	//We'll now make a D6 joint, these joints are special and allow
	//for the greatest amount of customization
	PxD6Joint* d6Joint = PxD6JointCreate(*physics,
		objectActor, objJointPose,
		ropeSegment->actor, segTransform);

	//And define its parameters
	d6Joint->setMotion(PxD6Axis::eX, PxD6Motion::eLOCKED);
	d6Joint->setMotion(PxD6Axis::eY, PxD6Motion::eLOCKED);
	d6Joint->setMotion(PxD6Axis::eZ, PxD6Motion::eLOCKED);
	d6Joint->setMotion(PxD6Axis::eTWIST, PxD6Motion::eFREE);
	d6Joint->setMotion(PxD6Axis::eSWING1, PxD6Motion::eFREE);
	d6Joint->setMotion(PxD6Axis::eSWING2, PxD6Motion::eFREE);

	//and finally set its limits
	d6Joint->setLinearLimit(PxJointLimit(0.1f,0.1f));
}
