using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Staff;
    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneToLoad);
        DontDestroyOnLoad(Player);
        DontDestroyOnLoad(Staff);
        // place player at origin and staff at (0,1,0.3) in new scene
        Player.transform.position = Vector3.zero;
        Staff.transform.position = new Vector3(0, 1, 0.3f);
    }
}
