using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BubbleDescription", menuName = "ScriptableObjects/SpawnBubbleDescription", order = 1)]
public class BubbleDescription : ScriptableObject
{
	public AudioClip[] FirstIntroClips;
	public AudioClip[] NextIntroClips;
	public AudioClip[] GreetingClips;
	public AudioClip[] BlowedOnClips;
	public AudioClip[] JustFloatingClips;
	public AudioClip[] nearDeathClips;
	public AudioClip[] AboutToPopClips;
	public AudioClip[] PopClips;
	public AudioClip[] GameOverReactionClips;

}
