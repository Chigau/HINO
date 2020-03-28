#pragma once

#include <vector>
#include <array>
#include <string>
#include <limits>
#include "Common.h"
#include <frame.h>

class Letter{
public:
    bool isSpace = false;

    // The value of the letter after the parsing
    std::string value, secondChoice;

    // Neural Network parsing error
    float error = 1;

    float firstOverSecondCorrectness = 1;

    // Matrix of pixels for the Neural Network
    std::array<float, 24*24> pixelsMatrix;

    // List of internal pixels in coords
    std::vector<Coord> pixels;

    // List of outline pixels in coords
    std::vector<Coord> outlinePixels;

    // Bounds
    int xMax = 0, xMin = std::numeric_limits<int>::max(), yMax = 0, yMin = std::numeric_limits<int>::max();

    void AddPixel(Coord coord);

    //av::VideoFrame ArrayToBitmap();
    //void GenerateArray();

};