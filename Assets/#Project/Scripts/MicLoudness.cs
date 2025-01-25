using UnityEngine;

public class MicLoudness : MonoBehaviour
{
    public float loudness = 0f; // This will store the loudness value
    
    private AudioSource audioSource;
    private float[] audioSamples = new float[256]; // Array to hold the audio data

    private const float LOW_THRESHOLD = 0.0009f; // Value below which loudness becomes 0
    private const float HIGH_THRESHOLD = 1f; // Value above which loudness becomes 1
   
    void Start()
    {
        // Get or add an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Start recording from the microphone
        audioSource.clip = Microphone.Start(null, true, 1, 44100); // Start recording with 1 second length and 44100 Hz
        audioSource.loop = true; // Loop the audio

        // Wait until the microphone starts recording
        while (!(Microphone.GetPosition(null) > 0)) { }
        audioSource.Play();
    }

    void Update()
    {
        // Get the microphone's audio data
        audioSource.GetOutputData(audioSamples, 0);

        // Calculate the loudness based on the audio data
        float sum = 0f;
        foreach (float sample in audioSamples)
        {
            sum += Mathf.Abs(sample);
        }

        // Normalize the loudness between 0 and 1
        loudness = sum / audioSamples.Length;

        Debug.LogError(loudness.ToString("F15"));
        
        // Map loudness to 0 or 1 based on the thresholds
        if (loudness < LOW_THRESHOLD)
        {
            loudness = 0f; // If below the threshold, set to 0
        }
        else if (loudness > LOW_THRESHOLD)
        {
            loudness = 1f; // If above the threshold, set to 1
        }
    }

    void OnApplicationQuit()
    {
        // Stop the microphone recording when the app quits
        Microphone.End(null);
    }
}
