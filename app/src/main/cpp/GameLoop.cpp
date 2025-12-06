#include "GameLoop.h"
#include <chrono>
#include "AndroidOut.h"
#include "AudioManager.h"
#include "EffectManager.h"

GameLoop::GameLoop(android_app* app)
: app_(app), renderer_(nullptr), board_(), input_(), lastTimeSec_(0.0), accumulatorSec_(0.0)
{
    aout << "GameLoop: initializing" << std::endl;
    // reuse Renderer created in main.cpp via userData if available
    if (app_ && app_->userData) {
        // userData may be a Renderer or GameLoop depending on main.cpp wiring; check type via pointer pattern
        // Assume legacy Renderer stored
        renderer_ = reinterpret_cast<Renderer*>(app_->userData);
        ownsRenderer_ = false;
    }
    // init timer
    lastTimeSec_ = 0.0;
}

GameLoop::GameLoop(android_app* app, Renderer* renderer)
: app_(app), renderer_(renderer), board_(), input_(), lastTimeSec_(0.0), accumulatorSec_(0.0)
{
    aout << "GameLoop: initializing with renderer" << std::endl;
    ownsRenderer_ = true;
}

GameLoop::~GameLoop() {
    aout << "GameLoop: shutting down" << std::endl;
    if (ownsRenderer_ && renderer_) {
        delete renderer_;
        renderer_ = nullptr;
    }
}

void GameLoop::handleInput() {
    // forward to InputManager
    input_.poll(app_);
}

void GameLoop::render() {
    // fixed timestep logic
    using clock = std::chrono::steady_clock;
    static auto start = clock::now();
    auto now = clock::now();
    double timeSec = std::chrono::duration<double>(now - start).count();

    if (lastTimeSec_ <= 0.0) lastTimeSec_ = timeSec;
    double frameTime = timeSec - lastTimeSec_;
    if (frameTime > 0.25) frameTime = 0.25; // avoid spiral of death
    lastTimeSec_ = timeSec;

    accumulatorSec_ += frameTime;
    while (accumulatorSec_ >= kTimeStep) {
        update(kTimeStep);
        accumulatorSec_ -= kTimeStep;
    }

    // update effects with frame delta
    EffectManager::update(static_cast<float>(frameTime));

    // render board visualization (simple debug)
    if (renderer_) renderer_->renderBoard(board_);

    // render via renderer
    if (renderer_) renderer_->render();

    // effects rendered by Renderer

    // render HUD
    if (renderer_) renderer_->renderHUD(score());
}

void GameLoop::update(double dt) {
    // TODO: advance gravity, lock timers, piece movement
    board_.update(dt, input_);

    // handle DAS/ARR
    const double das = 0.15; // seconds
    const double arr = 0.08; // seconds
    const auto &st = input_.state();
    if (st.moveLeft) { move( -1 ); }
    if (st.moveRight) { move(1); }

    // held left
    if (st.moveLeftHeld) {
        if (!leftWasHeld_) {
            dasAccumulator_ = 0.0;
            arrAccumulator_ = 0.0;
            leftWasHeld_ = true;
        } else {
            dasAccumulator_ += dt;
            if (dasAccumulator_ >= das) {
                arrAccumulator_ += dt;
                if (arrAccumulator_ >= arr) { move(-1); arrAccumulator_ = 0.0; }
            }
        }
    } else leftWasHeld_ = false;

    // held right
    if (st.moveRightHeld) {
        if (!rightWasHeld_) {
            dasAccumulator_ = 0.0;
            arrAccumulator_ = 0.0;
            rightWasHeld_ = true;
        } else {
            dasAccumulator_ += dt;
            if (dasAccumulator_ >= das) {
                arrAccumulator_ += dt;
                if (arrAccumulator_ >= arr) { move(1); arrAccumulator_ = 0.0; }
            }
        }
    } else rightWasHeld_ = false;

    // if board reported merges or clears, update score manager
    int merges = board_.lastMergeCount();
    int clears = board_.lastClearedLines();
    if (merges > 0 || clears > 0) {
        // approximate chainCount by 1 (GameBoard doesn't expose chain count currently)
        scoreManager_.addMerges(merges, merges > 1 ? 1 : 0);
        if (clears > 0) scoreManager_.addLineClear(clears);

        // trigger audio and effects for merges and clears
        if (merges > 0) {
            AudioManager::playSound(1);
            // spawn effects at each merged cell center (approx center)
            EffectManager::spawnMergeEffect(BOARD_W/2, BOARD_H/2);
        }
        if (clears > 0) {
            AudioManager::playSound(2); // line clear sound id
            EffectManager::spawnMergeEffect(BOARD_W/2, BOARD_H/2);
        }
    }

    // clear transient input state each frame
    input_.clear();
}

// helper to move active piece
void GameLoop::move(int dx) {
    // delegate to board and play step sound if moving succeeded
    if (board_.movePiece(dx)) {
        // small feedback sound optional
        AudioManager::playSound(0);
    }
}
