#include <jni.h>
#include <android/native_window_jni.h>
#include <game-activity/native_app_glue/android_native_app_glue.h>
#include "AndroidOut.h"
#include <thread>
#include <atomic>
#include <mutex>
#include <chrono>

// g_pApp is defined in main.cpp; declare it here so we can reference it.
extern struct android_app* g_pApp;

// Add includes so JNI surface handlers can create Renderer/GameLoop as a fallback
#include "Renderer.h"
#include "GameLoop.h"
#include "Assets.h"
#include "AudioManager.h"

extern "C" {

// Fallback renderer thread state (used only when native android_main loop isn't running)
static std::thread g_fallbackRenderThread;
static std::atomic<bool> g_fallbackRunning(false);
static std::mutex g_fallbackMutex;

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
    aout << "GameActivity:onSurfaceCreatedNative called" << std::endl;
    // convert Java Surface to ANativeWindow and attach to app
    ANativeWindow* window = ANativeWindow_fromSurface(env, surface);
    if (!window) {
        aout << "Failed to obtain ANativeWindow from Surface" << std::endl;
        return;
    }

    // If there's a native android_main loop with g_pApp, attach the window to it.
    if (g_pApp) {
        // If an existing window is present, release it first
        if (g_pApp->window && g_pApp->window != window) {
            ANativeWindow_release(g_pApp->window);
        }
        g_pApp->window = window;

        aout << "onSurfaceCreatedNative: g_pApp->onAppCmd=" << (void*)g_pApp->onAppCmd
             << " userData=" << (void*)g_pApp->userData << std::endl;

        // notify app that window was created. If the native game loop (android_main) is already
        // running it will receive the APP_CMD_INIT_WINDOW.
        if (g_pApp->onAppCmd) {
            aout << "onSurfaceCreatedNative: calling onAppCmd(APP_CMD_INIT_WINDOW)" << std::endl;
            g_pApp->onAppCmd(g_pApp, APP_CMD_INIT_WINDOW);
        } else {
            aout << "onSurfaceCreatedNative: onAppCmd not set; writing cmd" << std::endl;
            android_app_write_cmd(g_pApp, APP_CMD_INIT_WINDOW);
        }
        return;
    }

    // No native android_main loop is present. Start a safe fallback render thread
    // that creates a Renderer from the ANativeWindow and runs a simple loop.
    {
        std::lock_guard<std::mutex> lock(g_fallbackMutex);
        if (g_fallbackRunning.load()) {
            aout << "onSurfaceCreatedNative: fallback render thread already running" << std::endl;
            // We created a ref for this window above; release since we won't take it.
            ANativeWindow_release(window);
            return;
        }

        g_fallbackRunning.store(true);
        // Create a new thread that will own the ANativeWindow and Renderer
        g_fallbackRenderThread = std::thread([window]() {
            aout << "fallbackRenderThread: started" << std::endl;
            // Renderer will take ownership of the window pointer (we pass it directly)
            Renderer* renderer = nullptr;
            try {
                renderer = new Renderer(window); // Renderer::initRenderer uses window_
            } catch (...) {
                aout << "fallbackRenderThread: failed to create Renderer" << std::endl;
                if (renderer) delete renderer;
                g_fallbackRunning.store(false);
                return;
            }

            // run a simple render loop at ~60 FPS until signaled to stop
            using clock = std::chrono::steady_clock;
            auto prev = clock::now();
            const std::chrono::milliseconds frameMs(16); // ~60 FPS
            while (g_fallbackRunning.load()) {
                // simple frame timing
                renderer->render();
                // sleep to cap frame rate
                std::this_thread::sleep_until(prev + frameMs);
                prev = clock::now();
            }

            aout << "fallbackRenderThread: stopping, deleting renderer" << std::endl;
            delete renderer; // destructor will release ANativeWindow if owned
            aout << "fallbackRenderThread: exited" << std::endl;
        });
        // detach thread; we'll join on destroy to clean up properly
        g_fallbackRenderThread.detach();
    }
}

JNIEXPORT void JNICALL
Java_com_google_androidgamesdk_GameActivity_onSurfaceChangedNative(JNIEnv* env, jclass clazz, jlong nativePtr, jobject surface, jint width, jint height) {
    aout << "GameActivity:onSurfaceChangedNative called - size: " << width << "x" << height << std::endl;
    if (g_pApp) {
        // notify main loop the window was resized
        if (g_pApp->onAppCmd) {
            g_pApp->onAppCmd(g_pApp, APP_CMD_WINDOW_RESIZED);
        } else {
            android_app_write_cmd(g_pApp, APP_CMD_WINDOW_RESIZED);
        }
    } else {
        // If fallback is running we don't have a mechanism to forward size changes currently.
        aout << "GameActivity:onSurfaceChangedNative: fallback mode - size change ignored" << std::endl;
    }
}

JNIEXPORT void JNICALL
Java_com_google_androidgamesdk_GameActivity_onSurfaceDestroyedNative(JNIEnv* env, jclass clazz, jlong nativePtr) {
    aout << "GameActivity:onSurfaceDestroyedNative called" << std::endl;
    if (g_pApp) {
        // notify main loop the window is going away
        if (g_pApp->onAppCmd) {
            g_pApp->onAppCmd(g_pApp, APP_CMD_TERM_WINDOW);
        } else {
            android_app_write_cmd(g_pApp, APP_CMD_TERM_WINDOW);
        }
        if (g_pApp->window) {
            ANativeWindow_release(g_pApp->window);
            g_pApp->window = nullptr;
        }
        return;
    }

    // If we are in fallback mode, signal the thread to stop and wait briefly
    {
        std::lock_guard<std::mutex> lock(g_fallbackMutex);
        if (g_fallbackRunning.load()) {
            aout << "onSurfaceDestroyedNative: stopping fallback render thread" << std::endl;
            g_fallbackRunning.store(false);
            // Note: we detached the thread earlier; give it a short time to clean up. The Renderer
            // destructor will release the ANativeWindow. If you want strict join semantics, store
            // thread and join here instead of detach.
            std::this_thread::sleep_for(std::chrono::milliseconds(100));
        }
    }
}

JNIEXPORT void JNICALL
Java_com_google_androidgamesdk_GameActivity_onSurfaceRedrawNeededNative(JNIEnv* env, jclass clazz, jlong nativePtr, jobject surface) {
    aout << "GameActivity:onSurfaceRedrawNeededNative called" << std::endl;
    if (!g_pApp) return;
    if (g_pApp->onAppCmd) {
        g_pApp->onAppCmd(g_pApp, APP_CMD_WINDOW_REDRAW_NEEDED);
    } else {
        android_app_write_cmd(g_pApp, APP_CMD_WINDOW_REDRAW_NEEDED);
    }
}

JNIEXPORT void JNICALL
Java_com_google_androidgamesdk_GameActivity_onWindowFocusChangedNative(JNIEnv* env, jclass clazz, jlong nativePtr, jboolean hasFocus) {
    aout << "GameActivity:onWindowFocusChangedNative called (stub) - hasFocus=" << (hasFocus ? "true" : "false") << std::endl;
    if (!g_pApp) return;
    if (g_pApp->onAppCmd) {
        g_pApp->onAppCmd(g_pApp, hasFocus ? APP_CMD_GAINED_FOCUS : APP_CMD_LOST_FOCUS);
    } else {
        android_app_write_cmd(g_pApp, hasFocus ? APP_CMD_GAINED_FOCUS : APP_CMD_LOST_FOCUS);
    }
}

} // extern "C"
