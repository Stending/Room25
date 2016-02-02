using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class VictoryPanelScript : MonoBehaviour {

    public bool gameEnded;
    public Animator anim;
    public GameInfosScript gameInfos;
    public Text resultText;
    public Text victorySentenceText;
    public Text winnersText;

	// Use this for initialization
	void Start () {
        gameInfos = GameObject.FindGameObjectWithTag("GameInfos").GetComponent<GameInfosScript>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public void Appear()
    {
        anim.SetBool("Active", true);
    }

    public void LoadVictory(int type, List<int> winners)
    {
        if (!gameEnded)
        {
            gameEnded = true;
            if (winners.Contains(gameInfos.idJoueur))
                resultText.text = "Victoire";
            else
                resultText.text = "Zut !";
            bool isWinners;
            switch (type)
            {
                case 0:
                    victorySentenceText.text = "Deux prisonniers sont décédés";
                    break;
                case 1:
                    victorySentenceText.text = "Les prisonniers se sont enfuis";
                    break;
                case 2:
                    victorySentenceText.text = "Les prisonniers ne sont pas partis en temps et en heure";
                    break;
            }

            if (winners.Count > 1)
            {
                winnersText.text = "Les gagnants sont ";
                foreach (int i in winners)
                {
                    winnersText.text += gameInfos.players[i].Name + " ";
                }
            }else if (winners.Count == 1)
            {
                winnersText.text = "Le gagnant est ";
                winnersText.text += gameInfos.players[winners[0]].Name;
            }
            else
            {
                winnersText.text = "";
            }
        }
    }

    public void LoadDisconnection(int player)
    {
        if (!gameEnded)
        {
            gameEnded = true;
            resultText.text = "Déconnexion";
            victorySentenceText.text = "La partie a été arrêtée car " + gameInfos.players[player].Name + " s'est déconnecté";
            winnersText.gameObject.SetActive(false);
        }
    }

}
