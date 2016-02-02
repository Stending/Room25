using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlertScript : MonoBehaviour {

    public Text titleText;
    public Text sentenceText;
	// Use this for initialization
	public void SetTexts(string title,string sentence)
    {
        if (title.Length < 20)
            titleText.fontSize = 40;
        else if (title.Length < 40)
            titleText.fontSize = 30;
        else
            titleText.fontSize = 25;

        titleText.text = title;
        sentenceText.text = sentence;
    }

    public void SetTitleColor(Color col)
    {
        titleText.color = col;
    }
}
