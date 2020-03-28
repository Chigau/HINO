#pragma once

#include <vector>

struct Subtitle {
        [[nodiscard]] std::string toString() const;

        std::vector<Line> lines;
        std::vector<> discardedPixels;
};
