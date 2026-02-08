using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    private GameObject loginCanvas;
    [SerializeField]
    private GameObject signupCanvas;
    [SerializeField]
    private GameObject inventoryCanvas;
    [SerializeField]
    private GameObject leaderboardCanvas;
    [SerializeField]
    private GameObject shopCanvas;
    [SerializeField]
    private GameObject levelCanvas;

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

    public void ShowGame()
    {
        loginCanvas.SetActive(false);
        signupCanvas.SetActive(false);
        inventoryCanvas.SetActive(false);
        leaderboardCanvas.SetActive(false);
        shopCanvas.SetActive(false);
        levelCanvas.SetActive(false);
    }
    public void ShowLogin()
    {
        loginCanvas.SetActive(true);
        signupCanvas.SetActive(false);
        inventoryCanvas.SetActive(false);
        leaderboardCanvas.SetActive(false);
        shopCanvas.SetActive(false);
        levelCanvas.SetActive(false);
    }
    public void ShowSignup()
    {
        loginCanvas.SetActive(false);
        signupCanvas.SetActive(true);
        inventoryCanvas.SetActive(false);
        leaderboardCanvas.SetActive(false);
        shopCanvas.SetActive(false);
        levelCanvas.SetActive(false);
    }
    public void ShowInventory()
    {
        loginCanvas.SetActive(false);
        signupCanvas.SetActive(false);
        inventoryCanvas.SetActive(true);
        leaderboardCanvas.SetActive(false);
        shopCanvas.SetActive(false);
        levelCanvas.SetActive(false);
    }
    public void ShowLeaderboard()
    {
        loginCanvas.SetActive(false);
        signupCanvas.SetActive(false);
        inventoryCanvas.SetActive(false);
        leaderboardCanvas.SetActive(true);
        shopCanvas.SetActive(false);
        levelCanvas.SetActive(false);
    }
    public void ShowLevel()
    {
        loginCanvas.SetActive(false);
        signupCanvas.SetActive(false);
        inventoryCanvas.SetActive(false);
        leaderboardCanvas.SetActive(false);
        shopCanvas.SetActive(false);
        levelCanvas.SetActive(true);
    }

    public void ShowShop()
    {
        loginCanvas.SetActive(false);
        signupCanvas.SetActive(false);
        inventoryCanvas.SetActive(false);
        leaderboardCanvas.SetActive(false);
        shopCanvas.SetActive(true);
        levelCanvas.SetActive(false);
    }

    public void CloseInventory()
    {
        inventoryCanvas.SetActive(false);
    }
    public void CloseLeaderboard()
    {
        leaderboardCanvas.SetActive(false);
    }
    public void CloseShop()
    {
        shopCanvas.SetActive(false);
    }
    public void CloseLevel()
    {         
        levelCanvas.SetActive(false);
    }
}
