using System;
using UnityEngine;

public class MicInput : MonoBehaviour {

    private string _device;
    private static AudioClip _clipRecord;
    private static int _sampleWindow = 64;

    private bool _isInitialized;
    
    private void OnEnable() {
        InitMic();
    }

    private void OnDestroy() {
        StopMic();
    }
    
    private void OnApplicationFocus(bool hasFocus) {
        if (hasFocus) {
            if (!_isInitialized) {
                InitMic();
                _isInitialized = true;
            }
        } else {
            StopMic();
            _isInitialized = false;
        }
    }

    public static float GetLevelMax() {
        float levelMax = 0f;
        float[] waveData = new float[_sampleWindow];
        int micPosition = Microphone.GetPosition(null) - (_sampleWindow + 1);//null means the first microphone
        if (micPosition < 0) return 0f;
        _clipRecord.GetData(waveData, micPosition);
        //Get peak on last samples
        for (int i = 0; i < _sampleWindow; i++) {
            float wavePeak = waveData[i] * waveData[i];
            if (levelMax < wavePeak)
                levelMax = wavePeak;
        }

        return levelMax;
    }

    private void InitMic() {
        if (String.IsNullOrEmpty(_device)) _device = Microphone.devices[0];
        _clipRecord = Microphone.Start(_device, true, 999, 44100);
    }

    private void StopMic() {
        Microphone.End(_device);
    }
}