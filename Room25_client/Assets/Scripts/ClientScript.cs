using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;

public class ClientScript : MonoBehaviour {

    public SettingsManager settingsManager;
    public GameManager gameManager;

    public bool inGame = false;
    public AudioManager audioManager;
    public LogScript log;
    /*public TcpClient clientSocket;
    public NetworkStream serverStream;*/
    public Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    public GameInfosScript gameInfos;
    //private byte[] bytes = new byte[1024];
    private byte[] receiveBuffer = new byte[8142];
    private List<string> messages = new List<string>();
  
    // Use this for initialization
    void Start () {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        DontDestroyOnLoad(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        if (messages.Count > 0)
        {
            MessageReceived(messages[0]);
            messages.RemoveAt(0);
        }
	}


    public void ConnectTo(string ip, string port) {

        /*clientSocket.Connect(ip, int.Parse(port));
        serverStream = clientSocket.GetStream();*/
        socket.Connect(new IPEndPoint(IPAddress.Parse(ip), int.Parse(port)));
        socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);        
    }

    public void Send(string text)
    {
        //serverStream = clientSocket.GetStream();

        
        byte[] test = new byte[1024];
        test = System.Text.Encoding.ASCII.GetBytes(text.Normalize());

        byte[] outStream = new byte[test.Length + 1];
        test.CopyTo(outStream, 0);
        outStream[test.Length] = 0; 

        this.SendData( outStream);

    }




    private void ReceiveCallback(IAsyncResult AR)
    {
        int received = socket.EndReceive(AR);

        if (received <= 0)
            return;


        byte[] recData = new byte[received];
        Buffer.BlockCopy(receiveBuffer, 0, recData, 0, received);
        messages.Add(System.Text.Encoding.ASCII.GetString(recData, 0, received));
        //socket.EndReceive(AR);

        //receiveBuffer = new Byte[1024];
        socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
    }

    public void SendData(byte[] data)
    {
        //print("ah : " + System.Text.Encoding.ASCII.GetString(data, 0, data.Length));
        SocketAsyncEventArgs socketAsyncData = new SocketAsyncEventArgs();
        socketAsyncData.SetBuffer(data, 0, data.Length);
        socket.SendAsync(socketAsyncData);
    }

