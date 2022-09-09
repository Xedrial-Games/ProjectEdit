using System;
using System.Collections.Generic;

using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

using MoonSharp.Interpreter;

using ProjectEdit.Entities;

namespace ProjectEdit.Scripting
{
    [MoonSharpUserData]
    internal class InternalCalls
    {
        private static Dictionary<Entity, Script> Scripts => ScriptsSystem.Scripts;
        private static EntityManager EntityManager => EntitiesManager.EntityManager;
        private static ref EntityCommandBuffer CommandBuffer => ref EntitiesManager.CommandBuffer;

        public static Entity CreateEmptyEntity()
        {
            return EntitiesManager.CreateEmptyEntity();
        }

        public static Entity CreateEntity()
        {
            return EntitiesManager.ScheduleCreateEntity();
        }

        public static bool Entity_HasComponent(Entity entity, string componentType)
        {
            return EntityManager.HasComponent(entity, StringToComponent(componentType));
        }

        public static void AddComponent(Entity entity, string componentName)
        {
            switch (componentName)
            {
                case "Transform":
                    CommandBuffer.AddComponent<Translation>(entity);
                    CommandBuffer.AddComponent<Rotation>(entity);
                    CommandBuffer.AddComponent<Scale>(entity);
                    CommandBuffer.SetComponent(entity, new Scale { Value = 1 });
                    break;
            }
            
        }

        public static Table GetTranslation(Entity entity)
        {
            float3 value = EntityManager.GetComponentData<Translation>(entity).Value;
            var table = new Table(Scripts[entity].ScriptHandle)
            {
                ["x"] = value.x,
                ["y"] = value.y,
                ["z"] = value.z
            };

            return table;
        }

        public static void SetTranslation(Entity entity, Table vector)
        {
            var value = new float3
            (
                (float)(double)vector["x"],
                (float)(double)vector["y"],
                (float)(double)vector["z"]
            );

            CommandBuffer.SetComponent(entity, new Translation { Value = value });
        }

        public static Table GetRotation(Entity entity)
        {
            float4 value = EntityManager.GetComponentData<Rotation>(entity).Value.value;
            var table = new Table(Scripts[entity].ScriptHandle)
            {
                ["x"] = value.x,
                ["y"] = value.y,
                ["z"] = value.z,
                ["w"] = value.w
            };

            return table;
        }

        public static void SetRotation(Entity entity, Table vector)
        {
            var value = new quaternion
            (
                (float)(double)vector["x"],
                (float)(double)vector["y"],
                (float)(double)vector["z"],
                (float)(double)vector["w"]
            );

            CommandBuffer.SetComponent(entity, new Rotation { Value = value });
        }

        private static Type StringToComponent(string componentName)
        {
            switch (componentName)
            {
                case "Transform":
                    return typeof(Translation);
            }

            return null;
        }
    }
}
