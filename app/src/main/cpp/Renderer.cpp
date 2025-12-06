#include "Renderer.h"

#include <game-activity/native_app_glue/android_native_app_glue.h>
#include <GLES3/gl3.h>
#include <memory>
#include <vector>
#include <android/imagedecoder.h>
#include <jni.h>
#include <android/bitmap.h>

#include "AndroidOut.h"
#include "Shader.h"
#include "Utility.h"
#include "TextureAsset.h"
#include "EffectManager.h"

//! executes glGetString and outputs the result to logcat
#define PRINT_GL_STRING(s) {aout << #s": "<< glGetString(s) << std::endl;}

/*!
 * @brief if glGetString returns a space separated list of elements, prints each one on a new line
 *
 * This works by creating an istringstream of the input c-style string. Then that is used to create
 * a vector -- each element of the vector is a new element in the input string. Finally a foreach
 * loop consumes this and outputs it to logcat using @a aout
 */
#define PRINT_GL_STRING_AS_LIST(s) { \
std::istringstream extensionStream((const char *) glGetString(s));\
std::vector<std::string> extensionList(\
        std::istream_iterator<std::string>{extensionStream},\
        std::istream_iterator<std::string>());\
aout << #s":\n";\
for (auto& extension: extensionList) {\
    aout << extension << "\n";\
}\
aout << std::endl;\
}

//! Color for cornflower blue. Can be sent directly to glClearColor
#define CORNFLOWER_BLUE 100 / 255.f, 149 / 255.f, 237 / 255.f, 1

// Vertex shader, you'd typically load this from assets
static const char *vertex = R"vertex(#version 300 es
in vec3 inPosition;
in vec2 inUV;

out vec2 fragUV;

uniform mat4 uProjection;

void main() {
    fragUV = inUV;
    gl_Position = uProjection * vec4(inPosition, 1.0);
}
)vertex";

// Fragment shader, you'd typically load this from assets
static const char *fragment = R"fragment(#version 300 es
precision mediump float;

in vec2 fragUV;

uniform sampler2D uTexture;

out vec4 outColor;

void main() {
    outColor = texture(uTexture, fragUV);
}
)fragment";

// Particle shader: supports per-instance pos (vec2), size (float), color (vec4)
static const char *particleVertex = R"pv(#version 300 es
layout(location=0) in vec2 aPos; // quad vertex
layout(location=1) in vec2 aUV;
layout(location=2) in vec2 iOffset; // instance offset
layout(location=3) in float iSize; // instance size
layout(location=4) in vec4 iColor; // instance color
uniform mat4 uProjection;
out vec2 vUV;
out vec4 vColor;
void main() {
    vec2 pos = aPos * iSize + iOffset;
    gl_Position = uProjection * vec4(pos.xy, 0.0, 1.0);
    vUV = aUV;
    vColor = iColor;
}
)pv";

static const char *particleFragment = R"pf(#version 300 es
precision mediump float;
in vec2 vUV;
in vec4 vColor;
uniform sampler2D uTexture;
out vec4 outColor;
void main() {
    vec4 tex = texture(uTexture, vUV);
    // multiply texture alpha by color
    outColor = vec4(vColor.rgb, vColor.a * tex.a);
}
)pf";

/*!
 * Half the height of the projection matrix. This gives you a renderable area of height 4 ranging
 * from -2 to 2
 */
static constexpr float kProjectionHalfHeight = 2.f;

/*!
 * The near plane distance for the projection matrix. Since this is an orthographic projection
 * matrix, it's convenient to have negative values for sorting (and avoiding z-fighting at 0).
 */
static constexpr float kProjectionNearPlane = -1.f;

/*!
 * The far plane distance for the projection matrix. Since this is an orthographic porjection
 * matrix, it's convenient to have the far plane equidistant from 0 as the near plane.
 */
static constexpr float kProjectionFarPlane = 1.f;

