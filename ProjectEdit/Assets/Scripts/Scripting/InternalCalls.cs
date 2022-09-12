using System.Collections.Generic;

using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

using MoonSharp.Interpreter;

using ProjectEdit.Entities;
using UnityEngine;

namespace ProjectEdit.Scripting
{
    [MoonSharpUserData]
    internal class InternalCalls
    {
        private static Dictionary<Entity, Script> Scripts => ScriptsSystem.Scripts;
        private static EntityManager EntityManager => EntitiesManager.EntityManager;

        public static Entity CreateEmptyEntity()
        {
            return EntitiesManager.CreateEmptyEntity();
        }

        public static Entity CreateEntity()
        {
            return EntitiesManager.CreateEntity();
        }

        public static bool HasComponent(Entity entity, string componentTypeName)
        {
            ComponentType componentType = StringToComponent(componentTypeName);
            return componentType != null && EntityManager.HasComponent(entity, componentType);
        }

        public static void AddComponent(Entity entity, string componentName)
        {
            switch (componentName)
            {
                case "Transform":
                    EntityManager.AddComponent<Translation>(entity);
                    EntityManager.AddComponent<Rotation>(entity);
                    EntityManager.AddComponent<Scale>(entity);
                    EntityManager.SetComponentData(entity, new Scale { Value = 1 });
                    break;
                case "SpriteRenderer":
                    EntityManager.AddComponent<SpriteRenderer>(entity);
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

            EntityManager.SetComponentData(entity, new Translation { Value = value });
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

            EntityManager.SetComponentData(entity, new Rotation { Value = value });
        }


        public static Table SpriteRenderer_GetColor(Entity entity)
        {
            Color value = EntityManager.GetComponentObject<SpriteRenderer>(entity).color;
            var table = new Table(Scripts[entity].ScriptHandle)
            {
                [1] = value.r,
                [2] = value.g,
                [3] = value.b,
                [4] = value.a
            };

            return table;
        }

        public static void SpriteRenderer_SetColor(Entity entity, Table vector)
        {
            var value = new Color
            (
                (float)(double)vector[1],
                (float)(double)vector[2],
                (float)(double)vector[3],
                (float)(double)vector[4]
            );

            EntityManager.GetComponentObject<SpriteRenderer>(entity).color = value;
        }

        private static ComponentType StringToComponent(string componentName)
        {
            return componentName switch
            {
                "Transform" => typeof(Translation),
                "SpriteRenderer" => typeof(SpriteRenderer),
                _ => null
            };
        }
    }
}
