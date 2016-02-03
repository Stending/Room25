#include "socks.h"

char datagramme[256] = "";
Connexion connexions[6];
int nbr_clients = 0;
int type_attendu = 0;
int id_revele = -1;
int fini = 0;
int en_jeu = 0;

void init_socks(int nbjoueurs, int port) {
    WSADATA WSAData;
    SOCKET sock, csock;
    SOCKADDR_IN sin, csin;
    int sinsize, i, j;
    char buff[256] = "";
    WSAStartup(MAKEWORD(2,0), &WSAData);
    sock = socket(AF_INET, SOCK_STREAM, 0);
    sin.sin_addr.s_addr = INADDR_ANY;
    sin.sin_family = AF_INET;
    sin.sin_port = htons(port);
    bind(sock, (SOCKADDR *)&sin, sizeof(sin));
    listen(sock, 0);
    sinsize = sizeof(SOCKADDR_IN);
    while(nbr_clients < nbjoueurs) {
        csock = accept(sock, (SOCKADDR *)&(csin), &sinsize);
        for(i = 0; i < nbr_clients && !connexions[i].parti; i++);
        printf("%s connecte\n", inet_ntoa(csin.sin_addr));
        connexions[i].socket = csock;
        connexions[i].sockaddr = csin;
        connexions[i].parti = 0;
        nbr_clients++;
        if(CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)gerer_connexion, (PVOID*)&i, 0, NULL) == NULL) {
            perror("Le thread n'a pas pu etre cree.");
            exit(1);
        }
        attendre_datagramme(buff, 1);
        strcpy(connexions[i].nom, buff);
        for(j = 0; j < nbr_clients; j++) {
            if(!connexions[j].parti) {
                sprintf(buff, "02;%s;%d;%s;%d;%d", inet_ntoa(connexions[i].sockaddr.sin_addr), i, connexions[i].nom, nbr_clients, nbjoueurs);
                envoyer(connexions[j], buff);
            }
        }
        sprintf(buff, "06;%d", i);
        envoyer(connexions[i], buff);
        for(j = 0; j < nbr_clients; j++) {
            if(!connexions[j].parti && j != i) {
                sprintf(buff, "15;%s;%d;%s", inet_ntoa(connexions[j].sockaddr.sin_addr), j, connexions[j].nom);
                envoyer(connexions[i], buff);
            }
        }
    }
    sprintf(buff, "05");
    for(i = 0; i < nbjoueurs; i++) {
        envoyer(connexions[i], buff);
    }
}

void gerer_connexion(void *idd) {
    char buff[256] = "";
    char str[256] = "";
    int type, i, id = *(int*)idd;
    Connexion co = connexions[id];
    while(!fini) {
        if(recv(co.socket, buff, sizeof(buff), 0) <= 0) {
            closesocket(connexions[id].socket);
            connexions[id].parti = 1;
            sprintf(buff, "03;%d",id);
            for(i = 0; i < nbr_clients; i++) {
                envoyer(connexions[i], buff);
            }
            nbr_clients--;
            if(en_jeu) {
                finir_socks(nbr_clients);
                exit(0);
            }
            break;
        }
        sscanf(buff, "%d;%s", &type, str);
        if(type == type_attendu) {
            strcpy(datagramme, str);
            type_attendu = 0;
        }
        if(type == 4) {
            for(i = 0; i < nbr_clients; i++) {
                envoyer(connexions[i], buff);
            }
        }
        if(type == 12) {
            id_revele = atoi(str);
        }
        printf("%s <- %s (%s)\n",buff, connexions[id].nom, inet_ntoa(co.sockaddr.sin_addr));
    }
}

void attendre_datagramme(char* dst, int type) {
    type_attendu = type;
    while(type_attendu != 0);
    strcpy(dst, datagramme);
}

void envoyer(Connexion co, char* contenu) {
    if(!co.parti) {
        Sleep(50);
        send(co.socket, contenu, 256, 0);
        printf("%s -> %s (%s)\n", contenu, co.nom, inet_ntoa(co.sockaddr.sin_addr));
    }
}

void recevoir_choix_joueurs(int choix[6], int nbjoueurs) {
    char buff[256];
    int i, id, perso, nbchoix = 0;
    for(i = 0; i < 6; i++) {
        choix[i] = -1;
    }
    while(nbchoix < nbr_clients) {
        do {
            attendre_datagramme(buff, 7);
            sscanf(buff, "%d;%d", &id, &perso);
        } while(tableau_contient(perso, choix, nbchoix));
        if(perso == -1)
            nbchoix--;
        else if(choix[id] == -1)
            nbchoix++;
        choix[id] = perso;
        sprintf(buff, "07;%d;%d", id, perso);
        for(i = 0; i < nbjoueurs; i++) {
            envoyer(connexions[i], buff);
        }
    }
}

void init_noms(char noms[6][256], int nbjoueurs) {
    int i;
    for(i = 0; i < nbjoueurs; i++) {
        strcpy(noms[i], connexions[i].nom);
    }
}

