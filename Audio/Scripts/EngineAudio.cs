using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineAudio : MonoBehaviour {

    public float FullVolumePoint;
    public float MinPitch;
    public float MaxPitch;

    AudioSource _engineAudio;
    PlayerControllerRB _PC;
    float _pitchDiff;

    // Use this for initialization
	void Start () {
        _PC = gameObject.GetComponent<PlayerControllerRB>();
        _engineAudio = GetComponent<AudioSource>();
        _pitchDiff = MaxPitch - MinPitch;
	}
	
	// Update is called once per frame
	void Update () {
        AdjustAudio();
	}

    void AdjustAudio()
    {
        float eng = _PC.engine;
        _engineAudio.pitch = MinPitch + (_pitchDiff * eng);

        if (eng < FullVolumePoint)
        {
            _engineAudio.volume = eng * 5;
        }
        else
        {
            _engineAudio.volume = 1;
        }
        
    }
}
