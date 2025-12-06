#pragma once

#include <array>
#include <cstdint>
#include <vector>

#include "Tetromino.h"
#include "InputManager.h"
#include "Spawner.h"

// 10x20 board
static constexpr int BOARD_W = 10;
static constexpr int BOARD_H = 20;

class GameBoard {
public:
    GameBoard();

    // update game logic: gravity, lock, merges
    void update(double dt, const InputManager& input);

    // check collision of tetromino at position
    bool fits(const Tetromino& t, int x, int y, int rotation) const;

    // lock tetromino into board, returns true if merged or lines cleared
    bool place(const Tetromino& t, int x, int y, int rotation);

    // clear full lines and return indices cleared
    std::vector<int> clearFullLines();

    // basic access
    uint8_t cell(int x, int y) const;
    void setCell(int x, int y, uint8_t v);

    // debug: print board to log
    void debugPrint() const;

    // Active piece controls
    void spawnNextPiece();
    bool movePiece(int dx);
    bool rotatePiece(int dir); // +1 cw, -1 ccw
    void hardDrop();

    // Event reporting
    int lastMergeCount() const { return lastMergeCount_; }
    int lastClearedLines() const { return lastClearedLines_; }

private:
    std::array<uint8_t, BOARD_W * BOARD_H> cells_; // 0 = empty, otherwise number index

    // active piece
    Tetromino activeTetromino_;
    int activeX_;
    int activeY_;
    int activeRot_;
    bool hasActive_;

    double gravityTimerSec_;
    Spawner spawner_;

    int lastMergeCount_ = 0;
    int lastClearedLines_ = 0;
};
