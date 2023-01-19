using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Game
{
    public class DisableGeneralUpdate : MonoBehaviour
    {
        private void Start()
        {
            PlayerLoopExtensions.ModifyCurrentPlayerLoop((ref PlayerLoopSystem system) =>
            {
                system.RemoveSystem<Update.ScriptRunBehaviourUpdate>();
            });
        }

        private void Update()
        {
            Debug.unityLogger.Log(nameof(DisableGeneralUpdate), "Unity Update", this);
        }
    }
}