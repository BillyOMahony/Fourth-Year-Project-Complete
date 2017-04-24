using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIScoreManager : Photon.PunBehaviour {

    Teams teams;
    MatchManager _MM;

    Transform _blueTeamPanelContainer;
    Transform _redTeamPanelContainer;

    public Transform test, test1;
    public Text test2;

    public Scorepanel[] redTeamPanelChildren;
    public Scorepanel[] blueTeamPanelChildren;

    public int redCount = 0;
    public int blueCount = 0;

	// Use this for initialization
	void Start () {
        teams = GameObject.Find("GameManager").GetComponent<Teams>();

        _blueTeamPanelContainer = gameObject.transform.GetChild(2);
        _redTeamPanelContainer = gameObject.transform.GetChild(1);

        redCount = teams.redCount;
        blueCount = teams.blueCount;

        _MM = GameObject.Find("MatchManager").GetComponent<MatchManager>();
    }
	
	// Update is called once per frame
	void Update () {
        UpdateGUI();
	}

    void UpdateGUI()
    {
        redCount = teams.redCount;
        blueCount = teams.blueCount;
        if(_MM.RedScore > _MM.BlueScore) {
            _redTeamPanelContainer.localPosition = new Vector3(0, -40, 0);
            _blueTeamPanelContainer.localPosition = new Vector3(0, -40 - (redCount * 40), 0);
        }else
        {
            _blueTeamPanelContainer.localPosition = new Vector3(0, -40, 0);
            _redTeamPanelContainer.localPosition = new Vector3(0, -40 - (blueCount * 40), 0);
        }
        List<Scorepanel> blueTeamScores = new List<Scorepanel>();
        blueTeamPanelChildren = _blueTeamPanelContainer.gameObject.GetComponentsInChildren<Scorepanel>();
        foreach (Scorepanel child in blueTeamPanelChildren)
        {
            if (child.GetComponent<Scorepanel>() != null)
            {
                blueTeamScores.Add(child);
            }
        }

        List<Scorepanel> redTeamScores = new List<Scorepanel>();
        redTeamPanelChildren = _redTeamPanelContainer.gameObject.GetComponentsInChildren<Scorepanel>();
        foreach (Scorepanel child in redTeamPanelChildren)
        {
            if (child.GetComponent<Scorepanel>() != null)
            {
                redTeamScores.Add(child);
            }
        }

        //Do a quicksort with the lists
        if (blueTeamScores.Count > 1)
        {
            QuickSort(blueTeamScores, 0, blueTeamScores.Count -1);
        }

        if (redTeamScores.Count > 1)
        {
            QuickSort(redTeamScores, 0, redTeamScores.Count -1);
        }

        //Display the lists. Localposition += -40 for each
        int offset = 0;
        int i = 0;
        foreach(Scorepanel score in blueTeamScores)
        {
            i++;
            score.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "" + i;
            score.transform.localPosition = new Vector3(0, offset, 0);
            offset -= 40;
        }

        offset = 0;
        i = 0;
        foreach (Scorepanel score in redTeamScores)
        {
            i++;
            score.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "" + i;
            score.transform.localPosition = new Vector3(0, offset, 0);
            offset -= 40;
        }
    }

    void QuickSort(List<Scorepanel> list, int min, int max)
    {
        int i = min, j = max;
        Scorepanel temp;
        Scorepanel pivot = list[min + max / 2];

        while(i <= j)
        {
           
            while (int.Parse(list[i].transform.GetChild(2).GetComponentInChildren<Text>().text) 
                        > int.Parse(pivot.transform.GetChild(2).GetComponentInChildren<Text>().text))
            {
                i++;
            }

            test1 = list[j].transform.GetChild(2);
            test2 = list[j].transform.GetChild(2).GetComponentInChildren<Text>();


            while (int.Parse(list[j].transform.GetChild(2).GetComponentInChildren<Text>().text)
                        < int.Parse(pivot.transform.GetChild(2).GetComponentInChildren<Text>().text))
            {
                j--;
            }

            if( i <= j)
            {
                temp = list[i];
                list[i] = list[j];
                list[j] = temp;
                i++;
                j--;
            }
        }

        if(min < j)
        {
            QuickSort(list, min, j);
        }
        if(i < max){
            QuickSort(list, i, max);
        }

    }
}
