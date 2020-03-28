cmake_minimum_required(VERSION 3.12)

project(HINO LANGUAGES CXX)

add_library(g_options INTERFACE)
add_library(g_warnings INTERFACE)

target_compile_features(g_options INTERFACE cxx_std_20)

if(MSVC)
  target_compile_options(g_options
                         INTERFACE -DNOMINMAX
                                   -D_CRT_SECURE_NO_DEPRECATE
                                   -D_CRT_NONSTDC_NO_DEPRECATE
                                   -D_CRT_SECURE_NO_WARNINGS
																	 /permissive-
												)

  target_compile_options(g_warnings
                         INTERFACE /W3)

else()
  if(ENABLE_ASAN)
    target_compile_options(g_options INTERFACE -fsanitize=address)
    target_link_libraries(g_options INTERFACE -fsanitize=address)
  endif()

  if(NOT ${CMAKE_BUILD_TYPE} MATCHES "Release")
    target_compile_options(g_warnings
                           INTERFACE -Wall
                                     # -Werror someday
                                     -Wextra
                                     -Wshadow
                                     -Wunused
                                     -Wold-style-cast
                                     -Woverloaded-virtual
                                     -Wcast-align
                                     -Wnull-dereference)
  endif()

  target_link_libraries(g_options INTERFACE stdc++fs)
endif()

add_subdirectory(externals/avcpp EXCLUDE_FROM_ALL)
add_subdirectory(externals/catch EXCLUDE_FROM_ALL)

add_subdirectory(src)

enable_testing()
add_subdirectory(tests)
