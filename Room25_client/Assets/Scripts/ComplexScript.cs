using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComplexScript : MonoBehaviour {

    public float scale = 2.1f;
    public Object roomPrefab;

    public RoomScript[,] complex = new RoomScript[5, 5];
    public PlayerScript[] players = new PlayerScript[6];
    public int nbplayers = 2;

    public Object characterPrefab = null;
    // Use this for initialization
    void Start () {

       /* GameInfosScript gi = GameObject.FindGameObjectWithTag("GameInfos").GetComponent<GameInfosScript>();
        InstantiateComplex();
        InitializeComplex(gi.gridCode, gi.players, gi.nbJoueursTotal);
        InstantiateComplex();
        LoadComplex(GenerateRandomComplex(false, false));
        UpdateVisual();*/
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("n"))
            RevealEverything();
	}


    public void InitializeComplex(string complexCode, UserInfos[] users, int nbjoueurs)
    {
        int i;
        RoomType[,] comp = new RoomType[5, 5];
        string[] lines = complexCode.Split(':');
        for (i = 0; i < 5; i++)
        {
            string[] rooms = lines[i].Split(',');
            for(int j = 0; j < 5; j++)
            {
                int room = int.Parse(rooms[j]);
                print("salle actuelle : " + room);
                comp[i, j] = (RoomType)room;
            }
        }
        InstantiateComplex();
        LoadComplex(comp);

        for (i = 0; i < nbjoueurs; i++)
        {
            players[i] = InstantiateCharacter(users[i]);
        }
        nbplayers = nbjoueurs;
        ReplaceAllPlayers(2, 2);
    }

    private RoomType[,] GenerateRandomComplex(bool expert, bool suspiscion)
    {
        RoomType[,] comp = new RoomType[5, 5];
        comp[2, 2] = RoomType.DEPART;

        int i, j, random;
        List<RoomType> pioche = new List<RoomType>();
        pioche.Add(RoomType.VIDE); pioche.Add(RoomType.VIDE); pioche.Add(RoomType.VIDE); pioche.Add(RoomType.VIDE);
        pioche.Add(RoomType.NOIRE); pioche.Add(RoomType.NOIRE);
        pioche.Add(RoomType.FROIDE); pioche.Add(RoomType.FROIDE);
        pioche.Add(RoomType.PIEGE); pioche.Add(RoomType.PIEGE);
        pioche.Add(RoomType.INNONDABLE); pioche.Add(RoomType.INNONDABLE);
        pioche.Add(RoomType.ACIDE); pioche.Add(RoomType.ACIDE);
        pioche.Add(RoomType.VORTEX);
        pioche.Add(RoomType.MORTELLE);
        pioche.Add(RoomType.CONTROLE);

        if (expert)
        {
            pioche.Add(RoomType.ILLUSION);
            if (suspiscion)
            {
                pioche.Add(RoomType.VIDE); pioche.Add(RoomType.VIDE);
                pioche.Add(RoomType.MORTELLE);
                pioche.Add(RoomType.MOBILE);
            }
            else
            {
                pioche.Add(RoomType.JUMELLE); pioche.Add(RoomType.JUMELLE);
                pioche.Add(RoomType.PRISON); pioche.Add(RoomType.PRISON);
            }
        }
        else
        {
            pioche.Add(RoomType.VIDE); pioche.Add(RoomType.VIDE); pioche.Add(RoomType.VIDE); pioche.Add(RoomType.VIDE);
            pioche.Add(RoomType.VORTEX);
        }

        for (i = 0; i < 5; i++)
        {
            for (j = Mathf.Abs(i - 2); j <= 4 - Mathf.Abs(i - 2); j++)
            {
                if (i != 2 || j != 2)
                {
                    random = Random.Range(0, pioche.Count);
                    comp[i,j] = pioche[random];
                    pioche.RemoveAt(random);
                }
            }
        }
        pioche.Add(RoomType.R25); pioche.Add(RoomType.VISION);
        for (i = 0; i < 5; i++){
            for (j = 0; j < 5; j++){
                if (j < Mathf.Abs(i - 2) || j > 4 - Mathf.Abs(i - 2)) {
                    random = Random.Range(0, pioche.Count);
                    comp[i, j] = pioche[random];
                    pioche.RemoveAt(random);
                }
            }
        }

        return comp;
    }

    public void LoadComplex(RoomType[,] comp)
    {
        for (int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                complex[i, j].roomType = comp[i, j];
                
            }
        }

    }


    private void InstantiateComplex()
    {
        int i, j;
        for (i = 0; i < 5; i++)
        {
            for (j = 0; j < 5; j++)
            {
                    complex[i,j] = InstantiateRoom(i, j, (i == 2 && j == 2));
            }
        }
    }

    private RoomScript InstantiateRoom(int x,int y, bool vis)
    {
        GameObject room = Instantiate(roomPrefab) as GameObject;
        room.transform.parent = this.transform;
        room.transform.localPosition = new Vector3(x * scale - 2 * scale, -y * scale + 2 * scale, 0);
        RoomScript rs = room.GetComponent<RoomScript>();
        rs.x = x;
        rs.y = y;
        rs.visible = vis;
        return rs;

    }
    private PlayerScript InstantiateCharacter(UserInfos user) {
        GameObject character = Instantiate(characterPrefab) as GameObject;

        PlayerScript ps = character.GetComponent<PlayerScript>();
        ps.Initialize(user);
        ps.x = 2; ps.y = 2;
        Vector2 pos = GetRoomPositionInScene(2, 2);
        ps.transform.position = new Vector3(pos.x, pos.y, -2);
        ps.transform.parent = complex[2, 2].transform;
        return ps;

    }

    public void UpdateVisual()
    {
        for (int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                complex[i, j].UpdateVisual();
            }
        }

    }

    public void MoveLine(int i, Direction dir)
    {
        int p, pd, incr, end;
        int pv;

        switch (dir)
        {
            case Direction.BAS:
                p = i; pd = 0; incr = 1; end = 4;
                break;
            case Direction.HAUT:
                p = i; pd = 4; incr = -1; end = 0;
                break;
            case Direction.GAUCHE:
                p = i; pd = 4; incr = -1; end = 0;
                break;
            case Direction.DROITE:
                p = i; pd = 0; incr = 1; end = 4;
                break;
            default:
                p = i; pd = 0; incr = 1; end = 4;
                break;
        }

        RoomScript prevRoom = null, nextRoom = null, nextRoom2 = null;



            for(int k = 0;k<nbplayers;k++) {
            PlayerScript ps = players[k];
                if (ps != null)
                {
                    if (dir == Direction.BAS || dir == Direction.HAUT)
                    {
                        if (ps.x == p)
                        {
                            ps.y += incr;
                            if (ps.y < 0) ps.y = 4;
                            if (ps.y >= 5) ps.y = 0;
                        }
                    }
                    else
                    {
                        if (ps.y == p)
                        {
                            ps.x += incr;
                            if (ps.x < 0) ps.x = 4;
                            if (ps.x >= 5) ps.x = 0;
                        }
                    }
                }
            }

        for (pv = pd; pv != end + incr; pv += incr)
        {
            if (dir == Direction.BAS || dir == Direction.HAUT)
            {
                complex[p, pv].y += incr;
                if (pv == end)
                    complex[p, end].y = pd;
            }
            else
            {
                complex[pv, p].x += incr;
                if (pv == end)
                    complex[end, p].x = pd;
            }
        }
        for (pv = pd; pv != end + incr; pv += incr)
            {
                Vector2 nextPos;
                if (dir == Direction.BAS || dir == Direction.HAUT)
                {


                    if (pv == end)
                        nextPos = GetRoomPositionInScene(p, pd);
                    else
                        nextPos = GetRoomPositionInScene(p, pv + incr);


                    if (pv == pd)
                    {
                        prevRoom = complex[p, pv];
                        nextRoom = complex[p, pv + incr];
                        complex[p, pv + incr] = complex[p, pv];
                    }
                    else if (pv == end)
                    {
                        prevRoom = nextRoom;
                        complex[p, pd] = nextRoom;
                    }
                    else
                    {
                        prevRoom = nextRoom;
                        nextRoom2 = complex[p, pv + incr];
                        complex[p, pv + incr] = nextRoom;
                        nextRoom = nextRoom2;
                    }

                }
                else
                {

                    if (pv == end)
                        nextPos = GetRoomPositionInScene(pd, p);
                    else
                        nextPos = GetRoomPositionInScene(pv + incr, p);


                    if (pv == pd)
                    {
                        prevRoom = complex[pv, p];
                        nextRoom = complex[pv + incr, p];
                        complex[pv + incr, p] = complex[pv, p];
                    }
                    else if (pv == end)
                    {
                        prevRoom = nextRoom;
                        complex[pd, p] = nextRoom;
                    }
                    else
                    {
                        prevRoom = nextRoom;
                        nextRoom2 = complex[pv + incr, p];
                        complex[pv + incr, p] = nextRoom;
                        nextRoom = nextRoom2;
                    }
                }

                StartCoroutine(prevRoom.Move(nextPos, 2.0f));
            }



    }

    public Vector2 GetRoomPositionInScene(int x, int y)
    {
        return new Vector2(x * scale - scale * 2 + this.transform.position.x, -y * scale + scale * 2 + this.transform.position.y);

    }

    public void RevealEverything()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                complex[i, j].RevealHide();
            }
        }

    }


    public void UnlightAll()
    {
        for(int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                complex[i, j].Unlight();
            }
        }
    }

    public void SetSelectableRooms(string roomsString)
    {
        print("On split " + roomsString);
        string[] rooms = roomsString.Split(':');
        foreach(string room in rooms)
        {
           
                print("On split " + room);
                string[] coor = room.Split(',');
                print("on obtient " + coor[0] + " et " + coor[1]);
                complex[int.Parse(coor[0]), int.Parse(coor[1])].selectable = true;
        }

    }

    public void SetEveryRoomUnselectable()
    {
        foreach(RoomScript rs in complex)
        {
            rs.selectable = false;
        }
    }


    public void SetEveryRoomSelectable()
    {
        foreach (RoomScript rs in complex)
        {
            rs.selectable = true;
        }
    }

    public void SetSelectable(int x, int y, bool selectable)
    {
        complex[x, y].selectable = selectable;
    }

    public List<PlayerScript> GetPlayersInRoom(int x, int y)
    {
        List<PlayerScript> playersInRoom = new List<PlayerScript>();
        for(int i = 0; i < nbplayers; i++)
        {
            if(players[i].x == x && players[i].y == y)
            {
                playersInRoom.Add(players[i]);
            }
        }
        return playersInRoom;
    }

    public void ReplaceAllPlayers(int x, int y)
    {
        List<PlayerScript> players = GetPlayersInRoom(x, y);
        int nb = players.Count;
        for (int i = 0; i < nb; i++)
        {
            StartCoroutine(players[i].Move(GetRoomPositionInScene(x, y) + GetPositionInRoom(i, nb), 1.0f));
        }
    }
    public void ReplacePlayersBeforeNewOne(int x, int y)
    {
        List<PlayerScript> players = GetPlayersInRoom(x, y);
        int nb = players.Count;
        for (int i = 0; i < nb; i++)
        {
            StartCoroutine(players[i].Move(GetRoomPositionInScene(x, y) + GetPositionInRoom(i, nb+1), 1.0f));
        }
    }

    public Vector2 GetLastPlayerPositionInRoom(int x,int y)
    {
        int max = GetPlayersInRoom(x, y).Count+1;
        return GetRoomPositionInScene(x, y) + GetPositionInRoom(max - 1, max);

    }
    Vector2 GetPositionInRoom(int joueur, int nbjoueur)
    {
        switch (nbjoueur)
        {
            case 1:
                return new Vector2(0, 0);
            case 2:
                return new Vector2((float)joueur - 0.5f, 0);
            case 3:
                return new Vector2((float)(joueur - 1) / 3, (float)(joueur % 2) - 0.5f);
            case 4:
                return new Vector2((joueur % 2) - 0.5f, Mathf.Floor(joueur / 2) - 0.5f);
            case 5:
                if (joueur == 4)
                    return new Vector2(0, 0);
                else
                    return new Vector2((joueur % 2) - 0.5f, Mathf.Floor(joueur / 2) - 0.5f);
            case 6:
                return new Vector2((joueur - 1) / 3, Mathf.Floor(joueur / 3) - 0.5f);
            default:
                return new Vector2(0, 0);
        }
    }

}
