using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChoosePlayerPanelScript : MonoBehaviour {

    public GameManager gameManager;
    public GameInfosScript gameInfos;

    public Animator anim;
    public RectTransform rectTransform;
    public Button[] buttons = new Button[6];
    public int[] buttonPlayers = new int[6];
    int b, t, l, r;
    public Sprite[] charSprites = new Sprite[6];
	// Use this for initialization
	void Start () {
        gameInfos = GameObject.FindGameObjectWithTag("GameInfos").GetComponent<GameInfosScript>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public void LoadButtons(int bottom, int top, int left, int right, string selectablePlayersString)
    {
        b = bottom; t = top; l = left; r = right;
        string[] chars = selectablePlayersString.Split(':');
        int nb = chars.Length;
        print("nombre de joueur " + nb);
        rectTransform.sizeDelta = new Vector2((nb * 50 + 50), rectTransform.sizeDelta.y);
        for(int i = 0; i < 6; i++)
        {
            if(i< nb)
            {
                buttons[i].gameObject.SetActive(true);
                int playerId = int.Parse(chars[i]);
                buttonPlayers[i] = playerId;
                Image[] images = buttons[i].GetComponentsInChildren<Image>();
                images[1].sprite = charSprites[(int)gameInfos.players[playerId].Type];
                RectTransform butRT = (RectTransform)buttons[i].transform;
                butRT.localPosition = new Vector3((nb-1)*(-30) + i*60, butRT.localPosition.y, butRT.localPosition.z);
                
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }
    }

    public void SendPlayer(int i)
    {
        gameManager.SelectPlayer(b, t, l, r, buttonPlayers[i]);
        Disappear();
    }
    public void Appear()
    {
        anim.SetBool("Active", true);
    }

    public void Disappear()
    {
        anim.SetBool("Active", false);
    }
}
