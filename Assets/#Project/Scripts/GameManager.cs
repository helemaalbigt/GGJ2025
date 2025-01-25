using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Android;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {

    public event Action<State> OnStateChanged;

    public event Action<Bubble> OnBubbleSpawned;
	public BubbleDescription[] bubbleDescriptions;

	private State _state;
    public State state {
        get => _state;
        private set {
            if (_state != value) {
                _state = value;
                OnStateChanged?.Invoke(_state);
            }
        }
    }

    public List<Bubble> bubbles = new List<Bubble>();

    [Header("Game Variables")] 
    public float spawnFrequency = 15f;
    
    [Header("Asset/Scene References")] 
    public Bubble _bubblePrefab;
    public Transform _head;

    void Start()
    {
        //get mic permissions
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            Permission.RequestUserPermission(Permission.Microphone);

        StartCoroutine(WaitToStart());
    }

#region STATES

    IEnumerator WaitToStart() {
        state = State.waitToStart;
        
        //TODO: spawn title

        //Check for blow to start
        yield return CheckForBlow();

        StartCoroutine(Playing());
    }

    IEnumerator Playing() {
        state = State.playing;
        
        //Spawn first bubble
        bubbles.Clear();
        SpawnBubble();

        var startTime = Time.unscaledTime;
        var anyBubblePopped = false;

        while (!anyBubblePopped) {
            //TODO: spawn new bubble every X seconds

            if (Time.unscaledTime - startTime > spawnFrequency) {
                SpawnBubble(true);
                startTime = Time.unscaledTime;
            }
            
            //Check for popped bubbles
            foreach (var bubble in bubbles) {
                if (bubble.colliderManager.HasPopped())
                    anyBubblePopped = true;
                
                if(Input.GetKeyDown(KeyCode.Space))
                    anyBubblePopped = true;
            }
            
            yield return null;
        }

        StartCoroutine(GameOver());
    }

    IEnumerator GameOver() {
        state = State.gameOver;

        //pop all bubbles
        for (int i = 0; i < bubbles.Count; i++) {
            var bubble = bubbles[i];
            bubble.colliderManager.ForcePopBubbleImmediate();
            yield return new WaitForSeconds(0.3f);
        }
        
        //destroy all bubbles
        for (int i = 0; i < bubbles.Count; i++) {
            var bubble = bubbles[i];
            Destroy(bubble.gameObject);
        }
        bubbles.Clear();
        
        //TODO: show highscore
        yield return new WaitForSeconds(1f);

        //Check for blow to try again
        yield return CheckForBlow();
        
        StartCoroutine(Playing());
    }

#endregion

    private IEnumerator CheckForBlow() {
        var timeBlown = 0f;
        var hasBlown = false;
        while (!hasBlown) {

            if (MicInput.GetLevelMax() > 0.001f) {
                timeBlown += Time.unscaledDeltaTime;
            }

            if (timeBlown > 0.3f)
                hasBlown = true;
            
            if(Input.GetKeyDown(KeyCode.Space))
                hasBlown = true;
            
            yield return null;
        }
    }

    private void SpawnBubble(bool spawnInFrontOfPlayer = false) {
        var spawnPose = GetSpawnPose(!spawnInFrontOfPlayer);
        var firstBubble = Instantiate(_bubblePrefab, spawnPose.position, spawnPose.rotation);
        firstBubble.Setup(_head);

		bubbles.Add(firstBubble);

        firstBubble.bubbleDescription = bubbleDescriptions[Random.Range(0, bubbleDescriptions.Length)];
		OnBubbleSpawned?.Invoke(firstBubble);
	}
    
    private Pose GetSpawnPose(bool randomPos = true) {
        var randomOffset = randomPos ? Random.Range(-0.4f, 0.4f) : 0;
        var pos = _head.position + Vector3.ProjectOnPlane(_head.forward, Vector3.up).normalized * 0.5f + Vector3.up * 0.5f + Vector3.ProjectOnPlane(_head.right, Vector3.up).normalized * randomOffset;
        var posToHeadXZ = Vector3.ProjectOnPlane(pos - _head.position, Vector3.up).normalized;
        var rot = Quaternion.LookRotation(posToHeadXZ, Vector3.up);
        return new Pose(pos, rot);
    }

    public enum State {
        waitToStart,
        playing,
        gameOver
    }
}
