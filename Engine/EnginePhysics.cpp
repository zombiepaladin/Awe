#include "EnginePhysics.h"

namespace EnginePhysics
{
	#pragma region Enums
		
		typedef enum
		{
			ZERO,			//No Gravity or other forces are applied, objects retain momentum
			NORMAL,			//Standard gravity such that all objects are pulled down
			PULL_SINGLE,	//Pulls from a single point
			PULL_DOUBLE,	//Pulls from two points
			PULL_TRIPLE,	//Pulls from three points
			PULL_PUSH,  	//Pulls from left, pushes from right
			PUSH_PULL,		//Pushes from left, pulls from right
			PLANET_GRAVITY,	//Uses planets as gravity, gravity adheres to inverse squared law, scales with distance
			ORBITS			//Applys an force tangential to the center point
		} GravityState;

	#pragma endregion

	#pragma region Defines

		#define PLANET_HEIGHT 100
		#define PLANET_NUM 2
		#define PLANE_NUM 6
		#define BLOCK_NUM 1000
		#define STRONG_UNIVERSAL_GRAVITATIONAL_FORCE 200.0f
		#define WEAK_UNIVERSAL_GRAVITATIONAL_FORCE 100.0f
		#define INVERSE_SQUARE_GRAVITATIONAL_FORCE 200.0f

	#pragma endregion

	#pragma region Variables

		static PxPhysics* gPhysicsSDK = NULL;
		static PxDefaultErrorCallback gDefaultErrorCallback;
		static PxDefaultAllocator gDefaultAllocatorCallback;
		static PxSimulationFilterShader gDefaultFilterShader = PxDefaultSimulationFilterShader;

		PxScene* gScene = NULL;
		PxReal myTimestep = 1.0f/60.0f;

		vector<PhysXObject*> *allActors;
		vector<PhysXObject*> boxes;
		vector<PhysXObject*> planets;
		//vector<PxRigidActor*> planetJointHandles;
		vector<PhysXObject*> planes;

		PxTransform planePoses[6] = {
			PxTransform(PxVec3(0.0f,-PLANET_HEIGHT,0.0f), PxQuat(PxHalfPi, PxVec3(0.0f, 0.0f, 1.0f))),
			PxTransform(PxVec3(0.0f,PLANET_HEIGHT,0.0f), PxQuat(PxHalfPi, PxVec3(0.0f, 0.0f, -1.0f))),
			PxTransform(PxVec3(PLANET_HEIGHT,0.0f,0.0f), PxQuat(PxPi, PxVec3(0.0f, 0.0f, 1.0f))),
			PxTransform(PxVec3(-PLANET_HEIGHT,0.0f,0.0f), PxQuat(0, PxVec3(0.0f, 0.0f, 1.0f))),
			PxTransform(PxVec3(0.0f,0.0f,PLANET_HEIGHT), PxQuat(PxHalfPi, PxVec3(0.0f, 1.0f, 0.0f))),
			PxTransform(PxVec3(0.0f,0.0f,-PLANET_HEIGHT), PxQuat(PxHalfPi, PxVec3(0.0f, -1.0f, 0.0f)))
		};

		PxTransform planetTransforms[3] = {
			PxTransform(PxVec3(0, 0, 0), PxQuat::createIdentity()),
			PxTransform(PxVec3(20, 20,20), PxQuat::createIdentity()),
			PxTransform(PxVec3(-20, -20,-20),PxQuat::createIdentity())
		};

		time_t stepTimer = -1;

		GravityState currentGravState = GravityState::ZERO;

		bool isPaused = false;

	#pragma endregion

	#pragma region Prototypes

		void DisableGravity(PxRigidActor* actor);
		void EnableGravity(PxRigidActor* actor);
		void ApplyGravity(PxRigidActor* actor, PxVec3 source, PxReal power);
		void ApplyInverseSquareGravity(PxRigidActor* actor, PxVec3 source, PxReal power);
		void UpdatePhysXObject(PhysXObject* object);
		void ResetScene();

		void ApplyZeroGravity(PhysXObject* object);
		void ApplyNormalGravity(PhysXObject* object);
		void ApplyPlanetGravity(PhysXObject* object);
		void ApplyPullDouble(PhysXObject* object);
		void ApplyPullPush(PhysXObject* object);
		void ApplyPullSingle(PhysXObject* object);
		void ApplyPullTriple(PhysXObject* object);
		void ApplyPushPull(PhysXObject* object);
		void ApplyOrbitVelocity(PxRigidActor* box, float power);
		void SetVelocity(PxVec3 newVelocity, PhysXObject* object);
		void RandomVelocities(PhysXObject* object, int powerMax, int seedMultiplier = 1);
		PxVec3 RandomOrthogonalVector(PxVec3 normal);
		PxVec3 CreateRandomVector(int maxAxisValue, int seedMultiplier = 1);

