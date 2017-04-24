using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLobbyManager : MonoBehaviour {

    public string Name;
    public int icon = 0;
    public string team = "NA";
    public bool ready = false;

    PhotonView _pv;
    GameManager _gm;
    LobbyManager _lm;
    public int SpawnNumber;

    string SpawnerTeam = "NA";

	// Use this for initialization
	void Start () {
        _pv = GetComponent<PhotonView>();
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        _lm = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();

        transform.SetParent(GameObject.Find("PlayerInstance Container").transform);
        Name = _pv.owner.NickName;
        if (_pv.isMine)
        {
            icon = PlayerPrefs.GetInt("PlayerIcon");
        }

        _lm.UpdateGUI();
	}

    void Update()
    {
        if (_pv.isMine)
        {
            team = _gm.team;
            ready = _gm.ready;
            if(SpawnNumber == 0 && team != "NA" || team != SpawnerTeam)
            {
                GetSpawnNumber();
            }
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting == true && _pv.isMine)
        {
            stream.SendNext(Name);
            stream.SendNext(icon);
            stream.SendNext(team);
            stream.SendNext(ready);
            stream.SendNext(SpawnNumber);
        }
        else
        {
            Name = (string)stream.ReceiveNext();
            icon = (int)stream.ReceiveNext();
            team = (string)stream.ReceiveNext();
            ready = (bool)stream.ReceiveNext();
            SpawnNumber = (int)stream.ReceiveNext();
        }
    }

    void GetSpawnNumber()
    {
        List<int> listOfSpawnPositionsTaken = new List<int>();
        int number = 1;

        Transform[] Children = GameObject.Find("PlayerInstance Container").GetComponentsInChildren<Transform>();
        foreach (Transform child in Children)
        {
            if (child.name != "PlayerInstance Container")
            {
                PlayerLobbyManager plm = child.GetComponent<PlayerLobbyManager>();
                if (plm.team == team)
                {
                    if (plm.SpawnNumber != 0) listOfSpawnPositionsTaken.Add(plm.SpawnNumber);
                }
            }
        }

        while (listOfSpawnPositionsTaken.Contains(number))
        {
            number++;
        }

        SpawnerTeam = team;

        SpawnNumber = number;
        _gm.initialTeamSpawnNum = number;
    }
}
