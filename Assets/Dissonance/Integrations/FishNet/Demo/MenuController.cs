using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.UI;

namespace Dissonance.Integrations.FishNet.Demo
{
    public class MenuController : MonoBehaviour
    {
        public Text errorMessage;
        public GameObject[] hideOnClick;
        
        private NetworkManager _networkManager;

        
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
            _networkManager.ClientManager.StartConnection();
            HideAll();
        }

        private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj)
        {
        }


        private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs obj)
        {
            //var scene = SceneManager.GetScene("FishNet Dissonance GameWorld");
            //if (!scene.IsValid())
            //    ShowError("Cannot load scene 'FishNet Dissonance GameWorld' - ensure it is added to the build settings");

            var sld = new SceneLoadData("FishNet Dissonance GameWorld");
            sld.ReplaceScenes = ReplaceOption.All;

            _networkManager.SceneManager.LoadGlobalScenes(sld);
        }
    }
}
