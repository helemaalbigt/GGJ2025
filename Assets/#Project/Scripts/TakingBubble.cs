using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakingBubble : MonoBehaviour
{

    public Material BubbleMaterial;
    public AudioSource audioSource;


    public Texture textureWhenPlaying; 
    public Texture textureWhenNotPlaying; 

    private Renderer objectRenderer; 

    void Start()
    {
       
        objectRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
       
        if (audioSource.isPlaying)
        {
           
            objectRenderer.material.mainTexture = textureWhenPlaying;
        }
        else
        {
            
            objectRenderer.material.mainTexture = textureWhenNotPlaying;
        }
    }
}