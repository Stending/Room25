using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public enum WaitingEvent {
    W_NOTHING, W_ACTIONS,W_PLAYER_PUSH, W_MOVEDIRECTION, W_ROOM_TO_LOOK_DIRECTION, W_ROOM_TO_LOOK_DIRECTION_FIRSTTIME, W_CONTROLDIRECTION, W_PLAYER, W_PLAYERINROOM, W_VISION_ROOM, W_MOBILE_ROOM, W_ILLUSION_ROOM, W_CONTROL_ROOM
}



public class GameManager : MonoBehaviour
{

    public Canvas arrowsCanvas;
    public ComplexScript complex;
    public WaitingEvent waitingMode = WaitingEvent.W_NOTHING;

    public AudioManager audioManager;
    public LogScript log;

    public ActionType action1;
    public ActionType action2;

    public int actPlayer;
    public PlayerScript actualPlayer;
    public int actualRoomX, actualRoomY;

    public Object directionPanel;
    public Object pointerVisionObject;
    public Object alertObject;

    public GameObject revealGuardianButton;

    public PlayersPanelScript playersPanel;
    
    public ChooseActionsPanelScript actionsPanel;
    public ChoosePlayerPanelScript choosePlayerScript;
    public ImmediateActionsPanelScript immediateActionPanel;

    public Text turnsLeftText;

    public VictoryPanelScript victoryPanel;

    public GameInfosScript gameInfos;
    public ClientScript client;

    public Transform logPanel;
    public Transform mainCanvas;

    public ForegroundEffectScript foregroundEffect;
    // Use this for initialization
    void Start()
    {
        CreateAlert(AlertType.DEBUT);
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        audioManager.ChangeMusic(1);
        //GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasScript>().SetInGameMode();
        gameInfos = GameObject.FindGameObjectWithTag("GameInfos").GetComponent<GameInfosScript>();
        client = GameObject.FindGameObjectWithTag("Client").GetComponent<ClientScript>();
        client.gameManager = this;
        complex.InitializeComplex(gameInfos.gridCode, gameInfos.players, gameInfos.nbJoueursTotal);
        actualPlayer = complex.players[gameInfos.idJoueur];
        LoadEvents();
        log = GameObject.FindGameObjectWithTag("Log").GetComponent<LogScript>();

       
        log.transform.SetParent(logPanel.transform);
        

        if (gameInfos.players[gameInfos.idJoueur].Role == Role.GUARDIAN)
            revealGuardianButton.gameObject.SetActive(true);
        else
            revealGuardianButton.gameObject.SetActive(false);
        WaitForFirstRoomToLookDirection(1, 1, 1, 1);
    }

    public void WaitForFirstRoomToLookDirection(int bottom, int top, int left, int right)
    {

        waitingMode = WaitingEvent.W_ROOM_TO_LOOK_DIRECTION_FIRSTTIME;
        DirectionPanelScript dps = AskForDirectionAroundPlayer(gameInfos.idJoueur, bottom == 1, top == 1, left == 1, right == 1);
        dps.SetColor(ActionType.REGARDER);
    }

    public void WaitForRoomToLookDirection(int bottom, int top, int left, int right)
    {
        waitingMode = WaitingEvent.W_ROOM_TO_LOOK_DIRECTION;
        DirectionPanelScript dps = AskForDirectionAroundPlayer(gameInfos.idJoueur, bottom == 1, top == 1, left == 1, right == 1);
        dps.SetColor(ActionType.REGARDER);
    }


    public void WaitForMoveDirection(int bottom, int top, int left, int right)
    {
        waitingMode = WaitingEvent.W_MOVEDIRECTION;
        DirectionPanelScript dps = AskForDirectionAroundPlayer(gameInfos.idJoueur, bottom == 1, top == 1, left == 1, right == 1);
        dps.SetColor(ActionType.DEPLACER);
    }

    public void WaitForControlDirection(int bottom, int top, int left, int right)
    {
        waitingMode = WaitingEvent.W_CONTROLDIRECTION;
        DirectionPanelScript dps = AskForDirectionAroundPlayer(gameInfos.idJoueur, bottom == 1, top == 1, left == 1, right == 1);
        dps.SetColor(ActionType.CONTROLER);
    }

