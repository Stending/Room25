using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ConnectionPanel : MonoBehaviour {

    public AudioManager audioManager;
    public SettingsManager settingsManager;

    public InputField ipInput;
    public InputField nameInput;

    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }
    void OnGUI(){
        if(nameInput.isFocused && Input.GetKeyDown("tab"))
        {
            print("ah bah");
            ipInput.ActivateInputField();
        }
        if ((nameInput.isFocused || ipInput.isFocused) && Input.GetKeyDown(KeyCode.Return))
        {
            ConnectFromButton();
        }
    }

    public void ConnectFromButton()
    {
        audioManager.Validate();
        settingsManager.ConnectToServer(ipInput.text, nameInput.text);
    }

}
