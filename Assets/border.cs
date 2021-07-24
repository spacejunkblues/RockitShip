//kills player if they go off screen
//V1 Louie

using UnityEngine;
using System.Collections;

public class border : MonoBehaviour
{
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        PlanetReset trigger_plr;
        
        //Player has hit this object and is now dead. Game Over man, Game Over
        if (other.name == "ThePlayer")
        {
            trigger_plr = GameObject.Find("ScoreTrigger").GetComponent<PlanetReset>();
            trigger_plr.stopgame();
        }
    }
}
