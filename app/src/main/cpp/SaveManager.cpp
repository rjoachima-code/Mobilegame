#include "SaveManager.h"
#include <fstream>

bool SaveManager::save(const SaveState &state, const char *path) {
    std::ofstream ofs(path, std::ios::binary);
    if (!ofs) return false;
    ofs.write(reinterpret_cast<const char*>(&state), sizeof(state));
    return ofs.good();
}

bool SaveManager::load(SaveState &outState, const char *path) {
    std::ifstream ifs(path, std::ios::binary);
    if (!ifs) return false;
    ifs.read(reinterpret_cast<char*>(&outState), sizeof(outState));
    return ifs.good();
}

