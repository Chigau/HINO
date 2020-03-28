#include <catch.hpp>
#include "Common.h"

TEST_CASE("Coordinates") {
    Coord origin{0, 0};
    
    REQUIRE(Coord{0, -1} == get<pos::top>(origin));
    REQUIRE(Coord{0, 1} == get<pos::bottom>(origin));
    REQUIRE(Coord{-1, 0} == get<pos::left>(origin));
    REQUIRE(Coord{1, 0} == get<pos::right>(origin));
}
