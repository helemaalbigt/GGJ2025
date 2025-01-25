using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BlowForce : MonoBehaviour {
    public Transform _mouth;
    public BubbleForces _bubbleForces;
    
    void Update()
    {
        var trigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        SetBlowForce(trigger);
    }

    /// <summary>
    /// Set blow force for this frame. Value needs to be a value from 0 to 1
    /// </summary>
    public void SetBlowForce(float value) {
        var forwardForce = _mouth.forward * value;
        var mouthToBubble = (transform.position - _mouth.position).normalized;
        var forceOnBubble = Vector3.Dot(forwardForce, mouthToBubble);
        
        Debug.DrawLine(_mouth.position, _mouth.position + mouthToBubble * forceOnBubble);
        
        _bubbleForces.ApplyForce(mouthToBubble, forceOnBubble);
    }
}
