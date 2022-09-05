using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = .5f;
    [Range(.1f, 3f)]
    public float pitch = 1f;
    [Range(0f, 1f)]
    public float spacialBlend;
    public bool loop = false;

    [HideInInspector]
    public AudioSource source;
}
