using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSpawner : MonoBehaviour {
    public List<LetterPrefab> _letterPrefabs;
    public Transform _head;
    public Transform _parent;

    private List<GameObject> _spawnedLetters = new List<GameObject>();
    
    private void Update() {
        //position parent;
        var targetPos = _head.position + Vector3.ProjectOnPlane(_head.forward, Vector3.up) * 0.6f + Vector3.up * 0.15f;
        _parent.position = Vector3.Lerp(_parent.position, targetPos, Time.deltaTime * 2f);
        
        var bubbleToHead = transform.position - _head.position;
        var fwd = Vector3.Cross(Vector3.up, bubbleToHead).normalized; //face is on right so right needs to be fwd
        var up = Vector3.Cross(bubbleToHead, fwd).normalized;
        var targetRotation = Quaternion.LookRotation(fwd, up);
        _parent.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
    }

    public void SpawnTitle(string title) {
        
    }

    public void Clear() {
        
    }
}

[System.Serializable]
public class LetterPrefab {
    public string key;
    public GameObject prefab;
}
