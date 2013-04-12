
#include <PxPhysicsAPI.h>
#include <extensions\PxExtensionsAPI.h>
#include <extensions\PxDefaultErrorCallback.h>
#include <extensions\PxDefaultAllocator.h>
#include <extensions\PxDefaultSimulationFilterShader.h>
#include <extensions\PxDefaultCpuDispatcher.h>
#include <extensions\PxShapeExt.h>
#include <foundation\PxMat33.h>
#include <extensions\PxSimpleFactory.h>
#include <iostream>
#include <vector>

using namespace std;
using namespace physx;


static PxPhysics *physics = NULL;
PxFoundation *foundation = NULL;
PxScene* dxScene = NULL;
PxRigidActor *box;
static PxDefaultAllocator gDefaultAllocatorCallback;
static PxDefaultErrorCallback gDefaultErrorCallback;
static PxSimulationFilterShader gDefaultFilterShader=PxDefaultSimulationFilterShader;

PxReal timestep = 1.0f/60.0f;

void StepPhysX(){
	dxScene->simulate(timestep);
	
	while(!dxScene->fetchResults()){

	}
}

void initPhysX() {
	foundation = PxCreateFoundation(PX_PHYSICS_VERSION, gDefaultAllocatorCallback, gDefaultErrorCallback);
	physics = PxCreatePhysics(PX_PHYSICS_VERSION, *foundation, PxTolerancesScale());

	PxInitExtensions(*physics);
	
	PxSceneDesc sceneDesc(physics->getTolerancesScale());
	sceneDesc.gravity = PxVec3(0.0f,-9.8f,0.0f);

	if(!sceneDesc.cpuDispatcher)
	{
		PxDefaultCpuDispatcher* mCpuDispatcher = PxDefaultCpuDispatcherCreate(1);
		sceneDesc.cpuDispatcher = mCpuDispatcher;
	}

	if(!sceneDesc.filterShader)
		sceneDesc.filterShader  = gDefaultFilterShader;
	
	dxScene = physics->createScene(sceneDesc);

	//get objects from sceene graph and create the physics bodys.
	PxMaterial* mMaterial = physics->createMaterial(0.5,0.5,0.5);

	PxReal d = 0.0f;
	PxTransform pose = PxTransform(PxVec3(0.0f, 0, 0.0f),PxQuat(PxHalfPi, PxVec3(0.0f, 0.0f, 1.0f)));

	PxRigidStatic* plane = physics->createRigidStatic(pose);
	dxScene->addActor(*plane);

	PxReal density = 1.0f;
	PxTransform transform(PxVec3(0.0f, 10.0f, 0.0f), PxQuat::createIdentity());
	PxVec3 dimensions(0.5,0.5,0.5);
	PxBoxGeometry geometry(dimensions);
	
	PxRigidDynamic *actor = PxCreateDynamic(*physics, transform, geometry, *mMaterial, density);
	actor->setAngularDamping(0.75);
	actor->setLinearVelocity(PxVec3(0,0,0)); 
	dxScene->addActor(*actor);

	box = actor;
}

void getColumnMajor(PxMat33 m, PxVec3 t, float* mat) {
   mat[0] = m.column0[0];
   mat[1] = m.column0[1];
   mat[2] = m.column0[2];
   mat[3] = 0;

   mat[4] = m.column1[0];
   mat[5] = m.column1[1];
   mat[6] = m.column1[2];
   mat[7] = 0;

   mat[8] = m.column2[0];
   mat[9] = m.column2[1];
   mat[10] = m.column2[2];
   mat[11] = 0;

   mat[12] = t[0];
   mat[13] = t[1];
   mat[14] = t[2];
   mat[15] = 1;
}

void DrawBox(PxShape* pShape) {
	PxTransform pT = PxShapeExt::getGlobalPose(*pShape);
	PxBoxGeometry bg;
	pShape->getBoxGeometry(bg);
	PxMat33 m = PxMat33(pT.q);
	float mat[16];
	//might have to fix the whole app due to quadturians
	getColumnMajor(m,pT.q.getBasisVector0(),mat);
	//d3d render here
}
void DrawShape(PxShape* shape) 
{ 
	PxGeometryType::Enum type = shape->getGeometryType();
	switch(type) 
	{          
		case PxGeometryType::eBOX:
			DrawBox(shape);
		break;
	} 
} 

void DrawActor(PxRigidActor* actor) 
{  
	PxU32 nShapes = actor->getNbShapes(); 
	PxShape** shapes=new PxShape*[nShapes];
	
	actor->getShapes(shapes, nShapes);     
	while (nShapes--) 
	{ 
		DrawShape(shapes[nShapes]); 
	} 
	delete [] shapes;
}
void RenderActors(){
	DrawActor(box);
}

void destoryPhysX(){
	 
printf("shutting down\n");
 physics->release();
 foundation->release();
}

void OnShutdown(){
	destoryPhysX();
}

void OnRender(){
	//update dx here
	if(dxScene)
		StepPhysX();

	RenderActors();
}

void PhysicsStart(){
	atexit(OnShutdown);
	initPhysX();
}