#include "game.h"
#include "socks.h"

int main(int argc, char** argv) {
    Joueur joueurs[6];
    Mode mode;
    Salle complexe[5][5];
    int nbjoueurs, expert;
    if(argc == 3) {
        nbjoueurs = atoi(argv[1]);
        init_socks(nbjoueurs, atoi(argv[2]));
    }
    else if (argc == 2) {
        nbjoueurs = atoi(argv[1]);
        init_socks(nbjoueurs, 4148);
    }
    else {
        perror("2 ou 3 arguments attendus.");
        exit(1);
    }
    commencer_jeu();
    choix_mode(&mode, &expert, nbjoueurs);
    init_joueurs(joueurs, nbjoueurs);
    if(mode == M_SUSPISCION) {
        role_joueurs(joueurs, nbjoueurs);
    }
    creer_complexe(complexe, mode, expert, nbjoueurs);
    jouer(complexe, joueurs, nbjoueurs, mode, expert);
    finir_socks(nbjoueurs);
    getchar();
    return EXIT_SUCCESS;
}
