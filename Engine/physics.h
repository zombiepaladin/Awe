#ifndef PHYSICS_H
#define PHYSICS_H

#ifndef NDEBUG
#define NDEBUG
#endif

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

#include <iostream>
#include <vector>


#pragma comment(lib, "PhysX3CHECKED_x86.lib")
#pragma comment(lib, "PhysX3CommonCHECKED_x86.lib")
#pragma comment(lib, "PhysX3ExtensionsCHECKED.lib")
#pragma comment(lib, "PxTask.lib")


void StepPhysX();
void initPhysX();
void getColumnMajor(PxMat33, PxVec3, float*);
void DrawBox(PxShape*);
void DrawShape(PxShape*);
void DrawActor(PxRigidActor*);
void RenderActors();
void destoryPhysX();
void OnShutdown();
void OnRender();

#endif