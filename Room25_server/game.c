#include "game.h"
#include "socks.h"

void ajouter_pioche(int *pioche, int *taille, int elt) {
    pioche[*taille] = elt;
    (*taille)++;
}

void creer_complexe(Salle complexe[][5], Mode m, int expert, int nbjoueurs) {
    int pioche[22];
    char str[256], value[5];
    int taille = 0;
    int random;
    int i, j, k;
    complexe[2][2].type = S_DEPART;
    complexe[2][2].visible = 1;
    ajouter_pioche(pioche, &taille, (int)S_VIDE);
    ajouter_pioche(pioche, &taille, (int)S_VIDE);
    ajouter_pioche(pioche, &taille, (int)S_VIDE);
    ajouter_pioche(pioche, &taille, (int)S_VIDE);
    ajouter_pioche(pioche, &taille, (int)S_NOIRE);
    ajouter_pioche(pioche, &taille, (int)S_NOIRE);
    ajouter_pioche(pioche, &taille, (int)S_FROIDE);
    ajouter_pioche(pioche, &taille, (int)S_FROIDE);
    ajouter_pioche(pioche, &taille, (int)S_PIEGE);
    ajouter_pioche(pioche, &taille, (int)S_PIEGE);
    ajouter_pioche(pioche, &taille, (int)S_INONDABLE);
    ajouter_pioche(pioche, &taille, (int)S_INONDABLE);
    ajouter_pioche(pioche, &taille, (int)S_ACIDE);
    ajouter_pioche(pioche, &taille, (int)S_ACIDE);
    ajouter_pioche(pioche, &taille, (int)S_VORTEX);
    ajouter_pioche(pioche, &taille, (int)S_MORTELLE);
    ajouter_pioche(pioche, &taille, (int)S_CONTROLE);
    if(expert) {
        ajouter_pioche(pioche, &taille, (int)S_ILLUSION);
        if(m == M_SUSPISCION) {
            ajouter_pioche(pioche, &taille, (int)S_VIDE);
            ajouter_pioche(pioche, &taille, (int)S_VIDE);
            ajouter_pioche(pioche, &taille, (int)S_MORTELLE);
            ajouter_pioche(pioche, &taille, (int)S_MOBILE);
        }
        else {
            ajouter_pioche(pioche, &taille, (int)S_JUMELLE);
            ajouter_pioche(pioche, &taille, (int)S_JUMELLE);
            ajouter_pioche(pioche, &taille, (int)S_PRISON);
            ajouter_pioche(pioche, &taille, (int)S_PRISON);
        }
    }
    else {
        ajouter_pioche(pioche, &taille, (int)S_VIDE);
        ajouter_pioche(pioche, &taille, (int)S_VIDE);
        ajouter_pioche(pioche, &taille, (int)S_VIDE);
        ajouter_pioche(pioche, &taille, (int)S_VIDE);
        ajouter_pioche(pioche, &taille, (int)S_VORTEX);
    }
    srand(time(NULL));
    for(i = 0; i < 5; i++) {
        for(j = abs(i - 2); j <= 4 - abs(i - 2); j++) {
            if(i != 2 || j != 2) {
                random = rand() % taille;
                complexe[i][j].type = pioche[random];
                complexe[i][j].visible = 0;
                taille--;
                for(k = random; k < taille; k++) {
                    pioche[k] = pioche[k+1];
                }
            }
        }
    }
    pioche[taille] = S_25;
    taille++;
    pioche[taille] = S_VISION;
    taille++;
    for(i = 0; i < 5; i++) {
        for(j = 0; j < 5; j++) {
            if(j < abs(i - 2) || j > 4 - abs(i - 2)) {
                random = rand() % taille;
                complexe[i][j].type = pioche[random];
                complexe[i][j].visible = 0;
                taille--;
                for(k = random; k < taille; k++) {
                    pioche[k] = pioche[k+1];
                }
            }
        }
    }
    complexe[2][3].type = S_MOBILE;
    *str = '\0';
    for(i = 0; i < 5; i++) {
        for(j = 0; j < 5; j++) {
            itoa(complexe[i][j].type, value, 10);
            strcat(str, value);
            if(j < 4)
                strcat(str, ",");
        }
        strcat(str, ":");
    }
    envoyer_complexe(str, nbjoueurs);
}

