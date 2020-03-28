#pragma once

#include <frame.h>
#include <vector>
#include "Subtitle.h"

class ConversionThread{
    // The current frame where we're searching for subtitles
    av::VideoFrame frame;
    // The index of the current frame
    long frameIndex;
    // Pixels of the current frame that have been already checked
    std::vector<bool> filled[];

    // One array of subtitle foreach video
    std::vector<std::vector<Subtitle>> subtitles;
    // Subtitles that are in wait for user action
    std::vector<Subtitle> subtitles;
};
