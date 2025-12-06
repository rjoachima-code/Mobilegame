#pragma once

#include <cmath>

inline float clampf(float v, float a, float b) { return v < a ? a : (v > b ? b : v); }
inline int clampi(int v, int a, int b) { return v < a ? a : (v > b ? b : v); }

// small helper to convert hex color to normalized float RGBA
inline void hexToRGBA(unsigned int hex, float out[4]) {
    out[0] = static_cast<float>((hex >> 16) & 0xFF) / 255.0f;
    out[1] = static_cast<float>((hex >> 8) & 0xFF) / 255.0f;
    out[2] = static_cast<float>(hex & 0xFF) / 255.0f;
    out[3] = 1.0f;
}
