#include "AudioManager.h"
#include "AndroidOut.h"
#include <jni.h>

static JavaVM* g_jvm = nullptr;
static jobject g_audioBridge = nullptr; // global ref to NativeAudioBridge instance

bool AudioManager::init(JNIEnv* env, jobject activity) {
    aout << "AudioManager: init (JNI bridge)" << std::endl;
    if (!env) return false;

    // store JVM pointer
    if (env->GetJavaVM(&g_jvm) != JNI_OK) {
        aout << "AudioManager: GetJavaVM failed" << std::endl;
        g_jvm = nullptr;
    }

    // instantiate Java NativeAudioBridge if available
    jclass bridgeClass = env->FindClass("com/example/myapplication/NativeAudioBridge");
    if (!bridgeClass) {
        aout << "AudioManager: NativeAudioBridge class not found" << std::endl;
        return true; // not fatal, we'll fallback to no-op audio
    }
    jmethodID ctor = env->GetMethodID(bridgeClass, "<init>", "(Landroid/content/Context;)V");
    if (!ctor) {
        aout << "AudioManager: NativeAudioBridge ctor not found" << std::endl;
        env->DeleteLocalRef(bridgeClass);
        return true;
    }

    jobject bridge = env->NewObject(bridgeClass, ctor, activity);
    if (!bridge) {
        aout << "AudioManager: NewObject for NativeAudioBridge failed" << std::endl;
        env->DeleteLocalRef(bridgeClass);
        return true;
    }

    // create a global ref
    g_audioBridge = env->NewGlobalRef(bridge);
    env->DeleteLocalRef(bridge);
    env->DeleteLocalRef(bridgeClass);

    aout << "AudioManager: NativeAudioBridge created" << std::endl;
    return true;
}

void AudioManager::playSound(int id) {
    if (!g_jvm || !g_audioBridge) {
        aout << "AudioManager: no bridge, fallback log play " << id << std::endl;
        return;
    }
    JNIEnv* env = nullptr;
    // Attach current thread if needed
    if (g_jvm->GetEnv(reinterpret_cast<void**>(&env), JNI_VERSION_1_6) != JNI_OK) {
        jint res = g_jvm->AttachCurrentThread(&env, nullptr);
        if (res != JNI_OK || !env) {
            aout << "AudioManager: AttachCurrentThread failed" << std::endl;
            return;
        }
    }
    jclass bridgeClass = env->GetObjectClass(g_audioBridge);
    if (!bridgeClass) return;
    jmethodID playMethod = env->GetMethodID(bridgeClass, "play", "(I)V");
    if (playMethod) env->CallVoidMethod(g_audioBridge, playMethod, id);
    env->DeleteLocalRef(bridgeClass);
}

void AudioManager::shutdown() {
    aout << "AudioManager: shutdown" << std::endl;
    if (g_jvm && g_audioBridge) {
        JNIEnv* env = nullptr;
        if (g_jvm->GetEnv(reinterpret_cast<void**>(&env), JNI_VERSION_1_6) != JNI_OK) {
            if (g_jvm->AttachCurrentThread(&env, nullptr) != JNI_OK) env = nullptr;
        }
        if (env && g_audioBridge) {
            env->DeleteGlobalRef(g_audioBridge);
            g_audioBridge = nullptr;
        }
    }
}
