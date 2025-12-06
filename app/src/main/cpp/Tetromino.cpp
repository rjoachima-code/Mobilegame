#include "Tetromino.h"
#include <cstring>

Tetromino::Tetromino(PieceType type)
: type_(type), shapes_(), baseValue_(2)
{
    initShapes();
}

static const uint8_t SHAPE_I[4][16] = {
    // rotation 0
    {0,0,0,0,
     1,1,1,1,
     0,0,0,0,
     0,0,0,0},
    // rotation 90
    {0,0,1,0,
     0,0,1,0,
     0,0,1,0,
     0,0,1,0},
    // 180
    {0,0,0,0,
     1,1,1,1,
     0,0,0,0,
     0,0,0,0},
    // 270
    {0,1,0,0,
     0,1,0,0,
     0,1,0,0,
     0,1,0,0}
};

static const uint8_t SHAPE_O[4][16] = {
    {0,1,1,0,
     0,1,1,0,
     0,0,0,0,
     0,0,0,0},
    {0,1,1,0,
     0,1,1,0,
     0,0,0,0,
     0,0,0,0},
    {0,1,1,0,
     0,1,1,0,
     0,0,0,0,
     0,0,0,0},
    {0,1,1,0,
     0,1,1,0,
     0,0,0,0,
     0,0,0,0}
};

// We'll provide simplified T/S/Z/J/L shapes (no fancy offsets)
static const uint8_t SHAPE_T[4][16] = {
    {0,1,0,0,
     1,1,1,0,
     0,0,0,0,
     0,0,0,0},
    {0,1,0,0,
     0,1,1,0,
     0,1,0,0,
     0,0,0,0},
    {0,0,0,0,
     1,1,1,0,
     0,1,0,0,
     0,0,0,0},
    {0,1,0,0,
     1,1,0,0,
     0,1,0,0,
     0,0,0,0}
};

static const uint8_t SHAPE_S[4][16] = {
    {0,1,1,0,
     1,1,0,0,
     0,0,0,0,
     0,0,0,0},
    {0,1,0,0,
     0,1,1,0,
     0,0,1,0,
     0,0,0,0},
    {0,1,1,0,
     1,1,0,0,
     0,0,0,0,
     0,0,0,0},
    {0,1,0,0,
     0,1,1,0,
     0,0,1,0,
     0,0,0,0}
};

static const uint8_t SHAPE_Z[4][16] = {
    {1,1,0,0,
     0,1,1,0,
     0,0,0,0,
     0,0,0,0},
    {0,0,1,0,
     0,1,1,0,
     0,1,0,0,
     0,0,0,0},
    {1,1,0,0,
     0,1,1,0,
     0,0,0,0,
     0,0,0,0},
    {0,0,1,0,
     0,1,1,0,
     0,1,0,0,
     0,0,0,0}
};

static const uint8_t SHAPE_J[4][16] = {
    {1,0,0,0,
     1,1,1,0,
     0,0,0,0,
     0,0,0,0},
    {0,1,1,0,
     0,1,0,0,
     0,1,0,0,
     0,0,0,0},
    {0,0,0,0,
     1,1,1,0,
     0,0,1,0,
     0,0,0,0},
    {0,1,0,0,
     0,1,0,0,
     1,1,0,0,
     0,0,0,0}
};

static const uint8_t SHAPE_L[4][16] = {
    {0,0,1,0,
     1,1,1,0,
     0,0,0,0,
     0,0,0,0},
    {0,1,0,0,
     0,1,0,0,
     0,1,1,0,
     0,0,0,0},
    {0,0,0,0,
     1,1,1,0,
     1,0,0,0,
     0,0,0,0},
    {1,1,0,0,
     0,1,0,0,
     0,1,0,0,
     0,0,0,0}
};

void Tetromino::initShapes() {
    // copy based on type
    switch (type_) {
        case PieceType::I:
            memcpy(shapes_.data(), SHAPE_I, sizeof(SHAPE_I));
            break;
        case PieceType::O:
            memcpy(shapes_.data(), SHAPE_O, sizeof(SHAPE_O));
            break;
        case PieceType::T:
            memcpy(shapes_.data(), SHAPE_T, sizeof(SHAPE_T));
            break;
        case PieceType::S:
            memcpy(shapes_.data(), SHAPE_S, sizeof(SHAPE_S));
            break;
        case PieceType::Z:
            memcpy(shapes_.data(), SHAPE_Z, sizeof(SHAPE_Z));
            break;
        case PieceType::J:
            memcpy(shapes_.data(), SHAPE_J, sizeof(SHAPE_J));
            break;
        case PieceType::L:
            memcpy(shapes_.data(), SHAPE_L, sizeof(SHAPE_L));
            break;
    }
}

const uint8_t* Tetromino::shape(int rotation) const {
    rotation = rotation & 3;
    return &shapes_[rotation * 16];
}

uint8_t Tetromino::valueAt(int tx, int ty) const {
    // simple: all blocks of a tetromino share baseValue
    return baseValue_;
}

