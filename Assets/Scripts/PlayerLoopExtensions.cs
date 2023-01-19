using System;
using UnityEngine.LowLevel;

namespace Game
{
    public static class PlayerLoopExtensions
    {
        public delegate void ModifyPlayerLoop(ref PlayerLoopSystem playerLoopSystem);

        public static void ModifyCurrentPlayerLoop(ModifyPlayerLoop modifyPlayerLoop)
        {
            var currentLoop = PlayerLoop.GetCurrentPlayerLoop();
            modifyPlayerLoop(ref currentLoop);
            PlayerLoop.SetPlayerLoop(currentLoop);
        }

        public static ref PlayerLoopSystem GetSystem<TSystem>(in this PlayerLoopSystem system)
        {
            var type = typeof(TSystem);
            for (int i = 0; i < system.subSystemList.Length; i++)
            {
                ref var subSystem = ref system.subSystemList[i];
                if (subSystem.type == type)
                {
                    return ref subSystem;
                }
            }

            throw new InvalidOperationException($"Missing '{type.Namespace}' system");
        }

        public static void AddSystem<TSystem>(this ref PlayerLoopSystem system, PlayerLoopSystem.UpdateFunction action)
        {
            var type = typeof(TSystem);
            var newSystem = new PlayerLoopSystem { type = type, updateDelegate = action };
            system.subSystemList = system.subSystemList.Add(newSystem);
        }

        public static void RemoveSystem<TSystem>(this ref PlayerLoopSystem system, bool recursive = true)
        {
            if (system.subSystemList == null)
            {
                return;
            }

            var type = typeof(TSystem);
            for (int i = system.subSystemList.Length - 1; i >= 0; i--)
            {
                ref var subSystem = ref system.subSystemList[i];
                if (subSystem.type == type)
                {
                    system.subSystemList = system.subSystemList.RemoveAt(i);
                }
                else if (recursive)
                {
                    subSystem.RemoveSystem<TSystem>(recursive: true);
                }
            }
        }

        public static T[] Add<T>(this T[] array, T value)
        {
            if (array == null)
            {
                array = new T[1];
            }
            else
            {
                Array.Resize(ref array, array.Length + 1);
            }

            array[^1] = value;
            return array;
        }

        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be positive number");
            }

            var newArray = new T[array.Length - 1];
            if (index > 0)
            {
                Array.Copy(array, 0, newArray, 0, index);
            }

            if (index < newArray.Length)
            {
                Array.Copy(array, index + 1, newArray, index, array.Length - (index + 1));
            }

            return newArray;
        }
    }
}