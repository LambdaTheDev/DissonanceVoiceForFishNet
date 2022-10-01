using FishNet.Object;
using UnityEngine;

namespace Dissonance.Integrations.FishNet.Demos.Players
{
    public class DemoPlayerCharacterController : NetworkBehaviour
    {
        public float speed = 10;
        
        private void Update()
        {
            if(!IsOwner) return;

            float wayX = 0;
            float wayY = 0;

            if (Input.GetKey(KeyCode.W))
                wayY = 1;
            else if (Input.GetKey(KeyCode.S))
                wayY = -1;
            else if (Input.GetKey(KeyCode.A))
                wayX = -1;
            else if (Input.GetKey(KeyCode.D))
                wayX = 1;

            Vector3 position = transform.position;
            position.x += Time.deltaTime * wayX * speed;
            position.z += Time.deltaTime * wayY * speed;

            transform.position = position;

            if (Input.GetKeyDown(KeyCode.L))
            {
                ServerRpcForceMute();
            }
        }

        [ServerRpc]
        private void ServerRpcForceMute()
        {
            
        }
    }
}