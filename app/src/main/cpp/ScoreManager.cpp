#include "ScoreManager.h"

ScoreManager::ScoreManager() { reset(); }

void ScoreManager::reset() {
    score_ = 0;
    combo_ = 1;
}

void ScoreManager::addMerges(int merges, int chainCount) {
    if (merges <= 0) return;
    // Base points per merge scales with merges and chainCount
    int base = 100;
    int gained = base * merges * (1 + chainCount);
    // Increase combo multiplier for chained merges
    if (chainCount > 0) combo_ += chainCount;
    score_ += gained * combo_;
}

void ScoreManager::addLineClear(int lines) {
    if (lines <= 0) return;
    int base = 200;
    int gained = base * lines * combo_;
    score_ += gained;
    // small combo bump for clears
    combo_ += lines;
}

