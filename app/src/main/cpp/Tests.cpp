#include "Tests.h"
#include "AndroidOut.h"
#include "Tetromino.h"
#include "GameBoard.h"
#include "MergeLogic.h"

#include <cassert>
#include <sstream>

using namespace std;

static bool testTetrominoShapes() {
    aout << "TEST: Tetromino shapes" << std::endl;
    Tetromino t(PieceType::I);
    auto s0 = t.shape(0);
    // check that second row has four 1s
    int count = 0;
    for (int i=0;i<16;i++) count += s0[i];
    if (count != 4) { aout << "FAIL: I piece expected 4 cells got " << count << std::endl; return false; }
    Tetromino o(PieceType::O);
    auto so = o.shape(0);
    int oc = 0; for (int i=0;i<16;i++) oc += so[i];
    if (oc != 4) { aout << "FAIL: O piece expected 4 cells got " << oc << std::endl; return false; }
    aout << "PASS: Tetromino shapes" << std::endl;
    return true;
}

static bool testLineClear() {
    aout << "TEST: Line clear" << std::endl;
    GameBoard b;
    // fill bottom row
    for (int x=0;x<BOARD_W;++x) b.setCell(x, BOARD_H-1, 1);
    auto cleared = b.clearFullLines();
    if (cleared.size() != 1) { aout << "FAIL: expected 1 cleared line got " << cleared.size() << std::endl; return false; }
    aout << "PASS: Line clear" << std::endl;
    return true;
}

static bool testMergeLogicSimple() {
    aout << "TEST: MergeSimple" << std::endl;
    GameBoard b;
    // place two adjacent cells with value 2
    b.setCell(0, 0, 2);
    b.setCell(1, 0, 2);
    auto res = MergeLogic::runMergeChain(b);
    if (res.merges != 1) { aout << "FAIL: expected 1 merge got " << res.merges << std::endl; return false; }
    if (b.cell(0,0) != 4) { aout << "FAIL: expected merged cell value 4 got " << (int)b.cell(0,0) << std::endl; return false; }
    if (b.cell(1,0) != 0) { aout << "FAIL: expected cleared cell 0 got " << (int)b.cell(1,0) << std::endl; return false; }
    aout << "PASS: MergeSimple" << std::endl;
    return true;
}

bool Tests::runAllTests() {
    bool ok = true;
    ok &= testTetrominoShapes();
    ok &= testLineClear();
    ok &= testMergeLogicSimple();
    aout << (ok ? "ALL TESTS PASSED" : "SOME TESTS FAILED") << std::endl;
    return ok;
}

