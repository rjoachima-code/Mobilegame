#pragma once

#include <deque>
#include <random>
#include "Tetromino.h"

class Spawner {
public:
    Spawner();

    // Fill the bag and shuffle
    void refillBag();

    // Get next piece and peek
    Tetromino next();
    Tetromino peek(int idx) const;

private:
    std::deque<PieceType> bag_;
    mutable std::mt19937 rng_;
};

