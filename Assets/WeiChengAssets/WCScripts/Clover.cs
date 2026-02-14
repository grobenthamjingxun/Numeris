/*
* Author: Cheang Wei Cheng
* Date: 12/02/2026
* Description: This script is attached to the lucky clover that players obtain at the end of level 3.
* When the player interacts with the clover, it displays a congratulatory message in the feedback text UI element.
*/

using UnityEngine;
using TMPro;

public class Clover : MonoBehaviour
{
    public TMP_Text congraText;
    void Start()
    {
        if (congraText == null)
        {
            congraText = GameObject.Find("Test Text").GetComponent<TMP_Text>();
        }
    }

    /// <summary>
    /// When the player interacts with the clover, this method is called to display a congratulatory message in the feedback text UI element.
    /// </summary>
    public void Congratulations()
    {
        congraText.text = "Congratulations! You found the lucky clover! But the real treasure was the joy of learning!";
    }
}
