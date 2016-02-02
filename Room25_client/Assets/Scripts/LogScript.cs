using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class LogScript : MonoBehaviour {

    public ClientScript client;
    public GameInfosScript gameInfos;

    public bool OnScreen = true;
    public Animator logAnim;
    public List<string> events = new List<string>();
    public Text logText;
    public InputField chatInput;
	// Use this for initialization
	void Start () {
        GameObject.DontDestroyOnLoad(this.gameObject);
        GameObject.DontDestroyOnLoad(this.transform.parent.gameObject);
    }
	
    void OnGUI()
    {
        if (chatInput.isFocused && chatInput.text != "" && Input.GetKeyDown(KeyCode.Return))
        {
            SendChatMessage();
            chatInput.ActivateInputField();
        }
    }
	// Update is called once per frame
	void Update () {

        Display();
	}

    public void AddPlayerMessage(int p, string msg){
        events.Add("[" + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + "] "+ gameInfos.players[p].Name+ ": " + msg);
    }
    public void AddMessage(string msg)
    {
        //print ("MESSAGE : " + msg);
       events.Add("[" +System.DateTime.Now.Hour + ":"+ System.DateTime.Now.Minute.ToString("00") +"] " + msg);
    }

    public void Display()
    {
        logText.text = "";
        foreach (string msg in events)
        {
            logText.text += msg+ "\n";
        }
    }


    public void SendChatMessage()
    {
        if (chatInput.text != "")
        {
            client.Send("04;" + gameInfos.idJoueur + ";" + chatInput.text);
            chatInput.text = "";
        }
    }
}
