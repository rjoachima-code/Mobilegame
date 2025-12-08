#include "GameBoard.h"
#include "AndroidOut.h"
#include "MergeLogic.h"
#include "InputManager.h"

GameBoard::GameBoard() {
    cells_.fill(0);
    hasActive_ = false;
    gravityTimerSec_ = 0.0;
}

void GameBoard::spawnNextPiece() {
    activeTetromino_ = spawner_.next();
    // spawn top-center
    activeX_ = BOARD_W / 2 - 2;
    activeY_ = 0;
    activeRot_ = 0;
    hasActive_ = true;
    aout << "GameBoard: spawned piece type=" << static_cast<int>(activeTetromino_.type()) << " at " << activeX_ << "," << activeY_ << std::endl;
}

bool GameBoard::fits(const Tetromino& t, int x, int y, int rotation) const {
    auto shape = t.shape(rotation);
    for (int ty = 0; ty < 4; ++ty) {
        for (int tx = 0; tx < 4; ++tx) {
            int idx = ty * 4 + tx;
            if (!shape[idx]) continue;
            int bx = x + tx;
            int by = y + ty;
            if (bx < 0 || bx >= BOARD_W || by < 0 || by >= BOARD_H) return false;
            if (cells_[by * BOARD_W + bx] != 0) return false;
        }
    }
    return true;
}

bool GameBoard::place(const Tetromino& t, int x, int y, int rotation) {
    auto shape = t.shape(rotation);
    for (int ty = 0; ty < 4; ++ty) {
        for (int tx = 0; tx < 4; ++tx) {
            int idx = ty * 4 + tx;
            if (!shape[idx]) continue;
            int bx = x + tx;
            int by = y + ty;
            if (bx < 0 || bx >= BOARD_W || by < 0 || by >= BOARD_H) continue;
            cells_[by * BOARD_W + bx] = t.valueAt(tx, ty);
        }
    }
    aout << "GameBoard: placed piece at " << x << "," << y << " rot=" << rotation << std::endl;
    return true;
}

std::vector<int> GameBoard::clearFullLines() {
    std::vector<int> cleared;
    for (int y = BOARD_H - 1; y >= 0; --y) {
        bool full = true;
        for (int x = 0; x < BOARD_W; ++x) {
            if (cells_[y * BOARD_W + x] == 0) { full = false; break; }
        }
        if (full) {
            cleared.push_back(y);
            // shift above rows down
            for (int yy = y; yy > 0; --yy) {
                for (int x = 0; x < BOARD_W; ++x) {
                    cells_[yy * BOARD_W + x] = cells_[(yy - 1) * BOARD_W + x];
                }
            }
            // clear top row
            for (int x = 0; x < BOARD_W; ++x) cells_[x] = 0;
            ++y; // re-check same row as it now contains shifted content
        }
    }
    return cleared;
}

uint8_t GameBoard::cell(int x, int y) const {
    if (x < 0 || x >= BOARD_W || y < 0 || y >= BOARD_H) return 0;
    return cells_[y * BOARD_W + x];
}

void GameBoard::setCell(int x, int y, uint8_t v) {
    if (x < 0 || x >= BOARD_W || y < 0 || y >= BOARD_H) return;
    cells_[y * BOARD_W + x] = v;
}

uint8_t GameBoard::visibleCell(int x, int y) const {
    // first, check active tetromino overlay
    if (hasActive_) {
        auto shape = activeTetromino_.shape(activeRot_);
        for (int ty = 0; ty < 4; ++ty) {
            for (int tx = 0; tx < 4; ++tx) {
                int idx = ty * 4 + tx;
                if (!shape[idx]) continue;
                int bx = activeX_ + tx;
                int by = activeY_ + ty;
                if (bx == x && by == y) return activeTetromino_.valueAt(tx, ty);
            }
        }
    }
    return cell(x,y);
}

void GameBoard::debugPrint() const {
    for (int y = 0; y < BOARD_H; ++y) {
        std::string line;
        for (int x = 0; x < BOARD_W; ++x) {
            uint8_t c = cells_[y * BOARD_W + x];
            line += c ? 'X' : '.';
        }
        aout << line << std::endl;
    }
}

void GameBoard::hardDrop() {
    if (!hasActive_) return;
    while (fits(activeTetromino_, activeX_, activeY_ + 1, activeRot_)) {
        activeY_ += 1;
    }
    // lock
    place(activeTetromino_, activeX_, activeY_, activeRot_);
    hasActive_ = false;
    aout << "GameBoard: hardDropped and locked" << std::endl;
    auto res = MergeLogic::runMergeChain(*this);
    lastMergeCount_ = res.merges;
    lastClearedLines_ = (int)clearFullLines().size();
}

bool GameBoard::movePiece(int dx) {
    if (!hasActive_) return false;
    if (fits(activeTetromino_, activeX_ + dx, activeY_, activeRot_)) {
        activeX_ += dx;
        aout << "GameBoard: moved active piece to " << activeX_ << "," << activeY_ << std::endl;
        return true;
    }
    return false;
}

bool GameBoard::rotatePiece(int dir) {
    if (!hasActive_) return false;
    int newRot = (activeRot_ + dir) & 3;
    if (fits(activeTetromino_, activeX_, activeY_, newRot)) {
        activeRot_ = newRot;
        aout << "GameBoard: rotated active piece to rot=" << activeRot_ << std::endl;
        return true;
    }
    // simple wall-kick attempts
    if (fits(activeTetromino_, activeX_ - 1, activeY_, newRot)) { activeX_ -= 1; activeRot_ = newRot; aout << "GameBoard: rotated with wall-kick left" << std::endl; return true; }
    if (fits(activeTetromino_, activeX_ + 1, activeY_, newRot)) { activeX_ += 1; activeRot_ = newRot; aout << "GameBoard: rotated with wall-kick right" << std::endl; return true; }
    return false;
}

void GameBoard::update(double dt, const InputManager& input) {
    // spawn if needed
    if (!hasActive_) spawnNextPiece();

    // process interpreted input
    const auto &st = input.state();
    if (st.moveLeft) movePiece(-1);
    if (st.moveRight) movePiece(1);
    if (st.rotate) rotatePiece(1);
    if (st.hardDrop) hardDrop();
    // softDrop: accelerate gravity by moving one step immediately
    if (st.softDrop) {
        if (fits(activeTetromino_, activeX_, activeY_ + 1, activeRot_)) {
            activeY_ += 1;
        }
    }

    // gravity
    gravityTimerSec_ += dt;
    const double gravityInterval = 0.5; // placeholder
    if (gravityTimerSec_ >= gravityInterval) {
        gravityTimerSec_ = 0.0;
        if (fits(activeTetromino_, activeX_, activeY_ + 1, activeRot_)) {
            activeY_ += 1;
        } else {
            // lock
            place(activeTetromino_, activeX_, activeY_, activeRot_);
            hasActive_ = false;
            auto res = MergeLogic::runMergeChain(*this);
            lastMergeCount_ = res.merges;
            lastClearedLines_ = (int)clearFullLines().size();
        }
    }
}
