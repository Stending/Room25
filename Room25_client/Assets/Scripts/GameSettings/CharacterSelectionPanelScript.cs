using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterSelectionPanelScript : MonoBehaviour {

    public SettingsManager settingsManager;
    public GameInfosScript gameInfos;
    public Text[] playersTexts = new Text[6];
    public Dropdown characterDropDown;

    public CharacterButtonScript[] charactersButtons = new CharacterButtonScript[6];
    public int[] buttonsActive = new int[6];
	// Use this for initialization
	void Start () {
        EnableEverything();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SendCharacter(int i)
    {
        if (buttonsActive[i] == -1)
            settingsManager.SelectCharacter(i);
        else if (buttonsActive[i] == gameInfos.idJoueur)
            settingsManager.SelectCharacter(-1);
        //settingsManager.SelectCharacter(characterDropDown.value);
    }

    public void EnableEverything()
    {
        for(int i=0;i<6;i++)
            buttonsActive[i] = -1;
    }
    public void DisableCharacter(int idChar, int idPlayer)
    {
        int prevChar = PlayerChar(idPlayer);
        if (prevChar == -1)
        {
            buttonsActive[idChar] = idPlayer;
            charactersButtons[idChar].Disable(gameInfos.players[idPlayer].Name);
        }
        else
        {
            buttonsActive[prevChar] = -1;
            charactersButtons[prevChar].Enable();
            buttonsActive[idChar] = idPlayer;
            charactersButtons[idChar].Disable(gameInfos.players[idPlayer].Name);
        }
    }

    public void EnableCharacterPlayer(int idPlayer)
    {
        int playerChar = PlayerChar(idPlayer);
        buttonsActive[playerChar] = -1;
        charactersButtons[playerChar].Enable();

    }

    private int PlayerChar(int player)
    {
        for(int i = 0; i < 6; i++)
        {
            if(buttonsActive[i] == player)
                return i;
        }
        return -1;
    }

}