    public void WaitForVisionRoom(string roomsString)
    {
        waitingMode = WaitingEvent.W_VISION_ROOM;
        complex.SetSelectableRooms(roomsString);
    }

    public void WaitForMobileRoom(int x, int y, string roomsString)
    {
        waitingMode = WaitingEvent.W_MOBILE_ROOM;
        actualRoomX = x; actualRoomY = y;
        complex.SetSelectableRooms(roomsString);
    }

    public void WaitForControlLine()
    {
        waitingMode = WaitingEvent.W_CONTROL_ROOM;
        complex.SetEveryRoomSelectable();
        complex.SetSelectable(2, 2, false);
    }

    public void WaitForIllusionRoom(int x, int y, string roomsString)
    {
        waitingMode = WaitingEvent.W_ILLUSION_ROOM;
        actualRoomX = x; actualRoomY = y;
        complex.SetSelectableRooms(roomsString);
    }


    public void WaitForPlayerToPush(int bottom, int top, int left, int right, string playersString)
    {
        waitingMode = WaitingEvent.W_PLAYER_PUSH;
        choosePlayerScript.Appear();
        choosePlayerScript.LoadButtons(bottom, top, left, right, playersString);
    }

    public DirectionPanelScript AskForDirectionAroundPlayer(int idPlayer, bool bottom, bool top, bool left, bool right)
    {
        
        actualPlayer = complex.players[idPlayer];
        actualRoomX = actualPlayer.x; actualRoomY = actualPlayer.y;
        DirectionPanelScript dps = InstantiateDirectionPanel(new Vector3(actualPlayer.transform.position.x, actualPlayer.transform.position.y, -5));
        dps.transform.parent = complex.complex[actualRoomX, actualRoomY].transform;
        if (!bottom) dps.DisableDirection(Direction.BAS);
        if (!top) dps.DisableDirection(Direction.HAUT);
        if (!left) dps.DisableDirection(Direction.GAUCHE);
        if (!right) dps.DisableDirection(Direction.DROITE);

        return dps;
    }

    public DirectionPanelScript AskForDirectionAroundRoom(int x, int y, bool bottom, bool top, bool left, bool right)
    {
        actualRoomX = x; actualRoomY = y;
        Vector2 pos = complex.GetRoomPositionInScene(x, y);
        DirectionPanelScript dps = InstantiateDirectionPanel(new Vector3(pos.x, pos.y, -5));
        dps.transform.parent = actualPlayer.transform;
        if (!bottom) dps.DisableDirection(Direction.BAS);
        if (!top) dps.DisableDirection(Direction.HAUT);
        if (!left) dps.DisableDirection(Direction.GAUCHE);
        if (!right) dps.DisableDirection(Direction.DROITE);

        return dps;
    }

    public void SelectPlayer(int bottom, int top, int left, int right, int pl)
    {
        actPlayer = pl;
        DirectionPanelScript dps = AskForDirectionAroundPlayer(pl, bottom==1, top==1, left==1, right==1);
        dps.SetColor(ActionType.POUSSER);
    }

