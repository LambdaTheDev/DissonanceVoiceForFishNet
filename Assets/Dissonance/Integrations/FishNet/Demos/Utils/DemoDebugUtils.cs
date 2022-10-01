using System.Linq;
using System.Text;
using FishNet;
using UnityEngine;

namespace Dissonance.Integrations.FishNet.Demos.Utils
{
    internal class DemoDebugUtils : MonoBehaviour
    {
        private DissonanceComms _comms;

        private bool _muted;

        private void Awake()
        {
            _comms = FindObjectOfType<DissonanceComms>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                StringBuilder playerDebug = new StringBuilder();
                playerDebug.Append("Connected players:\n");
                foreach (var player in _comms.Players)
                {
                    playerDebug.Append(player.Name).Append(" ").Append(player.Tracker?.Type).Append("\n");
                }
                
                Debug.Log(playerDebug.ToString());
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                _muted = !_muted;
                _comms.Players.First(x => !x.IsLocalPlayer).IsLocallyMuted = _muted;
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                StringBuilder fnConnsDebug = new StringBuilder();
                fnConnsDebug.Append("Connected connections:\n");
                foreach (var client in InstanceFinder.ServerManager.Clients)
                {
                    fnConnsDebug.Append(client.Value.ClientId).Append('\n');
                }
                
                Debug.LogWarning(fnConnsDebug.ToString());
            }
        }
    }
}