Renderer::~Renderer() {
    if (display_ != EGL_NO_DISPLAY) {
        eglMakeCurrent(display_, EGL_NO_SURFACE, EGL_NO_SURFACE, EGL_NO_CONTEXT);
        if (context_ != EGL_NO_CONTEXT) {
            eglDestroyContext(display_, context_);
            context_ = EGL_NO_CONTEXT;
        }
        if (surface_ != EGL_NO_SURFACE) {
            eglDestroySurface(display_, surface_);
            surface_ = EGL_NO_SURFACE;
        }
        eglTerminate(display_);
        display_ = EGL_NO_DISPLAY;
    }
}

void Renderer::render() {
    // Check to see if the surface has changed size. This is _necessary_ to do every frame when
    // using immersive mode as you'll get no other notification that your renderable area has
    // changed.
    updateRenderArea();

    // When the renderable area changes, the projection matrix has to also be updated. This is true
    // even if you change from the sample orthographic projection matrix as your aspect ratio has
    // likely changed.
    if (shaderNeedsNewProjectionMatrix_) {
        // a placeholder projection matrix allocated on the stack. Column-major memory layout
        float projectionMatrix[16] = {0};

        // build an orthographic projection matrix for 2d rendering
        Utility::buildOrthographicMatrix(
                projectionMatrix,
                kProjectionHalfHeight,
                float(width_) / height_,
                kProjectionNearPlane,
                kProjectionFarPlane);

        // send the matrix to the shader
        // Note: the shader must be active for this to work. Since we only have one shader for this
        // demo, we can assume that it's active.
        shader_->setProjectionMatrix(projectionMatrix);

        // make sure the matrix isn't generated every frame
        shaderNeedsNewProjectionMatrix_ = false;
    }

    // clear the color buffer
    glClear(GL_COLOR_BUFFER_BIT);

    // Render all the models. There's no depth testing in this sample so they're accepted in the
    // order provided. But the sample EGL setup requests a 24 bit depth buffer so you could
    // configure it at the end of initRenderer
    if (!models_.empty()) {
        for (const auto &model: models_) {
            shader_->drawModel(model);
        }
    }

    // render particles via instancing
    auto &particles = EffectManager::getParticles();
    if (!particles.empty() && particleProgram_ != 0) {
        // update instance buffer: each instance = vec2 offset, float size, vec4 color (7 floats)
        std::vector<float> instanceData;
        instanceData.reserve(particles.size() * 7);
        for (const auto &p : particles) {
            instanceData.push_back(p.x);
            instanceData.push_back(p.y);
            instanceData.push_back(p.size);
            instanceData.push_back(p.r);
            instanceData.push_back(p.g);
            instanceData.push_back(p.b);
            instanceData.push_back(p.a);
        }
        glBindBuffer(GL_ARRAY_BUFFER, instanceVBO_);
        glBufferSubData(GL_ARRAY_BUFFER, 0, instanceData.size() * sizeof(float), instanceData.data());

        // bind particle program
        glUseProgram(particleProgram_);
        // set projection
        GLint projLoc = glGetUniformLocation(particleProgram_, "uProjection");
        float projectionMatrix[16] = {0};
        Utility::buildOrthographicMatrix(projectionMatrix, kProjectionHalfHeight, float(width_)/height_, kProjectionNearPlane, kProjectionFarPlane);
        glUniformMatrix4fv(projLoc, 1, GL_FALSE, projectionMatrix);
        // bind texture unit 0
        glActiveTexture(GL_TEXTURE0);
        glBindTexture(GL_TEXTURE_2D, hudTexture_->getTextureID());
        GLint texLoc = glGetUniformLocation(particleProgram_, "uTexture");
        glUniform1i(texLoc, 0);

        // bind quad VBO
        glBindBuffer(GL_ARRAY_BUFFER, quadVBO_);
        glEnableVertexAttribArray(0);
        glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, sizeof(float)*4, (void*)0);
        glEnableVertexAttribArray(1);
        glVertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, sizeof(float)*4, (void*)(sizeof(float)*2));

        // bind instance buffer
        glBindBuffer(GL_ARRAY_BUFFER, instanceVBO_);
        // iOffset
        glEnableVertexAttribArray(2);
        glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, sizeof(float)*7, (void*)(0));
        glVertexAttribDivisor(2, 1);
        // iSize
        glEnableVertexAttribArray(3);
        glVertexAttribPointer(3, 1, GL_FLOAT, GL_FALSE, sizeof(float)*7, (void*)(sizeof(float)*2));
        glVertexAttribDivisor(3, 1);
        // iColor
        glEnableVertexAttribArray(4);
        glVertexAttribPointer(4, 4, GL_FLOAT, GL_FALSE, sizeof(float)*7, (void*)(sizeof(float)*3));
        glVertexAttribDivisor(4, 1);

        // bind IBO
        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, quadIBO_);
        glDrawElementsInstanced(GL_TRIANGLES, 6, GL_UNSIGNED_SHORT, 0, (GLsizei)particles.size());

        // cleanup
        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);
        glBindBuffer(GL_ARRAY_BUFFER, 0);
        glUseProgram(0);
    }

    // Present the rendered image. This is an implicit glFlush.
    auto swapResult = eglSwapBuffers(display_, surface_);
    assert(swapResult == EGL_TRUE);
}