    public void SetDirection(Direction direction)
    {
        if (waitingMode == WaitingEvent.W_MOVEDIRECTION)
        {
            //MovePlayer(actualPlayer, direction);
            waitingMode = WaitingEvent.W_NOTHING;
            client.Send("17;" + gameInfos.idJoueur + ";2;" + (int)direction);
            //envoyer la direction au serveur

        }
        else if (waitingMode == WaitingEvent.W_ROOM_TO_LOOK_DIRECTION)
        {
            waitingMode = WaitingEvent.W_NOTHING;
            /*getRoomAtDir(actualPlayer.x, actualPlayer.y, direction).RevealHide();*/
            Vector2 pos = getPosAtDir(actualPlayer.x, actualPlayer.y, direction);
            client.Send("17;" + gameInfos.idJoueur + ";3;" + (int)pos.x + ";" + (int)pos.y);
        }
        else if (waitingMode == WaitingEvent.W_ROOM_TO_LOOK_DIRECTION_FIRSTTIME)
        {
            waitingMode = WaitingEvent.W_NOTHING;
            // getRoomAtDir(actualPlayer.x, actualPlayer.y, direction).RevealHide();
            Vector2 pos = getPosAtDir(actualPlayer.x, actualPlayer.y, direction);
            client.Send("11;" + gameInfos.idJoueur + ";" + (int)pos.x + ";" + (int)pos.y);
        }
        else if (waitingMode == WaitingEvent.W_CONTROLDIRECTION)
        {
            int linePos;
            waitingMode = WaitingEvent.W_NOTHING;
            if (direction == Direction.BAS || direction == Direction.HAUT)
                linePos = actualPlayer.x;
            else
                linePos = actualPlayer.y;

            client.Send("17;" + gameInfos.idJoueur + ";1;" + linePos + ";" + (int)direction);
            complex.UnlightAll();
            //complex.MoveLine(linePos, direction);
        }
        else if (waitingMode == WaitingEvent.W_CONTROL_ROOM)
        {
            int linePos;
            waitingMode = WaitingEvent.W_NOTHING;
            if (direction == Direction.BAS || direction == Direction.HAUT)
                linePos = actualRoomX;
            else
                linePos = actualRoomY;
            print("TEST");
            client.Send("24;" + gameInfos.idJoueur + ";" + linePos + ";" + (int)direction);
            //complex.MoveLine(linePos, direction);
        }else if(waitingMode == WaitingEvent.W_PLAYER_PUSH)
        {
            client.Send("17;" + gameInfos.idJoueur + ";0;" + actPlayer + ";" + (int)direction);
            waitingMode = WaitingEvent.W_NOTHING;
        }
        complex.UnlightAll();

    }

    public void ControlLine(int line, int dir)
    {
        //PlayerScript ps = complex.players[idplayer];
        complex.MoveLine(line, (Direction)dir);
    }
    public void MovePlayer(int idplayer, int dir)
    {
        if(idplayer == gameInfos.idJoueur)
            foregroundEffect.Disappear();
        PlayerScript ps = complex.players[idplayer];
        int x = ps.x, y = ps.y;
        switch ((Direction)dir)
        {
            case Direction.BAS:
                y++;
                break;
            case Direction.HAUT:
                y--;
                break;
            case Direction.GAUCHE:
                x--;
                break;
            case Direction.DROITE:
                x++;
                break;
        }

        complex.ReplacePlayersBeforeNewOne(x, y);
        StartCoroutine(ps.Move(complex.GetLastPlayerPositionInRoom(x, y), 1.0f));
        ps.transform.parent = complex.complex[x, y].transform;
        int precX = ps.x, precY = ps.y;
        ps.x = x; ps.y = y;
        complex.ReplaceAllPlayers(precX, precY);
    }

    public void PushPlayer(int idpusher, int idplayer, int dir)
    {
        if(idplayer == gameInfos.idJoueur)
            foregroundEffect.Disappear();
        PlayerScript ps = complex.players[idplayer];
        int x = ps.x, y = ps.y;
        switch ((Direction)dir)
        {
            case Direction.BAS:
                y++;
                break;
            case Direction.HAUT:
                y--;
                break;
            case Direction.GAUCHE:
                x--;
                break;
            case Direction.DROITE:
                x++;
                break;
        }
        //StartCoroutine(ps.Move(complex.GetRoomPositionInScene(x, y), 0.5f));
        complex.ReplacePlayersBeforeNewOne(x, y);
        StartCoroutine(ps.Move(complex.GetLastPlayerPositionInRoom(x, y), 0.5f));
        int precX = ps.x, precY = ps.y;
        ps.x = x; ps.y = y;
        complex.ReplaceAllPlayers(precX, precY);
        ps.transform.parent = complex.complex[x, y].transform;
    }


