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
    PlayerController ThePlayer_plc;
    Image boostcharge_img;
    private float Charge;
    private bool boosting;
    Animator booster_anm;
    AudioSource booster_aud;
    private int boostofftime;

	// Use this for initialization
	void Start ()
    {
        ThePlayer_plc = GameObject.Find("ThePlayer").GetComponent<PlayerController>();
        Charge = 50;
        boostcharge_img = GameObject.Find("BoostCharge").GetComponent<Image>();
        boosting = false;
        booster_anm = ThePlayer_plc.GetComponent<Animator>();
        booster_aud = GetComponent<AudioSource>();
        boostofftime = 0;
    }

    public void OnPointerClick(PointerEventData data)
    {
 //******** DELETE after ALPHA Testing**********************
     //   if (!GameObject.Find("Hold").GetComponent<alpha>().flag_bol)
     //   {
            if (Charge > 50)
            {
                Charge = Charge - 50;
                GameObject.Find("ThePlayer").GetComponent<PlayerController>().Boost(50);
                booster_anm.SetBool("booston", true);
                //boost_aud.Play();
                boostofftime = 0; //starts counting
            }
     //   }
    }
    
    public void BoostOn()
    {
 //******** DELETE after ALPHA Testing**********************
//        if(GameObject.Find("Hold").GetComponent<alpha>().flag_bol)
  //          boosting = true;
        if (Charge > 0)
            booster_aud.Play();
    }

    public void BoostOff()
    {
        boosting = false;
     //   if (GameObject.Find("constantspeed").GetComponent<alpha>().flag_bol)
     //       ThePlayer_plc.gravboostoff();
        ThePlayer_plc.SetBoostflag(false);
        booster_anm.SetBool("booston", false);
        booster_aud.Stop();
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        boostofftime++;
        if(boostofftime > 25) //50 is one second
            booster_anm.SetBool("booston", false);

        //boosting is always false. This is hold code that can enable holding down the booster.
        if (boosting)
        {
            if (Charge > 0)
            {
             //   if (GameObject.Find("constantspeed").GetComponent<alpha>().flag_bol)
               //     ThePlayer_plc.stopgravity();
             //   else
             //   {
                    ThePlayer_plc.Boost(8);//this will change velocity at a rate of 4 per sec
                    ThePlayer_plc.SetBoostflag(true);
             //   }
                booster_anm.SetBool("booston", true);
                Charge -= 100f * Time.deltaTime;  //100 charges a second means you get 1 second of boost
                if (Charge >= 100)
                {
                    //GetComponent<Image>().color = Color.green;
                    boostcharge_img.color = Color.green;
                }
                else if (Charge > 50)
                {
                    //GetComponent<Image>().color = Color.yellow;
                    boostcharge_img.color = Color.yellow;
                }
                else
                {
                    //GetComponent<Image>().color = Color.red;
                    boostcharge_img.color = Color.red;
                }
            }
            else
            {
               // if (GameObject.Find("constantspeed").GetComponent<alpha>().flag_bol)
               //     ThePlayer_plc.gravboostoff();
                ThePlayer_plc.SetBoostflag(false);
                booster_anm.SetBool("booston", false);
                booster_aud.Stop();
            }
        }
        else if (Charge < 100)//This is the incremental charge for the blaster, cannot exceed 100
        {
            Charge += 5f * Time.deltaTime;
            if (Charge >= 100)
            {
                //GetComponent<Image>().color = Color.green;
                boostcharge_img.color = Color.green;
            }
            else if (Charge > 50)
            {
                //GetComponent<Image>().color = Color.yellow;
                boostcharge_img.color = Color.yellow;
            }
            else
            {
                //GetComponent<Image>().color = Color.red;
                boostcharge_img.color = Color.red;
            }

        }

    }
}
