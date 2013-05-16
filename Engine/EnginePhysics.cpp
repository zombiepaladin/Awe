#include "EnginePhysics.h"
#include "PhysXStaticPlane.h"
#include "PhysXStaticCube.h"
#include "PhysXDynamicCube.h"
#include "PhysXRope.h"

#include <Windows.h>
#include <iostream>
#include <sstream>
#include <ctime>
#include <list>

#pragma region Static Definitions

PxPhysics* PhysXEngine::gPhysicsSDK = NULL;
const float PhysXEngine::PiOver180 = PxPi / 180.0f;
PxDefaultErrorCallback PhysXEngine::gDefaultErrorCallback;
PxDefaultAllocator PhysXEngine::gDefaultAllocatorCallback;
PxSimulationFilterShader PhysXEngine::gDefaultFilterShader = PxDefaultSimulationFilterShader;

		#define PLANET_HEIGHT 100
		#define PLANET_NUM 2
		#define PLANE_NUM 6
		#define BLOCK_NUM 1000
		#define STRONG_UNIVERSAL_GRAVITATIONAL_FORCE 200.0f
		#define WEAK_UNIVERSAL_GRAVITATIONAL_FORCE 100.0f
		#define INVERSE_SQUARE_GRAVITATIONAL_FORCE 200.0f


