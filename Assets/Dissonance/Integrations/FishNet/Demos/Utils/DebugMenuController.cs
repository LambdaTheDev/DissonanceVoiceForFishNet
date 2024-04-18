using System.Collections;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.UI;

namespace Dissonance.Integrations.FishNet.Demos.Utils
{
    public class DebugMenuController : MonoBehaviour
    {
        public string gameWorldSceneName;
        public Text errorMessage;
        public GameObject[] hideOnClick;
        
        private NetworkManager _networkManager;
        private bool _startingAsHost;

        
        private void Start()
        {
            _networkManager = InstanceFinder.NetworkManager;
            _networkManager.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
            _networkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        }

        private void OnDestroy()
        {
            if (_networkManager == null) return;
            _networkManager.ServerManager.OnServerConnectionState -= ServerManager_OnServerConnectionState;
            _networkManager.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;
        }

        private void HideAll()
        {
            if (hideOnClick == null)
                return;

            foreach (var item in hideOnClick)
                item.SetActive(false);
        }

        private void ShowError(string message)
        {
            if (errorMessage == null)
                return;

            errorMessage.text = message;
            errorMessage.gameObject.SetActive(true);
        }

        public void OnClickStartClient()
        {
            _networkManager.ClientManager.StartConnection();
            HideAll();
        }

        public void OnClickStartServer()
        {
            _networkManager.ServerManager.StartConnection();
            // _networkManager.ClientManager.StartConnection(); RIGHT NOW EXAMPLES WILL BE SERVER/CLIENT ONLY
            HideAll();
        }

        public void OnClickStartHost()
        {
            _startingAsHost = true;
            _networkManager.ServerManager.StartConnection();
            _networkManager.ClientManager.StartConnection();
            
            HideAll();
        }

        private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj)
        {
        }

        private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs obj)
        {
            if (obj.ConnectionState != LocalConnectionState.Started) return;
            if (_startingAsHost && !_networkManager.IsHostStarted)
            {
                StartCoroutine(DelayedLoadScene());
                return;
            }

            LoadGameScene();
        }

        private IEnumerator DelayedLoadScene()
        {
            int attempts = 10;
            
            while (attempts > 0 && !_networkManager.IsHostStarted)
            {
                attempts--;
                yield return new WaitForSeconds(1f);
            }

            if (attempts == 0)
            {
                Debug.LogError("Could not start client in host mode!");
            }
            else
            {
                LoadGameScene();
            }
        }

        private void LoadGameScene()
        {
            var scene = SceneManager.GetScene(gameWorldSceneName);
            if (!scene.IsValid())
                ShowError($@"Cannot load scene '{gameWorldSceneName}' - ensure it is added to the build settings");
            
            var sld = new SceneLoadData(gameWorldSceneName);
            sld.ReplaceScenes = ReplaceOption.All;

            _networkManager.SceneManager.LoadGlobalScenes(sld);
        }
    }
}