void Renderer::initRenderer() {
    // Choose your render attributes
    constexpr EGLint attribs[] = {
            EGL_RENDERABLE_TYPE, EGL_OPENGL_ES3_BIT,
            EGL_SURFACE_TYPE, EGL_WINDOW_BIT,
            EGL_BLUE_SIZE, 8,
            EGL_GREEN_SIZE, 8,
            EGL_RED_SIZE, 8,
            EGL_DEPTH_SIZE, 24,
            EGL_NONE
    };

    // The default display is probably what you want on Android
    auto display = eglGetDisplay(EGL_DEFAULT_DISPLAY);
    eglInitialize(display, nullptr, nullptr);

    // figure out how many configs there are
    EGLint numConfigs;
    eglChooseConfig(display, attribs, nullptr, 0, &numConfigs);

    // get the list of configurations
    std::unique_ptr<EGLConfig[]> supportedConfigs(new EGLConfig[numConfigs]);
    eglChooseConfig(display, attribs, supportedConfigs.get(), numConfigs, &numConfigs);

    // Find a config we like.
    // Could likely just grab the first if we don't care about anything else in the config.
    // Otherwise hook in your own heuristic
    auto config = *std::find_if(
            supportedConfigs.get(),
            supportedConfigs.get() + numConfigs,
            [&display](const EGLConfig &config) {
                EGLint red, green, blue, depth;
                if (eglGetConfigAttrib(display, config, EGL_RED_SIZE, &red)
                    && eglGetConfigAttrib(display, config, EGL_GREEN_SIZE, &green)
                    && eglGetConfigAttrib(display, config, EGL_BLUE_SIZE, &blue)
                    && eglGetConfigAttrib(display, config, EGL_DEPTH_SIZE, &depth)) {

                    aout << "Found config with " << red << ", " << green << ", " << blue << ", "
                         << depth << std::endl;
                    return red == 8 && green == 8 && blue == 8 && depth == 24;
                }
                return false;
            });

    aout << "Found " << numConfigs << " configs" << std::endl;
    aout << "Chose " << config << std::endl;

    // create the proper window surface
    EGLint format;
    eglGetConfigAttrib(display, config, EGL_NATIVE_VISUAL_ID, &format);
    EGLSurface surface = eglCreateWindowSurface(display, config, app_->window, nullptr);

    // Create a GLES 3 context
    EGLint contextAttribs[] = {EGL_CONTEXT_CLIENT_VERSION, 3, EGL_NONE};
    EGLContext context = eglCreateContext(display, config, nullptr, contextAttribs);

    // get some window metrics
    auto madeCurrent = eglMakeCurrent(display, surface, surface, context);
    assert(madeCurrent);

    display_ = display;
    surface_ = surface;
    context_ = context;

    // make width and height invalid so it gets updated the first frame in @a updateRenderArea()
    width_ = -1;
    height_ = -1;

    PRINT_GL_STRING(GL_VENDOR);
    PRINT_GL_STRING(GL_RENDERER);
    PRINT_GL_STRING(GL_VERSION);
    PRINT_GL_STRING_AS_LIST(GL_EXTENSIONS);

    shader_ = std::unique_ptr<Shader>(
            Shader::loadShader(vertex, fragment, "inPosition", "inUV", "uProjection"));
    assert(shader_);

    // Note: there's only one shader in this demo, so I'll activate it here. For a more complex game
    // you'll want to track the active shader and activate/deactivate it as necessary
    shader_->activate();

    // setup any other gl related global states
    glClearColor(CORNFLOWER_BLUE);

    // enable alpha globally for now, you probably don't want to do this in a game
    glEnable(GL_BLEND);
    glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

    // create particle shader
    auto compileShader = [](GLenum type, const char* src) -> GLuint {
        GLuint s = glCreateShader(type);
        glShaderSource(s, 1, &src, nullptr);
        glCompileShader(s);
        GLint compiled = 0; glGetShaderiv(s, GL_COMPILE_STATUS, &compiled);
        if (!compiled) {
            GLint len = 0; glGetShaderiv(s, GL_INFO_LOG_LENGTH, &len);
            if (len) { std::string log(len, '\0'); glGetShaderInfoLog(s, len, nullptr, log.data()); aout << "Shader compile error: " << log << std::endl; }
            glDeleteShader(s); return 0;
        }
        return s;
    };
    GLuint pVert = compileShader(GL_VERTEX_SHADER, particleVertex);
    GLuint pFrag = compileShader(GL_FRAGMENT_SHADER, particleFragment);
    particleProgram_ = glCreateProgram();
    if (pVert) glAttachShader(particleProgram_, pVert);
    if (pFrag) glAttachShader(particleProgram_, pFrag);
    glBindAttribLocation(particleProgram_, 0, "aPos");
    glBindAttribLocation(particleProgram_, 1, "aUV");
    glBindAttribLocation(particleProgram_, 2, "iOffset");
    glBindAttribLocation(particleProgram_, 3, "iSize");
    glBindAttribLocation(particleProgram_, 4, "iColor");
    glLinkProgram(particleProgram_);
    if (pVert) glDeleteShader(pVert);
    if (pFrag) glDeleteShader(pFrag);

    // create quad VBO (two triangles unit quad)
    float quadVerts[] = {
        // x,y, u,v
        0.5f, 0.0f, 1.0f, 0.0f,
        -0.5f, 0.0f, 0.0f, 0.0f,
        -0.5f, 1.0f, 0.0f, 1.0f,
        0.5f, 1.0f, 1.0f, 1.0f
    };
    unsigned short quadIdx[] = {0,1,2,0,2,3};
    glGenBuffers(1, &quadVBO_);
    glBindBuffer(GL_ARRAY_BUFFER, quadVBO_);
    glBufferData(GL_ARRAY_BUFFER, sizeof(quadVerts), quadVerts, GL_STATIC_DRAW);

    glGenBuffers(1, &quadIBO_);
    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, quadIBO_);
    glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(quadIdx), quadIdx, GL_STATIC_DRAW);

    // instance buffer (dynamic)
    glGenBuffers(1, &instanceVBO_);
    glBindBuffer(GL_ARRAY_BUFFER, instanceVBO_);
    // reserve space for e.g. 1024 particles
    glBufferData(GL_ARRAY_BUFFER, 1024 * (sizeof(float)*7), nullptr, GL_DYNAMIC_DRAW);

    glBindBuffer(GL_ARRAY_BUFFER, 0);
    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, 0);

    // get some demo models into memory
    createModels();

    // create simple 1x1 white texture for HUD
    {
        GLuint tex;
        glGenTextures(1, &tex);
        glBindTexture(GL_TEXTURE_2D, tex);
        uint8_t pixel[4] = {255,255,255,255};
        glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, 1, 1, 0, GL_RGBA, GL_UNSIGNED_BYTE, pixel);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
        glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
        hudTexture_ = TextureAsset::fromTextureId(tex);
    }

    // create font atlas by calling MainActivity.createFontAtlas via JNI
    if (app_ && app_->activity && app_->activity->vm) {
        JNIEnv* env = nullptr;
        app_->activity->vm->AttachCurrentThread(&env, nullptr);
        if (env) {
            jclass mainActClass = env->FindClass("com/example/myapplication/MainActivity");
            if (mainActClass) {
                jmethodID mid = env->GetStaticMethodID(mainActClass, "createFontAtlas", "(Landroid/content/Context;)Landroid/graphics/Bitmap;");
                if (mid) {
                    jobject bmp = env->CallStaticObjectMethod(mainActClass, mid, app_->activity->javaGameActivity);
                    if (bmp) {
                        // convert Bitmap to GL texture
                        AndroidBitmapInfo info;
                        void* pixels;
                        if (AndroidBitmap_getInfo(env, bmp, &info) == ANDROID_BITMAP_RESULT_SUCCESS && AndroidBitmap_lockPixels(env, bmp, &pixels) == ANDROID_BITMAP_RESULT_SUCCESS) {
                            GLuint tex;
                            glGenTextures(1, &tex);
                            glBindTexture(GL_TEXTURE_2D, tex);
                            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
                            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
                            glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, info.width, info.height, 0, GL_RGBA, GL_UNSIGNED_BYTE, pixels);
                            fontAtlasTexture_ = TextureAsset::fromTextureId(tex);
                            AndroidBitmap_unlockPixels(env, bmp);
                        }
                        env->DeleteLocalRef(bmp);
                    }
                }
                env->DeleteLocalRef(mainActClass);
            }
        }
    }
}

