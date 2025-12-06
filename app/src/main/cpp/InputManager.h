#pragma once

#include <vector>
#include <cstdint>

struct android_app;

struct TouchEvent {
    int id;
    float x;
    float y;
    int action; // AMOTION_EVENT_ACTION_*
};

struct InputState {
    bool moveLeft = false;
    bool moveRight = false;
    bool rotate = false;
    bool softDrop = false;
    bool hardDrop = false;

    // Held state for directional holds (for DAS/ARR)
    bool moveLeftHeld = false;
    bool moveRightHeld = false;
};

class InputManager {
public:
    InputManager();

    // poll input from android_app; called from main thread
    void poll(android_app* app);

    // get and clear queued touches
    const std::vector<TouchEvent>& touches() const { return touches_; }
    void clear();

    // interpreted actions since last poll
    const InputState& state() const { return state_; }

private:
    std::vector<TouchEvent> touches_;
    InputState state_;
};
