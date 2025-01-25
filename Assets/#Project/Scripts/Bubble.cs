using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour {
    public BlowForce blowForce;
    public BubbleColliderManager colliderManager;

    public void Setup(Transform head) {
        blowForce.Setup(head);
    }
}
