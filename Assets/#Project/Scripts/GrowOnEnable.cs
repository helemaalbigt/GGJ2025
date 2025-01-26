using System;
using System.Collections;
using UnityEngine;

public class GrowOnEnable : MonoBehaviour
{
    public bool X = true;
    public bool Y;
    public bool Z;
    public float initialDelay = 0;
    public float growTime;
    public AnimationCurve growCurve;
    public bool addDelayBasedOnIndex;
    public float delayFactor;

    private Vector3 _origScale;
    
    private void OnEnable() {
        _origScale = transform.localScale;
        transform.localScale = new Vector3(X ? 0.01f : _origScale.x, Y ? 0.01f : _origScale.y, Z ? 0.01f : _origScale.z);
        StartCoroutine(Grow());
    }

    private void OnDisable() {
        transform.localScale = _origScale;
    }

    IEnumerator Grow() {
        yield return new WaitForSeconds(initialDelay);
        yield return null;

        float delay = addDelayBasedOnIndex ? delayFactor * GetSiblingIndex() : 0;
        yield return new WaitForSeconds(delay);

        var startTime = Time.unscaledTime;
        while (Time.unscaledTime - startTime < growTime) {
            var f = Mathf.Clamp01((Time.unscaledTime - startTime) / growTime);
            transform.localScale = _origScale * growCurve.Evaluate(f);
            yield return null;
        }

        transform.localScale = _origScale;
    }

    private int GetSiblingIndex() {
        var siblings = transform.parent.GetComponentsInChildren<GrowOnEnable>();
        for (int i = 0; i < siblings.Length; i++) {
            if (siblings[i] == this)
                return i;
        }
        return 0;
    }

    private bool ContinueLerping() {
        return Vector3.Distance(_origScale, transform.localScale)>0.05f ;
    }
}