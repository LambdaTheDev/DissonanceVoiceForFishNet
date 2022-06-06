using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SceneManager = FishNet.Managing.Scened.SceneManager;

namespace Assets.Dissonance.Integrations.FishNet.Demo
{
    public class MenuController
        : MonoBehaviour
    {
        private NetworkManager _networkManager;
        public Text ErrorMessage;

        public GameObject[] HideOnClick;

        private void Start()
        {
            _networkManager = FindObjectOfType<NetworkManager>();
            _networkManager.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
            _networkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        }

        private void HideAll()
        {
            if (HideOnClick == null)
                return;

            foreach (var item in HideOnClick)
                item.SetActive(false);
        }

        private void ShowError(string message)
        {
            if (ErrorMessage == null)
                return;

            ErrorMessage.text = message;
            ErrorMessage.gameObject.SetActive(true);
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