void role_joueurs(Joueur *joueurs, int nbjoueurs) {
    int i, k, random;
    int taille = 0;
    int pioche[6];
    srand(time(NULL));
    ajouter_pioche(pioche, &taille, (int)J_GARDIEN);
    ajouter_pioche(pioche, &taille, (int)J_PRISONNIER);
    ajouter_pioche(pioche, &taille, (int)J_PRISONNIER);
    ajouter_pioche(pioche, &taille, (int)J_PRISONNIER);
    ajouter_pioche(pioche, &taille, (int)J_PRISONNIER);
    if(nbjoueurs >= 5)
        ajouter_pioche(pioche, &taille, (int)J_GARDIEN);
    for(i = 0; i < nbjoueurs; i++) {
        random = rand() % taille;
        joueurs[i].type = pioche[random];
        envoyer_type(i, joueurs[i].type);
        taille--;
        for(k = random; k < taille; k++) {
            pioche[k] = pioche[k+1];
        }
    }
}

void choix_mode(Mode *mode, int *expert, int nbjoueurs) {
    recevoir_choix_mode((int*)mode, expert, nbjoueurs);
}

void init_joueurs(Joueur *joueurs, int nbjoueurs) {
    int i, choix[6];
    char noms[6][256];
    recevoir_choix_joueurs(choix, nbjoueurs);
    init_noms(noms, nbjoueurs);
    for(i = 0; i < nbjoueurs; i++) {
        if(choix[i] != -1) {
            joueurs[i].perso = (Personnage)choix[i];
            strcpy(joueurs[i].nom, noms[i]);
            joueurs[i].revele = 0;
            joueurs[i].apne = 0;
            joueurs[i].froid = 0;
            joueurs[i].noire = 0;
            joueurs[i].piege = 0;
            joueurs[i].prison = 0;
            joueurs[i].mort = 0;
            joueurs[i].posx = 2;
            joueurs[i].posy = 2;
            joueurs[i].type = J_NORMAL;
            joueurs[i].action[0] = -1;
            joueurs[i].action[1] = -1;
            joueurs[i].dir[0] = -1;
            joueurs[i].dir[1] = -1;
            joueurs[i].equipier = -1;
        }
        else {
            joueurs[i].mort = 1;
            joueurs[i].revele = 1;
            joueurs[i].posx = -1;
            joueurs[i].posy = -1;
            joueurs[i].type = J_NORMAL;
            strcpy(joueurs[i].nom, noms[i]);
        }
    }
}

