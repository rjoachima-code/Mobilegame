#pragma once

#include <jni.h>

class AudioManager {
public:
    // Initialize audio bridge with JVM and activity object
    static bool init(JNIEnv* env, jobject activity);
    static void playSound(int id);
    static void shutdown();
};
