#include "InputManager.h"
#include <game-activity/native_app_glue/android_native_app_glue.h>
#include "AndroidOut.h"
#include <cmath>
#include <android/native_window.h>
#include <unordered_map>
#include <chrono>

struct PointerInfo {
    float x, y;
    std::chrono::steady_clock::time_point downTime;
    bool isDown;
};

static std::unordered_map<int, PointerInfo> g_pointerMap;

InputManager::InputManager() = default;

void InputManager::poll(android_app* app) {
    touches_.clear();
    state_ = InputState();
    auto *inputBuffer = android_app_swap_input_buffers(app);
    if (!inputBuffer) return;

    // Determine window size if available
    float width = 1080.0f;
    float height = 1920.0f;
    if (app && app->window) {
        width = static_cast<float>(ANativeWindow_getWidth(app->window));
        height = static_cast<float>(ANativeWindow_getHeight(app->window));
    }

    using clock = std::chrono::steady_clock;
    for (int i = 0; i < inputBuffer->motionEventsCount; ++i) {
        auto &motionEvent = inputBuffer->motionEvents[i];
        int action = motionEvent.action & AMOTION_EVENT_ACTION_MASK;
        if (action == AMOTION_EVENT_ACTION_MOVE) {
            for (int p = 0; p < motionEvent.pointerCount; ++p) {
                auto pointer = motionEvent.pointers[p];
                float x = GameActivityPointerAxes_getX(&pointer);
                float y = GameActivityPointerAxes_getY(&pointer);
                // normalize to 0..1
                float nx = x / width;
                float ny = y / height;
                touches_.push_back({pointer.id, nx, ny, action});

                // update pointer map
                auto it = g_pointerMap.find(pointer.id);
                if (it != g_pointerMap.end()) {
                    it->second.x = nx; it->second.y = ny;
                }

                // interpret move for soft drop if pointer is in bottom area
                if (ny > 0.8f) state_.softDrop = true;
            }
        } else {
            auto pointerIndex = (motionEvent.action & AMOTION_EVENT_ACTION_POINTER_INDEX_MASK) >>
                                AMOTION_EVENT_ACTION_POINTER_INDEX_SHIFT;
            if (pointerIndex >= 0 && pointerIndex < motionEvent.pointerCount) {
                auto pointer = motionEvent.pointers[pointerIndex];
                float x = GameActivityPointerAxes_getX(&pointer);
                float y = GameActivityPointerAxes_getY(&pointer);
                float nx = x / width;
                float ny = y / height;
                touches_.push_back({pointer.id, nx, ny, action});

                if (action == AMOTION_EVENT_ACTION_DOWN || action == AMOTION_EVENT_ACTION_POINTER_DOWN) {
                    // add to pointer map
                    g_pointerMap[pointer.id] = {nx, ny, clock::now(), true};
                    if (nx < 0.4f) state_.moveLeft = true;
                    else if (nx > 0.6f) state_.moveRight = true;
                    else if (ny < 0.3f) state_.rotate = true;
                }

                if (action == AMOTION_EVENT_ACTION_UP || action == AMOTION_EVENT_ACTION_POINTER_UP) {
                    // mark up and compute if it was a hard drop
                    auto it = g_pointerMap.find(pointer.id);
                    if (it != g_pointerMap.end()) {
                        auto down = it->second.downTime;
                        auto dur = std::chrono::duration_cast<std::chrono::milliseconds>(clock::now() - down).count();
                        // if was held briefly in bottom zone -> hard drop
                        if (ny > 0.85f && dur < 500) state_.hardDrop = true;
                        it->second.isDown = false;
                        g_pointerMap.erase(it);
                    } else {
                        if (ny > 0.85f) state_.hardDrop = true;
                    }
                }
            }
        }
    }

    // compute held states for DAS/ARR
    const int dasMs = 150; // initial delay before auto-repeat
    const int arrMs = 80;  // repeat interval
    auto now = clock::now();
    for (auto &kv : g_pointerMap) {
        auto &pi = kv.second;
        if (!pi.isDown) continue;
        // which side is pointer on?
        if (pi.x < 0.4f) {
            auto heldMs = std::chrono::duration_cast<std::chrono::milliseconds>(now - pi.downTime).count();
            if (heldMs >= dasMs) state_.moveLeftHeld = true;
        } else if (pi.x > 0.6f) {
            auto heldMs = std::chrono::duration_cast<std::chrono::milliseconds>(now - pi.downTime).count();
            if (heldMs >= dasMs) state_.moveRightHeld = true;
        }
    }

    android_app_clear_motion_events(inputBuffer);
}

void InputManager::clear() { touches_.clear(); state_ = InputState(); }