void Renderer::createModels() {
    // Create tileTemplate_ (unit quad) and load tile texture once
    std::vector<Vertex> vertices = {
            Vertex(Vector3{1, 1, 0}, Vector2{0, 0}), // 0
            Vertex(Vector3{-1, 1, 0}, Vector2{1, 0}), // 1
            Vertex(Vector3{-1, -1, 0}, Vector2{1, 1}), // 2
            Vertex(Vector3{1, -1, 0}, Vector2{0, 1}) // 3
    };
    std::vector<Index> indices = {0,1,2,0,2,3};
    auto assetManager = app_->activity->assetManager;
    spTileTexture_ = TextureAsset::loadAsset(assetManager, "android_robot.png");
    tileTemplate_ = Model(vertices, indices, spTileTexture_);

    // keep models_ initially empty; renderBoard will reuse tileTemplate_
}

void Renderer::updateRenderArea() {
    EGLint width;
    eglQuerySurface(display_, surface_, EGL_WIDTH, &width);

    EGLint height;
    eglQuerySurface(display_, surface_, EGL_HEIGHT, &height);

    if (width != width_ || height != height_) {
        width_ = width;
        height_ = height;
        glViewport(0, 0, width, height);

        // make sure that we lazily recreate the projection matrix before we render
        shaderNeedsNewProjectionMatrix_ = true;
    }
}

