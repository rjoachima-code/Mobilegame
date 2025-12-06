#include "MergeLogic.h"
#include "GameBoard.h"
#include <queue>

// Single-pass internal merge that returns number of merges performed this pass
static int runSinglePass(GameBoard &board) {
    const int w = BOARD_W;
    const int h = BOARD_H;
    std::vector<int> visited(w * h, 0);
    int merges = 0;

    auto idx = [&](int x, int y){ return y * w + x; };

    for (int y = 0; y < h; ++y) {
        for (int x = 0; x < w; ++x) {
            int id = idx(x,y);
            if (visited[id]) continue;
            uint8_t val = board.cell(x,y);
            if (val == 0) continue;

            // BFS to collect region
            std::vector<int> region;
            std::queue<std::pair<int,int>> q;
            q.emplace(x,y);
            visited[id] = 1;
            while (!q.empty()) {
                auto p = q.front(); q.pop();
                int rx = p.first, ry = p.second;
                region.push_back(idx(rx,ry));
                const int dirs[4][2] = {{1,0},{-1,0},{0,1},{0,-1}};
                for (auto &d: dirs) {
                    int nx = rx + d[0];
                    int ny = ry + d[1];
                    if (nx < 0 || nx >= w || ny < 0 || ny >= h) continue;
                    int nid = idx(nx,ny);
                    if (visited[nid]) continue;
                    if (board.cell(nx,ny) == val) {
                        visited[nid] = 1;
                        q.emplace(nx,ny);
                    }
                }
            }

            if (region.size() >= 2) {
                // perform merge: pick first cell as target
                int target = region[0];
                int tx = target % w;
                int ty = target / w;
                board.setCell(tx, ty, val * 2);
                // clear others
                for (size_t i = 1; i < region.size(); ++i) {
                    int idd = region[i];
                    int cx = idd % w;
                    int cy = idd / w;
                    board.setCell(cx, cy, 0);
                }
                merges += 1;
            }
        }
    }
    return merges;
}

MergeLogic::MergeResult MergeLogic::runMergeChain(GameBoard &board) {
    MergeLogic::MergeResult result;
    while (true) {
        int m = runSinglePass(board);
        if (m == 0) break;
        result.merges += m;
        result.chainCount += 1;
        // After merges, gravity-like behavior isn't modeled here. Caller may call clearFullLines() afterward.
    }
    return result;
}