int action(Joueur *joueurs, int nbjoueurs, int debut, Salle complexe[][5], int *compteur, Mode mode) {
    int i, n, nbmemecase, tmpdir, coor1, coor2, prec = -1, vict = 0;
    int jmemecase[6], lignes[5], colonnes[5];
    for(i = 0; i < 5; i++) {
        lignes[i] = -1;
        colonnes[i] = -1;
    }
    for(n = 0; n < 2; n++) {
        for(i = 0; i < nbjoueurs; i++) {
            if(!joueurs[(i+debut)%nbjoueurs].mort) {
                printf("(%d,%d)\n", joueurs[(i+debut)%nbjoueurs].posx, joueurs[(i+debut)%nbjoueurs].posy);
                if(joueurs[(i+debut)%nbjoueurs].type == J_GARDIEN && joueurs[(i+debut)%nbjoueurs].revele) {
                    if(joueurs[(i+debut)%nbjoueurs].froid != 2) {
                        demander_action_immediate((i+debut)%nbjoueurs, peut_pousser(joueurs, nbjoueurs, (i+debut)%nbjoueurs, complexe) && prec != A_POUSSER, peut_controler(joueurs[(i+debut)%nbjoueurs]) && prec != A_CONTROLER, peut_deplacer(joueurs, nbjoueurs, (i+debut)%nbjoueurs, complexe) && prec != A_DEPLACER, peut_regarder(joueurs[(i+debut)%nbjoueurs], complexe) && prec != A_REGARDER);
                        joueurs[(i+debut)%nbjoueurs].action[n] = recevoir_action_immediate((i+debut)%nbjoueurs);
                        prec = joueurs[(i+debut)%nbjoueurs].action[n];
                        if(joueurs[(i+debut)%nbjoueurs].froid == 1)
                            joueurs[(i+debut)%nbjoueurs].froid = 2;
                    }
                    else {
                        joueurs[(i+debut)%nbjoueurs].froid = 1;
                    }

                }
                switch(joueurs[(i+debut)%nbjoueurs].action[n]) {
                case A_DEPLACER:
                    demander_details_action((i+debut)%nbjoueurs, A_DEPLACER, peut_deplacer(joueurs, nbjoueurs, (i+debut)%nbjoueurs, complexe), praticable(complexe, joueurs, nbjoueurs, joueurs[(i+debut)%nbjoueurs].posx, joueurs[(i+debut)%nbjoueurs].posy, D_BAS), praticable(complexe, joueurs, nbjoueurs, joueurs[(i+debut)%nbjoueurs].posx, joueurs[(i+debut)%nbjoueurs].posy, D_HAUT), praticable(complexe, joueurs, nbjoueurs, joueurs[(i+debut)%nbjoueurs].posx, joueurs[(i+debut)%nbjoueurs].posy, D_GAUCHE), praticable(complexe, joueurs, nbjoueurs, joueurs[(i+debut)%nbjoueurs].posx, joueurs[(i+debut)%nbjoueurs].posy, D_DROITE), NULL, 0);
                    if(peut_deplacer(joueurs, nbjoueurs, (i+debut)%nbjoueurs, complexe)) {
                        recevoir_details_action((i+debut)%nbjoueurs, nbjoueurs, NULL, &tmpdir, NULL, NULL);
                        joueurs[(i+debut)%nbjoueurs].dir[n] = (Direction)tmpdir;
                        switch(joueurs[(i+debut)%nbjoueurs].dir[n]) {
                        case D_HAUT:
                            joueurs[(i+debut)%nbjoueurs].posy--;
                            break;
                        case D_BAS:
                            joueurs[(i+debut)%nbjoueurs].posy++;
                            break;
                        case D_GAUCHE:
                            joueurs[(i+debut)%nbjoueurs].posx--;
                            break;
                        case D_DROITE:
                            joueurs[(i+debut)%nbjoueurs].posx++;
                            break;
                        }
                        vict = effet_salle(complexe, joueurs, nbjoueurs, (i+debut)%nbjoueurs, &complexe[joueurs[(i+debut)%nbjoueurs].posx][joueurs[(i+debut)%nbjoueurs].posy], compteur, mode);
                    }
                    break;
                case A_POUSSER:
                    nbmemecase = joueurs_a_la_case(jmemecase, joueurs, nbjoueurs, joueurs[(i+debut)%nbjoueurs].posx, joueurs[(i+debut)%nbjoueurs].posy, (i+debut)%nbjoueurs, -1);
                    demander_details_action((i+debut)%nbjoueurs, A_POUSSER, peut_pousser(joueurs, nbjoueurs, (i+debut)%nbjoueurs, complexe), praticable(complexe, joueurs, nbjoueurs, joueurs[(i+debut)%nbjoueurs].posx, joueurs[(i+debut)%nbjoueurs].posy, D_BAS), praticable(complexe, joueurs, nbjoueurs, joueurs[(i+debut)%nbjoueurs].posx, joueurs[(i+debut)%nbjoueurs].posy, D_HAUT), praticable(complexe, joueurs, nbjoueurs, joueurs[(i+debut)%nbjoueurs].posx, joueurs[(i+debut)%nbjoueurs].posy, D_GAUCHE), praticable(complexe, joueurs, nbjoueurs, joueurs[(i+debut)%nbjoueurs].posx, joueurs[(i+debut)%nbjoueurs].posy, D_DROITE), jmemecase, nbmemecase);
                    if(peut_pousser(joueurs, nbjoueurs, (i+debut)%nbjoueurs, complexe)) {
                        recevoir_details_action((i+debut)%nbjoueurs, nbjoueurs, &(joueurs[(i+debut)%nbjoueurs].persopousse), &tmpdir, NULL, NULL);
                        joueurs[(i+debut)%nbjoueurs].dir[n] = (Direction)tmpdir;
                        switch(joueurs[(i+debut)%nbjoueurs].dir[n]) {
                        case D_HAUT:
                            joueurs[joueurs[(i+debut)%nbjoueurs].persopousse].posy = maxi(joueurs[joueurs[(i+debut)%nbjoueurs].persopousse].posy - 1, 0);
                            break;
                        case D_BAS:
                            joueurs[joueurs[(i+debut)%nbjoueurs].persopousse].posy = maxi(joueurs[joueurs[(i+debut)%nbjoueurs].persopousse].posy + 1, 0);
                            break;
                        case D_GAUCHE:
                            joueurs[joueurs[(i+debut)%nbjoueurs].persopousse].posx = maxi(joueurs[joueurs[(i+debut)%nbjoueurs].persopousse].posx - 1, 0);
                            break;
                        case D_DROITE:
                            joueurs[joueurs[(i+debut)%nbjoueurs].persopousse].posx = maxi(joueurs[joueurs[(i+debut)%nbjoueurs].persopousse].posx + 1, 0);
                            break;
                        }
                        vict = effet_salle(complexe, joueurs, nbjoueurs, joueurs[(i+debut)%nbjoueurs].persopousse, &complexe[joueurs[joueurs[(i+debut)%nbjoueurs].persopousse].posx][joueurs[joueurs[(i+debut)%nbjoueurs].persopousse].posy], compteur, mode);
                    }
                    break;
                case A_REGARDER:
                    demander_details_action((i+debut)%nbjoueurs, A_REGARDER, peut_regarder(joueurs[(i+debut)%nbjoueurs], complexe), regardable(complexe, joueurs[(i+debut)%nbjoueurs].posx, joueurs[(i+debut)%nbjoueurs].posy, D_BAS), regardable(complexe, joueurs[(i+debut)%nbjoueurs].posx, joueurs[(i+debut)%nbjoueurs].posy, D_HAUT), regardable(complexe, joueurs[(i+debut)%nbjoueurs].posx, joueurs[(i+debut)%nbjoueurs].posy, D_GAUCHE), regardable(complexe, joueurs[(i+debut)%nbjoueurs].posx, joueurs[(i+debut)%nbjoueurs].posy, D_DROITE), NULL, 0);
                    if(peut_regarder(joueurs[(i+debut)%nbjoueurs], complexe))
                        recevoir_details_action((i+debut)%nbjoueurs, nbjoueurs, NULL, NULL, &coor1, &coor2);
                    break;
                case A_CONTROLER:
                    demander_details_action((i+debut)%nbjoueurs, A_CONTROLER, peut_controler(joueurs[(i+debut)%nbjoueurs]), controlable(joueurs[(i+debut)%nbjoueurs].posx, colonnes, D_BAS), controlable(joueurs[(i+debut)%nbjoueurs].posx, colonnes, D_HAUT), controlable(joueurs[(i+debut)%nbjoueurs].posy, lignes, D_GAUCHE), controlable(joueurs[(i+debut)%nbjoueurs].posy, lignes, D_DROITE), NULL, 0);
                    if(peut_controler(joueurs[(i+debut)%nbjoueurs])) {
                        recevoir_details_action((i+debut)%nbjoueurs, nbjoueurs, NULL, &tmpdir, &coor1, NULL);
                        joueurs[(i+debut)%nbjoueurs].dir[n] = (Direction)tmpdir;
                        if(joueurs[(i+debut)%nbjoueurs].dir[n] == D_HAUT || joueurs[(i+debut)%nbjoueurs].dir[n] == D_BAS) {
                            vict = controler(complexe, joueurs, nbjoueurs, joueurs[(i+debut)%nbjoueurs].posx, joueurs[(i+debut)%nbjoueurs].dir[n], *compteur, mode);
                            colonnes[joueurs[(i+debut)%nbjoueurs].posx] = joueurs[(i+debut)%nbjoueurs].dir[n];
                        }
                        else {
                            vict = controler(complexe, joueurs, nbjoueurs, joueurs[(i+debut)%nbjoueurs].posy, joueurs[(i+debut)%nbjoueurs].dir[n], *compteur, mode);
                            lignes[joueurs[(i+debut)%nbjoueurs].posy] = joueurs[(i+debut)%nbjoueurs].dir[n];
                        }
                    }
                    break;
                }
                if(joueurs[(i+debut)%nbjoueurs].piege == 1)
                    joueurs[(i+debut)%nbjoueurs].piege = 2;
                else if(joueurs[(i+debut)%nbjoueurs].piege == 2) {
                    vict = tuer(joueurs, nbjoueurs, (i+debut)%nbjoueurs, mode);
                }
                if(joueurs[(i+debut)%nbjoueurs].apne == 1 && n == 1)
                    joueurs[(i+debut)%nbjoueurs].apne = 2;
                else if(joueurs[(i+debut)%nbjoueurs].apne == 2 && n == 1) {
                    vict = tuer(joueurs, nbjoueurs, (i+debut)%nbjoueurs, mode);
                }
                if(vict)
                    return 1;
            }
        }
    }
    for(i = 0; i < nbjoueurs; i++) {
        joueurs[i].action[0] = -1;
        joueurs[i].action[1] = -1;
        joueurs[i].dir[0] = -1;
        joueurs[i].dir[1] = -1;
    }
    return 0;
}