    public void SwitchRoomWithPlayers(int x1, int y1, int x2, int y2)
    {
        RoomScript rs1 = complex.complex[x1, y1];
        RoomScript rs2 = complex.complex[x2, y2];
        complex.complex[x1, y1] = rs2;
        StartCoroutine(rs2.Move(complex.GetRoomPositionInScene(x1, y1), 2.0f));
        complex.complex[x2, y2] = rs1;
        StartCoroutine(rs1.Move(complex.GetRoomPositionInScene(x2, y2), 2.0f));
        rs2.x = x1; rs2.y = y1;
        rs1.x = x2; rs1.y = y2;
        List<PlayerScript> psInRoom = complex.GetPlayersInRoom(x1, y1);
        foreach (PlayerScript ps in psInRoom)
        {
            ps.x = x2; ps.y = y2;
        }
    }

    public void SwitchRoomWithoutPlayers(int x1, int y1, int x2, int y2)
    {
        RoomScript rs1 = complex.complex[x1, y1];
        RoomScript rs2 = complex.complex[x2, y2];
        List<PlayerScript> psInRoom = complex.GetPlayersInRoom(x1, y1);
        foreach (PlayerScript ps in psInRoom)
        {
            StartCoroutine(ps.FlyAndLand(rs2.transform,2.1f));
        }
        complex.complex[x1, y1] = rs2;
        StartCoroutine(rs2.Move(complex.GetRoomPositionInScene(x1, y1), 2.0f));
        complex.complex[x2, y2] = rs1;
        StartCoroutine(rs1.Move(complex.GetRoomPositionInScene(x2, y2), 2.0f));
        rs2.x = x1; rs2.y = y1;
        rs1.x = x2; rs1.y = y2;
    }

    public void RevealYouAreGuardian()
    {
        if (gameInfos.players[gameInfos.idJoueur].Role == Role.GUARDIAN)
            client.Send("12;" + gameInfos.idJoueur);
    }

    public void RevealAGuardian(int id)
    {
        gameInfos.players[id].Role = Role.GUARDIAN;
        playersPanel.RevealGuardian(id);
    }


    public void NextTurn(int turnsLeft)
    {
        turnsLeftText.text = turnsLeft.ToString();
        playersPanel.MoveNext();
    }

    public DirectionPanelScript InstantiateDirectionPanel(Vector3 pos)
    {
        GameObject go = Instantiate(directionPanel) as GameObject;
        go.transform.position = pos;
        return go.GetComponent<DirectionPanelScript>();
    }

    public VisionPointerScript CreateVisionPointer(int character, int x, int y)
    {
        GameObject go = Instantiate(pointerVisionObject) as GameObject;
        Vector2 pos = complex.GetRoomPositionInScene(x, y);
        go.transform.position = new Vector3(pos.x, pos.y, -5);

        VisionPointerScript vps = go.GetComponent<VisionPointerScript>();
        vps.SetCharacter(character);
        return vps;
    }


    public void PlayerLooksRoom(int player, int x, int y)
    {
        VisionPointerScript vps = CreateVisionPointer((int)gameInfos.players[player].Type, x, y);
        Destroy(vps.gameObject, 3.0f);
    }
    public void DirectionHighlight(Direction dir, bool high)
    {
        if(high)
            audioManager.MouseHover();
        if (waitingMode == WaitingEvent.W_ROOM_TO_LOOK_DIRECTION)
        {
            RoomScript rs = getRoomAtDir(actualPlayer.x, actualPlayer.y, dir);
            if (high)
                rs.Highlight(Color.green, 0.3f);
            else
                rs.Unlight();
        }
        else if (waitingMode == WaitingEvent.W_CONTROLDIRECTION || waitingMode == WaitingEvent.W_CONTROL_ROOM)
        {
            if (high)
            {
                int x = actualRoomX, y = actualRoomY;
                float sh = 0.2f;
                switch (dir)
                {
                    case Direction.BAS:
                        for (y = 0; y < 5; y++, sh += 0.15f)
                        {
                            complex.complex[x, y].Highlight(Color.blue, sh);
                        }
                        break;
                    case Direction.HAUT:
                        for (y = 4; y >= 0; y--, sh += 0.15f)
                        {
                            complex.complex[x, y].Highlight(Color.blue, sh);
                        }
                        break;
                    case Direction.GAUCHE:
                        for (x = 4; x >= 0; x--, sh += 0.15f)
                        {
                            complex.complex[x, y].Highlight(Color.blue, sh);
                        }
                        break;
                    case Direction.DROITE:
                        for (x = 0; x < 5; x++, sh += 0.15f)
                        {
                            complex.complex[x, y].Highlight(Color.blue, sh);
                        }
                        break;
                }
            }
            else
            {
                complex.UnlightAll();
            }

        }
    }

