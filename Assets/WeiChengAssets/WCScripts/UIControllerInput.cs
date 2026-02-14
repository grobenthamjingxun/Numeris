/*
* Author: Cheang Wei Cheng
* Date: 08/02/2026
* Description: This script is responsible for handling VR controller input to toggle the inventory and level select UI canvases.
* It listens for input from the secondary buttons on both the left and right controllers, and calls the appropriate methods in the UIManager to show or hide the corresponding UI elements.
*/

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
    
    /// <summary>
    /// In Update(), the script checks if the secondary buttons on either controller were pressed this frame.
    /// If the right secondary button was pressed, it toggles the inventory canvas by calling ToggleInventory().
    /// If the left secondary button was pressed, it toggles the level select canvas by calling ToggleLevelSelect().
    /// </summary>
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
    
    /// <summary>
    /// This method toggles the inventory canvas by checking if it is currently active.
    /// If it is active, it calls UIManager.Instance.CloseInventory() to hide it.
    /// If it is not active, it calls UIManager.Instance.ShowInventory() to show it.
    /// </summary>
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
    
    /// <summary>
    /// This method toggles the level select canvas by checking if it is currently active.
    /// If it is active, it calls UIManager.Instance.CloseLevel() to hide it.
    /// If it is not active, it calls UIManager.Instance.ShowLevel() to show it.
    /// </summary>
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