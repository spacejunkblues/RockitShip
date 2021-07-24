//Controls Asteroid movement
//v2 Louie

using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour
{
    Rigidbody2D asteroid_rgb;
    Rigidbody2D[] allplanets_rgb;
    public int planetlocked_int; //This is the planet that the ship is currently orbiting around
    private int numofplanets_int; //Number of planets
    private int linenum_int; //this is the asteroids number. 1,2,3, etc
    private Vector2 relvel_vtr;//asteroids velocity relative to the planets
    private int die;    //0 means alive, 1 means start to die, actually dies at 3
    private int drift;  //0 means no drift, 1 means start to drift, stop drifting at 3
    private Vector2 driftforce;//the force used to control the drift. Comes from a near by asteroid after it explodes
    private bool onscreen_bol; //used to let it pass through the "planetin" trigger once
    private float gravity_flt;

    public void setrelvel(float velocity_flt)
    {
        relvel_vtr.x = velocity_flt;
    }
    
    // Use this for initialization
    void Start()
    {
        asteroid_rgb = GetComponent<Rigidbody2D>();
        asteroid_rgb.AddTorque(Random.Range(0, 10));
        planetlocked_int = 1;
        gravity_flt = .01f;     //change gravity here
        relvel_vtr.x = Random.Range(0, 3)+1;    //set starting speed here
        relvel_vtr.y = 0;
        if (name.Contains("Clone")) linenum_int = 2;
        else linenum_int = 1;
        die = 0;
        drift = 0;
        onscreen_bol = false;
        //GameObject.Find(this.name + "/burning").GetComponent<ParticleSystem>().Stop();
        //GameObject.Find(this.name + "/burning tail").GetComponent<ParticleSystem>().Stop();

        //init planets
        numofplanets_int = 10;
        allplanets_rgb = new Rigidbody2D[numofplanets_int];
        for (int i = 1; i <= numofplanets_int; i++)
        {
            allplanets_rgb[i - 1] = GameObject.Find("Planet" + i).GetComponent<Rigidbody2D>();
        }
    }

    private void burningOn()
    {
        ParticleSystem[] kids = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem x in kids)
        {
            x.Play();
        }

    }

    private void burningOff()
    {
        ParticleSystem[] kids = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem x in kids)
        {
            x.Stop();
        }

    }

    //Vector direction stays the same
    //Vector magnitude changes to "mag" value
    Vector2 SetMag(Vector2 vect, float mag)
    {
        float ratio = 0;
        ratio = vect.magnitude / mag;
        vect.x = vect.x / ratio;
        vect.y = vect.y / ratio;
        return vect;
    }

    // Update is called once per frame
    //Sets the asteroid velocity to match the velocity of the gravity locked planet
    void FixedUpdate()
    {
        Vector2 vect;
        float distance=-1;

        //keeps the asteroid moving locked in to the planets relative motion
        if(linenum_int!=1&&drift==0)
            asteroid_rgb.velocity = allplanets_rgb[planetlocked_int - 1].velocity-relvel_vtr;

        //set's planetlocked to closest planet
        vect.x = allplanets_rgb[0].position.x - asteroid_rgb.position.x;
        vect.y = allplanets_rgb[0].position.y - asteroid_rgb.position.y;
        distance = vect.magnitude;
        planetlocked_int = 1;
        for (int i=2;i<=numofplanets_int;i++)
        { 
            vect.x = allplanets_rgb[i - 1].position.x - asteroid_rgb.position.x;
            vect.y = allplanets_rgb[i - 1].position.y - asteroid_rgb.position.y;
            if (distance > vect.magnitude)
            {
                planetlocked_int = i;
                distance = vect.magnitude;
            }
        }

        //Set's direction of the vector towards the planet   A01 A02
        vect.x = allplanets_rgb[planetlocked_int - 1].position.x - asteroid_rgb.position.x;
        vect.y = allplanets_rgb[planetlocked_int - 1].position.y - asteroid_rgb.position.y;

        //if close to the planet asteroid will start to burn
        if (vect.magnitude < 2)
            burningOn();
        else
            burningOff();

        //Set's gravity; change in start function
        vect = SetMag(vect, gravity_flt);
        // vect.x = 0; //Gravity along the x-axis is acheived by moving all other objects the oppisite direction

        //Applies gravity
        if(onscreen_bol)
            relvel_vtr = relvel_vtr - vect;
        //relvel_vtr.x = relvel_vtr.x + vect.x;
        //relvel_vtr.y = relvel_vtr.y - vect.y;
        //asteroid_rgb.AddForce(vect);

        /* //allows asteroid to move when a close by asteroid explodes
         if (drift > 0)
         {
             drift++;

             //gives the asteroid 40 frames to drift
             if (drift == 40)
                 drift = 0;

             //driftforce is from a near by exploded asteroid
             //this will slowly remove the velocity from the asteroid
             asteroid_rgb.AddForce(SetMag(driftforce,-1*driftforce.magnitude/40));
         }*/

        if (drift>0)
        {
            drift++;

            //gives asteroid 3 frames to acclerate from the explosion
            if (drift == 3)
            {
                //save the new vector gotten by the explosion
                relvel_vtr = allplanets_rgb[planetlocked_int - 1].velocity - asteroid_rgb.velocity;
                drift = 0;
            }
        }

        //makes sure new asteroid stays alive long enough to ensure an explosion from the laser
        if (die>0)
        {
            if(die==3)
                Destroy(this.gameObject);
            die++;
        }
    }    

    //Gives the state of the asteroid
    //if it's dead, that means it in the process of exploding
    public bool isDead()
    {
        if (die > 0)
            return true;
        else
            return false;
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log("Made it");
        PlanetReset trigger_plr;

        //This Asteroid has hit the player. Game Over
        if (col.collider.name == "ThePlayer")
        {
            trigger_plr = GameObject.Find("ScoreTrigger").GetComponent<PlanetReset>();
            trigger_plr.stopgame();
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        BoxCollider2D asteroid_box;

        //asteroid is shot down
        if (other.name.Contains("Laser"))
        {
            //asteroid_box = GetComponent<BoxCollider2D>();
            asteroid_box = gameObject.AddComponent<BoxCollider2D>();
            //asteroid_box.size = new Vector2(asteroid_box.size.x * 10, asteroid_box.size.y * 10);
            //Destroy(this.gameObject);
            die = 1;
            asteroid_box.isTrigger = true;
            asteroid_box.size = new Vector2(20, 20);
            Debug.Log("Laser Collide, size: " + asteroid_box.size);
        }

        //asteroid has gone out of bounds, or hit a planet
        if ((other.name == "planetout" || (other.name == "planetin"&&onscreen_bol) || other.name == "Upper" || other.name == "Lower" || other.name.Contains("Planet")) && !isDead())
        {
            Destroy(this.gameObject);
        }

        //asteroid gets to enter, but never leave muhahahha
        if (other.name == "planetin")
            onscreen_bol = true;

        //Asteroid has entered another's death field
        if (other.name.Contains("Asteroid")&&!isDead())
        {
            //Gets vector inbetween this asteroid and the one that just exploded.
            Vector2 vect =  this.GetComponent<Rigidbody2D>().position-other.GetComponent<Rigidbody2D>().position;

            //calculates the force so that more force is given the closer the two are.
            driftforce = SetMag(vect, (3 - vect.magnitude)*150);

            //applies the force
            this.GetComponent<Rigidbody2D>().AddForce(driftforce);

            //Allows the asteroid to "drift".
            //it will not be locked on to the relative planet movement for given length of time.
            drift = 1;
        }
    }
}