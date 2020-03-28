#include "Letter.h"

void Letter::AddPixel(Coord coord)
{
    pixels.push_back(coord);
    
    if (coord.first > xMax)
        xMax = coord.first;
    if (coord.first < xMin)
        xMin = coord.first;

    if (coord.second > yMax)
        yMax = coord.second;
    if (coord.second < yMin)
        yMin = coord.second;
}