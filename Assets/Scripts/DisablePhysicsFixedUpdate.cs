using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Game
{
    public class DisablePhysicsFixedUpdate : MonoBehaviour
    {
        private void Start()
        {
            PlayerLoopExtensions.ModifyCurrentPlayerLoop((ref PlayerLoopSystem system) =>
            {
                system.RemoveSystem<FixedUpdate.PhysicsFixedUpdate>();
            });
        }
    }
}