    private void MessageReceived(string msg)
    {
        print("Message Received : " +msg);
        msg = msg.Normalize();
        int actPlayer, subtype;

        string[] msgParts = msg.Split(';');
        switch (int.Parse(msgParts[0]))
        {
            case 4:
                audioManager.ChatMessageReceive();
                log.AddPlayerMessage(int.Parse(msgParts[1]), DebugString(msgParts[2]));
                break;
            case 2:
                log.AddMessage(msgParts[3] + " a rejoint la partie");
                gameInfos.AddUser(msgParts[1], int.Parse(msgParts[2]), msgParts[3]);
                gameInfos.nbJoueursTotal = int.Parse(msgParts[5]);
                settingsManager.UpdateListJoueurs();
                break;
            case 15:

                log.AddMessage("Vous avez rejoint le joueur " + DebugString(msgParts[3]));
                gameInfos.AddUser(msgParts[1], int.Parse(msgParts[2]), DebugString(msgParts[3]));
                settingsManager.UpdateListJoueurs();
                break;
            case 3:
                if (inGame)
                {
                    log.AddMessage("Partie terminée, un joueur s'est déconnecté");
                    gameManager.CancelGame(int.Parse(msgParts[1]));
                }
                else
                {
                    log.AddMessage(gameInfos.players[int.Parse(msgParts[1])].Name + " a quitté la partie");
                    gameInfos.DelUser(int.Parse(msgParts[1]));
                }
                break;
            case 6:
                audioManager.PlayerConnected();
                log.AddMessage("Vous êtes le joueur " + int.Parse(msgParts[1]));
                gameInfos.idJoueur = int.Parse(msgParts[1]);
                settingsManager.SwitchToMode(2);// SwitchToMode(1);
                break;
            case 5:
                log.AddMessage("Tout le monde est connecté : C'est parti !");
                settingsManager.SwitchToMode(3);
                settingsManager.AskMode(gameInfos.idJoueur == 0);
                break;
            case 8:
                bool expert = (int.Parse(msgParts[2]) != 0);
                log.AddMessage("Le mode de la partie est en mode " + (Mode)int.Parse(msgParts[1]) + " en difficulté " + ((expert) ? "expert" : "normale"));
                gameInfos.mode = (Mode)int.Parse(msgParts[1]);
                gameInfos.expert = expert;
                settingsManager.SwitchToMode(4);
                break;
            case 7:
               
                if(int.Parse(msgParts[2]) != -1){
                    audioManager.Validate();
                    gameInfos.players[int.Parse(msgParts[1])].Type = (CaracterType)int.Parse(msgParts[2]);
                    settingsManager.characterSelectionPanel.DisableCharacter(int.Parse(msgParts[2]), int.Parse(msgParts[1]));
                    log.AddMessage(gameInfos.players[int.Parse(msgParts[1])].Name + " est " + (CaracterType)int.Parse(msgParts[2]));
                }
                else
                {
                    audioManager.Cancel();
                    print("on a reçu -1");
                    settingsManager.characterSelectionPanel.EnableCharacterPlayer(int.Parse(msgParts[1]));
                    log.AddMessage(gameInfos.players[int.Parse(msgParts[1])].Name + "a annulé son choix");
                }
                break;
            case 10:
                gameInfos.gridCode = msgParts[1];
                Application.LoadLevel("game scene");
                inGame = true;
                //gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
                break;
            case 9:
                gameInfos.players[gameInfos.idJoueur].Role = (Role)int.Parse(msgParts[1]);
                break;
            case 14:
                //gameManager.WaitForAction(int.Parse(msgParts[1]), int.Parse(msgParts[2]), int.Parse(msgParts[3]), int.Parse(msgParts[4]));
                // print("PROGAMMEZ VOS ACTIONS");
                log.AddMessage("Le serveur attend vos actions");
                gameManager.WaitForAction(int.Parse(msgParts[1]) != 0);
                break;
            case 31:
                log.AddMessage("Le serveur attend votre action");
                gameManager.WaitForImmediateAction(int.Parse(msgParts[1]), int.Parse(msgParts[2]), int.Parse(msgParts[3]), int.Parse(msgParts[4]));
                break;
            case 11:
                if (gameInfos.idJoueur == int.Parse(msgParts[1]))
                {
                    gameManager.complex.complex[int.Parse(msgParts[2]), int.Parse(msgParts[3])].RevealHide();
                    log.AddMessage("Vous regardez la case (" + int.Parse(msgParts[2]) + "," + int.Parse(msgParts[3]) + ")");
                }
                else
                {
                    gameManager.PlayerLooksRoom(int.Parse(msgParts[1]), int.Parse(msgParts[2]), int.Parse(msgParts[3]));
                    log.AddMessage(gameInfos.players[int.Parse(msgParts[1])].Name + " regarde la case (" + int.Parse(msgParts[2]) + "," + int.Parse(msgParts[3]) + ")");
                }
                break;
            case 16:
                actPlayer = int.Parse(msgParts[1]);
                subtype = int.Parse(msgParts[2]);
                if (gameInfos.idJoueur == actPlayer)
                {
                    if (int.Parse(msgParts[3]) == 1)
                    {
                        switch (subtype)
                        {
                            case 0:
                                log.AddMessage("Choisissez un jouer à pousser");
                                gameManager.WaitForPlayerToPush(int.Parse(msgParts[4]), int.Parse(msgParts[5]), int.Parse(msgParts[6]), int.Parse(msgParts[7]), DebugString(msgParts[8]));
                                break;
                            case 1:
                                log.AddMessage("Choisissez une direction dans laquelle contrôler");
                                gameManager.WaitForControlDirection(int.Parse(msgParts[4]), int.Parse(msgParts[5]), int.Parse(msgParts[6]), int.Parse(msgParts[7]));
                                break;
                            case 2:
                                log.AddMessage("Choisissez une direction dans laquelle bouger");
                                gameManager.WaitForMoveDirection(int.Parse(msgParts[4]), int.Parse(msgParts[5]), int.Parse(msgParts[6]), int.Parse(msgParts[7]));
                                break;
                            case 3:
                                log.AddMessage("Choisissez une salle à regarder");
                                gameManager.WaitForRoomToLookDirection(int.Parse(msgParts[4]), int.Parse(msgParts[5]), int.Parse(msgParts[6]), int.Parse(msgParts[7]));
                                break;
                        }
                    }
                    else
                    {
                        log.AddMessage("Action impossible, désolé !");
                    }
                }
                break;
            case 17:
                actPlayer = int.Parse(msgParts[1]);
                subtype = int.Parse(msgParts[2]);
                audioManager.ActionMade(subtype);
                switch (subtype)
                {
                    case 0:
                        log.AddMessage(gameInfos.players[actPlayer].Name + " Pousse  " + gameInfos.players[int.Parse(msgParts[3])].Name + "dans la direction " + (Direction)int.Parse(msgParts[4]));
                        gameManager.PushPlayer(actPlayer, int.Parse(msgParts[3]), int.Parse(msgParts[4]));
                        break;
                    case 1:
                        log.AddMessage(gameInfos.players[actPlayer].Name + " déplace la " + ((int.Parse(msgParts[4]) == 0 || int.Parse(msgParts[4]) == 1) ? "colonne " : "ligne ") + int.Parse(msgParts[3]) + " dans la direction " + (Direction)int.Parse(msgParts[4]));
                        gameManager.ControlLine(int.Parse(msgParts[3]), int.Parse(msgParts[4]));
                        break;
                    case 2:
                        log.AddMessage(gameInfos.players[actPlayer].Name + " se déplace à " + (Direction)int.Parse(msgParts[3]));
                        gameManager.MovePlayer(actPlayer, int.Parse(msgParts[3]));
                        break;
                    case 3:
                        if (gameInfos.idJoueur == int.Parse(msgParts[1]))
                        {
                            gameManager.complex.complex[int.Parse(msgParts[3]), int.Parse(msgParts[4])].RevealHide();
                            log.AddMessage("Vous regardez la case (" + int.Parse(msgParts[3]) + "," + int.Parse(msgParts[4]) + ")");
                        }
                        else
                        {
                            gameManager.PlayerLooksRoom(int.Parse(msgParts[1]), int.Parse(msgParts[3]), int.Parse(msgParts[4]));
                            log.AddMessage(gameInfos.players[int.Parse(msgParts[1])].Name + " regarde la case (" + int.Parse(msgParts[3]) + "," + int.Parse(msgParts[4]) + ")");
                        }
                        break;
                }
                break;
            case 18:
                actPlayer = int.Parse(msgParts[1]);
                int roomType = int.Parse(msgParts[2]);
                if(gameInfos.idJoueur  == actPlayer)
                    gameManager.ApplyEffect((RoomType)roomType);
                audioManager.RoomRevealed(roomType);
                
                switch ((RoomType)roomType)
                {
                    case RoomType.DEPART:
                        break;
                    case RoomType.R25:
                        break;
                    case RoomType.VORTEX:
                        log.AddMessage("SALLE VORTEX, RETOUR A LA CASE DEPART");
                        gameManager.VortexTeleport(actPlayer);
                        break;
                    case RoomType.VISION:
                        log.AddMessage("SALLE VISION, CHOISISSEZ UNE SALLE A REGARDER");
                        break;
                    case RoomType.PIEGE:
                        log.AddMessage("SALLE PIEGEE, DEPLACEZ VOUS AU PROCHAIN TOUR SINON AÏE AÏE AÏE");
                        break;
                    case RoomType.MOBILE:
                        log.AddMessage("SALLE MOBILE, CHOISISSEZ UNE SALLE AVEC LAQUELLE ECHANGER");
                        break;
                    case RoomType.CONTROLE:
                        if (actPlayer == gameInfos.idJoueur)
                            gameManager.WaitForControlLine();
                        log.AddMessage("SALLE DE CONTROLE, CHOISISSEZ UNE LIGNE A DEPLACER");
                        break;
                    case RoomType.JUMELLE:
                        log.AddMessage("SALLE JUMELLE");
                        break;
                    case RoomType.MORTELLE:
                        log.AddMessage("SALLE MORTELLE");
                        break;
                    case RoomType.PRISON:
                        log.AddMessage("SALLE PRISON");
                        break;
                    case RoomType.ILLUSION:
                        log.AddMessage("SALLE MOBILE, CHOISISSEZ UNE SALLE AVEC LAQUELLE ECHANGER");
                        break;
                    case RoomType.INNONDABLE:
                        log.AddMessage("SALLE INNONDABLE");
                        break;
                    case RoomType.ACIDE:
                        log.AddMessage("SALLE ACIDE");
                        break;
                    case RoomType.FROIDE:
                        log.AddMessage("SALLE FROIDE");
                        break;
                    case RoomType.NOIRE:
                        log.AddMessage("SALLE NOIRE");
                        break;
                }


                break;
            case 19:
                log.AddMessage("ON A TROUVÉ LA SALLE 25");
                break;
            case 22:
                RoomType room = gameManager.complex.complex[int.Parse(msgParts[1]), int.Parse(msgParts[2])].roomType;
                log.AddMessage("La salle (" + int.Parse(msgParts[1]) + "," + int.Parse(msgParts[2]) + " est révélée, c'était une " + room);
                gameManager.complex.complex[int.Parse(msgParts[1]), int.Parse(msgParts[2])].Reveal();
                if(room == RoomType.R25)
                    gameManager.CreateAlert(AlertType.ROOM25);
                break;
            case 21:
                switch ((RoomType)int.Parse(msgParts[1]))
                {
                    case RoomType.VISION:
                        log.AddMessage("vous devez choisir une case à regarder");
                        gameManager.WaitForVisionRoom(msgParts[4]);
                        break;
                    case RoomType.MOBILE:
                        log.AddMessage("vous devez choisir une case à avec laquelle échanger");
                        gameManager.WaitForMobileRoom(int.Parse(msgParts[2]), int.Parse(msgParts[3]), msgParts[4]);
                        break;
                    case RoomType.ILLUSION:
                        log.AddMessage("vous devez choisir une case sur laquelle vous déplacer");
                        gameManager.WaitForIllusionRoom(int.Parse(msgParts[2]), int.Parse(msgParts[3]), msgParts[4]);
                        break;

                }
                break;
            case 23:
                log.AddMessage("Echange de salle (salle mobile)");
                gameManager.SwitchRoomWithPlayers(int.Parse(msgParts[2]), int.Parse(msgParts[3]), int.Parse(msgParts[4]), int.Parse(msgParts[5]));
                break;
            case 25:
                log.AddMessage("Personnage déplacé (salle illusion");
                gameManager.SwitchRoomWithoutPlayers(int.Parse(msgParts[2]), int.Parse(msgParts[3]), int.Parse(msgParts[4]), int.Parse(msgParts[5]));
                break;
            case 20:

                if (gameInfos.idJoueur == int.Parse(msgParts[1]))
                {
                    log.AddMessage("Vous regardez une salle");
                    gameManager.complex.complex[int.Parse(msgParts[2]), int.Parse(msgParts[3])].RevealHide();
                }
                else
                {
                    gameManager.PlayerLooksRoom(int.Parse(msgParts[1]), int.Parse(msgParts[2]), int.Parse(msgParts[3]));
                    log.AddMessage(gameInfos.players[int.Parse(msgParts[1])].Name + " regarde la case ");
                }
                break;
            case 24:
                log.AddMessage("Une ligne est contrôlée (salle de contrôle)");
                gameManager.ControlLine(int.Parse(msgParts[2]), int.Parse(msgParts[3]));
                break;
            case 33:
                gameManager.NextTurn(int.Parse(msgParts[1]));
                if (int.Parse(msgParts[1]) == 5)
                {
                    audioManager.ChangeMusic(2);
                    gameManager.CreateAlert(AlertType.TOUR5);
                }else if (int.Parse(msgParts[1]) == 1)
                    gameManager.CreateAlert(AlertType.TOUR1);
                break;
            case 28:
                log.AddMessage("La majorité des joueurs est sur la room 25, on révèle les autres joueurs");
                gameManager.CreateAlert(AlertType.MASQUES);
                break;
            case 12:
                gameManager.RevealAGuardian(int.Parse(msgParts[1]));
                break;
            case 29:
                gameManager.KillPlayer(int.Parse(msgParts[1]));
                break;
            case 30:
                gameManager.EndGame(int.Parse(msgParts[1]), DebugString(msgParts[2]));
                break;
        }
    }


    public static string DebugString(string str)
    {
        return str.Substring(0, str.IndexOf('\0'));
    }
    
    public void Close()
    {
        socket.Close();
    }
}
