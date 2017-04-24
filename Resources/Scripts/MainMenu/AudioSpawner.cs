using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSpawner : MonoBehaviour {

    public AudioSource MainAudio;
    public AudioSource SubmitAudio;

    // Use this for initialization
    void Start () {

		if(GameObject.Find("MainAudio(Clone)") == null)
        {
            Instantiate(MainAudio);
        }
        if(GameObject.Find("SubmitAudio(Clone)") == null)
        {
            SubmitAudio = Instantiate(SubmitAudio);
        }

        DontDestroyOnLoad(SubmitAudio);
	}
	
}
