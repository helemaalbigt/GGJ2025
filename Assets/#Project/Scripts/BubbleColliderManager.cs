using System;
using System.Collections;
using UnityEngine;

public class BubbleColliderManager : MonoBehaviour {
   
   public event Action OnBubbleTouchedSurface;
   public event Action OnBubblePopped;
   public GameObject POP;
   public Renderer renderer;


    private bool _bubblePopped;

   private void OnCollisionEnter(Collision collision) {
      if (!_bubblePopped) {
         //todo: play touche surface audio
         
         OnBubbleTouchedSurface?.Invoke();
         
         StartCoroutine(PlayPopEffects());


            //object value = Instantiate(POP, OnBubbleTouchedSurface.position, Quaternion.identity);

        }
   }

   public bool HasPopped() {
      return _bubblePopped;


   }

   public void ForcePopBubbleImmediate() {
      if(!_bubblePopped)
         StartCoroutine(PlayPopEffects(0f));
   }

   private IEnumerator PlayPopEffects(float timeBeforePop = 1f) {
      //let it rest on the surface a bit
      yield return new WaitForSeconds(timeBeforePop);
      
      //todo: play audio
      
      //todo: do splash effect
      
      //dissolve bubble
      var popTime = 0.12f;
      var startTime = Time.unscaledTime;
      while (Time.unscaledTime - startTime < popTime) {
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
