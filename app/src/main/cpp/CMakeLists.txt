# For more information about using CMake with Android Studio, read the
# documentation: https://d.android.com/studio/projects/add-native-code.html

cmake_minimum_required(VERSION 3.22.1)

project("jacameno")

# Creates your game shared library. The name must be the same as the
# one used for loading in your Kotlin/Java or AndroidManifest.txt files.
add_library(jacameno SHARED
        main.cpp
        AndroidOut.cpp
        Renderer.cpp
        Shader.cpp
        TextureAsset.cpp
        Utility.cpp

        GameLoop.cpp
        GameLoop.h
        GameBoard.cpp
        GameBoard.h
        Tetromino.cpp
        Tetromino.h
        InputManager.cpp
        InputManager.h
        MergeLogic.cpp
        MergeLogic.h
        Assets.cpp
        Assets.h
        Utils.h
        Spawner.cpp
        Spawner.h
        Rules.cpp
        Rules.h
        SaveManager.cpp
        SaveManager.h
        AudioManager.cpp
        AudioManager.h
        EffectManager.cpp
        EffectManager.h
        ScoreManager.cpp
        GameActivityJNI.cpp
)

# Searches for a package provided by the game activity dependency
find_package(game-activity REQUIRED CONFIG)

# Forces the linker to keep the JNI entry point for GameActivity
set(CMAKE_SHARED_LINKER_FLAGS
        "${CMAKE_SHARED_LINKER_FLAGS} -u Java_com_google_androidgamesdk_GameActivity_initializeNativeCode")

target_include_directories(jacameno PRIVATE
    ${game-activity_INCLUDE_DIRS}
)

# Configure libraries CMake uses to link your target library.
target_link_libraries(jacameno
        # The game activity
        game-activity::game-activity_static

        # EGL and other dependent libraries required for drawing
        # and interacting with Android system
        EGL
        GLESv3
        jnigraphics
        android
        log)
