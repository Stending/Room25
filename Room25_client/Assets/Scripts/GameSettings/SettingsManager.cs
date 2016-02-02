using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Diagnostics;

public class SettingsManager : MonoBehaviour {


    public AudioManager audioManager;

    public int mode = 0;

    public ClientScript client;
    public PlayersListPanelScript playerListPanel;
    public GameInfosScript gameInfos;

    public ModePanelScript modePanel;
    public CharacterSelectionPanelScript characterSelectionPanel;

    public Animator titleAnim;
    public Animator logAnim;
    public Animator mainButtonsAnim;
    public Animator createGamePanelAnim;
    public Animator connectionPanelAnim;
    public Animator playersListPanelAnim;
    public Animator modeSelectPanelAnim;
    public Animator caracterSelectPanelAnim;


    public Animator loadingAnim;
    public Animator loadingTextAnim;
    public Text loadingText;
    // Use this for initialization
    void Start () {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        //GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasScript>().SetConnectionMode();
        Screen.SetResolution(600, 600, false);
        //connectionPanelAnim.SetBool("Active", true);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(mode == 1 || mode == 5)
                SwitchToMode(0);
        }
	
	}
    public void SwitchToMode(int m)
    {
        mode = m;
        switch (mode)
        {
            case 0:
                titleAnim.SetBool("Active", true);
                logAnim.SetBool("Active", false);
                mainButtonsAnim.SetBool("Active", true);
                createGamePanelAnim.SetBool("Active", false);
                connectionPanelAnim.SetBool("Active", false);
                playersListPanelAnim.SetBool("Active", false);
                modeSelectPanelAnim.SetBool("Active", false);
                caracterSelectPanelAnim.SetBool("Active", false);

                break;

            case 1:
                audioManager.Validate();
                //audioManager.WindowsMoved();
                mainButtonsAnim.SetBool("Active", false);
                connectionPanelAnim.SetBool("Active", true);
                break;
            case 2:
                audioManager.WindowsMoved();
                createGamePanelAnim.SetBool("Active", false);
                titleAnim.SetBool("Active", false);
                StartLoading("On attend que tous les joueurs soient là");
                connectionPanelAnim.SetBool("Active", false);
                playersListPanelAnim.SetBool("Active", true);
                logAnim.SetBool("Active", true);
                break;
            case 3:
                StopLoading();
                //playersListPanelAnim.SetBool("Active", false);
                if (gameInfos.idJoueur == 0)
                    modeSelectPanelAnim.SetBool("Active", true);
                else
                    StartLoading("L'hôte est en train de choisir le mode");
                break;
            case 4:
                audioManager.WindowsMoved();
                StopLoading();
                modeSelectPanelAnim.SetBool("Active", false);
                caracterSelectPanelAnim.SetBool("Active", true);
                break;
            case 5:
                audioManager.Validate();
                //audioManager.WindowsMoved();
                mainButtonsAnim.SetBool("Active", false);
                createGamePanelAnim.SetBool("Active", true);
                break;
        }
    }

    public void AskMode(bool b)
    {
        if (b)
        {
            modePanel.EnableInputs();
            modePanel.SetText("Choisissez le mode et la difficulté de la partie");
        }
        else
        {
            modePanel.DisableInputs();
            string str = "L'hote est en train de choisir le mode";
            print(str);
            modePanel.SetText(str);
        }
    }

    public void UpdateListJoueurs()
    {
        playerListPanel.UpdateNbJoueurs(gameInfos.nbJoueurs, gameInfos.nbJoueursTotal);
    }
    public void SelectMode(int mode, bool exp)
    {
        string msg = "08;" + mode + ";" + ((exp) ? "1":"0" );
        client.SendData(System.Text.Encoding.ASCII.GetBytes(msg));
    }

    public void SelectCharacter(int ch)
    {
        client.Send("07;" + gameInfos.idJoueur + ";" + ch);
    }

    public void StartLoading(string text)
    {
        loadingText.text = text;
        loadingTextAnim.SetBool("Active", true);
        loadingAnim.SetBool("Active", true);
    }

    public void StopLoading()
    {
        loadingTextAnim.SetBool("Active", false);
        loadingAnim.SetBool("Active", false);
    }

    public IEnumerator CreateGameAndConnect(string pseudo, int nbplayer)
    {
        Process.Start("Room 25 Server.exe", nbplayer.ToString());

        yield return new WaitForSeconds(1.0f);
        ConnectToServer("127.0.0.1", pseudo);
        SwitchToMode(2);
    }

    public void ConnectToServer(string ip, string pseudo)
    {
        print("pseudo avant conversion " + pseudo);
        while (pseudo.Contains(";"))
        {
            pseudo = pseudo.Remove(pseudo.IndexOf(';'), 1);
            print(pseudo);
        }
        print("pseudo après conversion : " + pseudo);
        client.ConnectTo(ip, "4148");
        client.Send("01;" + pseudo);
    }

}
