using FishNet.Managing;
using UnityEngine;

namespace KillCam
{
    public class UnityLoop : MonoBehaviour
    {
        private NetworkManager networkManager;
        private Client client;
        private Server server;

        private void Awake()
        {
            var asset = Resources.Load("NetworkManager");
            var instance = (GameObject)Instantiate(asset);
            networkManager = instance.GetComponent<NetworkManager>();
        }

        private void Start()
        {
            if (LaunchData.Instance.IsServer)
            {
                server = new Server();
                server.Init();
            }

            if (LaunchData.Instance.IsClient)
            {
                client = new Client();
                client.Init();
            }
        }

        private void OnDestroy()
        {
            server?.Clear();
            client?.Clear();
        }
    }
}