int maxi(int a, int b) {
    return(a >= b)?a:b;
}

int controler(Salle complexe[][5], Joueur joueurs[6], int nbjoueurs, int id, Direction dir, int compteur, Mode m) {
    int j, vict = 0;
    Salle tmp;
    switch(dir) {
    case D_HAUT:
        tmp = complexe[id][0];
        if(id != 2 && complexe[id][0].visible && complexe[id][0].type == S_25) {
            vict = sortir_complexe(joueurs, nbjoueurs, id, 0, compteur, m);
        }
        for(j = 0; j < 4; j++) {
            complexe[id][j] = complexe[id][j+1];
        }
        complexe[id][4] = tmp;
        for(j = 0; j < nbjoueurs; j++) {
            if(joueurs[j].posx == id) {
                joueurs[j].posy = joueurs[j].posy - 1;
                if(joueurs[j].posy == -1)
                    joueurs[j].posy = 4;
            }
        }
        break;
    case D_BAS:
        tmp = complexe[id][4];
        if(id != 2 && complexe[id][4].visible && complexe[id][4].type == S_25) {
            vict = sortir_complexe(joueurs, nbjoueurs, id, 4, compteur, m);
        }
        for(j = 4; j > 0; j--) {
            complexe[id][j] = complexe[id][j-1];
        }
        complexe[id][0] = tmp;
        for(j = 0; j < nbjoueurs; j++) {
            if(joueurs[j].posx == id)
                joueurs[j].posy = (joueurs[j].posy + 1)%5;
        }
        break;
    case D_GAUCHE:
        tmp = complexe[0][id];
        if(id != 2 && complexe[0][id].visible && complexe[0][id].type == S_25) {
            vict = sortir_complexe(joueurs, nbjoueurs, 0, id, compteur, m);
        }
        for(j = 0; j < 4; j++) {
            complexe[j][id] = complexe[j+1][id];
        }
        complexe[4][id] = tmp;
        for(j = 0; j < nbjoueurs; j++) {
            if(joueurs[j].posy == id) {
                joueurs[j].posx = joueurs[j].posx - 1;
                if(joueurs[j].posx == -1)
                    joueurs[j].posx = 4;
            }
        }
        break;
    case D_DROITE:
        tmp = complexe[4][id];
        if(id != 2 && complexe[4][id].visible && complexe[4][id].type == S_25) {
            vict = sortir_complexe(joueurs, nbjoueurs, 4, id, compteur, m);
        }
        for(j = 4; j > 0; j--) {
            complexe[j][id] = complexe[j-1][id];
        }
        complexe[0][id] = tmp;
        for(j = 0; j < nbjoueurs; j++) {
            if(joueurs[j].posy == id)
                joueurs[j].posx = (joueurs[j].posx + 1)%5;
        }
        break;
    }
    return vict;
}

