#pragma once

#include <string>

struct SaveState {
    uint64_t highScore = 0;
    int settingsFlags = 0;
    uint32_t version = 1;
};

class SaveManager {
public:
    static bool save(const SaveState &state, const char *path);
    static bool load(SaveState &outState, const char *path);
};

