#pragma once

#ifndef ENGINEPHYSX_4182013501
#define ENGINEPHYSX_4182013501

#include <vector>
#include <time.h>
#include <ctime>
#include <pxtask\PxTask.h>
#include <Windows.h>
#include <iostream>
#include <sstream>
#include "PhysXObject.h"
#include "PhysXGroup.h"
#include "PhysXRope.h"

using namespace std;
using namespace physx;

#pragma comment(lib, "PhysX3_x86.lib")
//#pragma comment(lib, "PhysX3Extensions.lib")
#pragma comment(lib, "PhysX3Common_x86.lib")

class PhysXEngine
{
	
	struct PhysXMouseObject
	{
		PhysXObject* mouseSphere;
		PhysXObject* selectedActor;
		PxDistanceJoint* mouseJoint;

		PhysXMouseObject()
		{
			this->mouseSphere = NULL;
			this->selectedActor = NULL;
			this->mouseJoint = NULL;
		}
	};

	#pragma region Enums
		
public: typedef enum
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

		#define PLANET_HEIGHT 200
		#define PLANET_NUM 2
		#define PLANE_NUM 6
		#define BLOCK_NUM 100
		#define BLOCK_HALF_EXTENTS PxVec3(0.5f, 0.5f, 0.5f)
		#define GRAVITY_GENERATOR_HALF_EXTENTS PxVec3(0.6f, 0.6f, 0.6f)
		#define GRAVITY_GENERATOR_INITIAL_FORCE 200
		#define FROZEN_HALF_EXTENTS PxVec3(0.4f, 0.4f, 0.4f)
		#define ROPE_SEGMENTS 500
		#define ROPE_SEGMENT_HALF_EXTENTS 1.0f
		#define ROPE_SEGMENT_Y_HALF_EXTENTS 0.5f
		#define ROPE_SEGMENT_Z_HALF_EXTENTS 0.5f
		#define GRAVITY_GENERATOR_INCREMENT 10.0f
		#define STRONG_UNIVERSAL_GRAVITATIONAL_FORCE 200.0f
		#define WEAK_UNIVERSAL_GRAVITATIONAL_FORCE 100.0f
		#define INVERSE_SQUARE_GRAVITATIONAL_FORCE 200.0f
		#define GRAVITY_CONST PxVec3(0.0f, -9.8f, 0.0f)
		#define ROOM_HALF_EXTENTS 200

	#pragma endregion

	#pragma region Macros
		
			#define DBOUT(s)\
			{\
				std::wostringstream os_;\
				os_ << s;\
				OutputDebugString(os_.str().c_str());\
			}

			#define RANDOM_BETWEEN(high, low) \
				(PxReal) ((rand() % (high - low)) + low)\

			
			#ifndef CLAMP
				#define CLAMP(value, high, low) ((value > high) ? high : ((value < low) ? low : value))
			#endif

	#pragma endregion


private:
	static PxPhysics* gPhysicsSDK;// = NULL;
	static PxDefaultErrorCallback gDefaultErrorCallback;
	static PxDefaultAllocator gDefaultAllocatorCallback;
	static PxSimulationFilterShader gDefaultFilterShader;// = PxDefaultSimulationFilterShader;
	
	int testingExtern;
	PxScene* gScene;// = NULL;
	PxReal myTimestep;// = 1.0f/60.0f;
	clock_t stepTimer;// = 0;
	double stepAccumulator;// = 0;

	PhysXGroup* allActors;
	PhysXGroup* boxes;
	PhysXGroup* planets;
	//vector<PxRigidActor*> planetJointHandles;
	PhysXGroup* planes;
	PhysXGroup* dynamicActors;
	PhysXGroup* staticActors;
	PhysXGroup* gravityGenerators;

	PhysXRope* rope;

	PxMaterial* mMaterial;

	PxRaycastHit* recentHit;
	PhysXMouseObject* mouseObject;

	PxTransform planePoses[6];

	PxTransform planetTransforms[3];

	GravityState currentGravState;

	bool isPaused;

	static const float PiOver180;
	
	void (*PhysXUnProject)(int, int, float, double&, double&, double&);
	void (*PhysXProject)(float, float, float, int&, int&, float&);

public:
	void StepPhysX();
	void InitializePhysX(vector<PhysXObject*>* &cubeList, 
		void (*UnProject)(int xi, int yi, float depth, double &rx, double &ry, double &rz),
		void (*Project)(float vx, float vy, float vz, int &xi, int &yi, float &depth));
	void ShutdownPhysX();
	void ProcessKey(unsigned char key, int x = 0, int y = 0);
	PhysXObject* PickAnyActor(float origX, float origY, float origZ, float dirX, float dirY, float dirZ);
	PxRaycastHit* PickActor(PxVec3 rayOrigin, PxVec3 rayDirection);
	PxRaycastHit* PickActor(float origX, float origY, float origZ, float dirX, float dirY, float dirZ);
	void UnpickActor();
	bool isPicking();
	void MoveActor(float x, float y, float z);
	void ToggleFreeze(PhysXObject* object);
	void ChangeGroup(PhysXObject* object);
	void ToggleGravityGenerator(PhysXObject* object);

private:
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
	void ApplyGravityGenerator(PhysXObject* generator, PhysXObject* child);

	void SetVelocity(PxVec3 newVelocity, PhysXObject* object);
	void RandomVelocities(PhysXObject* object, int powerMax, int seedMultiplier = 1);
	PxVec3 RandomOrthogonalVector(PxVec3 normal);
	PxVec3 CreateRandomVector(int maxAxisValue, int seedMultiplier = 1);
	void AdvanceScene();
	void InitializeVars();
	PhysXObject* CreateSphere(const PxVec3& pos, const PxReal radius, const PxReal density);

	void InitScene();

	void PreSimulation();
	void DuringSimulation();

	void FreezeObject(PhysXObject* object);
	void UnFreezeObject(PhysXObject* object);
	
	void ActivateGravityGenerator(PhysXObject* object);
	void DeactivateGravityGenerator(PhysXObject* object);

	PhysXObject* GetProjectedObject(int x, int y);

};

#endif