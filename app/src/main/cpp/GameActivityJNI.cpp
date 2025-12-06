#include <jni.h>
#include "AndroidOut.h"

extern "C" {

// GameActivity expects this native method to exist. Some versions of the Game SDK
// provide it in their native library; if it's missing at runtime the JVM will
// throw UnsatisfiedLinkError during activity creation or surface lifecycle
// callbacks. Provide safe stubs that do nothing. When ready, hook these into
// your renderer (ANativeWindow, EGL context) to start drawing.
JNIEXPORT void JNICALL
Java_com_google_androidgamesdk_GameActivity_setInputConnectionNative(JNIEnv* env, jclass clazz, jlong nativePtr, jobject inputConnection) {
    aout << "GameActivity:setInputConnectionNative called (stub)" << std::endl;
}

JNIEXPORT void JNICALL
Java_com_google_androidgamesdk_GameActivity_onStartNative(JNIEnv* env, jclass clazz, jlong nativePtr) {
    aout << "GameActivity:onStartNative called (stub)" << std::endl;
}

JNIEXPORT void JNICALL
Java_com_google_androidgamesdk_GameActivity_onResumeNative(JNIEnv* env, jclass clazz, jlong nativePtr) {
    aout << "GameActivity:onResumeNative called (stub)" << std::endl;
}

JNIEXPORT void JNICALL
Java_com_google_androidgamesdk_GameActivity_onPauseNative(JNIEnv* env, jclass clazz, jlong nativePtr) {
    aout << "GameActivity:onPauseNative called (stub)" << std::endl;
}

JNIEXPORT void JNICALL
Java_com_google_androidgamesdk_GameActivity_onStopNative(JNIEnv* env, jclass clazz, jlong nativePtr) {
    aout << "GameActivity:onStopNative called (stub)" << std::endl;
}

JNIEXPORT void JNICALL
Java_com_google_androidgamesdk_GameActivity_onDestroyNative(JNIEnv* env, jclass clazz, jlong nativePtr) {
    aout << "GameActivity:onDestroyNative called (stub)" << std::endl;
}

JNIEXPORT void JNICALL
Java_com_google_androidgamesdk_GameActivity_onWindowInsetsChangedNative(JNIEnv* env, jclass clazz, jlong nativePtr) {
    aout << "GameActivity:onWindowInsetsChangedNative called (stub)" << std::endl;
}

// Stub for onContentRectChangedNative(left, top, right, bottom)
JNIEXPORT void JNICALL
Java_com_google_androidgamesdk_GameActivity_onContentRectChangedNative(JNIEnv* env, jclass clazz, jlong nativePtr, jint left, jint top, jint right, jint bottom) {
    aout << "GameActivity:onContentRectChangedNative called (stub) - rect: "
         << left << "," << top << "," << right << "," << bottom << std::endl;
}

// Surface lifecycle stubs. If you implement rendering, replace these with code
// that creates/destroys an ANativeWindow and initializes your EGL/GLES context.
JNIEXPORT void JNICALL
Java_com_google_androidgamesdk_GameActivity_onSurfaceCreatedNative(JNIEnv* env, jclass clazz, jlong nativePtr, jobject surface) {
    aout << "GameActivity:onSurfaceCreatedNative called (stub)" << std::endl;
}

JNIEXPORT void JNICALL
Java_com_google_androidgamesdk_GameActivity_onSurfaceChangedNative(JNIEnv* env, jclass clazz, jlong nativePtr, jobject surface, jint width, jint height) {
    aout << "GameActivity:onSurfaceChangedNative called (stub) - size: " << width << "x" << height << std::endl;
}

JNIEXPORT void JNICALL
Java_com_google_androidgamesdk_GameActivity_onSurfaceDestroyedNative(JNIEnv* env, jclass clazz, jlong nativePtr) {
    aout << "GameActivity:onSurfaceDestroyedNative called (stub)" << std::endl;
}

JNIEXPORT void JNICALL
Java_com_google_androidgamesdk_GameActivity_onSurfaceRedrawNeededNative(JNIEnv* env, jclass clazz, jlong nativePtr, jobject surface) {
    aout << "GameActivity:onSurfaceRedrawNeededNative called (stub)" << std::endl;
}

JNIEXPORT void JNICALL
Java_com_google_androidgamesdk_GameActivity_onWindowFocusChangedNative(JNIEnv* env, jclass clazz, jlong nativePtr, jboolean hasFocus) {
    aout << "GameActivity:onWindowFocusChangedNative called (stub) - hasFocus=" << (hasFocus ? "true" : "false") << std::endl;
}

} // extern "C"
