//Boost Controller
//acts as an intermitter to the button and boost function from player control
//controls the charge and output to the screen of the charge window
//V1 Louie

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using System;

public class Boost : MonoBehaviour, IPointerClickHandler
{
    private int Charge;
    //AudioSource boost_aud;

	// Use this for initialization
	void Start ()
    {
        Charge = 0;
        //boost_aud = GameObject.Find("Boost").GetComponent<AudioSource>();
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (Charge > 50)
        {
            Charge = Charge - 50;
            GameObject.Find("ThePlayer").GetComponent<PlayerController>().Boost();
            //boost_aud.Play();
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (Charge < 100)//This is the incremental charge for the blaster, cannot exceed 100
        {
            Charge++;
            if (Charge == 100)
            {
                GetComponent<Image>().color = Color.green;
            }
            else if (Charge > 50)
            {
                GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                GetComponent<Image>().color = Color.red;
            }

        }

    }
}
