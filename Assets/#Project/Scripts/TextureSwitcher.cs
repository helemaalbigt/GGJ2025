using UnityEngine;

public class TextureSwitcher : MonoBehaviour
{
    public AudioSource audioSource;    // Reference to the AudioSource
    public Texture textureWhenPlaying; // Texture to apply when the audio is playing
    public Texture textureWhenNotPlaying; // Texture to apply when the audio is not playing
    public float switchInterval = .1f;  // Interval in seconds between texture swaps

    private Renderer objectRenderer; 
    private float timer = 0f;        
    private bool usingTexture1 = true; 

    void Start()
    {
        // Get the Renderer component of the GameObject this script is attached to
        objectRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
       
        if (audioSource.isPlaying)
        {
           
            timer += Time.deltaTime;

            // Check if enough time has passed to switch the texture
            if (timer >= switchInterval)
            {
                // Reset the timer
                timer = 0f;

                // Alternate between the two textures
                if (usingTexture1)
                {
                    objectRenderer.material.mainTexture = textureWhenNotPlaying;
                }
                else
                {
                    objectRenderer.material.mainTexture = textureWhenPlaying;
                }

                // Toggle the flag for the next texture swap
                usingTexture1 = !usingTexture1;
            }
        }
        else
        {
            // If audio is not playing, ensure the material has the 'not playing' texture
            objectRenderer.material.mainTexture = textureWhenNotPlaying;
        }
    }
}