void Renderer::renderBoard(const GameBoard &board) {
    // create models for each filled cell (lightweight, reuse template)
    models_.clear();
    for (int y = 0; y < BOARD_H; ++y) {
        for (int x = 0; x < BOARD_W; ++x) {
            uint8_t c = board.cell(x,y);
            if (c == 0) continue;
            // create a small quad model at board coordinates by copying vertices and shifting
            std::vector<Vertex> verts = {
                    Vertex(Vector3{(float)x, (float)(-y), 0}, Vector2{0, 0}),
                    Vertex(Vector3{(float)x - 1, (float)(-y), 0}, Vector2{1, 0}),
                    Vertex(Vector3{(float)x - 1, (float)(-y) + 1, 0}, Vector2{1, 1}),
                    Vertex(Vector3{(float)x, (float)(-y) + 1, 0}, Vector2{0, 1})
            };
            models_.emplace_back(verts, std::vector<Index>{0,1,2,0,2,3}, spTileTexture_);
        }
    }
}

void Renderer::handleInput() {
    // handle all queued inputs
    auto *inputBuffer = android_app_swap_input_buffers(app_);
    if (!inputBuffer) {
        // no inputs yet.
        return;
    }

    // handle motion events (motionEventsCounts can be 0).
    for (auto i = 0; i < inputBuffer->motionEventsCount; i++) {
        auto &motionEvent = inputBuffer->motionEvents[i];
        auto action = motionEvent.action;

        // Find the pointer index, mask and bitshift to turn it into a readable value.
        auto pointerIndex = (action & AMOTION_EVENT_ACTION_POINTER_INDEX_MASK)
                >> AMOTION_EVENT_ACTION_POINTER_INDEX_SHIFT;
        aout << "Pointer(s): ";

        // get the x and y position of this event if it is not ACTION_MOVE.
        auto &pointer = motionEvent.pointers[pointerIndex];
        auto x = GameActivityPointerAxes_getX(&pointer);
        auto y = GameActivityPointerAxes_getY(&pointer);

        // determine the action type and process the event accordingly.
        switch (action & AMOTION_EVENT_ACTION_MASK) {
            case AMOTION_EVENT_ACTION_DOWN:
            case AMOTION_EVENT_ACTION_POINTER_DOWN:
                aout << "(" << pointer.id << ", " << x << ", " << y << ") "
                     << "Pointer Down";
                break;

            case AMOTION_EVENT_ACTION_CANCEL:
                // treat the CANCEL as an UP event: doing nothing in the app, except
                // removing the pointer from the cache if pointers are locally saved.
                // code pass through on purpose.
            case AMOTION_EVENT_ACTION_UP:
            case AMOTION_EVENT_ACTION_POINTER_UP:
                aout << "(" << pointer.id << ", " << x << ", " << y << ") "
                     << "Pointer Up";
                break;

            case AMOTION_EVENT_ACTION_MOVE:
                // There is no pointer index for ACTION_MOVE, only a snapshot of
                // all active pointers; app needs to cache previous active pointers
                // to figure out which ones are actually moved.
                for (auto index = 0; index < motionEvent.pointerCount; index++) {
                    pointer = motionEvent.pointers[index];
                    x = GameActivityPointerAxes_getX(&pointer);
                    y = GameActivityPointerAxes_getY(&pointer);
                    aout << "(" << pointer.id << ", " << x << ", " << y << ")";

                    if (index != (motionEvent.pointerCount - 1)) aout << ",";
                    aout << " ";
                }
                aout << "Pointer Move";
                break;
            default:
                aout << "Unknown MotionEvent Action: " << action;
        }
        aout << std::endl;
    }
    // clear the motion input count in this buffer for main thread to re-use.
    android_app_clear_motion_events(inputBuffer);

    // handle input key events.
    for (auto i = 0; i < inputBuffer->keyEventsCount; i++) {
        auto &keyEvent = inputBuffer->keyEvents[i];
        aout << "Key: " << keyEvent.keyCode <<" ";
        switch (keyEvent.action) {
            case AKEY_EVENT_ACTION_DOWN:
                aout << "Key Down";
                break;
            case AKEY_EVENT_ACTION_UP:
                aout << "Key Up";
                break;
            case AKEY_EVENT_ACTION_MULTIPLE:
                // Deprecated since Android API level 29.
                aout << "Multiple Key Actions";
                break;
            default:
                aout << "Unknown KeyEvent Action: " << keyEvent.action;
        }
        aout << std::endl;
    }
    // clear the key input count too.
    android_app_clear_key_events(inputBuffer);
}

