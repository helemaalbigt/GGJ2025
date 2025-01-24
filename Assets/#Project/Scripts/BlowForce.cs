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

        var forwardForce = _mouth.forward * trigger;
        var mouthToBubble = (transform.position - _mouth.position).normalized;
        var forceOnBubble = Vector3.Dot(forwardForce, mouthToBubble);
        
        Debug.DrawLine(_mouth.position, _mouth.position + mouthToBubble * forceOnBubble);
        
        _bubbleForces.ApplyForce(mouthToBubble, forceOnBubble);
    }
}
