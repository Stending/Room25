using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayersListPanelScript : MonoBehaviour {

    public Text nbJoueursText = null;
    public Text[] playerTexts = new Text[6];
	
    public void EmptyText(int i)
    {
        playerTexts[i].text = "";
    }
    public void SetText(int id, string ip, string name)
    {
        playerTexts[id].text = "(" + ip + ") " + name;
    }

    public void UpdateNbJoueurs(int nb, int total)
    {
        nbJoueursText.text = "("+nb + "/" + total + ")";
    }
}
