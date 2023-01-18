using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.LowLevel;

namespace Game
{
    public interface ILoopSystem
    {
        [MustUseReturnValue]
        IDisposable Start(Action updatable);
    }
    
    internal class LoopSystem<TUpdateGroup> : ILoopSystem
    {
        private UpdatableSlot[] _updatables = new UpdatableSlot[1000];
        private readonly List<int> _updatablesToRemove = new();
        private int _count;

        private void OnUpdate()
        {
            for (int i = 0; i < _updatablesToRemove.Count; i++)
            {
                var updatableIndex = _updatablesToRemove[i];
                RemoveInReal(updatableIndex);
            }
            _updatablesToRemove.Clear();
            
            for (int i = 0; i < _count; i++)
            {
                var updatableSlot = _updatables[i];
                updatableSlot.Updatable();
            }
        }

        public IDisposable Start(Action updatable)
        {
            if (_count == _updatables.Length)
            {
                Array.Resize(ref _updatables, (int) (_count * 1.5f));
            }

            var updatableSlot = new UpdatableSlot(this, updatable, _count);
            _updatables[_count] = updatableSlot;

            if (_count == 0)
            {
                PlayerLoopExtensions.ModifyCurrentPlayerLoop((ref PlayerLoopSystem rootSystem) =>
                {
                    ref var groupSystem = ref rootSystem.GetSystem<TUpdateGroup>();
                    groupSystem.AddSystem<LoopSystem<TUpdateGroup>>(OnUpdate);
                });
            }
            
            _count += 1;

            return updatableSlot.Registration;
        }

        private void RemoveInReal(int index)
        {
            var lastUpdatableIndex = _count - 1;
            if (index == lastUpdatableIndex)
            {
                _updatables[index] = default;
            }
            else
            {
                ref var lastUpdatable = ref _updatables[lastUpdatableIndex];
                lastUpdatable.Registration.Index = index;
                _updatables[index] = lastUpdatable;
                _updatables[lastUpdatableIndex] = default;
            }

            if (_count == 0)
            {
                PlayerLoopExtensions.ModifyCurrentPlayerLoop((ref PlayerLoopSystem rootSystem) =>
                {
                    var groupSystem = rootSystem.GetSystem<TUpdateGroup>();
                    groupSystem.RemoveSystem<LoopSystem<TUpdateGroup>>(false);
                });
            }

            _count -= 1;
        }

        private void RemoveAt(int index)
        {
            _updatablesToRemove.Add(index);
        }
        
        private struct UpdatableSlot
        {
            public readonly Action Updatable;
            public readonly UpdatableRegistration Registration;

            public UpdatableSlot(LoopSystem<TUpdateGroup> loopSystem, Action updatable, int registration)
            {
                Updatable = updatable;
                Registration = new UpdatableRegistration(loopSystem, registration);
            }
        }

        private class UpdatableRegistration : IDisposable
        {
            private readonly LoopSystem<TUpdateGroup> _loopSystem;
            internal int Index;

            public UpdatableRegistration(LoopSystem<TUpdateGroup> loopSystem, int index)
            {
                _loopSystem = loopSystem;
                Index = index;
            }
            
            public void Dispose()
            {
                if (Index < 0)
                {
                    return;
                }
                
                _loopSystem.RemoveAt(Index);
                Index = -1;
            }
        }
    }
}