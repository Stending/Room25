using UnityEngine;
using System.Collections;

public class GameInfosScript : MonoBehaviour {


    public SettingsManager settingsManager;
    public UserInfos[] players = new UserInfos[6];
    public int nbJoueurs = 0;
    public int nbJoueursTotal = 0;
    public int idJoueur;
    public Mode mode;
    public bool expert;

    public string gridCode = null;
        
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AddUser(string ip, int id, string name)
    {
        players[id] = new UserInfos(ip, name);
        nbJoueurs++;
        settingsManager.playerListPanel.SetText(id, ip, name);
        
    }

    public void DelUser(int id)
    {
        players[id] = null;
        nbJoueurs--;
        settingsManager.playerListPanel.EmptyText(id);
    }
}
