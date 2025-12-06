#pragma once

#include <memory>
#include <vector>

#include <game-activity/native_app_glue/android_native_app_glue.h>

#include "Renderer.h"
#include "GameBoard.h"
#include "InputManager.h"
#include "ScoreManager.h"

class GameLoop {
public:
    explicit GameLoop(android_app* app);
    GameLoop(android_app* app, Renderer* renderer);
    ~GameLoop();

    // called every frame from android_main
    void handleInput();
    void render();

    int score() const { return scoreManager_.score(); }

private:
    android_app* app_;
    Renderer* renderer_; // uses existing Renderer class
    bool ownsRenderer_ = false;

    GameBoard board_;
    InputManager input_;
    ScoreManager scoreManager_;

    // fixed timestep accumulator
    double lastTimeSec_;
    double accumulatorSec_;
    static constexpr double kTimeStep = 1.0 / 60.0; // 60Hz game logic

    void update(double dt);

    // DAS/ARR state
    double dasAccumulator_ = 0.0;
    double arrAccumulator_ = 0.0;
    bool leftWasHeld_ = false;
    bool rightWasHeld_ = false;

    // helper to move active piece left/right
    void move(int dx);
};
