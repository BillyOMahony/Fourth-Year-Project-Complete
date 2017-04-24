using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Scoreboard : MonoBehaviour {

    public Text BlueScoreText;
    public Text RedScoreText;

    MatchManager _mm;

    void Start()
    {
        _mm = GameObject.Find("MatchManager").GetComponent<MatchManager>();
    }

	// Update is called once per frame
	void Update () {
        UpdateScoreboard();
	}

    void UpdateScoreboard()
    {
        BlueScoreText.text = "" + _mm.BlueScore;
        RedScoreText.text = "" + _mm.RedScore;
    }
}
