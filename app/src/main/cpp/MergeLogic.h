#pragma once

#include <vector>
#include <cstdint>

class GameBoard;

namespace MergeLogic {
    struct MergeResult {
        int merges = 0;      // number of merge operations performed this pass
        int chainCount = 0;  // total chain passes performed
    };

    // Run merge scanning on the board until stable. Returns total merges and chain count.
    MergeResult runMergeChain(GameBoard& board);
}
