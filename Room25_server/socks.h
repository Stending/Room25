#ifndef SOCKS
#define SOCKS
#include <stdio.h>
#include <stdlib.h>
#include <winsock2.h>
#include <string.h>
#include "util.h"
#endif // SOCKS

typedef struct {
    SOCKET socket;
    SOCKADDR_IN sockaddr;
    char nom[256];
    int parti;
} Connexion;

void init_socks(int nbjoueurs, int port);
void gerer_connexion(void *in);
void attendre_datagramme(char* dst, int type);
void envoyer(Connexion co, char* contenu);
void recevoir_choix_joueurs(int choix[6], int nbjoueurs);
void init_noms(char noms[6][256], int nbjoueurs);
void recevoir_choix_mode(int *mode, int *expert, int nbjoueurs);
void envoyer_type(int id, int type);
void envoyer_complexe(char *comp, int nbjoueurs);
void envoyer_regard(int nbjoueurs);
void tester_revelation(int *revele[6]);
void demander_action(int id, int gele);
void recevoir_actions(int actions[6][2], int nbjoueurs);
void demander_details_action(int id, int ac, int possible, int allowbas, int allowhaut, int allowgauche, int allowdroite, int jmemecase[6], int nbmemecase);
void recevoir_details_action(int id, int nbjoueurs, int *persoapousser, int *dir, int *coor1, int *coor2);
void envoyer_salle(int salle, int nbjoueurs, int id);
void envoyer_alerte(int nbjoueurs);
void recevoir_vision(int id, int nbjoueurs);
void envoyer_revelation_salle(int nbjoueurs, int x, int y);
void envoyer_salles_selectionnables(int id, int tab[24][2], int n, int salle, int x, int y);
void recevoir_mobile(int id, int nbjoueurs, int *x, int *y);
void recevoir_controle(int id, int nbjoueurs, int *n, int *dir);
void recevoir_illusion(int id, int nbjoueurs, int *x, int *y);
void envoyer_jumelles(int nbjoueurs, int id, int x1, int y1, int x2, int y2);
void envoyer_revelation_role(int id, int role, int nbjoueurs);
void envoyer_attention_les_masques_sont_tombes(int nbjoueurs);
void envoyer_mort(int id, int nbjoueurs);
void envoyer_victoire(int ids[6], int nb, int nbjoueurs, int type);
void demander_action_immediate(int id, int allowpousse, int allowcontrole, int allowdeplace, int allowregard);
int recevoir_action_immediate(int id);
void envoyer_nouveau_tour(int compteur, int nbjoueurs);
void finir_socks(int nbjoueurs);
void commencer_jeu();
