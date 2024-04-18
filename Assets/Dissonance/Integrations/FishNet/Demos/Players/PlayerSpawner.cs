using FishNet;
using FishNet.Connection;
using UnityEngine;

namespace Dissonance.Integrations.FishNet.Demos.Players
{
    public class PlayerSpawner : MonoBehaviour
    {
        public GameObject playerPrefab;
        
        private void Awake()
        {
            InstanceFinder.SceneManager.OnClientLoadedStartScenes += SceneManagerOnOnClientLoadedStartScenes;
            if(InstanceFinder.IsHostStarted) SceneManagerOnOnClientLoadedStartScenes(InstanceFinder.ClientManager.Connection, true);
        }

        private void SceneManagerOnOnClientLoadedStartScenes(NetworkConnection arg1, bool arg2)
        {
            GameObject spawnedPrefab = Instantiate(playerPrefab);
            InstanceFinder.ServerManager.Spawn(spawnedPrefab, arg1);
        }
    }
}