using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour {
    
    public BlowForce blowForce;
    public BubbleColliderManager colliderManager;
    private GameManager _gameManager;

    private Transform _head;
    
	public BubbleDescription bubbleDescription;

	public AudioSource audioSource;
    public bool IsTalking;

    public void Setup(Transform head) {
        _head = head;
        
        blowForce.Setup(_head);
    }

    void Update() {
        LookAtPlayer();
    }

    private void LookAtPlayer() {
        var bubbleToHead = transform.position - _head.position;
        var fwd = Vector3.Cross(Vector3.up, bubbleToHead).normalized; //face is on right so right needs to be fwd
        var up = Vector3.Cross(bubbleToHead, fwd).normalized;
        var targetRotation = Quaternion.LookRotation(fwd, up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 1.5f);
    }


    public Action SayHiToOtherBubbles;
    public void SubscribeToEvents(GameManager gameManager) 
    {
		_gameManager = gameManager;
        _gameManager.OnAnyBubbleSpawned += SayHiToOtherBubbles;
	}

    public void OnDestroy()
	{
		_gameManager.OnAnyBubbleSpawned -= SayHiToOtherBubbles;
	}
}
