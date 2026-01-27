using UnityEngine;
using TMPro;
using System.Collections;

public class CoinDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI coinText;
    // [SerializeField] private ParticleSystem coinChangeParticles;
    
    [Header("Animation Settings")]
    [SerializeField] private float countDuration = 0.5f;
    [SerializeField] private float punchScale = 1.2f;
    [SerializeField] private float punchDuration = 0.3f;
    
    private int displayedCoins = 0;
    private Coroutine countCoroutine;
    
    void Start()
    {
        if (CoinManager.Instance != null)
        {
            displayedCoins = CoinManager.Instance.CurrentCoins;
            UpdateCoinText(displayedCoins);
            CoinManager.Instance.OnCoinsChanged.AddListener(OnCoinsChanged);
        }
    }
    
    void OnCoinsChanged(int newAmount)
    {
        if (countCoroutine != null)
        {
            StopCoroutine(countCoroutine);
        }
        countCoroutine = StartCoroutine(AnimateCoinCount(displayedCoins, newAmount));
        
        // if (coinChangeParticles != null)
        // {
        //     coinChangeParticles.Play();
        // }
        
        StartCoroutine(PunchScale());
    }
    
    IEnumerator AnimateCoinCount(int startValue, int endValue)
    {
        float elapsed = 0f;
        
        while (elapsed < countDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / countDuration;
            int currentValue = (int)Mathf.Lerp(startValue, endValue, t);
            UpdateCoinText(currentValue);
            yield return null;
        }
        
        displayedCoins = endValue;
        UpdateCoinText(endValue);
    }
    
    IEnumerator PunchScale()
    {
        Vector3 originalScale = coinText.transform.localScale;
        Vector3 targetScale = originalScale * punchScale;
        
        float elapsed = 0f;
        while (elapsed < punchDuration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (punchDuration / 2);
            coinText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        elapsed = 0f;
        while (elapsed < punchDuration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (punchDuration / 2);
            coinText.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
        
        coinText.transform.localScale = originalScale;
    }
    
    void UpdateCoinText(int amount)
    {
        coinText.text = $"Coins: {amount}";
    }
    
    void OnDestroy()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinsChanged.RemoveListener(OnCoinsChanged);
        }
    }
}
