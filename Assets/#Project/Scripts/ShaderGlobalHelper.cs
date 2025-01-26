using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderGlobalHelper : MonoBehaviour {
    public GameManager gameManger;
    public Transform head;

    private const int maxBubblePoints = 10;
    void Update()
    {
        Shader.SetGlobalVector("_headPos", new Vector3(head.position.x, head.position.y, head.position.z));

        for (int i = 0; i < maxBubblePoints; i++) {
            var pos = Vector4.zero;
            if (i < gameManger.bubbles.Count) {
                var bubble = gameManger.bubbles[i];
                if (bubble.colliderManager.HasPopped()) {
                    pos = Vector4.zero;
                } else {
                    pos = new Vector4(bubble.transform.position.x, bubble.transform.position.y, bubble.transform.position.z, 0);
                }
            }
            var variable = "_bub" + i;
            Shader.SetGlobalVector(variable, pos);
        }
    }
}