#pragma region Public Methods
	
	void PhysXEngine::InitializePhysX(vector<PhysXObject*>* &cubeList,
		void (*UnProject)(int xi, int yi, float depth, double &rx, double &ry, double &rz),
		void (*Project)(float vx, float vy, float vz, int &xi, int &yi, float &depth))
	{
		InitializeVars();

		PhysXUnProject = UnProject;
		PhysXProject = Project;

		srand(time(NULL));

		allActors = new PhysXGroup();

		PxFoundation* foundation = PxCreateFoundation(PX_PHYSICS_VERSION, 
			gDefaultAllocatorCallback, gDefaultErrorCallback);
		gPhysicsSDK = PxCreatePhysics(PX_PHYSICS_VERSION, *foundation, PxTolerancesScale());

		if(gPhysicsSDK == NULL)
		{
			exit(1);
		}

		PxInitExtensions(*gPhysicsSDK);

		PxSceneDesc sceneDesc(gPhysicsSDK->getTolerancesScale());

		//-0.60
		sceneDesc.gravity = GRAVITY_CONST;

		if(!sceneDesc.cpuDispatcher)
		{
			PxDefaultCpuDispatcher* mCpuDispatcher = PxDefaultCpuDispatcherCreate(3);

			sceneDesc.cpuDispatcher = mCpuDispatcher;
		}

		if(!sceneDesc.filterShader)
			sceneDesc.filterShader = gDefaultFilterShader;

		gScene = gPhysicsSDK->createScene(sceneDesc);

		gScene->setVisualizationParameter(PxVisualizationParameter::eSCALE, 1.0);
		gScene->setVisualizationParameter(PxVisualizationParameter::eCOLLISION_SHAPES, 1.0f);

		InitScene();

		for(int i = 0; i < allActors->objectList->size(); i++)
		{
			gScene->addActor(*(*(allActors->objectList))[i]->actor);
		}

		/*
		////1) Create Planes
		//PxMaterial* mMaterial = gPhysicsSDK->createMaterial(0.5, 0.5, 0.5);

		//for(int i = 0; i < 1; i++)
		//{
		//	PhysXStaticPlane* plane = new PhysXStaticPlane(gPhysicsSDK, planePoses[i], mMaterial);


		//	plane->AddToScene(gScene);
		//	allActors->objectList->push_back(plane);
		//	planes->objectList->push_back(plane);
		//}

		////2) Create Planets

		//for(int i = 0; i < PLANET_NUM; i++)
		//{
		//	PhysXStaticCube* planet = new PhysXStaticCube(gPhysicsSDK, planetTransforms[i], PxVec3(2,2,200), mMaterial);

		//	planet->AddToScene(gScene);
		//	allActors->objectList->push_back(planet);
		//	planets->objectList->push_back(planet);

		//	//HACK: 
		//	/* Create the joint handlers for distance limiting
		//	/* We need to do this because a distance joint attached to an actor
		//	/* seems to void collisions between those two actors (i.e. "phases through")
		//	/* So we make another actor in the same position to hold the position
		//	
		//	PhysXObject* newHandle = new PhysXObject;
		//	newHandle->actor = PxCreateStatic(*gPhysicsSDK, tran, boxgeom, *mMaterial);

		//	gScene->addActor(*(newHandle->actor));
		//	planetJointHandles.push_back(newHandle);
		//	//We also don't need to worry about drawing the joints, for obvious reasons
		//	*//*
		//}

		////3) Create Cubes
		//PxTransform transform = PxTransform::createIdentity();

		//for(int i = 0; i < BLOCK_NUM; i++)
		//{
		//	transform.p = PxVec3(RANDOM_BETWEEN(PLANET_HEIGHT, -PLANET_HEIGHT),
		//		RANDOM_BETWEEN(PLANET_HEIGHT, -PLANET_HEIGHT),
		//		RANDOM_BETWEEN(PLANET_HEIGHT, -PLANET_HEIGHT));

		//	PxVec3 axisOfRotation = PxVec3(RANDOM_BETWEEN(PLANET_HEIGHT, -PLANET_HEIGHT),
		//		RANDOM_BETWEEN(PLANET_HEIGHT, -PLANET_HEIGHT),
		//		RANDOM_BETWEEN(PLANET_HEIGHT, -PLANET_HEIGHT)).getNormalized();

		//		
		//	transform.q = PxQuat(RANDOM_BETWEEN(360,0) * PiOver180,axisOfRotation);

		//	//                       Transform                        Dimensions         Material  Density
		//	//cube->actor = CreateCube(transform, PxVec3(0.5,0.5,0.5), mMaterial, 1.0f);

		//	PhysXDynamicCube* cube = new PhysXDynamicCube(gPhysicsSDK, transform, PxVec3(0.5,0.5,0.5), mMaterial, 1.0f);

		//	//Create Distance Joints between planets here
		//		//Not included for run time optimizations
		//	//End creating distance joints

		//	//Create D6 Joints between planets here
		//		//Not included for run time optimizations
		//	//End creating distance joints

		//	cube->AddToScene(gScene);
		//	allActors->objectList->push_back(cube);
		//	boxes->objectList->push_back(cube);
		//}

		////4) Create Rope
		//PxReal segmentDensity = 1.0f;
		//PxVec3 segmentDimension(ROPE_SEGMENT_HALF_EXTENTS, ROPE_SEGMENT_Y_HALF_EXTENTS, ROPE_SEGMENT_Z_HALF_EXTENTS);

		//PhysXRope* myRope = new PhysXRope(gPhysicsSDK, PxVec3(0,-4,0), PxVec3(0,0,1), ROPE_SEGMENTS, segmentDimension, segmentDensity, mMaterial);

		//myRope->AddToScene(gScene);
		//rope = myRope;
		//myRope->AddTo(allActors->objectList);

		//PxTransform fixedSpacialPoint(PxVec3(0,-4,0), PxQuat::createIdentity());
		//myRope->PinJoint(gPhysicsSDK, NULL,fixedSpacialPoint, myRope->GetFirst());
		*/

		stepTimer = clock();

		cubeList = allActors->objectList;
	}

	bool step = false;
	void PhysXEngine::StepPhysX()
	{
		double dt = double(clock() - stepTimer) / CLOCKS_PER_SEC;

		stepAccumulator -= dt;

		//Comment this line out to debug code step by step
		step=true;

		if(!isPaused && gScene && stepAccumulator <= 0 && step)
		{
			step = false;

			stepAccumulator = myTimestep;

			PreSimulation();

			gScene->simulate(myTimestep);

			bool objectsUpdated = false;

			while(!gScene->fetchResults())
			{
				DuringSimulation();
			}
		}

		stepTimer = clock();
	}

	void PhysXEngine::ShutdownPhysX()
	{
		if(gScene)
		{
			for(int i = 0; i < allActors->objectList->size(); i++)
			{
				gScene->removeActor(*(*(allActors->objectList))[i]->actor);
			}
		}

		if(allActors)
		{	
			delete allActors;
		}

		if(gScene){gScene->release();gScene=NULL;}

		if(gPhysicsSDK){gPhysicsSDK->release();gPhysicsSDK=NULL;}
	}

	void PhysXEngine::ProcessKey(unsigned char key, int mouseX, int mouseY)
	{
		DBOUT("Screen Center = {" << mouseX << ", " << mouseY << "}" << endl);
		switch(key)
		{
			case 'P':
			case 'p':
				isPaused = !isPaused;
				break;

			case 'Z':
			case 'z':
				step = true;
				break;

			case 'C':
			case 'c':
				ChangeGroup(GetProjectedObject(mouseX, mouseY));
				break;

			case 'E':
			case 'e':
				ToggleFreeze(GetProjectedObject(mouseX, mouseY));
				break;

			case 'G':
			case 'g':
				ToggleGravityGenerator(GetProjectedObject(mouseX, mouseY));
				break;
		}
	}

	PhysXObject* PhysXEngine::PickAnyActor(float origX, float origY, float origZ, float dirX, float dirY, float dirZ)
	{
		PxVec3 origin(origX, origY, origZ);
		PxVec3 direction(dirX, dirY, dirZ);

		direction -= origin;
		float length = direction.magnitude();
		direction.normalize();

		PxShape* closestShape;

		if(!gScene)return NULL;

		PxRaycastHit* rayCast = new PxRaycastHit();

		gScene->raycastSingle(origin, direction, length, PxSceneQueryFlag::eIMPACT, *rayCast);

		closestShape = rayCast->shape;

		if(!closestShape) return NULL;

		//PhysXObject* object = GetObjectWithActor(&closestShape->getActor());

		PhysXObject* object = allActors->GetByActor(&closestShape->getActor());

		return object;
	}

	PxRaycastHit* PhysXEngine::PickActor(PxVec3 rayOrigin, PxVec3 rayDirection)
	{
		rayDirection -= rayOrigin;
		float length = rayDirection.magnitude();
		rayDirection.normalize();

		//Cast the ray and find what it hits
		PxShape* closestShape;

		if(!gScene) return NULL;

		gScene->raycastSingle(rayOrigin, rayDirection, length, PxSceneQueryFlag::eIMPACT, *recentHit);

		closestShape = recentHit->shape;

		if(!closestShape) return NULL;

		if(!closestShape->getActor().isRigidDynamic()) return NULL;

		int hitx, hity;

		if(!mouseObject) return NULL;

		mouseObject->mouseSphere = CreateSphere(recentHit->impact, 0.1f, 1.0f);

		mouseObject->mouseSphere->actor->isRigidDynamic()->setRigidDynamicFlag(PxRigidDynamicFlag::eKINEMATIC,true); //Px_BF_Kinematic

		//mouseObject->selectedActor = GetObjectWithActor((&closestShape->getActor()));

		mouseObject->selectedActor = allActors->GetByActor(&closestShape->getActor());
		mouseObject->selectedActor->actor->isRigidDynamic()->wakeUp();

		PxTransform mFrame, sFrame;
		
		mFrame.q = mouseObject->mouseSphere->actor->isRigidDynamic()->getGlobalPose().q;
		mFrame.p = mouseObject->mouseSphere->actor->isRigidDynamic()->getGlobalPose().transformInv(recentHit->impact);
		sFrame.q = mouseObject->selectedActor->actor->isRigidDynamic()->getGlobalPose().q;
		sFrame.p = mouseObject->selectedActor->actor->isRigidDynamic()->getGlobalPose().transformInv(recentHit->impact);
		
		mouseObject->mouseJoint = PxDistanceJointCreate(*gPhysicsSDK, mouseObject->mouseSphere->actor, mFrame, mouseObject->selectedActor->actor, sFrame);
		mouseObject->mouseJoint->setDamping(1);
		mouseObject->mouseJoint->setSpring(200);
		mouseObject->mouseJoint->setMinDistance(0);
		mouseObject->mouseJoint->setMaxDistance(0);
		mouseObject->mouseJoint->setDistanceJointFlag(PxDistanceJointFlag::eMAX_DISTANCE_ENABLED, true);
		mouseObject->mouseJoint->setDistanceJointFlag(PxDistanceJointFlag::eSPRING_ENABLED, true);

		return recentHit;
	}

	PxRaycastHit* PhysXEngine::PickActor(float origX, float origY, float origZ, float dirX, float dirY, float dirZ)
	{
		return PickActor(PxVec3(origX, origY, origZ), PxVec3(dirX, dirY, dirZ));
	}

	void PhysXEngine::UnpickActor()
	{
		if(mouseObject->mouseJoint)
			mouseObject->mouseJoint->release();
		mouseObject->mouseJoint = NULL;

		if(mouseObject->mouseSphere)
		{
			if(mouseObject->mouseSphere->actor)
				mouseObject->mouseSphere->actor->release();
			mouseObject->mouseSphere->actor = NULL;
			delete mouseObject->mouseSphere;
		}
		mouseObject->mouseSphere = NULL;
		mouseObject->selectedActor = NULL;
	}

	bool PhysXEngine::isPicking()
	{
		if(mouseObject->mouseJoint)
			return true;
		
		return false;
	}
	
	void PhysXEngine::MoveActor(float x, float y, float z)
	{
		if(!mouseObject->selectedActor) return;

		PxVec3 pos(x,y,z);

		mouseObject->mouseSphere->actor->isRigidDynamic()->setGlobalPose(PxTransform(pos));
	}
	
	void PhysXEngine::ToggleFreeze(PhysXObject* object)
	{
		if(mouseObject->selectedActor)
			object = mouseObject->selectedActor;

		if(!object) return;

		if(object->isFrozen)
		{
			UnFreezeObject(object);
		}
		else
		{
			FreezeObject(object);
		}
	}

	void PhysXEngine::ToggleGravityGenerator(PhysXObject* object)
	{
		if(mouseObject->selectedActor)
			object = mouseObject->selectedActor;

		if(!object) return;

		if(object->isGravityGenerator)
		{
			DeactivateGravityGenerator(object);
		}
		else
		{
			ActivateGravityGenerator(object);
		}
	}

	
	void PhysXEngine::ChangeGroup(PhysXObject* object)
	{
		if(mouseObject->selectedActor)
			object = mouseObject->selectedActor;

		if(!object) return;
		if(planes->ContainsActor(object)) return;

		switch(object->group)
		{
			case Red:
				object->group = ObjectGrouping::Green;
				break;

			case Green:
				object->group = ObjectGrouping::Blue;
				break;

			case Blue:
				object->group = ObjectGrouping::Yellow;
				break;

			case Yellow:
				object->group = ObjectGrouping::None;
				break;

			case None:
				object->group = ObjectGrouping::Red;
				break;
		}
	}