void recevoir_choix_mode(int *mode, int *expert, int nbjoueurs) {
    int i;
    char buff[256];
    attendre_datagramme(buff, 8);
    sscanf(buff, "%d;%d", mode, expert);
    for(i = 0; i < nbjoueurs; i++) {
        sprintf(buff, "08;%d;%d", *mode, *expert);
        envoyer(connexions[i], buff);
    }
}

void envoyer_type(int id, int type) {
    char buff[256];
    sprintf(buff, "09;%d", type);
    envoyer(connexions[id], buff);
}

void envoyer_complexe(char *comp, int nbjoueurs) {
    char buff[256];
    int i;
    sprintf(buff, "10;%s",comp);
    for(i = 0; i < nbjoueurs; i++) {
        envoyer(connexions[i], buff);
    }
}

void envoyer_regard(int nbjoueurs) {
    int i;
    char buff[256], str[256];
    attendre_datagramme(buff, 11);
    sprintf(str, "11;%s", buff);
    for(i = 0; i < nbjoueurs; i++) {
        envoyer(connexions[i], str);
    }
}

void tester_revelation(int *revele[6]) {
    int i, n;
    char buff[256];
    do {
        if(id_revele >= 0) {
            *revele[id_revele] = 1;
            sprintf(buff, "12;%d", id_revele);
            id_revele = -1;
            for(i = 0; i < nbr_clients; i++) {
                envoyer(connexions[i], buff);
            }
        }
        n = 0;
        for(i = 0; i < nbr_clients; i++) {
            if(*revele[i])
                n++;
        }
    } while(n < nbr_clients);
}

void demander_action(int id, int gele) {
    char buff[256];
    sprintf(buff, "14;%d", gele);
    envoyer(connexions[id], buff);
}

void recevoir_actions(int actions[6][2], int nbjoueurs) {
    char buff[256], str[256];
    int id, nbchoix = 0;
    while(nbchoix < nbjoueurs) {
        attendre_datagramme(buff, 13);
        sscanf(buff, "%d;%s", &id, str);
        sscanf(str, "%d;%d", &actions[id][0], &actions[id][1]);
        nbchoix++;
    }
}

void demander_details_action(int id, int ac, int possible, int allowbas, int allowhaut, int allowgauche, int allowdroite, int jmemecase[6], int nbmemecase) {
    char buff[256];
    int i;
    sprintf(buff, "16;%d;%d;%d;%d;%d;%d;%d;", id, ac, possible, allowbas, allowhaut, allowgauche, allowdroite);
    for(i = 0; i < nbmemecase; i++) {
        sprintf(buff, "%s%d", buff, jmemecase[i]);
        if(i < nbmemecase - 1)
            strcat(buff, ":");
    }
    envoyer(connexions[id], buff);
}

void recevoir_details_action(int id, int nbjoueurs, int *persoapousser, int *dir, int *coor1, int *coor2) {
    char buff[256], str[256];
    int tmp, i;
    do {
        attendre_datagramme(buff, 17);
        sscanf(buff, "%d;%s", &tmp, str);
    } while(tmp != id);
    sscanf(str, "%d;%s", &tmp, buff);
    switch(tmp) {
    case 0:
        sscanf(buff, "%d;%d", persoapousser, dir);
        sprintf(str,"17;%d;0;%d;%d", id, *persoapousser, *dir);
        break;
    case 1:
        sscanf(buff, "%d;%d", coor1, dir);
        sprintf(str, "17;%d;1;%d;%d", id, *coor1, *dir);
        break;
    case 2:
        *dir = atoi(buff);
        sprintf(str, "17;%d;2;%d", id, *dir);
        break;
    case 3:
        sscanf(buff, "%d;%d", coor1, coor2);
        sprintf(str, "17;%d;3;%d;%d", id, *coor1, *coor2);
        break;
    }
    for(i = 0; i < nbjoueurs; i++) {
        envoyer(connexions[i], str);
    }
}

void envoyer_salle(int salle, int nbjoueurs, int id) {
    int i;
    char buff[256];
    sprintf(buff, "18;%d;%d", id, salle);
    for(i = 0; i < nbjoueurs; i++) {
        envoyer(connexions[i], buff);
    }
}

void envoyer_alerte(int nbjoueurs) {
    int i;
    for(i = 0; i < nbjoueurs; i++) {
        envoyer(connexions[i], "19");
    }
}

void recevoir_vision(int id, int nbjoueurs) {
    char buff[256], str[256];
    int i, x, y, tmp;
    do {
        attendre_datagramme(buff, 20);
        sscanf(buff, "%d;%s", &tmp, str);
    } while(tmp != id);
    sscanf(str, "%d;%d", &x, &y);
    sprintf(buff, "20;%d;%d;%d", id, x, y);
    for(i = 0; i < nbjoueurs; i++) {
        envoyer(connexions[i], buff);
    }
}

