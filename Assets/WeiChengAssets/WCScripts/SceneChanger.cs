/*
* Author: Cheang Wei Cheng
* Date: 06/02/2026
* Description: This script is responsible for changing scenes in the game. It is attached to a GameObject in the scene and can be called to load a new scene.
* It also ensures that certain important GameObjects, such as the player and UI canvases, are not destroyed when loading a new scene, allowing for a seamless transition between scenes without losing important data or references.
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject gameManagers;
    [SerializeField] private Canvas InventoryCanvas;
    [SerializeField] private Canvas LevelCanvas;

    /// <summary>
    /// In Start(), the script attempts to auto-assign references for the player, canvases, and game managers by finding them in the scene by name, since these variables are in a different scene and will not be assigned in the inspector.
    /// It also checks if the current active scene is the Bootstrap scene, and if so, it immediately loads the GrobenLobby scene to start the game.
    /// </summary>
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

    /// <summary>
    /// This method is called to change the scene to the specified scene name.
    /// It uses SceneManager.LoadScene to load the new scene, and calls DontDestroyOnLoad on the player, canvases, and game managers to ensure they persist across scenes.
    /// After loading the new scene, it also resets the player's position to the origin to ensure they start in a consistent location in the new scene.
    /// </summary>
    /// <param name="sceneToLoad"></param>
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