using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BubbleDescription", menuName = "ScriptableObjects/SpawnBubbleDescription", order = 1)]
public class BubbleDescription : ScriptableObject
{
	public AudioClip[] SpawnClips;
	public AudioClip[] JustFloatingClips;
	public AudioClip[] AboutToPopClips;
	public AudioClip[] PopClips;
}
