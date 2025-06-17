using Unity.Netcode;
using UnityEngine;

namespace KillCam
{
    public class UnityLoop : MonoBehaviour, IGameLoop
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

        private void Update()
        {
            OnUpdate();
        }

        private void LateUpdate()
        {
            OnLateUpdate();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();
        }

        public void OnUpdate()
        {
            server?.OnUpdate();
            client?.OnUpdate();
        }

        public void OnLateUpdate()
        {
            server?.OnLateUpdate();
            client?.OnLateUpdate();
        }

        public void OnFixedUpdate()
        {
            server?.OnFixedUpdate();
            client?.OnFixedUpdate();
        }

        private void OnDestroy()
        {
            server?.Clear();
            client?.Clear();
        }
    }

    public interface IGameLoop
    {
        void OnUpdate();
        void OnLateUpdate();
        void OnFixedUpdate();
    }
}