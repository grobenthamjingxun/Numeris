using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    private GameObject gamePanel;
    [SerializeField]
    private GameObject loginPanel;
    [SerializeField]
    private GameObject signupPanel;

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
        gamePanel.SetActive(true);
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
    }

    public void ShowLogin()
    {
        gamePanel.SetActive(false);
        loginPanel.SetActive(true);
        signupPanel.SetActive(false);
    }

    public void ShowSignup()
    {
        gamePanel.SetActive(false);
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
    }
}
