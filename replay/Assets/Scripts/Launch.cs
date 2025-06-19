using UnityEngine;
using UnityEngine.SceneManagement;

namespace KillCam
{
    public class Launch : MonoBehaviour
    {
        private void Start()
        {
            LaunchData.Create();
        }

        private void OnGUI()
        {
            bool isEnterMain = false;
            if (GUILayout.Button("Host"))
            {
                LaunchData.Instance.IsClient = true;
                LaunchData.Instance.IsServer = true;
                isEnterMain = true;
            }

            if (GUILayout.Button("Client"))
            {
                LaunchData.Instance.IsClient = true;
                isEnterMain = true;
            }

            if (GUILayout.Button("Server"))
            {
                LaunchData.Instance.IsServer = true;
                isEnterMain = true;
            }

            if (isEnterMain)
            {
                SceneManager.LoadScene("GameMain", LoadSceneMode.Single);
            }
        }
    }
}