int sortir_complexe(Joueur joueurs[6], int nbjoueurs, int x, int y, int compteur, Mode m) {
    int i, nbpri = 0, nbpri25 = 0;
    int jpri25[6];
    nbpri25 = joueurs_a_la_case(jpri25, joueurs, nbjoueurs, x, y, -1, J_PRISONNIER);
    if(m == M_SUSPISCION) {
        for(i = 0; i < nbjoueurs; i++) {
            if(joueurs[i].type == J_PRISONNIER)
                nbpri++;
        }
        if((compteur == 0 && nbpri25 >= nbpri - 1) || (nbpri25 == nbpri)) {
            victoire(jpri25, nbpri25, nbjoueurs, V_ROOM25);
            return 1;
        }
        else {
            return 0;
        }
    }
    /*else if(m == M_EQUIPE) {
        for(i = 0; i < nb25; i++) {
            if(tableau_contient(joueurs[i].equipier, j25, nb25));
        }
    }*/
    return 0;
}

void victoire(int jvict[6], int nbvict, int nbjoueurs, int type) {
    envoyer_victoire(jvict, nbvict, nbjoueurs, type);
}

int effet_salle(Salle complexe[][5], Joueur joueurs[6], int nbjoueurs, int idjoueur, Salle *salle_actuelle, int *compteur, Mode mode) {
    int x, y, id, nbsalles, tmp, x1, y1, x2, y2, i, nb25, booleen = 0, vict = 0;
    int coorsalles[24][2];
    int joueurs25[6];
    Joueur *joueur_actuel = &joueurs[idjoueur];
    Direction dir;
    joueur_actuel->apne = 0;
    joueur_actuel->froid = 0;
    joueur_actuel->noire = 0;
    joueur_actuel->piege = 0;
    joueur_actuel->prison = 0;
    if(!salle_actuelle->visible) {
        envoyer_revelation_salle(nbjoueurs, joueur_actuel->posx, joueur_actuel->posy);
    }
    envoyer_salle((int)(salle_actuelle->type), nbjoueurs, idjoueur);
    switch(salle_actuelle->type) {
    case S_DEPART:
        break;
    case S_25:
        if(!salle_actuelle->visible && (mode == M_COMPETITION || mode == M_EQUIPE || mode == M_SUSPISCION) && *compteur > 5) {
            envoyer_alerte(nbjoueurs);
            *compteur = 5;
            if(mode == M_SUSPISCION) {
                nb25 = joueurs_a_la_case(joueurs25, joueurs, nbjoueurs, joueur_actuel->posx, joueur_actuel->posy, -1, -1);
                if(nb25 >= nbjoueurs / 2) {
                    for(i = 0; i < nbjoueurs; i++) {
                        if(!tableau_contient(i, joueurs25, nb25) && !joueurs[i].revele) {
                            if(!booleen) {
                                booleen = 1;
                                envoyer_attention_les_masques_sont_tombes(nbjoueurs);
                            }
                            envoyer_revelation_role(i, joueurs[i].type, nbjoueurs);
                        }
                    }
                }
            }
        }
        break;
    case S_VORTEX:
        joueur_actuel->posx = 2;
        joueur_actuel->posy = 2;
        if(!salle_actuelle->visible)
            salle_actuelle->visible = 1;
        vict = effet_salle(complexe,joueurs, nbjoueurs, idjoueur, &complexe[2][2], compteur, mode);
        break;
    case S_VISION:
        if(non_decouvert(complexe)) {
            nbsalles = salles_cachees(complexe, coorsalles);
            envoyer_salles_selectionnables(idjoueur, coorsalles, nbsalles, S_VISION, joueur_actuel->posx, joueur_actuel->posy);
            recevoir_vision(idjoueur, nbjoueurs);
        }
        break;
    case S_PIEGE:
        joueur_actuel->piege = 1;
        break;
    case S_MOBILE:
        if(non_decouvert(complexe)) {
            nbsalles = salles_cachees(complexe, coorsalles);
            envoyer_salles_selectionnables(idjoueur, coorsalles, nbsalles, S_MOBILE, joueur_actuel->posx, joueur_actuel->posy);
            recevoir_mobile(idjoueur, nbjoueurs, &x, &y);
            complexe[joueur_actuel->posx][joueur_actuel->posy].type = complexe[x][y].type;
            complexe[joueur_actuel->posx][joueur_actuel->posy].visible = 0;
            complexe[x][y].type = S_MOBILE;
            complexe[x][y].visible = 1;
            nb25 = joueurs_a_la_case(joueurs25, joueurs, nbjoueurs, joueur_actuel->posx, joueur_actuel->posy, -1, -1);
            for(i = 0; i < nb25; i++) {
                joueurs[joueurs25[i]].posx = x;
                joueurs[joueurs25[i]].posy = y;
            }
            return vict;
        }
        break;
    case S_CONTROLE:
        recevoir_controle(idjoueur, nbjoueurs, &id, &tmp);
        dir = (Direction)tmp;
        vict = controler(complexe, joueurs, nbjoueurs, id, dir, *compteur, mode);
        break;
    case S_JUMELLE:
        x = joueur_actuel->posx;
        y = joueur_actuel->posy;
        x1 = x;
        y1 = y;
        if(chercher_jumelle(complexe, &x, &y)) {
            joueur_actuel->posx = x;
            joueur_actuel->posy = y;
            x2 = x;
            y2 = y;
        }
        else {
            x2 = -1;
            y2 = -1;
        }
        envoyer_jumelles(nbjoueurs, idjoueur, x1, y1, x2, y2);
        break;
    case S_VIDE:
        break;
    case S_MORTELLE:
        vict = tuer(joueurs, nbjoueurs, idjoueur, mode);
        break;
    case S_PRISON:
        joueur_actuel->prison = 1;
        break;
    case S_ILLUSION:
        if(non_decouvert(complexe)) {
            nbsalles = salles_cachees(complexe, coorsalles);
            envoyer_salles_selectionnables(idjoueur, coorsalles, nbsalles, S_ILLUSION, joueur_actuel->posx, joueur_actuel->posy);
            recevoir_illusion(idjoueur, nbjoueurs, &x, &y);
            complexe[joueur_actuel->posx][joueur_actuel->posy].type = complexe[x][y].type;
            complexe[joueur_actuel->posx][joueur_actuel->posy].visible = 0;
            complexe[x][y].type = S_ILLUSION;
            complexe[x][y].visible = 1;
            vict = effet_salle(complexe, joueurs, nbjoueurs, idjoueur, &complexe[joueur_actuel->posx][joueur_actuel->posy], compteur, mode);
        }
        break;
    case S_INONDABLE:
        joueur_actuel->apne = 1;
        break;
    case S_ACIDE:
        id = contient_joueur(joueurs, nbjoueurs, joueur_actuel->posx, joueur_actuel->posy, idjoueur);
        if(id != -1) {
            vict = tuer(joueurs, nbjoueurs, id, mode);
        }
        break;
    case S_FROIDE:
        joueur_actuel->froid = 1;
        break;
    case S_NOIRE:
        joueur_actuel->noire = 1;
    }
    if(!salle_actuelle->visible)
        salle_actuelle->visible = 1;
    return vict;
}