void envoyer_revelation_salle(int nbjoueurs, int x, int y) {
    int i;
    char buff[256];
    sprintf(buff, "22;%d;%d", x, y);
    for(i = 0; i < nbjoueurs; i++) {
        envoyer(connexions[i], buff);
    }
}

void envoyer_salles_selectionnables(int id, int tab[24][2], int n, int salle, int x, int y) {
    char buff[256];
    int i;
    sprintf(buff, "21;%d;%d;%d;", salle, x, y);
    for(i = 0; i < n; i++) {
        if(tab[i][0] != x || tab[i][1] != y) {
            sprintf(buff, "%s%d,%d", buff, tab[i][0], tab[i][1]);
            if(i < n-1)
                strcat(buff, ":");
        }
    }
    envoyer(connexions[id], buff);
}

void recevoir_mobile(int id, int nbjoueurs, int *x, int *y) {
    char buff[256], str[256];
    int i, tmp, x1, y1;
    do {
        attendre_datagramme(buff, 23);
        sscanf(buff, "%d;%s", &tmp, str);
    } while(tmp != id);
    sscanf(str, "%d;%d;%d;%d", &x1, &y1, x, y);
    sprintf(buff, "23;%d;%s", id, str);
    for(i = 0; i < nbjoueurs; i++) {
        envoyer(connexions[i], buff);
    }
}

void recevoir_controle(int id, int nbjoueurs, int *n, int *dir) {
    int i, tmp;
    char buff[256], str[256];
    do {
        attendre_datagramme(buff, 24);
        sscanf(buff, "%d;%s", &tmp, str);
    } while(tmp != id);
    sscanf(str, "%d;%d", n, dir);
    sprintf(buff, "24;%d;%s", id, str);
    for(i = 0; i < nbjoueurs; i++) {
        envoyer(connexions[i], buff);
    }
}

void recevoir_illusion(int id, int nbjoueurs, int *x, int *y) {
    char buff[256], str[256];
    int i, tmp, x1, y1;
    do {
        attendre_datagramme(buff, 25);
        sscanf(buff, "%d;%s", &tmp, str);
    } while(tmp != id);
    sscanf(str, "%d;%d;%d;%d", &x1, &y1, x, y);
    sprintf(buff, "25;%d;%s", id, str);
    for(i = 0; i < nbjoueurs; i++) {
        envoyer(connexions[i], buff);
    }
}

void envoyer_jumelles(int nbjoueurs, int id, int x1, int y1, int x2, int y2) {
    char buff[256];
    int i;
    sprintf(buff, "26;%d;%d;%d;%d;%d", id, x1, y1, x2, y2);
    for(i = 0; i < nbjoueurs; i++) {
        envoyer(connexions[i], buff);
    }
}

void envoyer_revelation_role(int id, int role, int nbjoueurs) {
    int i;
    char buff[256];
    sprintf(buff, "27;%d;%d", id, role);
    for(i = 0; i < nbjoueurs; i++) {
        envoyer(connexions[i], buff);
    }
}

void envoyer_attention_les_masques_sont_tombes(int nbjoueurs) {
    int i;
    char buff[256] = "28";
    for(i = 0; i < nbjoueurs; i++) {
        envoyer(connexions[i], buff);
    }
}

void envoyer_mort(int id, int nbjoueurs) {
    int i;
    char buff[256];
    sprintf(buff, "29;%d", id);
    for(i = 0; i < nbjoueurs; i++) {
        envoyer(connexions[i], buff);
    }
}

void envoyer_victoire(int ids[6], int nb, int nbjoueurs, int type) {
    int i;
    char buff[256];
    sprintf(buff, "30;%d;", type);
    for(i = 0; i < nb; i++) {
        sprintf(buff, "%s%d", buff, ids[i]);
        if(i < nb - 1)
            strcat(buff, ",");
    }
    for(i = 0; i < nbjoueurs; i++) {
        envoyer(connexions[i], buff);
    }
}

void demander_action_immediate(int id, int allowpousse, int allowcontrole, int allowdeplace, int allowregard) {
    char buff[256];
    sprintf(buff, "31;%d;%d;%d;%d", allowpousse, allowcontrole, allowdeplace, allowregard);
    envoyer(connexions[id], buff);
}

int recevoir_action_immediate(int id) {
    int idrecu;
    char buff[256], str[256];
    do {
        attendre_datagramme(buff, 32);
        sscanf(buff, "%d;%s", &idrecu, str);
    } while(idrecu != id);
    return atoi(str);
}

void envoyer_nouveau_tour(int compteur, int nbjoueurs) {
    int i;
    char buff[256];
    sprintf(buff, "33;%d", compteur);
    for(i = 0; i < nbjoueurs; i++) {
        envoyer(connexions[i], buff);
    }
}

void finir_socks(int nbjoueurs) {
    int i;
    fini = 1;
    for(i = 0; i < nbjoueurs; i++) {
        if(!connexions[i].parti)
            closesocket(connexions[i].socket);
    }
    WSACleanup();
}

void commencer_jeu() {
    en_jeu = 1;
}