    public Vector2 getPosAtDir(int x, int y, Direction dir)
    {
        switch (dir)
        {
            case Direction.BAS:
                y++;
                break;
            case Direction.HAUT:
                y--;
                break;
            case Direction.GAUCHE:
                x--;
                break;
            case Direction.DROITE:
                x++;
                break;
        }
        return new Vector2(x, y);
    }

    public RoomScript getRoomAtDir(int x, int y, Direction dir)
    {
        switch (dir)
        {
            case Direction.BAS:
                y++;
                break;
            case Direction.HAUT:
                y--;
                break;
            case Direction.GAUCHE:
                x--;
                break;
            case Direction.DROITE:
                x++;
                break;
        }
        if (x <= 4 && x >= 0 && y <= 4 && y >= 0)
            return complex.complex[x, y];
        else
            return complex.complex[2, 2];
    }

    public void WaitForAction(bool gele)
    {
        waitingMode = WaitingEvent.W_ACTIONS;
        //actionsPanel.SetActionsEnabled((allowpousse == 1), (allowregard == 1), (allowcontrole == 1), (allowdeplace == 1));
        actionsPanel.EnableButtons(gele);
    }

    public void ActionsSet(int action1, int action2)
    {
        waitingMode = WaitingEvent.W_NOTHING;
        client.Send("13;" + gameInfos.idJoueur + ";" + (action1 - 1) + ";" + (action2 - 1));
        actionsPanel.DisableButtons();
    }

    public void WaitForImmediateAction(int allowpousse, int allowcontrole, int allowdeplace, int allowregard)
    {
        immediateActionPanel.Appear();
        immediateActionPanel.AllowActions(allowpousse != 0, allowcontrole != 0, allowdeplace != 0, allowregard != 0);
        waitingMode = WaitingEvent.W_ACTIONS;
    }
    public void ImmediateActionSet(int a)
    {
        immediateActionPanel.Disappear();
        client.Send("32;" + gameInfos.idJoueur + ";" + a);
        waitingMode = WaitingEvent.W_ACTIONS;
    }

    public void VortexTeleport(int p)
    {
        if(p == gameInfos.idJoueur)
            foregroundEffect.Disappear();
        PlayerScript ps = complex.players[p];
        //ps.transform.position = new Vector3(roomPos.x, roomPos.y, ps.transform.position.z);
        complex.ReplacePlayersBeforeNewOne(2, 2);
        StartCoroutine(ps.Move(complex.GetLastPlayerPositionInRoom(2, 2),1.0f));
        int precX = ps.x, precY = ps.y;
        ps.x = 2; ps.y = 2;
        complex.ReplaceAllPlayers(precX, precY);

        //ps.Move(roomPos, 1);
    }

