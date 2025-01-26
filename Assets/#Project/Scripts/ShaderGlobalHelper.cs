using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderGlobalHelper : MonoBehaviour {
    public GameManager gameManger;
    public Transform head;

    private const int maxBubblePoints = 10;
    void Update()
    {
        Shader.SetGlobalVector("_headPos", new Vector4(head.position.x, head.position.y, head.position.z, 0));

        for (int i = 0; i < gameManger.bubbles.Count && i < maxBubblePoints; i++) {
            var bubble = gameManger.bubbles[i];
            var variable = "_bub" + i;
            Shader.SetGlobalVector(variable, new Vector4(bubble.transform.position.x,bubble.transform.position.y,bubble.transform.position.z, 0));
        }
    }
}
