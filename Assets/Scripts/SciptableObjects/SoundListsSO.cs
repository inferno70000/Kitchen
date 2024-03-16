using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SoundListsSO : ScriptableObject
{
    public AudioClip[] chop;
    public AudioClip[] delivery_fail;
    public AudioClip[] delivery_success;
    public AudioClip[] footstep;
    public AudioClip[] object_drop;
    public AudioClip[] object_pickup;
    public AudioClip pan_sizzle;
    public AudioClip[] trash;
    public AudioClip[] warning;
}
