using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;

namespace Minecraft
{
    public class SpawnOneBlockFromMesh : MonoBehaviour
    {

        public static EntityArchetype BlockArchetype;

        [Header("Mesh Info")]
        public Mesh blockMesh;

        [Header("Nature Block Type")]
        public Material blockMaterial;

        public EntityManager manager;
        public Entity entities;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {

            EntityManager manager = World.Active.EntityManager;

            // Create an archetype for basic blocks.
            BlockArchetype = manager.CreateArchetype(
                typeof(Translation),
                typeof(Rotation),
                typeof(LocalToWorld)
            );
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        void Start()
        {
            manager = World.Active.EntityManager;

            Entity entities = manager.CreateEntity(BlockArchetype);
            manager.SetComponentData(entities, new Translation { Value = new int3() });
            manager.SetComponentData(entities, new Rotation { Value = new quaternion() });
            manager.AddComponentData(entities, new BlockTag { });
            manager.AddSharedComponentData(entities, new RenderMesh
            {
                mesh = blockMesh,
                material = blockMaterial
            });

//            manager.AddComponentData(entities, new RenderBounds
//            {
//                Value = new AABB()
//                {
//                    Center = new float3(0, 0, 0),
//                    Extents = new float3(0.5f, 0.5f, 0.5f)
//                }
//            });
//
//            manager.AddComponentData(entities, new ChunkWorldRenderBounds
//            {
//                Value = new AABB()
//                {
//                    Center = new float3(0, 0, 0),
//                    Extents = new float3(0.5f, 0.5f, 0.5f)
//                }
//            });
//
//            manager.AddComponentData(entities, new WorldRenderBounds
//            {
//                Value = new AABB()
//                {
//                    Center = new float3(0, 0, 0),
//                    Extents = new float3(0.5f, 0.5f, 0.5f)
//                }
//            });
//
//            manager.AddComponentData(entities, new LocalToWorld());

            manager.Instantiate(entities);
        }
    }
}