    private void LoadEvents()
    {
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                complex.complex[i, j].OnEnter += MouseEnterRoom;
                complex.complex[i, j].OnExit += MouseExitRoom;
                complex.complex[i, j].OnSelect += RoomSelected;
            }
        }
    }
    public void RoomSelected(int x, int y)
    {
        if(waitingMode == WaitingEvent.W_VISION_ROOM)
        {
            client.Send("20;" + gameInfos.idJoueur + ";" + x + ";" + y);
            waitingMode = WaitingEvent.W_NOTHING;

        }
        else if (waitingMode == WaitingEvent.W_MOBILE_ROOM)
        {
            client.Send("23;" + gameInfos.idJoueur + ";"+actualRoomX+";"+actualRoomY+";" + x + ";" + y);
            waitingMode = WaitingEvent.W_NOTHING;
        }
        else if (waitingMode == WaitingEvent.W_ILLUSION_ROOM)
        {
            client.Send("25;" + gameInfos.idJoueur + ";" + actualRoomX + ";" + actualRoomY + ";" + x + ";" + y);
            waitingMode = WaitingEvent.W_NOTHING;
        }
        else if (waitingMode == WaitingEvent.W_CONTROL_ROOM)
        {
            actualRoomX = x; actualRoomY = y;
            DirectionPanelScript dps = AskForDirectionAroundRoom(x, y, x != 2, x != 2, y != 2, y != 2);
            dps.SetColor(ActionType.CONTROLER);
        }
        complex.UnlightAll();
        complex.SetEveryRoomUnselectable();

    }
    public void MouseEnterRoom(int x, int y)
    {
        audioManager.MouseHover();
        if (waitingMode == WaitingEvent.W_CONTROL_ROOM)
        {
            int i;
            for(i = 0; i < 5; i++)
            {
                if (y != 2)
                    complex.complex[i, y].Highlight(Color.white, 0.2f);
                if (x != 2)
                    complex.complex[x, i].Highlight(Color.white, 0.2f);
            }
            
        }
        complex.complex[x, y].Highlight(Color.white, 0.3f);
    }
    public void MouseExitRoom(int x, int y)
    {
        complex.UnlightAll();
        //complex.complex[x, y].Unlight();
    }

    public void ApplyEffect(RoomType room)
    {

        foregroundEffect.DisplayEffect(room);
    }
    public void CreateAlert(AlertType type)
    {
        GameObject alertGO = Instantiate(alertObject) as GameObject;
        RectTransform alertRT = (RectTransform)alertGO.transform; 
        alertRT.SetParent(mainCanvas);
        AlertScript als = alertGO.GetComponent<AlertScript>();
        switch (type)
        {
            case AlertType.DEBUT:
                als.SetTexts("LA PARTIE COMMENCE", "veuillez choisir une première case à regarder !");
                break;
            case AlertType.ROOM25:
                als.SetTexts("LA ROOM 25 EST DECOUVERTE", "Dépechez vous d'y accéder avant que les gardiens vous en empechent !");
                break;
            case AlertType.TOUR5:
                als.SetTexts("5 TOURS RESTANTS", "Il reste 5 tours, il serait tant de se dépecher !");
                break;
            case AlertType.TOUR1:
                als.SetTexts("DERNIER TOUR", "C'est l'heure du dernier tour, j'espere que vous pouvez vous échapper !");
                break;
            case AlertType.MASQUES:
                als.SetTexts("LES MASQUES TOMBENT", "la majorité des joueurs sont dans la Room25, les autres joueurs voient leurs roles révélés");
                break;
            default:
                break;
        }
        Destroy(alertGO, 7.0f);

    }

    public void KillPlayer(int p)
    {
        complex.players[p].gameObject.SetActive(false);
        playersPanel.SetPlayerDead(p);
    }

    public void CancelGame(int p)
    {
        victoryPanel.Appear();
        victoryPanel.LoadDisconnection(p);
    }
    public void EndGame(int victoryType, string winnersStr)
    {
        List<int> winnersList = new List<int>();
        if (winnersStr.Length > 0)
        {
            if (winnersStr.Contains(","))
            {
                string[] winners = winnersStr.Split(',');

                foreach (string str in winners)
                {
                    winnersList.Add(int.Parse(str));
                }
            }
            else
            {
                winnersList.Add(int.Parse(winnersStr));
            }
        }
        victoryPanel.Appear();
        victoryPanel.LoadVictory(victoryType, winnersList);
    }

    public void BackToMenu()
    {
        client.Close();
        Destroy(audioManager.gameObject);
        Destroy(client.gameObject);
        Destroy(gameInfos.gameObject);
        Destroy(log.gameObject);
        Application.LoadLevel("menu scene");
    }

}