int salles_cachees(Salle complexe[][5], int tab[24][2]) {
    int i, j, k = 0;
    for(i = 0; i < 5; i++) {
        for(j = 0; j < 5; j++) {
            if(!complexe[i][j].visible) {
                tab[k][0] = i;
                tab[k][1] = j;
                k++;
            }
        }
    }
    return k;
}

int non_decouvert(Salle complexe[][5]) {
    int i, j;
    for(i = 0; i < 5; i++) {
        for(j = 0; j < 5; j++) {
            if(complexe[i][j].visible == 0)
                return 1;
        }
    }
    return 0;
}

int chercher_jumelle(Salle complexe[][5], int *x, int *y) {
    int i, j;
    for(i = 0; i < 5; i++) {
        for(j = 0; j < 5; j++) {
            if(complexe[i][j].type == S_JUMELLE && (i != *x || j != *y)) {
                *x = i;
                *y = j;
                return 1;
            }
        }
    }
    return 0;
}

int tuer(Joueur joueurs[6], int nbjoueurs, int id, Mode m) {
    int i, compte = 0;
    int tab[6];
    joueurs[id].mort = 1;
    joueurs[id].posx = -1;
    joueurs[id].posy = -1;
    envoyer_mort(id, nbjoueurs);
    compte = 0;
    if(m == M_SUSPISCION) {
        for(i = 0; i < nbjoueurs; i++) {
            if(joueurs[i].type == J_GARDIEN) {
                tab[compte] = i;
                compte++;
            }
        }
        for(i = 0; i < nbjoueurs; i++) {
            if(joueurs[i].mort && i != id) {
                if(!joueurs[i].revele) {
                    joueurs[i].revele = 1;
                    envoyer_revelation_role(i, joueurs[i].type, nbjoueurs);
                }
                if(joueurs[i].type == J_PRISONNIER) {
                    if(!joueurs[id].revele) {
                        joueurs[id].revele = 1;
                        envoyer_revelation_role(id, joueurs[i].type, nbjoueurs);
                    }
                    if(joueurs[id].type == J_PRISONNIER) {
                        victoire(tab, compte, nbjoueurs, V_PRIMORT);
                        return 1;
                    }
                }
            }
        }
    }
    return 0;
}

