#include "Assets.h"
#include <android/asset_manager_jni.h>
#include <vector>

static AAssetManager* g_mgr = nullptr;

void Assets::init(AAssetManager* mgr) {
    g_mgr = mgr;
}

AAssetManager* Assets::manager() {
    return g_mgr;
}

std::string Assets::readText(const char* path) {
    if (!g_mgr) return {};
    AAsset* asset = AAssetManager_open(g_mgr, path, AASSET_MODE_BUFFER);
    if (!asset) return {};
    off_t size = AAsset_getLength(asset);
    std::string out;
    out.resize(size);
    int read = AAsset_read(asset, &out[0], size);
    AAsset_close(asset);
    if (read <= 0) return {};
    return out;
}

