using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public GameManager gameManager;

    [Tooltip("How often does the bubble talk")]
    public float SpeakInterval = 5;
    public float SpeakIntervalRandom = 2;
    private float SpeakTimer;

    private bool _bubbleInDistress;

	void Start()
    {
        gameManager.OnBubbleSpawned += OnBubbleSpawned;
		gameManager.OnStateChanged += GameManager_OnStateChanged;

        // initial value for timer.
        SpeakTimer = 3;
	}

    private void GameManager_OnStateChanged(GameManager.State obj)
    {
        if (obj == GameManager.State.gameOver)
        {
            // Nearing the floor/death
            Bubble survivingBubble = gameManager.GetSurvivingBubble();
            if (survivingBubble != null) 
            {
    			PlayRandomClip(survivingBubble, survivingBubble.bubbleDescription.GameOverReactionClips, true);
            }

            StopAllCoroutines();
		}
    }

	void Update()
    {
        if (gameManager.bubbles.Count == 0)
            return;

        if(gameManager.IsAnyBubbleTalking())
			return;

        if (_bubbleInDistress)
            return;

        // Just floating talk
		SpeakTimer -= Time.deltaTime;
		if (SpeakTimer < 0)
        {
            // pick random bubble
            int randomBubbleIndex = Random.Range(0, gameManager.bubbles.Count);

            // make that bubble talk
            Bubble bubble = gameManager.bubbles[randomBubbleIndex];
			PlayRandomClip(bubble, bubble.bubbleDescription.JustFloatingClips);

            SpeakTimer = SpeakInterval + Random.Range(-SpeakIntervalRandom, SpeakIntervalRandom);
		}
    }

	void OnBubbleSpawned(Bubble bubble)
	{
		if (gameManager.bubbles.Count == 1)
			PlayRandomClip(bubble, bubble.bubbleDescription.FirstIntroClips); // Hi with intro
		else
			PlayRandomClip(bubble, bubble.bubbleDescription.NextIntroClips); // Hi without intro

        StartCoroutine(SayHiToOtherBubbles(bubble));

		// Nearing the floor/death
		bubble.colliderManager.OnBubbleNearingSurface += () => 
        {
            _bubbleInDistress = true; 
            PlayRandomClip(bubble, bubble.bubbleDescription.nearDeathClips, true); 
        };
		// about to pop
		bubble.colliderManager.OnBubbleTouchedSurface += () => { PlayRandomClip(bubble, bubble.bubbleDescription.AboutToPopClips); };
		//pop
		bubble.colliderManager.OnBubblePopped += () => { PlayRandomClip(bubble, bubble.bubbleDescription.PopClips); };

	}

	private void PlayRandomClip(Bubble bubble, AudioClip[] clips, bool interrupCurrentClip = false)
	{
		int randomIndex = Random.Range(0, clips.Length);
        if(interrupCurrentClip)
    		bubble.audioSource.Stop();
		bubble.audioSource.PlayOneShot(clips[randomIndex]);
	}


	IEnumerator SayHiToOtherBubbles(Bubble spawnedBubble)
	{
        if (_bubbleInDistress)
		{
			InterruptAllBubbles(spawnedBubble);
		}

		// wait for other bubble(s) to stop talking.
		while (spawnedBubble.IsTalking)
		{
			yield return null;
		}

		foreach (var bubble in gameManager.bubbles)
        {
            if(bubble != spawnedBubble)
			{
				PlayRandomClip(bubble, bubble.bubbleDescription.GreetingClips);
				break;
			}
        }

	}

	private void InterruptAllBubbles(Bubble spawnedBubble)
	{
		foreach (var bubble in gameManager.bubbles)
		{
			if (bubble != spawnedBubble)
			{
				bubble.audioSource.Stop();
			}
		}
	}
}