	#pragma endregion
		
	#pragma region Public Methods

		void InitializePhysX(vector<PhysXObject*>* &cubeList)
		{
			allActors = new vector<PhysXObject*>;

			PxFoundation* foundation = PxCreateFoundation(PX_PHYSICS_VERSION, 
				gDefaultAllocatorCallback, gDefaultErrorCallback);
			gPhysicsSDK = PxCreatePhysics(PX_PHYSICS_VERSION, *foundation, PxTolerancesScale());

			if(gPhysicsSDK == NULL)
			{
				exit(1);
			}

			PxInitExtensions(*gPhysicsSDK);

			PxSceneDesc sceneDesc(gPhysicsSDK->getTolerancesScale());

			sceneDesc.gravity=PxVec3(0.0f, -9.8f, 0.0f);

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


			//1) Create Planes
			PxMaterial* mMaterial = gPhysicsSDK->createMaterial(0.5, 0.5, 0.5);

			for(int i = 0; i < 1; i++)
			{
				PhysXObject* plane = new PhysXObject;
				plane->actor = gPhysicsSDK->createRigidStatic(planePoses[i]);

				PxShape* shape = plane->actor->createShape(PxPlaneGeometry(), *mMaterial);

				gScene->addActor(*(plane->actor));
				allActors->push_back(plane);
				planes.push_back(plane);
			}

			//2) Create Planets
			PxReal planetDensity = 1.0f;
			PxVec3 planetDimensions(2,2,2);
			PxBoxGeometry planetGeom(planetDimensions);
			PxTransform planetTransform;

			for(int i = 0; i < PLANET_NUM; i++)
			{
				planetTransform = planetTransforms[i];
				PhysXObject* planet = new PhysXObject;
				planet->actor = PxCreateStatic(*gPhysicsSDK, planetTransform, planetGeom, *mMaterial);

				EnableGravity(planet->actor);

				gScene->addActor(*(planet->actor));
				allActors->push_back(planet);
				planets.push_back(planet);

				//HACK: 
				/* Create the joint handlers for distance limiting
				/* We need to do this because a distance joint attached to an actor
				/* seems to void collisions between those two actors (i.e. "phases through")
				/* So we make another actor in the same position to hold the position
			
				PhysXObject* newHandle = new PhysXObject;
				newHandle->actor = PxCreateStatic(*gPhysicsSDK, tran, boxgeom, *mMaterial);

				gScene->addActor(*(newHandle->actor));
				planetJointHandles.push_back(newHandle);
				//We also don't need to worry about drawing the joints, for obvious reasons
				*/
			}

			//3) Create Cubes
			PxReal density = 1.0f;
			PxTransform transform(PxVec3(0.0f, 0.0f, 0.0f), PxQuat::createIdentity());
			PxVec3 dimensions(0.5, 0.5, 0.5);
			PxBoxGeometry geometry(dimensions);

			for(int i = 0; i < BLOCK_NUM; i++)
			{
				srand((time(NULL) * i) + time(NULL));

				transform.p = PxVec3((float)((rand() % (2 * PLANET_HEIGHT)) - PLANET_HEIGHT),
					(float)((rand() % (2 * PLANET_HEIGHT)) - PLANET_HEIGHT),
					(float)((rand() % (2 * PLANET_HEIGHT)) - PLANET_HEIGHT));

				PhysXObject* cube = new PhysXObject;

				cube->actor = PxCreateDynamic(*gPhysicsSDK, transform, geometry, *mMaterial, density);

				//Create Distance Joints between planets here
					//Not included for run time optimizations
				//End creating distance joints

				//Create D6 Joints between planets here
					//Not included for run time optimizations
				//End creating distance joints

				cube->actor->isRigidDynamic()->setAngularDamping(0.75);
				cube->actor->isRigidDynamic()->setLinearVelocity(PxVec3(0,0,0));

				gScene->addActor(*(cube->actor));
				allActors->push_back(cube);
				boxes.push_back(cube);
			}

			cubeList = allActors;
		}

