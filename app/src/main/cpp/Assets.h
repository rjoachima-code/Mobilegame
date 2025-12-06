#pragma once

#include <android/asset_manager.h>
#include <string>

class Assets {
public:
    static void init(AAssetManager* mgr);
    static AAssetManager* manager();
    static std::string readText(const char* path);
};

