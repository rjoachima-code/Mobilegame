package com.jacameno.mobilegame;

import android.content.res.AssetFileDescriptor;
import android.content.Context;
import android.media.SoundPool;
import android.media.AudioAttributes;
import android.util.Log;
import android.media.ToneGenerator;

public class NativeAudioBridge {
    private SoundPool pool;
    private int mergeSound = -1;
    private int clearSound = -1;
    private ToneGenerator toneGen = null;

    public NativeAudioBridge(Context ctx) {
        AudioAttributes attrs = new AudioAttributes.Builder()
                .setUsage(AudioAttributes.USAGE_GAME)
                .setContentType(AudioAttributes.CONTENT_TYPE_SONIFICATION)
                .build();
        pool = new SoundPool.Builder().setAudioAttributes(attrs).setMaxStreams(4).build();
        boolean loaded = false;
        try {
            AssetFileDescriptor afd = ctx.getAssets().openFd("sfx_merge.wav");
            mergeSound = pool.load(afd, 1);
            afd.close();
            afd = ctx.getAssets().openFd("sfx_clear.wav");
            clearSound = pool.load(afd, 1);
            afd.close();
            loaded = true;
        } catch (Exception e) {
            Log.e("NativeAudioBridge", "Failed to load sfx: " + e.getMessage());
        }
        if (!loaded) {
            // fallback to ToneGenerator if assets not present
            try {
                toneGen = new ToneGenerator(android.media.AudioManager.STREAM_MUSIC, 100);
            } catch (Exception e) {
                Log.e("NativeAudioBridge", "ToneGenerator init failed: " + e.getMessage());
                toneGen = null;
            }
        }
    }

    public void play(int id) {
        if (pool != null && ((id == 1 && mergeSound != -1) || (id == 2 && clearSound != -1))) {
            if (id == 1 && mergeSound != -1) pool.play(mergeSound, 1f, 1f, 1, 0, 1f);
            if (id == 2 && clearSound != -1) pool.play(clearSound, 1f, 1f, 1, 0, 1f);
            return;
        }
        if (toneGen != null) {
            // simple beep differences for merge vs clear
            if (id == 1) toneGen.startTone(ToneGenerator.TONE_PROP_BEEP, 120);
            else if (id == 2) toneGen.startTone(ToneGenerator.TONE_PROP_BEEP2, 160);
        }
    }

    public void shutdown() {
        if (pool != null) {
            pool.release();
            pool = null;
        }
        if (toneGen != null) {
            toneGen.release();
            toneGen = null;
        }
    }
}
