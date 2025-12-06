#ifndef ANDROIDGLINVESTIGATIONS_RENDERER_H
#define ANDROIDGLINVESTIGATIONS_RENDERER_H

#include <EGL/egl.h>
#include <memory>

#include "Model.h"
#include "Shader.h"
#include "GameBoard.h"

struct android_app;

class Renderer {
public:
    /*!
     * @param pApp the android_app this Renderer belongs to, needed to configure GL
     */
    inline Renderer(android_app *pApp) :
            app_(pApp),
            display_(EGL_NO_DISPLAY),
            surface_(EGL_NO_SURFACE),
            context_(EGL_NO_CONTEXT),
            width_(0),
            height_(0),
            shaderNeedsNewProjectionMatrix_(true),
            window_(nullptr),
            ownsWindow_(false) {
        initRenderer();
    }

    // Alternate constructor: create renderer directly from ANativeWindow*
    inline Renderer(ANativeWindow* window) :
            app_(nullptr),
            display_(EGL_NO_DISPLAY),
            surface_(EGL_NO_SURFACE),
            context_(EGL_NO_CONTEXT),
            width_(0),
            height_(0),
            shaderNeedsNewProjectionMatrix_(true),
            window_(window),
            ownsWindow_(true) {
        initRenderer();
    }

    virtual ~Renderer();

    /*!
     * Handles input from the android_app.
     *
     * Note: this will clear the input queue
     */
    void handleInput();

    /*!
     * Renders all the models in the renderer
     */
    void render();

    // Render a simple visualization of the game board (creates models for each filled cell)
    void renderBoard(const GameBoard &board);

    // Renders HUD overlays like score
    void renderHUD(int score);

private:
    /*!
     * Performs necessary OpenGL initialization. Customize this if you want to change your EGL
     * context or application-wide settings.
     */
    void initRenderer();

    /*!
     * @brief we have to check every frame to see if the framebuffer has changed in size. If it has,
     * update the viewport accordingly
     */
    void updateRenderArea();

    /*!
     * Creates the models for this sample. You'd likely load a scene configuration from a file or
     * use some other setup logic in your full game.
     */
    void createModels();

    // HUD model (a simple quad)
    Model hudModel_ = Model({}, {}, nullptr);
    std::shared_ptr<TextureAsset> hudTexture_;
    std::shared_ptr<TextureAsset> fontAtlasTexture_;

    // cached tile texture and template model for reuse
    std::shared_ptr<TextureAsset> spTileTexture_;
    Model tileTemplate_ = Model({}, {}, nullptr);

    // render numeric text using the font atlas
    void renderText(const std::string &text, float x, float y, float glyphW, float glyphH);

    android_app *app_;
    EGLDisplay display_;
    EGLSurface surface_;
    EGLContext context_;
    EGLint width_;
    EGLint height_;

    bool shaderNeedsNewProjectionMatrix_;

    std::unique_ptr<Shader> shader_;
    std::vector<Model> models_;

    // particle rendering (instanced)
    GLuint particleProgram_ = 0;
    GLuint quadVBO_ = 0;
    GLuint quadIBO_ = 0;
    GLuint instanceVBO_ = 0;

    // If created from a raw ANativeWindow, keep it here
    ANativeWindow* window_ = nullptr;
    bool ownsWindow_ = false;
};

#endif //ANDROIDGLINVESTIGATIONS_RENDERER_H