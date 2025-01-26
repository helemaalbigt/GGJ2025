using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BlowForce : MonoBehaviour {
    public BubbleForces _bubbleForces;

    public AnimationCurve micInputToValueCurve; //smooth out the top values
    public AnimationCurve distanceFalloffCurve;
    public float maxInfluenceDistance = 0.7f;

    private Transform _mouth;
    
    private const float MicMin = 0.0000001f;
    private const float MicMax = 0.01f;

	public event Action OnBubbleBlowedUpon;

    public Bubble bubble;

	public void Setup(Transform head) {
        _mouth = head;
    }
    
    void Update()
    {
        if(_mouth == null)
            return;
        
        //Debug/accessibly input
        var triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        
        var micValue = MicInput.GetLevelMax();
        var micValueNormalized = (micValue - MicMin) / (MicMax - MicMin);

        var value = Mathf.Max(triggerValue, micValueNormalized);
        
        SetBlowForce(value);
    }

    /// <summary>
    /// Set blow force for this frame. Value needs to be a value from 0 to 1
    /// </summary>
    public void SetBlowForce(float value) {
        //distance force falloff
        var distToBubble = Vector3.Distance(_mouth.position, transform.position);
        var distFactor = Mathf.Clamp01(distToBubble / maxInfluenceDistance);
        var force = distanceFalloffCurve.Evaluate(distFactor) * value;
        
        var forwardForce = _mouth.forward * force;
        var mouthToBubble = (transform.position - _mouth.position).normalized;
        var forceOnBubble = Vector3.Dot(forwardForce, mouthToBubble);
        
        Debug.DrawLine(_mouth.position, _mouth.position + mouthToBubble * forceOnBubble);
        
        _bubbleForces.ApplyForce(mouthToBubble, forceOnBubble);


		if(!bubble.IsTalking && forceOnBubble > 0.1f) 
        {
            OnBubbleBlowedUpon?.Invoke();
		}

	}
}
