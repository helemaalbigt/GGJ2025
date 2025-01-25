using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSpawner : MonoBehaviour {
    public List<LetterPrefab> _letterPrefabs;
    public Transform _head;
    public Transform _parent;

    private List<GameObject> _spawnedLetters = new List<GameObject>();

    private const float CharacterW = 0.1f;
    private const float Margin = 0.02f;

    private void Start() {
        SpawnTitle();
    }

    private void Update() {
        //position parent;
        var targetPos = _head.position + Vector3.ProjectOnPlane(_head.forward, Vector3.up) * 0.6f + Vector3.up * 0.15f;
        _parent.position = Vector3.Lerp(_parent.position, targetPos, Time.deltaTime * 2f);
        
        var fwd = (transform.position - _head.position).normalized;
        var right = Vector3.Cross(fwd, Vector3.up).normalized;; //face is on right so right needs to be fwd
        var up = Vector3.Cross(right, fwd).normalized;
        var targetRotation = Quaternion.LookRotation(fwd, up);
        _parent.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
    }

    public void SpawnTitle() {
        SpawnString("BUBBLE", 0f);
        SpawnString("JUGGLE", -0.23f);
    }

    public void Clear() {
        
    }
    
    private void SpawnString(string value, float vertOffset) {
        var chars = value.ToCharArray();
        var totalW = chars.Length * CharacterW + (chars.Length - 1) * Margin;
        var halfW = totalW / 2f;
        var startPos = new Vector3(-halfW, vertOffset, 0);

        for (int i = 0; i < chars.Length; i++) {
            var spawnPos = startPos + new Vector3(i * (CharacterW + Margin), 0, 0);
            var letter = Instantiate(GetLetterPrefab(chars[i]), _parent);
            letter.transform.localPosition = spawnPos;
        }
    }

    private GameObject GetLetterPrefab(char value) {
        foreach (var letter in _letterPrefabs) {
            if (letter.key == value) {
                return letter.prefab;
            }
        }

        return _letterPrefabs[0].prefab;
    }
}

[System.Serializable]
public class LetterPrefab {
    public char key;
    public GameObject prefab;
}
