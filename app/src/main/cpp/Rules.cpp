#include "Rules.h"
#include <algorithm>
#include <cmath>

int Rules::scoreForClears(int count, int level) {
    // simple scoring: base points per lines
    int base = 100;
    return base * count * (level + 1);
}

double Rules::gravityForLevel(int level) {
    // simple: decrease timestep with higher level
    return std::max(0.02, 1.0 / (30.0 + level * 2.0));
}
