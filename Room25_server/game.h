#ifndef GAME
#define GAME
#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <time.h>
#include <string.h>
#include "util.h"
#endif // GAME

typedef enum {S_DEPART, S_25, S_VORTEX, S_VISION, S_PIEGE, S_MOBILE, S_CONTROLE, S_JUMELLE, S_VIDE, S_MORTELLE, S_PRISON, S_ILLUSION, S_INONDABLE, S_ACIDE, S_FROIDE, S_NOIRE} TypeSalle;
typedef enum {M_SUSPISCION, M_COMPETITION, M_EQUIPE, M_COOPERATION, M_SOLO} Mode;
typedef enum {J_GARDIEN, J_PRISONNIER, J_NORMAL} TypeJoueur;
typedef enum {A_POUSSER, A_CONTROLER, A_DEPLACER, A_REGARDER} Action;
typedef enum {D_BAS, D_HAUT, D_GAUCHE, D_DROITE} Direction;
typedef enum {P_BG, P_BIMBO, P_COLOSSE, P_FILLE, P_GEEK, P_SAVANT} Personnage;
enum {V_PRIMORT, V_ROOM25, V_REBOURS};
typedef struct {
    TypeJoueur type;
    int revele;
    char nom[256];
    Personnage perso;
    int posx;
    int posy;
    int mort;
    Action action[2];
    Direction dir[2];
    int persopousse;
    int piege;
    int prison;
    int apne;
    int froid;
    int noire;
    int equipier;
} Joueur;
typedef struct {
    TypeSalle type;
    int visible;
} Salle;

void ajouter_pioche(int *pioche, int *taille, int elt);
void creer_complexe(Salle complexe[][5], Mode m, int expert, int nbjoueurs);
void role_joueurs(Joueur *joueurs, int nbjoueurs);
void init_joueurs(Joueur *joueurs, int nbjoueurs);
int action(Joueur *joueurs, int nbjoueurs, int debut, Salle complexe[][5], int *compteur, Mode mode);
int maxi(int a, int b);
int controler(Salle complexe[][5], Joueur joueurs[6], int nbjoueurs, int id, Direction dir, int compteur, Mode m);
void jouer(Salle complexe[][5], Joueur joueurs[6], int nbjoueurs, Mode mode, int expert);
void choix_mode(Mode *mode, int *expert, int nbjoueurs);
int peut_deplacer(Joueur joueurs[6], int nbjoueurs, int id, Salle complexe[][5]);
int peut_controler(Joueur j);
int peut_regarder(Joueur j, Salle complexe[][5]);
int peut_pousser(Joueur joueurs[6], int nbjoueurs, int id, Salle complexe[][5]);
int praticable_autour(Salle complexe[][5], Joueur joueurs[6], int nbjoueurs, int x, int y);
int praticable(Salle complexe[][5], Joueur joueurs[6], int nbjoueurs, int x, int y, Direction dir);
int contient_joueur(Joueur joueurs[6], int nbjoueurs, int x, int y, int id);
int joueurs_a_la_case(int jmemecase[6], Joueur joueurs[6], int nbjoueurs, int x, int y, int id, int type);
int tuer(Joueur joueurs[6], int nbjoueurs, int id, Mode m);
int chercher_jumelle(Salle complexe[][5], int *x, int *y);
int non_decouvert(Salle complexe[][5]);
int regardable(Salle complexe[][5], int x, int y, Direction dir);
int controlable(int id, int prec[5], Direction dir);
int regardable_autour(Salle complexe[][5], int x, int y);
int effet_salle(Salle complexe[][5], Joueur joueurs[6], int nbjoueurs, int idjoueur, Salle *salle_actuelle, int *compteur, Mode mode);
int salles_cachees(Salle complexe[][5], int tab[24][2]);
int sur_zone_sortie(int x, int y);
int sortir_complexe(Joueur joueurs[6], int nbjoueurs, int x, int y, int compteur, Mode m);
void victoire(int jvict[6], int nbvict, int nbjoueurs, int type);
