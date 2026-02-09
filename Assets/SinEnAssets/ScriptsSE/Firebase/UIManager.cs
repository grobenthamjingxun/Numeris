using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    private GameObject loginCanvas;
    [SerializeField]
    private GameObject signupCanvas;
    [SerializeField]
    public GameObject inventoryCanvas;
    [SerializeField]
    private GameObject leaderboardCanvas;
    [SerializeField]
    private GameObject shopCanvas;
    [SerializeField]
    public GameObject levelCanvas;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ShowLogin();
    }

    // Helper method to safely set active
    private void SetCanvasActive(GameObject canvas, bool active)
    {
        if (canvas != null)
        {
            canvas.SetActive(active);
        }
    }

    public void ShowGame()
    {
        SetCanvasActive(loginCanvas, false);
        SetCanvasActive(signupCanvas, false);
        SetCanvasActive(inventoryCanvas, false);
        SetCanvasActive(leaderboardCanvas, false);
        SetCanvasActive(shopCanvas, false);
        SetCanvasActive(levelCanvas, false);
    }
    
    public void ShowLogin()
    {
        SetCanvasActive(loginCanvas, true);
        SetCanvasActive(signupCanvas, false);
        SetCanvasActive(inventoryCanvas, false);
        SetCanvasActive(leaderboardCanvas, false);
        SetCanvasActive(shopCanvas, false);
        SetCanvasActive(levelCanvas, false);
    }
    
    public void ShowSignup()
    {
        SetCanvasActive(loginCanvas, false);
        SetCanvasActive(signupCanvas, true);
        SetCanvasActive(inventoryCanvas, false);
        SetCanvasActive(leaderboardCanvas, false);
        SetCanvasActive(shopCanvas, false);
        SetCanvasActive(levelCanvas, false);
    }
    
    public void ShowInventory()
    {
        SetCanvasActive(loginCanvas, false);
        SetCanvasActive(signupCanvas, false);
        SetCanvasActive(inventoryCanvas, true);
        SetCanvasActive(leaderboardCanvas, false);
        SetCanvasActive(shopCanvas, false);
        SetCanvasActive(levelCanvas, false);
    }
    
    public void ShowLeaderboard()
    {
        SetCanvasActive(loginCanvas, false);
        SetCanvasActive(signupCanvas, false);
        SetCanvasActive(inventoryCanvas, false);
        SetCanvasActive(leaderboardCanvas, true);
        SetCanvasActive(shopCanvas, false);
        SetCanvasActive(levelCanvas, false);
    }
    
    public void ShowLevel()
    {
        SetCanvasActive(loginCanvas, false);
        SetCanvasActive(signupCanvas, false);
        SetCanvasActive(inventoryCanvas, false);
        SetCanvasActive(leaderboardCanvas, false);
        SetCanvasActive(shopCanvas, false);
        SetCanvasActive(levelCanvas, true);
    }

    public void ShowShop()
    {
        SetCanvasActive(loginCanvas, false);
        SetCanvasActive(signupCanvas, false);
        SetCanvasActive(inventoryCanvas, false);
        SetCanvasActive(leaderboardCanvas, false);
        SetCanvasActive(shopCanvas, true);
        SetCanvasActive(levelCanvas, false);
    }

    public void CloseInventory()
    {
        SetCanvasActive(inventoryCanvas, false);
    }
    public void CloseLeaderboard()
    {
        SetCanvasActive(leaderboardCanvas, false);
    }
    public void CloseShop()
    {
        SetCanvasActive(shopCanvas, false);
    }
    public void CloseLevel()
    {         
        SetCanvasActive(levelCanvas, false);
    }
}