#pragma endregion

#pragma region Private Methods
		
	void PhysXEngine::ApplyZeroGravity(PhysXObject* object)
	{
		DisableGravity(object->actor);
	}

	void PhysXEngine::ApplyNormalGravity(PhysXObject* object)
	{
		EnableGravity(object->actor);
	}

	void PhysXEngine::ApplyPlanetGravity(PhysXObject* object)
	{
		DisableGravity(object->actor);
		for(int j = 0; j < planets->objectList->size(); j++)
		{
			ApplyInverseSquareGravity(object->actor, (*(planets->objectList))[j]->actor->getGlobalPose().p, INVERSE_SQUARE_GRAVITATIONAL_FORCE);
		}
	}

	void PhysXEngine::ApplyPullDouble(PhysXObject* object)
	{
			DisableGravity(object->actor);

			ApplyGravity(object->actor, planetTransforms[1].p, STRONG_UNIVERSAL_GRAVITATIONAL_FORCE);
			ApplyGravity(object->actor, planetTransforms[2].p, STRONG_UNIVERSAL_GRAVITATIONAL_FORCE);
	}

	void PhysXEngine::ApplyPullPush(PhysXObject* object)
	{
			DisableGravity(object->actor);
			
			ApplyGravity(object->actor, planetTransforms[0].p, STRONG_UNIVERSAL_GRAVITATIONAL_FORCE);
			ApplyGravity(object->actor, planetTransforms[2].p, -WEAK_UNIVERSAL_GRAVITATIONAL_FORCE);
	}

	void PhysXEngine::ApplyPullSingle(PhysXObject* object)
	{
			DisableGravity(object->actor);
			
			ApplyGravity(object->actor, planetTransforms[0].p, STRONG_UNIVERSAL_GRAVITATIONAL_FORCE);
	}

	void PhysXEngine::ApplyPullTriple(PhysXObject* object)
	{
			DisableGravity(object->actor);
			
			ApplyGravity(object->actor, planetTransforms[0].p, STRONG_UNIVERSAL_GRAVITATIONAL_FORCE);
			ApplyGravity(object->actor, planetTransforms[2].p, STRONG_UNIVERSAL_GRAVITATIONAL_FORCE);
			ApplyGravity(object->actor, planetTransforms[1].p, STRONG_UNIVERSAL_GRAVITATIONAL_FORCE);
	}

	void PhysXEngine::ApplyPushPull(PhysXObject* object)
	{
			DisableGravity(object->actor);
			
			ApplyGravity(object->actor, planetTransforms[0].p, -WEAK_UNIVERSAL_GRAVITATIONAL_FORCE);
			ApplyGravity(object->actor, planetTransforms[2].p, STRONG_UNIVERSAL_GRAVITATIONAL_FORCE);
	}
	
	void PhysXEngine::ApplyOrbitVelocity(PxRigidActor* box, float power)
	{
		for(int i = 0; i < planets->objectList->size(); i++)
		{
			PxVec3 dir = (*(planets->objectList))[i]->actor->getGlobalPose().p - box->getGlobalPose().p;

			dir.normalize();

			PxVec3 velocity = RandomOrthogonalVector(dir) * power;

			box->isRigidDynamic()->addForce(velocity,PxForceMode::eACCELERATION);
		}
	}
	
	void PhysXEngine::ApplyGravityGenerator(PhysXObject* generator, PhysXObject* child)
	{
		ApplyGravity(child->actor, generator->actor->getGlobalPose().p, generator->forceOfGravity);
	}