		void StepPhysX()
		{
			if(!isPaused && gScene)
			{
				for(int i = 0; i < boxes.size(); i++)
				{
					switch(currentGravState)
					{
						case GravityState::ZERO:
							ApplyZeroGravity(boxes[i]);
							break;

						case GravityState::NORMAL:
							ApplyNormalGravity(boxes[i]);
							break;
				
						case GravityState::PLANET_GRAVITY:
							ApplyPlanetGravity(boxes[i]);
							break;

						case GravityState::PULL_DOUBLE:
							ApplyPullDouble(boxes[i]);
							break;

						case GravityState::PULL_PUSH:
							ApplyPullPush(boxes[i]);
							break;

						case GravityState::PULL_SINGLE:
							ApplyPullSingle(boxes[i]);
							break;

						case GravityState::PULL_TRIPLE:
							ApplyPullTriple(boxes[i]);
							break;

						case GravityState::PUSH_PULL:
							ApplyPushPull(boxes[i]);
							break;

						case GravityState::ORBITS:
							boxes[i]->actor->isRigidDynamic()->setLinearVelocity(PxVec3(0,0,0));
							ApplyOrbitVelocity(boxes[i]->actor, 40);
							break;
					}

					UpdatePhysXObject(boxes[i]);
				}

				gScene->simulate(myTimestep);

				while(!gScene->fetchResults())
				{
					//we can do some work here while the
					//frame is simulating, but I don't have anything
					//for the moment
				}
			}
		}

		void ShutdownPhysX()
		{
			if(allActors)
			{
				for(int i = 0; i < allActors->size(); i++)
				{
					gScene->removeActor(*(*allActors)[i]->actor);
					if((*allActors)[i]->actor){(*allActors)[i]->actor->release();}
					delete (*allActors)[i];
				}

				allActors->clear();
				allActors->shrink_to_fit();
			}
			if(gScene){gScene->release();gScene=NULL;}

			if(gPhysicsSDK){gPhysicsSDK->release();gPhysicsSDK=NULL;}
		}

		void ProcessKey(unsigned char key)
		{
			switch(key)
			{
				case '0':
					currentGravState = GravityState::ZERO;
					break;

				case '1':
					currentGravState = GravityState::NORMAL;
					break;
					
				case '2':
					currentGravState = GravityState::PLANET_GRAVITY;
					for(int i = 0; i < boxes.size(); i++)
					{
						PxVec3 dir = planets[0]->actor->getGlobalPose().p - boxes[i]->actor->getGlobalPose().p;

						ApplyOrbitVelocity(boxes[i]->actor, 100);
					}
					break;

				case '3':
					currentGravState = GravityState::PULL_SINGLE;
					break;

				case '4':
					currentGravState = GravityState::PULL_DOUBLE;
					break;

				case '5':
					currentGravState = GravityState::PULL_TRIPLE;
					break;

				case '6':
					currentGravState = GravityState::PULL_PUSH;
					break;

				case '7':
					currentGravState = GravityState::PUSH_PULL;
					break;

				case '8':
					currentGravState = GravityState::ORBITS;
					break;

				case 'p':
					isPaused = !isPaused;
					break;

				case 'r':
				case ' ':
					ResetScene();
					break;

				case '\n':
				case '\r':
					{
						for(int i = 0; i < boxes.size(); i++)
						{
							SetVelocity(PxVec3(0,0,0), boxes[i]);
						}
					}
					break;

				case 'v':
					{
						for(int i = 0; i < boxes.size(); i++)
						{
							RandomVelocities(boxes[i], 50, i);
						}
					}
					break;
			}
		}

	#pragma endregion

	#pragma region Private Methods
		
		void ApplyZeroGravity(PhysXObject* object)
		{
			DisableGravity(object->actor);
		}

		void ApplyNormalGravity(PhysXObject* object)
		{
			EnableGravity(object->actor);
		}

		void ApplyPlanetGravity(PhysXObject* object)
		{
			DisableGravity(object->actor);
			for(int j = 0; j < planets.size(); j++)
			{
				ApplyInverseSquareGravity(object->actor, planets[j]->actor->getGlobalPose().p, INVERSE_SQUARE_GRAVITATIONAL_FORCE);
			}
		}

		void ApplyPullDouble(PhysXObject* object)
		{
				DisableGravity(object->actor);

				ApplyGravity(object->actor, planetTransforms[1].p, STRONG_UNIVERSAL_GRAVITATIONAL_FORCE);
				ApplyGravity(object->actor, planetTransforms[2].p, STRONG_UNIVERSAL_GRAVITATIONAL_FORCE);
		}

		void ApplyPullPush(PhysXObject* object)
		{
				DisableGravity(object->actor);
			
				ApplyGravity(object->actor, planetTransforms[0].p, STRONG_UNIVERSAL_GRAVITATIONAL_FORCE);
				ApplyGravity(object->actor, planetTransforms[2].p, -WEAK_UNIVERSAL_GRAVITATIONAL_FORCE);
		}

		void ApplyPullSingle(PhysXObject* object)
		{
				DisableGravity(object->actor);
			
				ApplyGravity(object->actor, planetTransforms[0].p, STRONG_UNIVERSAL_GRAVITATIONAL_FORCE);
		}

		void ApplyPullTriple(PhysXObject* object)
		{
				DisableGravity(object->actor);
			
				ApplyGravity(object->actor, planetTransforms[0].p, STRONG_UNIVERSAL_GRAVITATIONAL_FORCE);
				ApplyGravity(object->actor, planetTransforms[2].p, STRONG_UNIVERSAL_GRAVITATIONAL_FORCE);
				ApplyGravity(object->actor, planetTransforms[1].p, STRONG_UNIVERSAL_GRAVITATIONAL_FORCE);
		}

