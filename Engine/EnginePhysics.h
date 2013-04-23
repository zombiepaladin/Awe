#pragma once

#ifndef ENGINEPHYSX_4182013501
#define ENGINEPHYSX_4182013501

/* Include files found to be unneccessary
#include <PxPhysicsAPI.h>
#include <extensions\PxExtensionsAPI.h>
#include <extensions\PxDefaultErrorCallback.h>
#include <extensions\PxDefaultAllocator.h>
#include <extensions\PxDefaultSimulationFilterShader.h>
#include <extensions\PxDefaultCpuDispatcher.h>
#include <extensions\PxShapeExt.h>
#include <foundation\PxMat33.h>
#include <extensions\PxSimpleFactory.h>
#include <include\PxToolkit.h>
#include <RepX\RepX.h>
*/
#include <vector>
#include <time.h>
#include "PhysXObject.h"

using namespace std;
using namespace physx;

#pragma comment(lib, "PhysX3_x86.lib")
//#pragma comment(lib, "PxTask.lib")
#pragma comment(lib, "PhysX3Extensions.lib")
#pragma comment(lib, "PhysX3Common_x86.lib")
//#pragma comment(lib, "PhysX3Cooking_x86.lib")
//#pragma comment(lib, "PxToolkitDEBUG.lib")

namespace EnginePhysics
{	
	void StepPhysX();

	void InitializePhysX(vector<PhysXObject*>* &cubeList);

	void ShutdownPhysX();
	
	void ProcessKey(unsigned char key);
}

#endif