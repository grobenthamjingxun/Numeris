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
    public void Congradulations()
    {
        congraText.text = "Congratulations! You found the lucky clover! But the real treasure was the joy of learning!";
    }
}
