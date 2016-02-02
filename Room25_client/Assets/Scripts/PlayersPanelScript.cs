using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayersPanelScript : MonoBehaviour {

    public GameInfosScript gameInfos;

    public Object mainPlayerPanelPrefab;
    public Object otherPlayerPanelPrefab;
    List<PlayerInfosPanelScript> playersPanel = new List<PlayerInfosPanelScript>();

    

    // Use this for initialization
    void Start () {
        gameInfos = GameObject.FindGameObjectWithTag("GameInfos").GetComponent<GameInfosScript>();
        for(int i = 0; i < gameInfos.nbJoueurs; i++)
        {
            AddPlayer(gameInfos.players[i], i == gameInfos.idJoueur);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void MoveNext()
    {
        playersPanel.Add(playersPanel[0]);
        playersPanel.RemoveAt(0);
        for (int i = 0; i < playersPanel.Count; i++)
        {
            StartCoroutine(playersPanel[i].MoveTo(GetPositionFor(i), 1.0f));
        }
    }

    public Vector2 GetPositionFor(int i)
    {
        if (i == 0) return new Vector2(0, 0);
        int heightSum = 2;
        for(int j = 0;j< i; j++)
        {
            heightSum += playersPanel[j].Height;
        }
        print(heightSum);
        return new Vector2(0, -heightSum);
    }

    public void AddPlayer(UserInfos ui, bool mainPlayer)
    {
        GameObject newPanel;
        if(mainPlayer)
            newPanel = Instantiate(mainPlayerPanelPrefab) as GameObject;
        else 
            newPanel = Instantiate(otherPlayerPanelPrefab) as GameObject;
        
        PlayerInfosPanelScript pips = newPanel.GetComponent<PlayerInfosPanelScript>();
        pips.LoadInfos(ui);
        pips.rect.parent = (RectTransform)this.transform;
        pips.rect.anchoredPosition = GetPositionFor(playersPanel.Count);
        playersPanel.Add(pips);

    }

    public void RevealGuardian(int id)
    {
        UserInfos ui = gameInfos.players[id];
        for(int i = 0; i < playersPanel.Count; i++)
        {
            if (playersPanel[i].infos == ui)
                playersPanel[i].RevealRole();
        }
    }

    public void SetPlayerDead(int id)
    {
        UserInfos ui = gameInfos.players[id];
        for (int i = 0; i < playersPanel.Count; i++)
        {
            if (playersPanel[i].infos == ui)
                playersPanel[i].SetDead();
        }
    }
}
