using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject gameManagers;
    [SerializeField] private Canvas InventoryCanvas;
    [SerializeField] private Canvas LevelCanvas;
    void Start()
    {
        // find unassigned variables by object name
        if (Player == null)
        {
            Player = GameObject.Find("XR Origin (XR Rig)");
        }
        if (canvas == null)
        {
            canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        }
        if (gameManagers == null)
        {
            gameManagers = GameObject.Find("Managers");
        }
        if (InventoryCanvas == null)
        {
            InventoryCanvas = GameObject.Find("InventoryCanvas").GetComponent<Canvas>();
        }
        if (LevelCanvas == null)
        {
            LevelCanvas = GameObject.Find("LevelCanvas").GetComponent<Canvas>();
        }
        // Integrate Bootstrap scene; if the player starts in it, load GrobenLobby scene
        if (SceneManager.GetActiveScene().name == "Bootstrap")
        {
            ChangeScene("GrobenLobby");
        }
    }
    public void ChangeScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
        DontDestroyOnLoad(Player);
        DontDestroyOnLoad(canvas.gameObject);
        DontDestroyOnLoad(gameManagers);
        DontDestroyOnLoad(InventoryCanvas.gameObject);
        DontDestroyOnLoad(LevelCanvas.gameObject);
        // place player at origin in new scene
        Player.transform.position = Vector3.zero;
    }
}