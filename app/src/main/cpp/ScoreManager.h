#pragma once

class ScoreManager {
public:
    ScoreManager();

    void reset();

    void addMerges(int merges, int chainCount);
    void addLineClear(int lines);

    int score() const { return score_; }
    int combo() const { return combo_; }

private:
    int score_ = 0;
    int combo_ = 0; // current combo multiplier
};

