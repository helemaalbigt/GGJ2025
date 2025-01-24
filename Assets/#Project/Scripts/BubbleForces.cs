using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleForces : MonoBehaviour 
{
    public Rigidbody rigidbody;
    public AnimationCurve forceCurve;
    public float maxForce = 20f;
    
    [Header("Debug")] 
    [Range(0, 1)] public float _debugWindStrength;
    
    void FixedUpdate() {
        ApplyForce(Vector3.forward, _debugWindStrength);
    }

    public void ApplyForce(Vector3 direction, float strengthFactor) {
        var strength = forceCurve.Evaluate(strengthFactor);
        var force = direction.normalized * strength;
        rigidbody.AddForce(force);
    }
}