#pragma endregion

#pragma region Helper Methods
	
	void PhysXEngine::InitializeVars()
	{
		gScene = NULL;
		myTimestep = 1.0f/60.0f;
		stepTimer = 0;
		stepAccumulator = 0;
		isPaused = false;

		allActors = new PhysXGroup();
		boxes = new PhysXGroup();
		planets = new PhysXGroup();
		planes = new PhysXGroup();
		dynamicActors = new PhysXGroup();
		staticActors = new PhysXGroup();
		gravityGenerators = new PhysXGroup();

		mouseObject = new PhysXMouseObject();
		recentHit = new PxRaycastHit();

		currentGravState = GravityState::ZERO;

		planePoses[0] = PxTransform(PxVec3(0.0f,0.0f,0.0f), PxQuat(PxHalfPi, PxVec3(0.0f, 0.0f, 1.0f)));		//Ground
		planePoses[1] = PxTransform(PxVec3(ROOM_HALF_EXTENTS, 0, 0), PxQuat(PxPi, PxVec3(0.0f, 1.0f, 0.0f)));	//+X Wall
		planePoses[2] = PxTransform(PxVec3(-ROOM_HALF_EXTENTS, 0, 0), PxQuat::createIdentity());				//-X Wall
		planePoses[3] = PxTransform(PxVec3(0, 0, ROOM_HALF_EXTENTS), PxQuat(PxHalfPi, PxVec3(0, 1, 0)));		//+Z Wall
		planePoses[4] = PxTransform(PxVec3(0, 0, -ROOM_HALF_EXTENTS), PxQuat(PxHalfPi, PxVec3(0, -1, 0)));		//-Z Wall
		planePoses[5] = PxTransform(PxVec3(0, 2 * ROOM_HALF_EXTENTS, 0), PxQuat(PxHalfPi, PxVec3(0, 0, -1.0f)));	//Ceiling

		planetTransforms[0] = PxTransform(PxVec3(0, 0, 0), PxQuat::createIdentity());
		planetTransforms[1] = PxTransform(PxVec3(0, -35,-10), PxQuat(PxHalfPi, PxVec3(0.0f, 1.0f, 0.0f)));
		planetTransforms[2] = PxTransform(PxVec3(-20, -20,-20),PxQuat::createIdentity());

		mMaterial = NULL; //Will be initialized in InitScene(), since gPhysicsSDK needs to initialize first
	}
	
	void PhysXEngine::InitScene()
	{
		mMaterial = gPhysicsSDK->createMaterial(0.5, 0.5, 0.1);

		//1) Create Planes
		for(int i = 0; i < PLANE_NUM; i++)
		{
			//Create the plane we need with the defined position
			PhysXObject* newPlane = new PhysXStaticPlane(gPhysicsSDK, planePoses[i], mMaterial);

			//Add the plane to our list of actors
			allActors->AddObject(newPlane);

			//And to our personal list of planes
			planes->AddObject(newPlane);

			//And to our static actor list
			staticActors->AddObject(newPlane);
		}
		
		//2) Create Cubes
		PxTransform cubeTransform;
		PxVec3 cubePosition;
		PxQuat cubeQuaternion;

		for(int i = 0; i < BLOCK_NUM; i++)
		{
			//Get a random position for this cube
			cubePosition = PxVec3(
				RANDOM_BETWEEN(ROOM_HALF_EXTENTS, -ROOM_HALF_EXTENTS),
				RANDOM_BETWEEN(2 * ROOM_HALF_EXTENTS, 0),
				RANDOM_BETWEEN(ROOM_HALF_EXTENTS, -ROOM_HALF_EXTENTS));

			//Get a random orientation for this cube, and making sure it's normalized
			cubeQuaternion = PxQuat(RANDOM_BETWEEN(360,0) * PiOver180, PxVec3(
				RANDOM_BETWEEN(ROOM_HALF_EXTENTS, -ROOM_HALF_EXTENTS),
				RANDOM_BETWEEN(2 * ROOM_HALF_EXTENTS, 0),
				RANDOM_BETWEEN(ROOM_HALF_EXTENTS, -ROOM_HALF_EXTENTS)).getNormalized());

			//Make the Transformation
			cubeTransform = PxTransform(cubePosition, cubeQuaternion);

			//Make the cube
			PhysXObject* newCube = new PhysXDynamicCube(gPhysicsSDK, cubeTransform, BLOCK_HALF_EXTENTS, mMaterial, 1.0f);
			
			//Add the cube to the list of actors
			allActors->AddObject(newCube);

			//And to our dynamic actor list
			dynamicActors->AddObject(newCube);
		}

	}

	void PhysXEngine::FreezeObject(PhysXObject* object)
	{
		if(!object->isFrozen && !planes->ContainsActor(object))
		{
			object->isFrozen = true;

			PhysXStaticCube* newCube = new PhysXStaticCube(gPhysicsSDK, object->actor->getGlobalPose(), FROZEN_HALF_EXTENTS, mMaterial);

			//TODO: This is probably a memory leak, newCube itself is never deleted and its reference is lost

			object->actor->release();
			object->actor = newCube->actor;

			dynamicActors->RemoveObject(object);
			staticActors->AddObject(object);

			gScene->addActor((*object->actor));
		}
	}

	void PhysXEngine::UnFreezeObject(PhysXObject* object)
	{
		if(object->isFrozen && !planes->ContainsActor(object))
		{
			object->isFrozen = false;

			PhysXDynamicCube* newCube = new PhysXDynamicCube(gPhysicsSDK, object->actor->getGlobalPose(), BLOCK_HALF_EXTENTS, mMaterial, 1.0f);

			//TODO: This is probably a memory leak, newCube itself is never deleted and its reference is lost

			object->actor->release();
			object->actor = newCube->actor;

			staticActors->RemoveObject(object);
			dynamicActors->AddObject(object);

			gScene->addActor(*object->actor);
		}
	}

	void PhysXEngine::PreSimulation()
	{
		PhysXObject* currentObject;
		PhysXObject* currentGravGenerator;
		bool belongsToGenerator;

		//We need to check each dynamic actor, since these are the ones that will move
		for(int i = 0; i < dynamicActors->objectList->size(); i++)
		{
			currentObject = (*dynamicActors->objectList)[i];
			belongsToGenerator = false;

			//Next we need to check if we need to push it toward any of the gravity generators
			for(int j = 0; j < gravityGenerators->objectList->size(); j++)
			{
				currentGravGenerator = (*gravityGenerators->objectList)[j];

				if(currentGravGenerator->group == currentObject->group)
				{
					belongsToGenerator = true;
					ApplyGravityGenerator(currentGravGenerator, currentObject);
				}
			}

			//If there are no gravity generators that act on this object, then
			//we need to use normal gravity for it
			if(!belongsToGenerator)
			{
				EnableGravity(currentObject->actor);
			}
			else
			{
				DisableGravity(currentObject->actor);
			}
		}
	}

	void PhysXEngine::DuringSimulation()
	{
		for(int i = 0; i < allActors->objectList->size(); i++)
		{
			(*allActors->objectList)[i]->Update();
		}
	}

	void PhysXEngine::AdvanceScene()
	{
		for(int i = 0; i < boxes->objectList->size(); i++)
			{
				switch(currentGravState)
				{
					case GravityState::ZERO:
						ApplyZeroGravity((*(boxes->objectList))[i]);
						break;

					case GravityState::NORMAL:
						ApplyNormalGravity((*(boxes->objectList))[i]);
						break;
				
					case GravityState::PLANET_GRAVITY:
						ApplyPlanetGravity((*(boxes->objectList))[i]);
						break;

					case GravityState::PULL_DOUBLE:
						ApplyPullDouble((*(boxes->objectList))[i]);
						break;

					case GravityState::PULL_PUSH:
						ApplyPullPush((*(boxes->objectList))[i]);
						break;

					case GravityState::PULL_SINGLE:
						ApplyPullSingle((*(boxes->objectList))[i]);
						break;

					case GravityState::PULL_TRIPLE:
						ApplyPullTriple((*(boxes->objectList))[i]);
						break;

					case GravityState::PUSH_PULL:
						ApplyPushPull((*(boxes->objectList))[i]);
						break;

					case GravityState::ORBITS:
						(*(boxes->objectList))[i]->actor->isRigidDynamic()->setLinearVelocity(PxVec3(0,0,0));
						ApplyOrbitVelocity((*(boxes->objectList))[i]->actor, 40);
						break;
				}
			}

			gScene->simulate(myTimestep);

			bool objectsUpdated = false;

			while(!gScene->fetchResults())
			{
				//we can do some work here while the
				//frame is simulating, but I don't have anything
				//for the moment
					
				if(!objectsUpdated)
				{
					for(int i = 0; i < allActors->objectList->size(); i++)
					{
						UpdatePhysXObject((*allActors->objectList)[i]);
					}
				}
			}
	}

	void PhysXEngine::SetVelocity(PxVec3 newVelocity, PhysXObject* object)
	{
		object->actor->isRigidDynamic()->setLinearVelocity(newVelocity);
	}

	void PhysXEngine::DisableGravity(PxRigidActor* actor)
	{
		actor->setActorFlag(PxActorFlag::eDISABLE_GRAVITY, true);
	}

	void PhysXEngine::EnableGravity(PxRigidActor* actor)
	{
		actor->setActorFlag(PxActorFlag::eDISABLE_GRAVITY, false);
	}

	void PhysXEngine::ApplyGravity(PxRigidActor* actor, PxVec3 source, PxReal power)
	{
		PxVec3 dir, norm, force;
		
		//Disables the scene gravity so we can apply our own
		DisableGravity(actor);

		dir = source - actor->getGlobalPose().p;

		norm = dir.getNormalized();

		force = (norm * power);

		actor->isRigidBody()->addForce(force, PxForceMode::eACCELERATION);
	}

	void PhysXEngine::ApplyInverseSquareGravity(PxRigidActor* actor, PxVec3 source, PxReal power)
	{
		PxVec3 dir;
		PxReal distSquared;
		PxVec3 norm;
		PxVec3 force;
		int objectNum = boxes->objectList->size();

		for(int i = 0; i < objectNum; i++)
		{
			//Disables the scene gravity so we can apply our own
			DisableGravity((*(boxes->objectList))[i]->actor);

			dir = source - (*(boxes->objectList))[i]->actor->getGlobalPose().p;

			distSquared = dir.magnitudeSquared();
			distSquared = (distSquared < 10) ? 10000 : distSquared;

			norm = dir.getNormalized();
			force = (norm * power) / distSquared;

			(*(boxes->objectList))[i]->actor->isRigidBody()->addForce(force, PxForceMode::eACCELERATION);
		}
	}

	void PhysXEngine::UpdatePhysXObject(PhysXObject* object)
	{
		PxVec3 pos = object->actor->getGlobalPose().p;

		object->x = pos.x;
		object->y = pos.y;
		object->z = pos.z;
	}

	void PhysXEngine::ResetScene()
	{
		PxVec3 resetPosition;
		for(int i = 0; i < boxes->objectList->size(); i++)
		{
			srand((time(NULL) * i) + time(NULL));

			resetPosition = PxVec3((float)((rand() % (2 * PLANET_HEIGHT)) - PLANET_HEIGHT),
				(float)((rand() % (2 * PLANET_HEIGHT)) - PLANET_HEIGHT),
				(float)((rand() % (2 * PLANET_HEIGHT)) - PLANET_HEIGHT));

			(*(boxes->objectList))[i]->actor->isRigidDynamic()->setLinearVelocity(PxVec3(0,0,0));
			(*(boxes->objectList))[i]->actor->setGlobalPose(PxTransform(resetPosition, PxQuat::createIdentity()));
		}
	}

	void PhysXEngine::RandomVelocities(PhysXObject* object, int powerMax, int seedMultiplier)
	{
		PxVec3 dir = CreateRandomVector(powerMax, seedMultiplier);
		
		srand((time(NULL) * seedMultiplier) + time(NULL));

		int power = rand() % powerMax;

		dir.normalize();

		SetVelocity(dir * power, object);
	}

	PxVec3 PhysXEngine::RandomOrthogonalVector(PxVec3 normal)
	{
		PxVec3 random = CreateRandomVector(10);

		return random - (normal * random.dot(normal));
	}

	PxVec3 PhysXEngine::CreateRandomVector(int maxAxisValue, int seedMultiplier)
	{
		srand((time(NULL) * seedMultiplier) + time(NULL));

		return PxVec3((rand() % (2*maxAxisValue)) - maxAxisValue,
			(rand() % (2*maxAxisValue)) - maxAxisValue,
			(rand() % (2*maxAxisValue)) - maxAxisValue);
	}
	
	PhysXObject* PhysXEngine::CreateSphere(const PxVec3& pos, const PxReal radius, const PxReal density)
	{
		PxTransform transform(pos, PxQuat::createIdentity());
		PxBoxGeometry geometry(radius/2, radius/2, radius/2);

		PxMaterial* mMaterial = gPhysicsSDK->createMaterial(0.5, 0.5, 0.5);

		PhysXDynamicCube* actor = new PhysXDynamicCube(gPhysicsSDK, transform, geometry, mMaterial, density);

		if(!actor)
			cerr<<"create actor failed!"<<endl;

		actor->actor->isRigidDynamic()->setAngularDamping(0.75);
		actor->actor->isRigidDynamic()->setLinearVelocity(PxVec3(0,0,0));

		gScene->addActor(*(actor->actor));

		return actor;
	}

	void PhysXEngine::ActivateGravityGenerator(PhysXObject* object)
	{
		if(planes->ContainsActor(object)) return;

		object->isGravityGenerator = true;
		object->forceOfGravity = GRAVITY_GENERATOR_INITIAL_FORCE;

		dynamicActors->RemoveObject(object);
		gravityGenerators->AddObject(object);
	}

	void PhysXEngine::DeactivateGravityGenerator(PhysXObject* object)
	{
		if(planes->ContainsActor(object)) return;

		object->isGravityGenerator = true;

		gravityGenerators->RemoveObject(object);
		dynamicActors->AddObject(object);
	}

	PhysXObject* PhysXEngine::GetProjectedObject(int x, int y)
	{
		double origX, origY, origZ,
			dirX, dirY, dirZ;

		(*PhysXUnProject)(x, y, 0.0f, origX, origY, origZ);
		(*PhysXUnProject)(x, y, 1.0f, dirX, dirY, dirZ);

		return PickAnyActor((float)origX, (float)origY, (float)origZ, (float)dirX, (float)dirY, (float)dirZ);
	}

#pragma endregion