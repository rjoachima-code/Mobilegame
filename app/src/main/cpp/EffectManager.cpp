#include "EffectManager.h"
#include "AndroidOut.h"
#include <cstdlib>

static std::vector<Particle> g_particles;

void EffectManager::spawnMergeEffect(int x, int y) {
    // spawn 16 particles around x,y
    for (int i = 0; i < 16; ++i) {
        Particle p;
        float ang = (float) (rand() % 360) * 3.14159f / 180.0f;
        float speed = 1.0f + (rand() % 100) / 100.0f * 2.0f;
        p.vx = cosf(ang) * speed;
        p.vy = sinf(ang) * speed;
        p.x = x + 0.5f;
        p.y = -y + 0.5f; // board uses negative y in renderer
        p.life = 0.8f + (rand() % 100) / 100.0f * 0.8f;
        p.size = 0.15f + (rand() % 100) / 100.0f * 0.2f;
        p.r = 1.0f; p.g = 0.9f; p.b = 0.4f; p.a = 1.0f;
        g_particles.push_back(p);
    }
    aout << "Effect: spawn " << 16 << " particles at " << x << "," << y << std::endl;
}

void EffectManager::update(float dt) {
    for (auto &p : g_particles) {
        p.x += p.vx * dt * 3.0f;
        p.y += p.vy * dt * 3.0f - 0.5f * dt; // gravity-ish
        p.life -= dt;
        p.a = p.life > 0 ? (p.life / 1.0f) : 0.0f;
    }
    // remove dead
    g_particles.erase(std::remove_if(g_particles.begin(), g_particles.end(), [](const Particle &p){ return p.life <= 0.0f; }), g_particles.end());
}

const std::vector<Particle>& EffectManager::getParticles() { return g_particles; }

void EffectManager::clear() { g_particles.clear(); }
