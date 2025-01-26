using System;
using System.Collections;
using UnityEngine;

public class BubbleColliderManager : MonoBehaviour
{

	public event Action OnBubbleNearingSurface;
	public event Action OnBubbleTouchedSurface;
	public event Action OnBubblePopped;
	public GameObject POP;
	public Renderer renderer;

	public ParticleSystem[] particleSystems;


	private Vector3 _collisionPoint;
	private bool _bubblePopped;

	private Bubble _bubble;

	private void Start()
	{
		_bubble = GetComponent<Bubble>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!_bubblePopped)
		{
			//todo: play touche surface audio
			OnBubbleTouchedSurface?.Invoke();

			// Instantiate POP effect at the point of collision
			_collisionPoint = collision.contacts[0].point;

			StartCoroutine(PlayPopEffects());
		}
	}

	public float DistanceCheck = .5f;
	public Vector3 _direction;
	public Vector3 _prevPos;
	bool _isNearSurface;
	float _isNearSurfaceCooldown;
	private void FixedUpdate()
	{
		_direction = transform.position - _prevPos;
		_prevPos = transform.position;
		RaycastHit hit;
		if (Physics.SphereCast(transform.position, .1f, _direction, out hit, DistanceCheck))
		{
			if (!_isNearSurface)
			{
				OnBubbleNearingSurface?.Invoke();
				_isNearSurface = true;
				_isNearSurfaceCooldown = 1;// seconds cooldown
			}
			
			Debug.DrawRay(transform.position, _direction * DistanceCheck, Color.red);
		}
		else
		{
			// can reset close to floor when not talking
			if (!_bubble.IsTalking)
			{
				_isNearSurfaceCooldown-= Time.deltaTime;
				if( _isNearSurfaceCooldown < 0 )
					_isNearSurface = false;
			}
			Debug.DrawRay(transform.position, _direction * DistanceCheck, Color.green);
		}
	}

	public bool HasPopped()
	{
		return _bubblePopped;
	}

	public void ForcePopBubbleImmediate()
	{
		if (!_bubblePopped)
			StartCoroutine(PlayPopEffects(0f));

		POP.transform.position = transform.position;
	}

	private IEnumerator PlayPopEffects(float timeBeforePop = 1f)
	{
		//let it rest on the surface a bit
		yield return new WaitForSeconds(timeBeforePop);

		//waiting for audio to finish
		while (_bubble.IsTalking)
			yield return null;

		//splash effect
		foreach (var particles in particleSystems)
		{
			particles.Play();
		}

		//dissolve bubble
		var popTime = 0.35f;
		var startTime = Time.unscaledTime;
		while (Time.unscaledTime - startTime < popTime)
		{
			var f = 1f - Mathf.Clamp01((Time.unscaledTime - startTime) / popTime);
			renderer.material.SetFloat("_BubbleDissolve", f);
			yield return null;
		}
		renderer.material.SetFloat("_BubbleDissolve", 0f);

		//Invoke event
		OnBubblePopped?.Invoke();

		_bubblePopped = true;
	}
}
