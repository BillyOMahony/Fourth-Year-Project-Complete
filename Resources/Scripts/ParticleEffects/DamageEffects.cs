using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEffects : MonoBehaviour {

    public GameObject DamageOne;
    public GameObject DamageTwo;

    //Percentage of Max health effect activated at
    public float dmgOnePercentage = 0.66f;
    public float dmgTwoPercentage = 0.33f;

    float dmgOne;
    float dmgTwo;
    public float health;

    AudioSource _damageAudio;
    PlayerManager _pm;

    bool mine;

	// Use this for initialization
	void Start () {
        _damageAudio = GetComponent<AudioSource>();
        _pm = transform.parent.GetComponent<PlayerManager>();

        dmgOne = _pm.OriginalHealth * dmgOnePercentage;
        dmgTwo = _pm.OriginalHealth * dmgTwoPercentage;

        mine = _pm.gameObject.GetComponent<PhotonView>().isMine;

    }
	
	// Update is called once per frame
	void Update () {

        UpdateEffects();

	}

    void UpdateEffects()
    {
        health = _pm.Health;
        

        if(health <= dmgOne && health > dmgTwo)
        {
            DamageOne.GetComponent<ParticleSystem>().Play(true);
            DamageTwo.GetComponent<ParticleSystem>().Stop(true);
            _damageAudio.Stop();
        }else if(health <= dmgTwo && health > 0)
        {
            DamageOne.GetComponent<ParticleSystem>().Play(true);
            DamageTwo.GetComponent<ParticleSystem>().Play(true);
            if (mine)
            {
                _damageAudio.Play();
            }
        }
        else
        {
            DamageOne.GetComponent<ParticleSystem>().Stop(true);
            DamageTwo.GetComponent<ParticleSystem>().Stop(true);
            DamageOne.GetComponent<ParticleSystem>().Clear();
            DamageTwo.GetComponent<ParticleSystem>().Clear();
            _damageAudio.Stop();
        }


    } 

}
