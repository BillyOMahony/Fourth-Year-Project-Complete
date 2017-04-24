using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scorepanel : Photon.PunBehaviour {

    public Text nameText;
    public Text killsText;
    public Text deathsText;
    public Text scoreText;
    public string team;

    public string Name;
    public int kills;
    public int deaths;
    public float score = 0.0f;

    PhotonView _pv;
    GameObject _gm;
    RectTransform _rt;

    // Use this for initialization
    void Start() {
        _pv = gameObject.GetComponent<PhotonView>();
        Name = _pv.owner.NickName;
        _gm = GameObject.Find("GameManager");

        PhotonNetwork.sendRate = 40;
        PhotonNetwork.sendRateOnSerialize = 20;

        nameText.text = Name;

        _rt = gameObject.GetComponent<RectTransform>();
        _rt.localPosition = new Vector3(0, 0, 0);
        _rt.localRotation = new Quaternion(0, 0, 0, 0);
        _rt.localScale = new Vector3(1, 1, 1);
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (_pv.isMine)
        {
            GetKillsAndDeaths();
        }

        UpdateScore();
    }

    void GetKillsAndDeaths()
    {
        IndividualScore _is = GameObject.Find("IndividualScore").GetComponent<IndividualScore>();
        kills = _is.kills;
        deaths = _is.deaths;
    }

    void UpdateScore()
    {
        killsText.text = "" + kills;
        deathsText.text = "" + deaths;
        if (deaths != 0)
        {
            score = (float)kills / (float)deaths;
        }
        else
        {
            score = kills;
        }
        scoreText.text = score.ToString("0.00");
    }

    [PunRPC]
    void SetTeam(string team)
    {
        this.team = team;

        GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(true);
        if (team == "red")
        {
            gameObject.transform.SetParent(GameObject.Find("Red Team Panel").transform);
        }
        else if (team == "blue")
        {
            gameObject.transform.SetParent(GameObject.Find("Blue Team Panel").transform);
        }
        GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(false);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        _pv = gameObject.GetComponent<PhotonView>();

        if (_pv.isMine && stream.isWriting == true)
        {
            stream.SendNext(Name);
            stream.SendNext(kills);
            stream.SendNext(deaths);
        }
        else
        {
            Name = (string)stream.ReceiveNext();
            kills = (int)stream.ReceiveNext();
            deaths = (int)stream.ReceiveNext();
        }
    }
}
