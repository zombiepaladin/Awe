#pragma region includes
#include <PxPhysicsAPI.h>
#include <PxPhysics.h>
#include <extensions\PxExtensionsAPI.h>
#include <extensions\PxDefaultErrorCallback.h>
#include <extensions\PxDefaultAllocator.h>
#include <extensions\PxDefaultSimulationFilterShader.h>
#include <extensions\PxDefaultCpuDispatcher.h>
#include <extensions\PxShapeExt.h>
#include <extensions\PxSimpleFactory.h>
#include <foundation\PxFoundation.h>

#include <stdio.h>
#include <iostream>
#include <vector>

#include <DXUT.h>
#pragma endregion includes

#pragma region namespaces
using namespace std;
using namespace physx;
#pragma endregion namespaces



#pragma comment(lib, "PhysX3CHECKED_x86.lib")
#pragma comment(lib, "PhysX3CommonCHECKED_x86.lib")
#pragma comment(lib, "PhysX3ExtensionsCHECKED.lib")
#pragma comment(lib, "PxTask.lib")

PxPhysics *physics = NULL;
PxFoundation *foundation = NULL;
PxScene* dxScene = NULL;
static PxDefaultAllocator gDefaultAllocatorCallback;
static PxDefaultErrorCallback gDefaultErrorCallback;


void initPhysX() {
	foundation = PxCreateFoundation(PX_PHYSICS_VERSION, gDefaultAllocatorCallback, gDefaultErrorCallback);
	physics = PxCreatePhysics(PX_PHYSICS_VERSION, *foundation, PxTolerancesScale());

	PxInitExtensions(*physics);
	
	PxSceneDesc sceneDesc(physics->getTolerancesScale());
	sceneDesc.gravity = PxVec3(0.0f,-9.8f,0.0f);


}

void destoryPhysX(){
	 
printf("shutting down\n");
 physics->release();
 foundation->release();
}

int main(){
	initPhysX();
}