add_executable(hino_test
    main.cpp
    coords.cpp
)

target_link_libraries(hino_test PRIVATE g_options g_warnings catch)
target_include_directories(hino_test PRIVATE ../src)

add_test(hino_test hino_test)
