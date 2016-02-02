using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ModePanelScript : MonoBehaviour {

    public SettingsManager settingsManager;

    public Text infoText;
    public Dropdown modeDropDown;
    public Toggle diffToggle;
    public Button sendButton;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SendMode()
    {
        if (modeDropDown.value == 0)
        {
            settingsManager.SelectMode(modeDropDown.value, diffToggle.isOn);
        }
    }

    public void EnableInputs()
    {
        modeDropDown.gameObject.SetActive(true);
        diffToggle.gameObject.SetActive(true);
        sendButton.gameObject.SetActive(true);
    }
    public void DisableInputs()
    {
        print("on desactive tout");
        modeDropDown.gameObject.SetActive(false);
        diffToggle.gameObject.SetActive(false);
        sendButton.gameObject.SetActive(false);
    }

    public void SetText(string str) { 

        print(str);
        infoText.text = str;
    }

}

