#include "Spawner.h"
#include <algorithm>

Spawner::Spawner() : rng_(std::random_device{}()) {
    refillBag();
}

void Spawner::refillBag() {
    bag_.clear();
    std::vector<PieceType> all = {PieceType::I, PieceType::O, PieceType::T, PieceType::S, PieceType::Z, PieceType::J, PieceType::L};
    std::shuffle(all.begin(), all.end(), rng_);
    for (auto &p : all) bag_.push_back(p);
}

Tetromino Spawner::next() {
    if (bag_.empty()) refillBag();
    PieceType t = bag_.front();
    bag_.pop_front();
    if (bag_.empty()) refillBag();
    return Tetromino(t);
}

Tetromino Spawner::peek(int idx) const {
    if (idx < 0) idx = 0;
    if (idx >= (int)bag_.size()) idx = (int)bag_.size() - 1;
    return Tetromino(bag_[idx]);
}