int contient_joueur(Joueur joueurs[6], int nbjoueurs, int x, int y, int id) {
    int i;
    for(i = 0; i < nbjoueurs; i++) {
        if(joueurs[i].posx == x && joueurs[i].posy == y && i != id)
            return i;
    }
    return -1;
}

int joueurs_a_la_case(int jmemecase[6], Joueur joueurs[6], int nbjoueurs, int x, int y, int id, int type) {
    int nbjoueurcase = 0;
    int i;
    int j = 0;
    for(i = 0; i < nbjoueurs; i++) {
        if(joueurs[i].posx == x && joueurs[i].posy == y && i != id && (type == -1 || (int)(joueurs[i].type) == type)) {
            nbjoueurcase++;
            jmemecase[j] = i;
            j++;
        }
    }
    return nbjoueurcase;
}

int praticable(Salle complexe[][5], Joueur joueurs[6], int nbjoueurs, int x, int y, Direction dir) {
    int prison = (complexe[x][y].type == S_PRISON);
    switch(dir) {
    case D_BAS:
        y++;
        break;
    case D_DROITE :
        x++;
        break;
    case D_GAUCHE :
        x--;
        break;
    case D_HAUT :
        y--;
        break;
    }
    if(x < 0 || x > 4 || y < 0 || y > 4)
        return 0;
    if(complexe[x][y].type == S_INONDABLE && complexe[x][y].visible)
        return 0;
    if(prison && (contient_joueur(joueurs, nbjoueurs, x, y, -1) < 0))
        return 0;
    return 1;
}

