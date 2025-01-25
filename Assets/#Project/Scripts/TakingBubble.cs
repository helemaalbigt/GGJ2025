using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakingBubble : MonoBehaviour
{

    public Material BubbleMaterial;
    public AudioSource audioSource;

    public float enumIndex;
    public enum face
    {
        NORMAL,
        TALKING

    }


    private void Update()
    {
        BubbleMaterial.SetFloat("_FACE", enumIndex);

        //if (audioSource.isPlaying)
        //{
        //    Debug.Log("playing audio");

        //    BubbleMaterial.SetInt("_FACE", 1);
            

        //}
        //else
        //{
        //    // Otherwise, set the face to NORMAL
        //    BubbleMaterial.SetInt("_FACE", 0);
        //}
    }
}