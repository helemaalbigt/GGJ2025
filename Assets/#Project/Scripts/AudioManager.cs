using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameManager gameManager;


    [Tooltip("How often does the bubble talk")]
    public float SpeakInterval = 5;
    public float SpeakIntervalRandom = 2;
    private float SpeakTimer;


    void Start()
    {
        gameManager.OnBubbleSpawned += OnBubbleSpawned;
	}

    void Update()
    {
		if(gameManager.bubbles.Count != 0)
        {
            // check if any bubble is talking
            bool isAnyBubbleTalking = false;
            for (int i = 0; i < gameManager.bubbles.Count; i++)
            {
                if (!isAnyBubbleTalking)
                {
                    isAnyBubbleTalking = gameManager.bubbles[i].audioSource.isPlaying;
                    break;
                }
			}

            if(!isAnyBubbleTalking ) 
            {
                SpeakTimer -= Time.deltaTime;

				if (SpeakTimer < 0)
                {
                    // pick random bubble
                    int randomBubbleIndex = Random.Range(0, gameManager.bubbles.Count);

                    // make bubble talk
                    Bubble bubble = gameManager.bubbles[randomBubbleIndex];
					PlayRandomClip(bubble, bubble.bubbleDescription.JustFloatingClips);

                    SpeakTimer = SpeakInterval + Random.Range(-SpeakIntervalRandom, SpeakIntervalRandom);

				}
			}
        }
	}

    void OnBubbleSpawned(Bubble bubble)
	{
        PlayRandomClip(bubble, bubble.bubbleDescription.SpawnClips);

        bubble.colliderManager.OnBubbleTouchedSurface += () => { PlayRandomClip(bubble, bubble.bubbleDescription.AboutToPopClips); };
        bubble.colliderManager.OnBubblePopped += () => { PlayRandomClip(bubble, bubble.bubbleDescription.PopClips); };
	}

    private void PlayRandomClip(Bubble bubble, AudioClip[] clips)
	{
		int randomIndex = Random.Range(0, clips.Length);
		bubble.audioSource.PlayOneShot(clips[randomIndex]);
	}
}