int regardable(Salle complexe[][5], int x, int y, Direction dir) {
    switch(dir) {
    case D_BAS:
        y++;
        break;
    case D_DROITE :
        x++;
        break;
    case D_GAUCHE :
        x--;
        break;
    case D_HAUT :
        y--;
        break;
    }
    if(x < 0 || x > 4 || y < 0 || y > 4)
        return 0;
    return !complexe[x][y].visible;
}

int controlable(int id, int prec[5], Direction dir) {
    switch(dir) {
    case D_BAS:
        return (id != 2 && prec[id] != D_HAUT);
        break;
    case D_HAUT:
        return (id != 2 && prec[id] != D_BAS);
        break;
    case D_GAUCHE:
        return (id != 2 && prec[id] != D_DROITE);
        break;
    case D_DROITE:
        return (id != 2 && prec[id] != D_GAUCHE);
        break;
    default:
        return 0;
    }
}

int regardable_autour(Salle complexe[][5], int x, int y) {
    return (regardable(complexe, x, y, D_BAS) || regardable(complexe, x, y, D_HAUT) || regardable(complexe, x, y, D_GAUCHE) || regardable(complexe, x, y, D_DROITE));
}

int praticable_autour(Salle complexe[][5], Joueur joueurs[6], int nbjoueurs, int x, int y) {
    return (praticable(complexe, joueurs, nbjoueurs, x, y, D_BAS) || praticable(complexe, joueurs, nbjoueurs, x, y, D_DROITE) || praticable(complexe, joueurs, nbjoueurs, x, y, D_GAUCHE) || praticable(complexe, joueurs, nbjoueurs, x, y, D_HAUT));
}

int peut_pousser(Joueur joueurs[6], int nbjoueurs, int id, Salle complexe[][5]) {
    return ((joueurs[id].posx != 2 || joueurs[id].posy != 2) && praticable_autour(complexe, joueurs, nbjoueurs, joueurs[id].posx, joueurs[id].posy) && contient_joueur(joueurs, nbjoueurs, joueurs[id].posx, joueurs[id].posy, id) >= 0);
}

int peut_regarder(Joueur j, Salle complexe[][5]) {
    return (!j.noire && regardable_autour(complexe, j.posx, j.posy));
}

int peut_controler(Joueur j) {
    return (j.posx != 2 || j.posy != 2);
}

int peut_deplacer(Joueur joueurs[6], int nbjoueurs, int id, Salle complexe[][5]) {
    return praticable_autour(complexe, joueurs, nbjoueurs, joueurs[id].posx, joueurs[id].posy);
}

void jouer(Salle complexe[][5], Joueur joueurs[6], int nbjoueurs, Mode mode, int expert) {
    int i, fini, debut, compteur, nbgard = 0;
    int *revele[6];
    int actions[6][2];
    int gard[6];
    int nbjoueursnongard = nbjoueurs;
    fini = 0;
    debut = 0;
    if(mode == M_SOLO || mode == M_COOPERATION)
        compteur = 8;
    else
        compteur = 10;
    for(i = 0; i < 6; i++) {
        revele[i] = &(joueurs[i].revele);
    }
    CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)tester_revelation, (PVOID*)revele, 0, NULL);
    for(i = 0; i < nbjoueurs; i++) {
        envoyer_regard(nbjoueurs);
    }
    envoyer_victoire(gard, 0, nbjoueurs, -1);
    while(!fini) {
        nbjoueursnongard = nbjoueurs;
        for(i = 0; i < nbjoueurs; i++) {
            if(!joueurs[i].mort && (joueurs[i].type != J_GARDIEN || !joueurs[i].revele)) {
                demander_action(i, joueurs[i].froid);
            }
            else {
                nbjoueursnongard--;
            }
        }
        recevoir_actions(actions, nbjoueursnongard);
        for(i = 0; i < nbjoueurs; i++) {
            if(joueurs[i].type != J_GARDIEN || !joueurs[i].revele) {
                joueurs[i].action[0] = actions[i][0];
                joueurs[i].action[1] = actions[i][1];
            }
        }
        fini = action(joueurs, nbjoueurs, debut, complexe, &compteur, mode);
        compteur--;
        if(compteur == 0) {
            for(i = 0; i < nbjoueurs; i++) {
                if(joueurs[i].type == J_GARDIEN) {
                    gard[nbgard] = i;
                    nbgard++;
                }
            }
            victoire(gard, nbgard, nbjoueurs, V_REBOURS);
            fini = 1;
        }
        do {
            debut = (debut + 1) % nbjoueurs;
        } while(joueurs[debut].mort);
        envoyer_nouveau_tour(compteur, nbjoueurs);
    }
}
