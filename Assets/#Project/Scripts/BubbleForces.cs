using Meta.XR.ImmersiveDebugger.UserInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class BubbleForces : MonoBehaviour 
{
    public Rigidbody rigidbody;
    //public ConstantForce constantForce;
    public AnimationCurve forceCurve;
    public float gravity = 9.81f;
    public float turbulanceRadius = 0.03f;
    private float turbulanceRotationTime = 2f;

    private float timePhase = 0f;
    private float anglePerSecond;

    [Header("Debug")] 
    [Range(0, 1)] public float _debugWindStrength;
    
    void FixedUpdate() {
        ApplyForce(Vector3.forward, _debugWindStrength);
        

        //Jiggle the down vector to emulate turbulance
        timePhase = Time.time % turbulanceRotationTime;

        anglePerSecond = (2*math.PI) / turbulanceRotationTime;
        var currentAngle = anglePerSecond * timePhase;

        var x = turbulanceRadius * Mathf.Cos(currentAngle);
        var y = turbulanceRadius * Mathf.Sin(currentAngle);

        var newDown = Vector3.down + Vector3.right * x + Vector3.forward * y;

        //apply gravity
        rigidbody.AddForce(newDown * gravity, ForceMode.Acceleration);

        //drag

        //rigidbody.AddForce(rigidbody.velocity.sqrMagnitude * drag * -rigidbody.velocity.normalized, ForceMode.Acceleration);
    }

    public void ApplyForce(Vector3 direction, float strengthFactor) {
        var strength = forceCurve.Evaluate(strengthFactor);
        var force = direction.normalized * strength;
        rigidbody.AddForce(force, ForceMode.Acceleration);
    }
}
