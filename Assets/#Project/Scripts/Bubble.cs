using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour {
    
    public BlowForce blowForce;
    public BubbleColliderManager colliderManager;


	public BubbleDescription bubbleDescription;

	public AudioSource audioSource;
    public bool IsTalking;

    public void Setup(Transform head) {
        blowForce.Setup(head);
    }
}
