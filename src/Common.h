#pragma once
#include <utility>

using Coord = std::pair<int, int>;

namespace pos {
    constexpr std::pair<int, int> top {0, -1};
    constexpr std::pair<int, int> bottom {0, 1};
    constexpr std::pair<int, int> left {-1, 0};
    constexpr std::pair<int, int> right {1, 0};
}

constexpr Coord operator+(const Coord &lhs, const Coord &rhs) {
    return {lhs.first + rhs.first, lhs.second + rhs.second};
}

template<Coord pos>
Coord get(const Coord &rel) {
    return rel + pos;
}