void Renderer::renderText(const std::string &text, float x, float y, float glyphW, float glyphH) {
    if (!fontAtlasTexture_) return;
    // assume atlas contains digits 0-9 in a single row
    for (size_t i = 0; i < text.size(); ++i) {
        char c = text[i];
        if (c < '0' || c > '9') continue;
        int idx = c - '0';
        float gx = (float)(idx);
        float atlasCols = 10.0f;
        float u0 = gx / atlasCols;
        float u1 = (gx + 1.0f) / atlasCols;
        float px = x + i * glyphW;
        float py = y;
        std::vector<Vertex> verts = {
                Vertex(Vector3{px + glyphW, py, 0}, Vector2{u0,0}),
                Vertex(Vector3{px, py, 0}, Vector2{u1,0}),
                Vertex(Vector3{px, py + glyphH, 0}, Vector2{u1,1}),
                Vertex(Vector3{px + glyphW, py + glyphH, 0}, Vector2{u0,1})
        };
        std::vector<Index> inds = {0,1,2,0,2,3};
        Model m(verts, inds, fontAtlasTexture_);
        shader_->drawModel(m);
    }
}

void Renderer::renderHUD(int score) {
    // draw numeric score in upper-left in world coords
    std::string s = std::to_string(score);
    renderText(s, -1.8f, 1.6f, 0.35f, 0.6f);
}
