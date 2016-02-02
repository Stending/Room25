using UnityEngine;
using System.Collections;

public enum RoomType : int
{
    DEPART, R25, VORTEX, VISION, PIEGE, MOBILE, CONTROLE, JUMELLE, VIDE, MORTELLE, PRISON, ILLUSION, INNONDABLE, ACIDE, FROIDE, NOIRE
}

public enum Direction : int
{
    BAS, HAUT, GAUCHE, DROITE
}

public enum Mode : int
{
    SUSPISCION, COMPETITION, EQUIPE, COOPERATION, SOLO
}

public enum Role : int
{
    GUARDIAN, PRISONNER, UNKNOWN
}


public enum ActionType : int
{
    POUSSER, CONTROLER, DEPLACER, REGARDER
}
public enum CaracterType : int
{
    BG, BIMBO, COLOSSE, FILLE, GEEK, SAVANT
}

public enum AlertType :int
{
    DEBUT, ROOM25, TOUR5, TOUR1, MASQUES
}