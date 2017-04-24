using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAudio : MonoBehaviour {

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        DestroyOnGameStart();
	}

    void DestroyOnGameStart()
    {
        GameObject MM = GameObject.Find("MatchManager");
        if(MM != null)
        {
            Destroy(gameObject);
        }
    }
}
