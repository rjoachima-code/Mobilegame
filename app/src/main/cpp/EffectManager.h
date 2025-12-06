#pragma once

#include <vector>

struct Particle {
    float x, y;
    float vx, vy;
    float life; // remaining life in seconds
    float size;
    float r, g, b, a;
};

class EffectManager {
public:
    static void spawnMergeEffect(int x, int y);
    static void update(float dt);
    static const std::vector<Particle>& getParticles();
    static void clear();
};
