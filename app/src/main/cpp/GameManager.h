#pragma once

class GameBoard;

class GameManager {
public:
    GameManager();

    void updateFromBoard(const GameBoard &board);

    uint64_t score() const { return score_; }
    int level() const { return level_; }
private:
    uint64_t score_ = 0;
    int level_ = 0;
    int combo_ = 0;
};

