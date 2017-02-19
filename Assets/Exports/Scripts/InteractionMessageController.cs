using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InteractionMessageController : MonoBehaviour {

    public Text textField;
    public string COLOR_ORANGE_HEX;

    // Use this for initialization
    void Start ()
    {
	
	}

    public void DisplayText(string textToDisplay)
    {
        textField.text = textToDisplay.Replace("\\n", "\n");
        textField.text = textField.text.Replace("<color=COLOR_ORANGE>", "<color=#" + COLOR_ORANGE_HEX + ">");
    }

    public void EraseText()
    {
        textField.text = "";
    }
}
