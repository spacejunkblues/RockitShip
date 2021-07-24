//attached to the fire button
//creates a laser
//controls the laser charge indications
//V3 Louie

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using UnityEngine.UI;

public class Blaster : MonoBehaviour, IPointerClickHandler
{
    //Here I define my variables, I will need the rocket position and current tilt.  I will also need to know if the charge is good
    float Charge; //This variable will tell me whether or not the charge is sufficient to fire
    float CurrentShipTilt;//This is the current tilt of the ship to use to instantiate the laser
    Vector2 CurrentShipPosition;//this is the current position of the ship to use with the instantiate laser
    public GameObject LaserPrefab;//this is the laser with an attached box collider
    public float ForcePublic;
    Image blastercharge_img;
    bool laseron;   //can activate or deactivate the laser

    void Start()
    {
        blastercharge_img = GameObject.Find("BlasterCharge").GetComponent<Image>();
        laseron = true;
    }

    Vector2 SetMag(Vector2 vect, float mag)
    {
        float ratio = 0;
        ratio = vect.magnitude / mag;
        vect.x = vect.x / ratio;
        vect.y = vect.y / ratio;
        return vect;
    }

    public void turnonlaser()
    {
        laseron = true;
    }

    public void turnofflaser()
    {
        laseron = false;
    }

    public void OnPointerClick(PointerEventData data)
    {
       // Vector2 relmotion_vct; //force multiplier to ensure the laser is speed increases as relative motion increases
        if (Charge>50&&laseron)
        {
            Charge = Charge - 50;
            CurrentShipTilt = GameObject.Find("ThePlayer").transform.eulerAngles.z;//Current Z rotation which is the axis the ship rotates around
            CurrentShipPosition = GameObject.Find("ThePlayer").transform.position;//Current ship position
            //Below makes a game object on top of the blaster of the ship.  The first variable is ship position, then comes the hypotenuse of the distance
            //from ship center to the blaster for center of ship to center of laser, then we add in a base angle due to the offset to be added to any additional
            //angle from the ship and take the components to determine where to spawn the laser.  KEEPING IN MIND THAT COS AND SIN TAKE RADIANS AND NOT DEGREES.
            //The tilt is applied to the laser to match the ship.  This is all assigned to KickTheLaser which is seen below.      
            GameObject KickTheLaser=Instantiate(LaserPrefab, new Vector2(CurrentShipPosition.x+(1.14236246436934f * Mathf.Cos(CurrentShipTilt*Mathf.Deg2Rad+ 0.18042148f)),
                CurrentShipPosition.y+(1.14236246436934f* Mathf.Sin(CurrentShipTilt*Mathf.Deg2Rad+ 0.18042148f))),Quaternion.Euler(0,0,CurrentShipTilt)) as GameObject;
  
//******DELETE this after alpha testing*******
//            if (GameObject.Find("lasertest").GetComponent<alpha>().flag_bol)
  //          {
                //Sets intial velocity to player so that the laser will fire relavitly infront of the ship
                //relmotion_vct = SetMag(GameObject.Find("Planet1").GetComponent<Rigidbody2D>().velocity, ForcePublic);
                KickTheLaser.GetComponent<Rigidbody2D>().velocity = GameObject.Find("ThePlayer").GetComponent<Rigidbody2D>().velocity;

                //This Takes the prefab and gives the force that is then componetized based on its tilt
                KickTheLaser.GetComponent<Rigidbody2D>().AddForce(new Vector2( ForcePublic * Mathf.Cos(CurrentShipTilt * Mathf.Deg2Rad),
                    ForcePublic * Mathf.Sin(CurrentShipTilt * Mathf.Deg2Rad)), ForceMode2D.Impulse);
/*            }
            else
            {
                //Sets intial velocity to player so that the laser will fire relavitly infront of the ship
                relmotion_vct = SetMag(GameObject.Find("Planet1").GetComponent<Rigidbody2D>().velocity, ForcePublic);
                KickTheLaser.GetComponent<Rigidbody2D>().velocity = GameObject.Find("Planet1").GetComponent<Rigidbody2D>().velocity;

                //This Takes the prefab and gives the force that is then componetized based on its tilt
                KickTheLaser.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1 * relmotion_vct.x * ForcePublic * Mathf.Cos(CurrentShipTilt * Mathf.Deg2Rad),
                    -1 * relmotion_vct.x * ForcePublic * Mathf.Sin(CurrentShipTilt * Mathf.Deg2Rad)), ForceMode2D.Impulse);
            }*/

            //play laser noise
            GetComponent<AudioSource>().Play();
            //KickTheLaser.AddComponent<LaserGovernance>(); Don't need this. I added the class to the Prefab
        }
    }

    void FixedUpdate()
    {
        if (Charge < 100)//This is the incremental charge for the blaster, cannot exceed 100
        {
            // Charge++;
            Charge += 25f * Time.deltaTime;//increases at a rate of 25 charges a second. or full charge at 4 seconds
            if(Charge>=100)
            {
                //GetComponent<Image>().color = Color.green;
                blastercharge_img.color=Color.green;
            }
            else if (Charge > 50)
            {
                //GetComponent<Image>().color = Color.yellow;
                blastercharge_img.color = Color.yellow;
            }
            else
            {
                //GetComponent<Image>().color = Color.red;
                blastercharge_img.color = Color.red;
            }

        }
    }
}
