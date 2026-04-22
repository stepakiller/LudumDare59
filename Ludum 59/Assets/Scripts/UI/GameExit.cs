using UnityEngine;
using UnityEngine.SceneManagement;
public class GameExit : MonoBehaviour
{
    void Awake() => Application.targetFrameRate = 240;
    public void Exit() => Application.Quit();
    public void Restart() => SceneManager.LoadScene(0);
}
