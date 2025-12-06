#include <jni.h>

#include <game-activity/native_app_glue/android_native_app_glue.h>
#include <game-activity/GameActivity.h>

#include "AndroidOut.h"
#include "Renderer.h"
#include "GameLoop.h"
#include "Assets.h"
#include "AudioManager.h"

extern "C" {

static struct android_app* g_pApp = nullptr;

/*!
 * Handles commands sent to this Android application
 * @param pApp the app the commands are coming from
 * @param cmd the command to handle
 */
void handle_cmd(android_app *pApp, int32_t cmd) {
    switch (cmd) {
        case APP_CMD_INIT_WINDOW:
            // A new window is created, create Renderer and GameLoop and store GameLoop in userData
            if (pApp->window) {
                // initialize asset manager for native code
                Assets::init(pApp->activity->assetManager);

                // init audio
                // initialize audio bridge with JVM and Activity
                JNIEnv* env = nullptr;
                if (pApp->activity && pApp->activity->vm) {
                    pApp->activity->vm->AttachCurrentThread(&env, nullptr);
                    if (env) AudioManager::init(env, pApp->activity->javaGameActivity);
                    // Note: keep thread attached; AudioManager stores JavaVM
                }

                auto renderer = new Renderer(pApp);
                // Renderer constructor initializes GL internally
                auto loop = new GameLoop(pApp, renderer);
                pApp->userData = loop;
            }
            break;
        case APP_CMD_TERM_WINDOW:
            // The window is being destroyed. Clean up userData
            if (pApp->userData) {
                auto *pLoop = reinterpret_cast<GameLoop *>(pApp->userData);
                pApp->userData = nullptr;
                delete pLoop;
            }
            // shutdown audio
            AudioManager::shutdown();
            break;
        default:
            break;
    }
}

/*!
 * Enable the motion events you want to handle; not handled events are
 * passed back to OS for further processing. For this example case,
 * only pointer and joystick devices are enabled.
 *
 * @param motionEvent the newly arrived GameActivityMotionEvent.
 * @return true if the event is from a pointer or joystick device,
 *         false for all other input devices.
 */
bool motion_event_filter_func(const GameActivityMotionEvent *motionEvent) {
    auto sourceClass = motionEvent->source & AINPUT_SOURCE_CLASS_MASK;
    return (sourceClass == AINPUT_SOURCE_CLASS_POINTER ||
            sourceClass == AINPUT_SOURCE_CLASS_JOYSTICK);
}

/*!
 * This the main entry point for a native activity
 */
void android_main(struct android_app *pApp) {
    g_pApp = pApp;
    // Can be removed, useful to ensure your code is running
    aout << "Welcome to android_main" << std::endl;

    // Register an event handler for Android events
    pApp->onAppCmd = handle_cmd;

    // Set input event filters (set it to NULL if the app wants to process all inputs).
    android_app_set_motion_event_filter(pApp, motion_event_filter_func);

    // This sets up a typical game/event loop. It will run until the app is destroyed.
    do {
        // Process all pending events before running game logic.
        bool done = false;
        while (!done) {
            // 0 is non-blocking.
            int timeout = 0;
            int events;
            android_poll_source *pSource;
            int result = ALooper_pollOnce(timeout, nullptr, &events,
                                          reinterpret_cast<void**>(&pSource));
            switch (result) {
                case ALOOPER_POLL_TIMEOUT:
                    [[clang::fallthrough]];
                case ALOOPER_POLL_WAKE:
                    // No events occurred before the timeout or explicit wake. Stop checking for events.
                    done = true;
                    break;
                case ALOOPER_EVENT_ERROR:
                    aout << "ALooper_pollOnce returned an error" << std::endl;
                    break;
                case ALOOPER_POLL_CALLBACK:
                    break;
                default:
                    if (pSource) {
                        pSource->process(pApp, pSource);
                    }
            }
        }

        // Check if any user data is associated.
        if (pApp->userData) {
            // Our user data is a GameLoop now
            auto *pLoop = reinterpret_cast<GameLoop *>(pApp->userData);

            // Process game input
            pLoop->handleInput();

            // Render a frame
            pLoop->render();
        }
    } while (!pApp->destroyRequested);
}

/*!
 * Called by GameActivity to allow the native library to perform any initialization it needs.
 */
extern "C" JNIEXPORT void JNICALL
Java_com_google_androidgamesdk_GameActivity_initializeNativeCode(JNIEnv* env, jclass clazz, jobject activity) {
    // Minimal initialization: log and no-op. GameActivity will later create the native app via android_main.
    aout << "Java_com_google_androidgamesdk_GameActivity_initializeNativeCode: native library loaded" << std::endl;
}

extern "C" JNIEXPORT jint JNICALL
Java_com_example_myapplication_MainActivity_nativeGetScore(JNIEnv* env, jclass clazz) {
    if (!g_pApp) return 0;
    if (!g_pApp->userData) return 0;
    auto *pLoop = reinterpret_cast<GameLoop*>(g_pApp->userData);
    return static_cast<jint>(pLoop->score());
}

} // extern "C"
