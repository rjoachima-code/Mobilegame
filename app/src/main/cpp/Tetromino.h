#pragma once

#include <array>
#include <cstdint>

// Basic tetromino representation with 4x4 matrices
enum class PieceType : uint8_t { I, O, T, S, Z, J, L };

class Tetromino {
public:
    Tetromino(PieceType type = PieceType::I);

    // returns pointer to 4x4 mask for rotation (0..3)
    const uint8_t* shape(int rotation) const;

    // value to write into board (for M2-style numbers); default 2
    uint8_t valueAt(int tx, int ty) const;

    PieceType type() const { return type_; }

private:
    PieceType type_;
    // store shapes as 4 rotations * 16 entries
    std::array<uint8_t, 4 * 16> shapes_;
    uint8_t baseValue_;

    void initShapes();
};

