cmake_minimum_required(VERSION 3.10)

project(HINO LANGUAGES CXX)

add_subdirectory(externals/avcpp)

add_subdirectory(src)

enable_testing()
add_subdirectory(tests)
