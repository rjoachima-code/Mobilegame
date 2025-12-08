package com.jacameno.mobilegame

import android.view.View
import android.os.Handler
import android.os.Looper
import com.google.androidgamesdk.GameActivity

class MainActivity : GameActivity() {
    companion object {
        init {
            System.loadLibrary("jacameno")
        }
        @JvmStatic external fun nativeGetScore(): Int

        @JvmStatic fun createFontAtlas(ctx: android.content.Context): android.graphics.Bitmap {
            val cols = 10
            val glyphW = 64
            val glyphH = 64
            val bmp = android.graphics.Bitmap.createBitmap(cols * glyphW, glyphH, android.graphics.Bitmap.Config.ARGB_8888)
            val canvas = android.graphics.Canvas(bmp)
            canvas.drawColor(android.graphics.Color.TRANSPARENT)
            val paint = android.graphics.Paint()
            paint.isAntiAlias = true
            paint.color = android.graphics.Color.WHITE
            paint.textSize = 48f
            paint.textAlign = android.graphics.Paint.Align.CENTER
            val fm = paint.fontMetrics
            val baseY = glyphH/2f - (fm.ascent + fm.descent)/2f
            for (i in 0 until cols) {
                val ch = '0' + i
                val cx = i * glyphW + glyphW/2f
                canvas.drawText(ch.toString(), cx, baseY, paint)
            }
            return bmp
        }
    }

    private val handler = Handler(Looper.getMainLooper())
    private val updateRunnable = object : Runnable {
        override fun run() {
            val s = nativeGetScore()
            handler.postDelayed(this, 100)
        }
    }

    override fun onCreate(savedInstanceState: android.os.Bundle?) {
        super.onCreate(savedInstanceState)
        // nothing else here; GL renderer will request font atlas later
    }

    override fun onWindowFocusChanged(hasFocus: Boolean) {
        super.onWindowFocusChanged(hasFocus)
        if (hasFocus) {
            hideSystemUi()
        }
    }

    override fun onResume() {
        super.onResume()
    }

    override fun onPause() {
        super.onPause()
    }

    private fun hideSystemUi() {
        val decorView = window.decorView
        decorView.systemUiVisibility = (View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
                or View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                or View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                or View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                or View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
                or View.SYSTEM_UI_FLAG_FULLSCREEN)
    }
}