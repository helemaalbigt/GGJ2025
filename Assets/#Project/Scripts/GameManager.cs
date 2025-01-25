using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Android;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {

    public event Action<State> OnStateChanged;
    public event Action<Bubble> OnBubbleSpawned;
	public Action OnAnyBubbleSpawned;


	public BubbleDescription[] bubbleDescriptions;
	private List<BubbleDescription> _leftoverBubbleDescriptions;

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
    public TitleSpawner _titleSpawner;

    void Start()
    {
        _leftoverBubbleDescriptions = bubbleDescriptions.ToList();

		//get mic permissions
		if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            Permission.RequestUserPermission(Permission.Microphone);

        StartCoroutine(WaitToStart());
    }

#region STATES

    IEnumerator WaitToStart() {
        state = State.waitToStart;

        yield return StartCoroutine(_titleSpawner.SpawnTitle());

        //Check for blow to start
        yield return CheckForBlow();

        StartCoroutine(Playing());
    }

    IEnumerator Playing() {
        state = State.playing;
        
        //Remove title
        _titleSpawner.Clear();
        
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

		// wait untill all bubbles are done talking
		while (IsAnyBubbleTalking())
            yield return null;

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

        // pick random description and remove it from the list so its not picked again.
        int bubleDescriptionIndex = Random.Range(0, _leftoverBubbleDescriptions.Count);
		firstBubble.bubbleDescription = bubbleDescriptions[bubleDescriptionIndex];
		_leftoverBubbleDescriptions.RemoveAt(bubleDescriptionIndex);
        // if all descriptions have been used, then just reuse all of them,
        // maybe its better to just have a max amount of bubble instead? :)
        if (_leftoverBubbleDescriptions.Count == 0)
			_leftoverBubbleDescriptions = bubbleDescriptions.ToList();

		OnAnyBubbleSpawned?.Invoke();
		OnBubbleSpawned?.Invoke(firstBubble);
	}
    
    private Pose GetSpawnPose(bool randomPos = true) {
        var randomOffset = randomPos ? Random.Range(-0.4f, 0.4f) : 0;
        var pos = _head.position + Vector3.ProjectOnPlane(_head.forward, Vector3.up).normalized * 0.5f + Vector3.up * 0.5f + Vector3.ProjectOnPlane(_head.right, Vector3.up).normalized * randomOffset;
        var posToHeadXZ = Vector3.ProjectOnPlane(pos - _head.position, Vector3.up).normalized;
        var rot = Quaternion.LookRotation(posToHeadXZ, Vector3.up);
        return new Pose(pos, rot);
    }

    public bool IsAnyBubbleTalking()
    {
		// check if any bubble is talking
		for (int i = 0; i < bubbles.Count; i++)
		{
			if (bubbles[i].IsTalking)
			{
				return true;
			}
		}

        return false;
	}

    public Bubble GetSurvivingBubble()
    {
        foreach (var bbl in bubbles)
        {
            if(!bbl.colliderManager.HasPopped())
                return bbl;
        }
        return null;
    }


	public enum State {
        waitToStart,
        playing,
        gameOver
    }

    public bool _debugIsTalking;
	private void Update()
	{
		_debugIsTalking = IsAnyBubbleTalking();
	}
}
