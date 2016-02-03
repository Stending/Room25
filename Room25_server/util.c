#include "util.h"

int tableau_contient(int x, int *tab, int n) {
    int i;
    for(i = 0; i < n; i++) {
        if(tab[i] == x)
            return 1;
    }
    return 0;
}
