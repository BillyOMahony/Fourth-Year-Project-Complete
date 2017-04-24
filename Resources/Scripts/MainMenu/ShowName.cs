using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShowName : MonoBehaviour {

    #region private variables

    string _name;

    #endregion


    #region MonoBehaviour methods

    void Update () {
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            _name = PlayerPrefs.GetString("PlayerName");
        }
        GetComponent<Text>().text = "Welcome " + _name;
        PhotonNetwork.playerName = _name + " ";
    }

    #endregion

}
