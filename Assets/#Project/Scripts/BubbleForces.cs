using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleForces : MonoBehaviour 
{
    public Rigidbody rigidbody;
    //public ConstantForce constantForce;
    public AnimationCurve forceCurve;
    public float maxForce = 20f;
    public float gravity = 9.81f;
    public float drag = 0.03f;
    
    [Header("Debug")] 
    [Range(0, 1)] public float _debugWindStrength;
    
    void FixedUpdate() {
        ApplyForce(Vector3.forward, _debugWindStrength);
        rigidbody.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        
        //drag
        //rigidbody.AddForce(rigidbody.velocity.sqrMagnitude * drag * -rigidbody.velocity.normalized, ForceMode.Acceleration);
    }

    public void ApplyForce(Vector3 direction, float strengthFactor) {
        var strength = forceCurve.Evaluate(strengthFactor);
        var force = direction.normalized * strength;
        rigidbody.AddForce(force, ForceMode.Acceleration);
    }
}
