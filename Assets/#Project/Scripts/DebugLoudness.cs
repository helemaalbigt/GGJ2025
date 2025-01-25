using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugLoudness : MonoBehaviour {
    public TextMeshPro text;
    
    void Update() {
        text.text = MicInput.GetLevelMax().ToString("F10");
    }
}
