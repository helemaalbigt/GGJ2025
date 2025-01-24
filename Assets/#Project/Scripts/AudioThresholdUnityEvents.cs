using UnityEngine;
using UnityEngine.Events;

public class AudioThresholdUnityEvents : MonoBehaviour
{
    public string microphoneName;          // Optional: Choose a specific microphone
    public float upperDecibelThreshold = -30f;  // Decibel level to detect loud sound
    public float lowerDecibelThreshold = -50f;  // Decibel level to detect silence
    public float delayAfterSilence = 1f;        // Delay in seconds before triggering actions

    public UnityEvent onSoundThresholdExceeded; // UnityEvent for when sound exceeds threshold
    public UnityEvent onSilenceDetected;        // UnityEvent for when silence is detected
    public UnityEvent onDelayedTrigger;         // UnityEvent for delayed action after silence

    private AudioClip micClip;
    private bool isAboveThreshold = false;      // Whether sound exceeded the upper threshold
    private float silenceStartTime = -1f;       // Timestamp for when silence starts
    private float thresholdCrossTime = -1f;     // Timestamp for when threshold is crossed

    void Start()
    {
        // Start recording from the microphone
        if (Microphone.devices.Length > 0)
        {
            microphoneName = Microphone.devices[0];  // Default to the first microphone
            micClip = Microphone.Start(microphoneName, true, 10, AudioSettings.outputSampleRate);
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }
    }

    void Update()
    {
        if (micClip == null) return;

        // Analyze audio data
        float[] samples = new float[1024];
        micClip.GetData(samples, Microphone.GetPosition(microphoneName) - samples.Length);

        float rms = CalculateRMS(samples);
        float decibels = 20 * Mathf.Log10(rms / 0.1f);

        if (!isAboveThreshold && decibels > upperDecibelThreshold)
        {
            // Mark that the sound exceeded the upper threshold
            isAboveThreshold = true;
            thresholdCrossTime = Time.time; // Record the timestamp
            silenceStartTime = -1f;  // Reset silence timer

            Debug.Log($"Sound exceeded threshold at {thresholdCrossTime:F2} seconds.");
            onSoundThresholdExceeded?.Invoke(); // Invoke event for sound threshold exceeded
        }

        if (isAboveThreshold && decibels < lowerDecibelThreshold)
        {
            // Start silence timer when sound drops below the lower threshold
            if (silenceStartTime < 0)
            {
                silenceStartTime = Time.time; // Record silence start timestamp
                Debug.Log($"Sound dropped to silence at {silenceStartTime:F2} seconds.");
                onSilenceDetected?.Invoke(); // Invoke event for silence detection
            }

            // Trigger delayed action after silence
            if (Time.time >= silenceStartTime + delayAfterSilence)
            {
                Debug.Log($"Delayed action triggered at {Time.time:F2} seconds.");
                onDelayedTrigger?.Invoke(); // Invoke delayed trigger event
                ResetState();
            }
        }
    }

    float CalculateRMS(float[] samples)
    {
        float sum = 0f;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }
        return Mathf.Sqrt(sum / samples.Length);
    }

    void ResetState()
    {
        isAboveThreshold = false;
        silenceStartTime = -1f;
        thresholdCrossTime = -1f;
    }
}