		void ApplyPushPull(PhysXObject* object)
		{
				DisableGravity(object->actor);
			
				ApplyGravity(object->actor, planetTransforms[0].p, -WEAK_UNIVERSAL_GRAVITATIONAL_FORCE);
				ApplyGravity(object->actor, planetTransforms[2].p, STRONG_UNIVERSAL_GRAVITATIONAL_FORCE);
		}
	
		void ApplyOrbitVelocity(PxRigidActor* box, float power)
		{
			for(int i = 0; i < planets.size(); i++)
			{
				PxVec3 dir = planets[i]->actor->getGlobalPose().p - box->getGlobalPose().p;

				dir.normalize();

				PxVec3 velocity = RandomOrthogonalVector(dir) * power;

				box->isRigidDynamic()->addForce(velocity,PxForceMode::eACCELERATION);
			}
		}

	#pragma endregion

	#pragma region Helper Methods

		void SetVelocity(PxVec3 newVelocity, PhysXObject* object)
		{
			object->actor->isRigidDynamic()->setLinearVelocity(newVelocity);
		}

		void DisableGravity(PxRigidActor* actor)
		{
			actor->setActorFlag(PxActorFlag::eDISABLE_GRAVITY, true);
		}

		void EnableGravity(PxRigidActor* actor)
		{
			actor->setActorFlag(PxActorFlag::eDISABLE_GRAVITY, false);
		}

		void ApplyGravity(PxRigidActor* actor, PxVec3 source, PxReal power)
		{
			PxVec3 dir, norm, force;
		
			//Disables the scene gravity so we can apply our own
			DisableGravity(actor);

			dir = source - actor->getGlobalPose().p;

			norm = dir.getNormalized();

			force = (norm * power);

			actor->isRigidBody()->addForce(force, PxForceMode::eACCELERATION);
		}

		void ApplyInverseSquareGravity(PxRigidActor* actor, PxVec3 source, PxReal power)
		{
			PxVec3 dir;
			PxReal distSquared;
			PxVec3 norm;
			PxVec3 force;
			int objectNum = boxes.size();

			for(int i = 0; i < objectNum; i++)
			{
				//Disables the scene gravity so we can apply our own
				DisableGravity(boxes[i]->actor);

				dir = source - boxes[i]->actor->getGlobalPose().p;

				distSquared = dir.magnitudeSquared();
				distSquared = (distSquared < 10) ? 10000 : distSquared;

				norm = dir.getNormalized();
				force = (norm * power) / distSquared;

				boxes[i]->actor->isRigidBody()->addForce(force, PxForceMode::eACCELERATION);
			}
		}

		void UpdatePhysXObject(PhysXObject* object)
		{
			PxVec3 pos = object->actor->getGlobalPose().p;

			object->x = pos.x;
			object->y = pos.y;
			object->z = pos.z;
		}

		void ResetScene()
		{
			PxVec3 resetPosition;
			for(int i = 0; i < boxes.size(); i++)
			{
				srand((time(NULL) * i) + time(NULL));

				resetPosition = PxVec3((float)((rand() % (2 * PLANET_HEIGHT)) - PLANET_HEIGHT),
					(float)((rand() % (2 * PLANET_HEIGHT)) - PLANET_HEIGHT),
					(float)((rand() % (2 * PLANET_HEIGHT)) - PLANET_HEIGHT));

				boxes[i]->actor->isRigidDynamic()->setLinearVelocity(PxVec3(0,0,0));
				boxes[i]->actor->setGlobalPose(PxTransform(resetPosition, PxQuat::createIdentity()));
			}
		}

		void RandomVelocities(PhysXObject* object, int powerMax, int seedMultiplier)
		{
			PxVec3 dir = CreateRandomVector(powerMax, seedMultiplier);
		
			srand((time(NULL) * seedMultiplier) + time(NULL));

			int power = rand() % powerMax;

			dir.normalize();

			SetVelocity(dir * power, object);
		}

		PxVec3 RandomOrthogonalVector(PxVec3 normal)
		{
			PxVec3 random = CreateRandomVector(10);

			return random - (normal * random.dot(normal));
		}

		PxVec3 CreateRandomVector(int maxAxisValue, int seedMultiplier)
		{
			srand((time(NULL) * seedMultiplier) + time(NULL));

			return PxVec3((rand() % (2*maxAxisValue)) - maxAxisValue,
				(rand() % (2*maxAxisValue)) - maxAxisValue,
				(rand() % (2*maxAxisValue)) - maxAxisValue);
		}

	#pragma endregion

}