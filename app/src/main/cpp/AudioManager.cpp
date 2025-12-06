#include "AudioManager.h"
#include "AndroidOut.h"
#include <jni.h>

static JavaVM* g_jvm = nullptr;
static jobject g_audioBridge = nullptr; // global ref to NativeAudioBridge instance

bool AudioManager::init(JNIEnv* env, jobject activity) {
    aout << "AudioManager: init (JNI bridge)" << std::endl;
    if (!env->GetJavaVM(&g_jvm)) {
        // instantiate Java NativeAudioBridge
        jclass activityClass = env->GetObjectClass(activity);
        // find context (activity) and create NativeAudioBridge
        jclass bridgeClass = env->FindClass("com/example/myapplication/NativeAudioBridge");
        if (!bridgeClass) { aout << "AudioManager: NativeAudioBridge class not found" << std::endl; return false; }
        jmethodID ctor = env->GetMethodID(bridgeClass, "<init>", "(Landroid/content/Context;)V");
        if (!ctor) { aout << "AudioManager: ctor not found" << std::endl; return false; }
        jobject bridge = env->NewObject(bridgeClass, ctor, activity);
        g_audioBridge = env->NewGlobalRef(bridge);
        return true;
    }
    return false;
}

void AudioManager::playSound(int id) {
    if (!g_jvm || !g_audioBridge) {
        aout << "AudioManager: no bridge, fallback log play " << id << std::endl;
        return;
    }
    JNIEnv* env = nullptr;
    g_jvm->AttachCurrentThread(&env, nullptr);
    jclass bridgeClass = env->GetObjectClass(g_audioBridge);
    jmethodID playMethod = env->GetMethodID(bridgeClass, "play", "(I)V");
    if (playMethod) env->CallVoidMethod(g_audioBridge, playMethod, id);
}

void AudioManager::shutdown() {
    aout << "AudioManager: shutdown" << std::endl;
    if (g_jvm && g_audioBridge) {
        JNIEnv* env = nullptr;
        g_jvm->AttachCurrentThread(&env, nullptr);
        env->DeleteGlobalRef(g_audioBridge);
        g_audioBridge = nullptr;
    }
}
