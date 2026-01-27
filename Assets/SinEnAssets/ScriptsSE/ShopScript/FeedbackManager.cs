using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FeedbackManager : MonoBehaviour
{
    [Header("Feedback Panel")]
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Image feedbackBackground;
    [SerializeField] private float displayDuration = 2f;
    
    [Header("Colors")]
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color failColor = Color.red;
    
    // [Header("Particle Effects")]
    // [SerializeField] private ParticleSystem coinSpendParticles;
    // [SerializeField] private ParticleSystem successParticles;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    
    void Start()
    {
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
        }
    }
    
    public void ShowPurchaseSuccess(string itemName)
    {
        StopAllCoroutines();
        StartCoroutine(ShowFeedback($"Purchased {itemName}!", successColor));
        
        // if (successParticles != null)
        // {
        //     successParticles.Play();
        // }
        
        // if (coinSpendParticles != null)
        // {
        //     coinSpendParticles.Play();
        // }
    }
    
    public void ShowPurchaseFailed()
    {
        StopAllCoroutines();
        StartCoroutine(ShowFeedback("Not Enough Coins!", failColor));
    }
    
    IEnumerator ShowFeedback(string message, Color color)
    {
        feedbackText.text = message;
        feedbackBackground.color = color;
        feedbackPanel.SetActive(true);
        
        CanvasGroup canvasGroup = feedbackPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = feedbackPanel.AddComponent<CanvasGroup>();
        }
        
        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        
        // Wait
        yield return new WaitForSeconds(displayDuration);
        
        // Fade out
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        feedbackPanel.SetActive(false);
    }
}