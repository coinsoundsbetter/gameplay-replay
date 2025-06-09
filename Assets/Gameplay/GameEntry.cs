using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEntry : MonoBehaviour {
    
    private void Start() {
        Launch.Create();
    }

    private void OnGUI() {
        if (GUILayout.Button("Host")) {
            Launch.Singleton.StartAsServer = true;
            Launch.Singleton.StartAsClient = true;
            Launch.Singleton.UseReplay = true;
            Launch.Singleton.StartGameMode = GameMode.Battle1V1;
            SceneManager.LoadScene("GameplayMain", LoadSceneMode.Single);
        }
    }
}
