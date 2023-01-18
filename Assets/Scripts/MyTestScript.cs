using System;
using UnityEngine;

namespace Game
{
    public class MyTestScript : MonoBehaviour
    {
        private IDisposable _updateRegistrationHandle;

        private void Start()
        {
            _updateRegistrationHandle = Loops.Update.Start(OnUpdate);
        }

        private void OnDestroy()
        {
            _updateRegistrationHandle.Dispose();
        }

        private void OnUpdate()
        {
            Debug.Log("Update", this);
        }
    }
}