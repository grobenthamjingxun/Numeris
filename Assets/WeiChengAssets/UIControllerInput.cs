using UnityEngine;
using UnityEngine.InputSystem;

public class UIControllerInput : MonoBehaviour
{
    [Header("VR Input Actions")]
    [SerializeField] private InputActionProperty rightSecondaryButton;
    [SerializeField] private InputActionProperty leftSecondaryButton;
    
    private void OnEnable()
    {
        rightSecondaryButton.action.Enable();
        leftSecondaryButton.action.Enable();
    }
    
    private void OnDisable()
    {
        rightSecondaryButton.action.Disable();
        leftSecondaryButton.action.Disable();
    }
    
    void Update()
    {
        if (UIManager.Instance == null) return;
        
        if (rightSecondaryButton.action.WasPressedThisFrame())
        {
            ToggleInventory();
        }
        
        if (leftSecondaryButton.action.WasPressedThisFrame())
        {
            ToggleLevelSelect();
        }
    }
    
    private void ToggleInventory()
    {
        if (UIManager.Instance == null) return;
        
        if (UIManager.Instance.inventoryCanvas.activeSelf)
        {
            UIManager.Instance.CloseInventory();
        }
        else
        {
            UIManager.Instance.ShowInventory();
        }
    }
    
    private void ToggleLevelSelect()
    {
        if (UIManager.Instance == null) return;
        
        if (UIManager.Instance.levelCanvas.activeSelf)
        {
            UIManager.Instance.CloseLevel();
        }
        else
        {
            UIManager.Instance.ShowLevel();
        }
    }
}