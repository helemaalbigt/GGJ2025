using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleSpawner : MonoBehaviour {
    public event Action OnLetterSpawned;
    
    public List<LetterPrefab> _letterPrefabs;
    public Transform _head;
    public Transform _parent;
    public TextMeshPro _subTitle;

    private List<SpawnedLetter> _spawnedLetters = new List<SpawnedLetter>();

    private const float CharacterW = 0.1f;
    private const float Margin = 0.02f;

    private void Update() {
        //position parent;
        var targetPos = _head.position + Vector3.ProjectOnPlane(_head.forward, Vector3.up) * 0.6f + Vector3.up * 0.15f;
        _parent.position = Vector3.Lerp(_parent.position, targetPos, Time.deltaTime * 2f);
        
        var fwd = (transform.position - _head.position).normalized;
        var right = Vector3.Cross(fwd, Vector3.up).normalized;; //face is on right so right needs to be fwd
        var up = Vector3.Cross(right, fwd).normalized;
        var targetRotation = Quaternion.LookRotation(fwd, up);
        _parent.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        
        //position letters
        foreach (var letter in _spawnedLetters) {
            letter.instance.transform.localPosition = letter.targetLocalPos;
            letter.instance.transform.localRotation = Quaternion.identity;
        }
    }

    public IEnumerator SpawnTitle() {
        yield return StartCoroutine(SpawnString("BUBBLE", 0f));
        yield return StartCoroutine(SpawnString("JUGGLE", -0.23f));

        yield return new WaitForSeconds(1f);
        
        SetAllSpawnedBlowable();
        _subTitle.text = "BLOW TO START";
        _subTitle.transform.localPosition = new Vector3(0,- 0.45f, 0);
        _subTitle.enabled = true;
    }
    
    public IEnumerator SpawnEnd() {
        yield return new WaitForSeconds(1f);
        
        yield return StartCoroutine(SpawnString("REPLAY?", 0f));
        
        yield return new WaitForSeconds(1f);
        
        SetAllSpawnedBlowable();
        _subTitle.text = "BLOW TO RETRY";
        _subTitle.transform.localPosition = new Vector3(0,- 0.45f, 0);
        _subTitle.enabled = true;
    }

    public void Clear() {
        StopAllCoroutines();
        for (int i = 0; i < _spawnedLetters.Count; i++) {
            var letter = _spawnedLetters[i];
            Destroy(letter.instance);
        }
        _spawnedLetters.Clear();
        _subTitle.enabled = false;
    }
    
    private IEnumerator SpawnString(string value, float vertOffset) {
        var chars = value.ToCharArray();
        var totalW = chars.Length * CharacterW + (chars.Length - 1) * Margin;
        var halfW = totalW / 2f;
        var startPos = new Vector3(-halfW, vertOffset, 0);

        for (int i = 0; i < chars.Length; i++) {
            yield return new WaitForSeconds(0.1f);
            
            var spawnPos = startPos + new Vector3(i * (CharacterW + Margin), 0, 0);
            var prefab = GetLetterPrefab(chars[i]);
            var letter = prefab != null ? Instantiate(prefab, _parent) : new GameObject();
            letter.transform.localPosition = spawnPos;
            var rigidBody = letter.GetComponent<Rigidbody>();
            var blowForce = letter.GetComponent<BlowForce>();
            
            _spawnedLetters.Add(new SpawnedLetter() {
                instance = letter,
                rigidbody = rigidBody,
                blowForce = blowForce,
                targetLocalPos = spawnPos
            });
            
            OnLetterSpawned?.Invoke();
        }
    }

    private GameObject GetLetterPrefab(char value) {
        foreach (var letter in _letterPrefabs) {
            if (letter.key == value) {
                return letter.prefab;
            }
        }

        return null;
    }

    private void SetAllSpawnedBlowable() {
        for (int i = 0; i < _spawnedLetters.Count; i++) {
            var letter = _spawnedLetters[i];
            letter.rigidbody.isKinematic = false;
            letter.blowForce.enabled = true;
        }
    }
}

[System.Serializable]
public class LetterPrefab {
    public char key;
    public GameObject prefab;
}

public class SpawnedLetter {
    public GameObject instance;
    public Rigidbody rigidbody;
    public BlowForce blowForce;
    public Vector3 targetLocalPos;
}
