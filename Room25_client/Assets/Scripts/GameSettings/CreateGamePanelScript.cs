using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreateGamePanelScript : MonoBehaviour {


    public SettingsManager settingsManager;

    public InputField pseudoField;
    public Dropdown nbPlayersDropdown;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Send()
    {
        StartCoroutine(settingsManager.CreateGameAndConnect(pseudoField.text, nbPlayersDropdown.value + 4));
    }
}
