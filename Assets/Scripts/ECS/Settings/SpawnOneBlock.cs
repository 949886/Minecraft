using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;

namespace Minecraft
{
    public class SpawnOneBlock : MonoBehaviour
    {

        public static EntityArchetype BlockArchetype;

        [Header("Mesh Info")]
        public Mesh blockMesh;

        [Header("Nature Block Type")]
        public Material blockMaterial;

        public EntityManager manager;
        public Entity entities;
        public GameObject Prefab_ref;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {

            EntityManager manager = World.Active.EntityManager;

            // Create an archetype for basic blocks.
            BlockArchetype = manager.CreateArchetype(
                typeof(Translation)
            );
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        void Start()
        {
            manager = World.DefaultGameObjectInjectionWorld.EntityManager;

//            Entity entities = manager.CreateEntity(BlockArchetype);
//            manager.SetComponentData(entities, new Translation { Value = new int3(2, 0, 0) });
//            manager.AddComponentData(entities, new BlockTag { });
//
//            manager.AddSharedComponentData(entities, new RenderMesh
//            {
//                mesh = blockMesh,
//                material = blockMaterial
//            });


            //use prefab to create a entity
            if (Prefab_ref)
            {
//                NativeArray<Entity> entityArray = new NativeArray<Entity>(1, Allocator.Temp);

                var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefab_ref, World.Active);

                // Efficiently instantiate a bunch of entities from the already converted entity prefab
                var instance = manager.Instantiate(prefab);

                // Place the instantiated entity in a grid with some noise
                manager.SetComponentData(instance, new Translation { Value = new int3(0, 0, 0) });
                manager.AddComponentData(instance, new BlockTag { });

                //                manager.Instantiate(entities);
                //                manager.SetComponentData(entityArray[0], new Translation { Value = new float3(4, 0f, 0f) });
                //                entityArray.Dispose();
            }
        }
    }
}
