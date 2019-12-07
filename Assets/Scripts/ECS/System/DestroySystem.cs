using System.Collections.Generic;
using Sora.Core.Dispatch;
using Unity.Collections;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

namespace Minecraft
{
//    public class DestroySystem : ComponentSystem
//    {
//        protected override void OnUpdate()
//        {
//            var destroyList = Entities.WithAll<DestroyTag, Translation>().ToEntityQuery().ToEntityArray(Allocator.TempJob);
//
//            Entities
//                .WithAll<BlockTag>()
//                .ForEach((Entity e, ref Translation t0) =>
//                {
//                    for (int i = 0; i < destroyList.Length; i++)
//                    {
//                        var d = destroyList[i];
//                        var t1 = EntityManager.GetComponentData<Translation>(d);
//
//                        Vector3 offset = t0.Value - t1.Value;
//                        float sqrLen = offset.sqrMagnitude;
//                        if (sqrLen == 0)
//                        {
//                            Debug.Log($"Delete");
//                            PostUpdateCommands.DestroyEntity(d);
//                            PostUpdateCommands.DestroyEntity(e);
//                        }
//                    }
//                });
//        }
//    }



//    [UpdateInGroup(typeof(SimulationSystemGroup))]
//    public class DestroySystem : JobComponentSystem
//    {
//        // BeginInitializationEntityCommandBufferSystem is used to create a command buffer which will then be played back
//        // when that barrier system executes.
//        //
//        // Though the instantiation command is recorded in the SpawnJob, it's not actually processed (or "played back")
//        // until the corresponding EntityCommandBufferSystem is updated. To ensure that the transform system has a chance
//        // to run on the newly-spawned entities before they're rendered for the first time, the SpawnerSystem_FromEntity
//        // will use the BeginSimulationEntityCommandBufferSystem to play back its commands. This introduces a one-frame lag
//        // between recording the commands and instantiating the entities, but in practice this is usually not noticeable.
//        //
//        BeginInitializationEntityCommandBufferSystem CommandBufferSystem;
//
//        protected override void OnCreate()
//        {
//            // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
//            CommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
//        }
//
//        protected override JobHandle OnUpdate(JobHandle inputDependencies)
//        {
//            // Instead of performing structural changes directly, a Job can add a command to an EntityCommandBuffer to
//            // perform such changes on the main thread after the Job has finished. Command buffers allow you to perform
//            // any, potentially costly, calculations on a worker thread, while queuing up the actual insertions and
//            // deletions for later.
//            var commandBuffer = CommandBufferSystem.CreateCommandBuffer().ToConcurrent();
//
//            // Schedule the job that will add Instantiate commands to the EntityCommandBuffer.
//            // Since this job only runs on the first frame, we want to ensure Burst compiles it before running to get the best performance (3rd parameter of WithBurst)
//            // The actual job will be cached once it is compiled (it will only get Burst compiled once).
//            var destroyList = new List<Translation>();
//            var createDestroyListJobHandle = Entities
////                .WithName("DestroyTag")
//                .WithAll<DestroyTag>()
//                .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
//                .ForEach((Entity e, int entityInQueryIndex, ref Translation c0) =>
//                {
//                    Debug.Log($"{c0.Value}");
////                    destroyList.Add(c0);
//                    commandBuffer.DestroyEntity(entityInQueryIndex, e);
//                }).Schedule(inputDependencies);
//
//            var jobHandle = Entities
//                .WithAll<BlockTag>()
//                .WithBurst(FloatMode.Default, FloatPrecision.Standard, true)
//                .ForEach((Entity e, int entityInQueryIndex, ref Translation t0) =>
//                {
//                    for (int i = 0; i < destroyList.Count; i++)
//                    {
//                        var t1 = destroyList[i];
//
//                        Vector3 offset = t0.Value - t1.Value;
//                        float sqrLen = offset.sqrMagnitude;
//                        if (sqrLen == 0)
//                        {
//                            commandBuffer.DestroyEntity(entityInQueryIndex, e);
//                        }
//                        //for (int i = 0; i < sourceBlock.Length; i++)
//                        //                    {
//                        //                        for (int j = 0; j < targetBlocks.Length; j++)
//                        //                        {
//                        //                            Vector3 offset = targetBlocks.positions[j].Value - sourceBlock.positions[i].Value;
//                        //                            float sqrLen = offset.sqrMagnitude;
//                        //
//                        //                            //find the block to destroy
//                        //                            if (sqrLen == 0)
//                        //                            {
//                        //                                //remove the plant from the surface;
//                        //                                for (int k = 0; k < surfaceplants.Length; k++)
//                        //                                {
//                        //                                    float3 tmpPos = new float3(surfaceplants.positions[k].Value.x, surfaceplants.positions[k].Value.y + Vector3.down.y, surfaceplants.positions[k].Value.z);
//                        //                                    offset = targetBlocks.positions[j].Value - tmpPos;
//                        //                                    sqrLen = offset.sqrMagnitude;
//                        //
//                        //                                    if (sqrLen == 0)
//                        //                                    {
//                        //                                        PostUpdateCommands.DestroyEntity(surfaceplants.entity[k]);
//                        //                                    }
//                        //                                }
//                        //
//                        //                                //remove blocks
//                        //                                PostUpdateCommands.DestroyEntity(sourceBlock.entity[i]);
//                        //                                PostUpdateCommands.DestroyEntity(targetBlocks.entity[j]);
//                        //                            }
//                        //                        }
//                        //                    }
//                    }
//                }).Schedule(createDestroyListJobHandle);
//
//            // SpawnJob runs in parallel with no sync point until the barrier system executes.
//            // When the barrier system executes we want to complete the SpawnJob and then play back the commands
//            // (Creating the entities and placing them). We need to tell the barrier system which job it needs to
//            // complete before it can play back the commands.
//            CommandBufferSystem.AddJobHandleForProducer(jobHandle);
//
//            return jobHandle;
//        }
//    }


    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class DestroySystem : JobComponentSystem
    {
        [BurstCompile]
        struct DestoryBlockJob : IJobForEachWithEntity<Translation, BlockTag>
        {
            public EntityCommandBuffer.Concurrent CommandBuffer;
            public Translation Target;

            public void Execute(Entity entity, int index, ref Translation t1, [ReadOnly] ref BlockTag c2)
            {
                Vector3 offset = Target.Value - t1.Value;
                float sqrLen = offset.sqrMagnitude;
                if (sqrLen == 0)
                {
                    DispatchQueue.Main.Async(delegate
                    {
                        Debug.Log("Deleted Block.");
                    });
                    CommandBuffer.DestroyEntity(index, entity);
                }
            }
        }

        [BurstCompile]
        struct DestoryPlantJob : IJobForEachWithEntity<Translation, SurfacePlantTag>
        {
            public EntityCommandBuffer.Concurrent CommandBuffer;
            public Translation Target;

            public void Execute(Entity entity, int index, ref Translation t1, [ReadOnly] ref SurfacePlantTag c2)
            {
                float3 tmpPos = new float3(t1.Value.x, t1.Value.y + Vector3.down.y, t1.Value.z);
                Vector3 offset = Target.Value - tmpPos;
                float sqrLen = offset.sqrMagnitude;
                if (sqrLen == 0)
                {
                    Debug.Log("Deleted Plant.");
                    CommandBuffer.DestroyEntity(index, entity);
                }
            }
        }


        public static Queue<Translation> destroyQueue = new Queue<Translation>();
        public EndSimulationEntityCommandBufferSystem CommandBufferSystem;

        protected override void OnCreate()
        {
            CommandBufferSystem = World
                .DefaultGameObjectInjectionWorld
                .GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        [BurstCompile]
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var commandBuffer = CommandBufferSystem.CreateCommandBuffer().ToConcurrent();

            if (destroyQueue.Count != 0)
            {
                var target = destroyQueue.Dequeue();

                DestoryBlockJob destoryBlockJob = new DestoryBlockJob()
                {
                    CommandBuffer = commandBuffer,
                    Target = target
                };
                var destoryBlockJobHandle = destoryBlockJob.Schedule(this, inputDependencies);

                DestoryPlantJob destoryPlantJob = new DestoryPlantJob()
                {
                    CommandBuffer = commandBuffer,
                    Target = target
                };
                var destoryPlantJobHandle = destoryPlantJob.Schedule(this, destoryBlockJobHandle);

                JobHandle finalDependencies = JobHandle.CombineDependencies(destoryBlockJobHandle, destoryPlantJobHandle);
                CommandBufferSystem.AddJobHandleForProducer(finalDependencies);

                return finalDependencies;
            }
            else
            {
                return inputDependencies;
            };
        }

    }

}
