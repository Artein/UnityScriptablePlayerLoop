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
        private UpdatableSlot[] _updatableSlots = new UpdatableSlot[1000];
        private readonly List<int> _updatableSlotsToRemove = new();
        private int _count_BF;

        private int Count
        {
            get => _count_BF;
            set
            {
                if (_count_BF == value)
                {
                    return;
                }
                
                var prevValue = _count_BF;
                _count_BF = value;

                if (value < prevValue && value == 0) // decreased to zero
                {
                    PlayerLoopExtensions.ModifyCurrentPlayerLoop((ref PlayerLoopSystem rootSystem) =>
                    {
                        var groupSystem = rootSystem.GetSystem<TUpdateGroup>();
                        groupSystem.RemoveSystem<LoopSystem<TUpdateGroup>>(false);
                    });
                }
                else if (value > prevValue && prevValue == 0) // increasing from zero
                {
                    PlayerLoopExtensions.ModifyCurrentPlayerLoop((ref PlayerLoopSystem rootSystem) =>
                    {
                        ref var groupSystem = ref rootSystem.GetSystem<TUpdateGroup>();
                        groupSystem.AddSystem<LoopSystem<TUpdateGroup>>(OnUpdate);
                    });
                }
            }
        }

        public IDisposable Start(Action updatable)
        {
            if (Count == _updatableSlots.Length)
            {
                Array.Resize(ref _updatableSlots, (int) (Count * 1.5f));
            }

            var updatableSlot = new UpdatableSlot(this, updatable, Count);
            _updatableSlots[Count] = updatableSlot;

            Count += 1;

            return updatableSlot.Registration;
        }

        private void OnUpdate()
        {
            for (int i = 0; i < _updatableSlotsToRemove.Count; i++)
            {
                var slotIndex = _updatableSlotsToRemove[i];
                RemoveInReal(slotIndex);
            }
            _updatableSlotsToRemove.Clear();
            
            for (int i = 0; i < Count; i++)
            {
                var updatableSlot = _updatableSlots[i];
                updatableSlot.Updatable();
            }
        }

        private void RemoveInReal(int index)
        {
            var lastUpdatableIndex = Count - 1;
            if (index < lastUpdatableIndex)
            {
                ref var lastUpdatable = ref _updatableSlots[lastUpdatableIndex];
                lastUpdatable.Registration.Index = index;
                _updatableSlots[index] = lastUpdatable;
            }
            
            _updatableSlots[lastUpdatableIndex] = default;
            Count -= 1;
        }

        private void RemoveAt(int index)
        {
            _updatableSlotsToRemove.Add(index);
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
                if (Index >= 0)
                {
                    _loopSystem.RemoveAt(Index);
                    Index = -1;
                }
            }
        }
    }
}