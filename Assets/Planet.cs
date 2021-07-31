//Controls planet movement
//v4 Louie

using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour
{
    Rigidbody2D planet_rgb;
    Rigidbody2D[] allplanets_rgb;
    Rigidbody2D player_rgb;
    Rigidbody2D prevplanet_rgb; //the planet in front of the current one. Used to keep the order of planets
    public int planetlocked_int; //This is the planet that the ship is currently orbiting around
    public float Gravity_flt;
    private int numofplanets_int; //Number of planets
    private int linenum_int; //this is the planets number. 1,2,3, etc

    //called by player controller
    //Adds a force in the direction of the ship
    public void Boost(int dir,int power)
    {
        Vector2 vect;
        //Debug.Log("made it to "+this.name+ " with a linenum of " + linenum_int);
      //  if(linenum_int==planetlocked_int)
      //  {
      //      Debug.Log("Boosting off " + this.name);
            vect.x = -power * Mathf.Cos(dir / 57.33f);//dividing by 57.33 turns degrees into radians
            vect.y = 0;
            planet_rgb.AddForce(vect);
       // }
    }

    //turns off the gravitational field
    public void stopgravity()
    {
        Gravity_flt = 0;
    }

    //turns on the gravitational field
    public void startgravity()
    {
        Gravity_flt = 1;
    }

    public void stopgame()
    {
        Vector2 vect;
        
        vect.x = 0;
        vect.y = 0;
        planet_rgb.velocity = vect;
        Gravity_flt = 0;
    }

    //controls how planets reset
    //random x,y; image; size
    private void resetplanet()
    {
        Vector2 vect;
        Sprite planet_spr;
        float random_flt;

        //randomly get a new image
        switch(Random.Range(1,4))   //returns random number from 1 to 3, does not include the "4"
        {
            case 1:
                planet_spr = Resources.Load<Sprite>("planetmike_crop");
                break;
            case 2:
                planet_spr = Resources.Load<Sprite>("planetmike1_crop");
                break;
            case 3:
                planet_spr = Resources.Load<Sprite>("planetmike2_crop");
                break;
            default:
                planet_spr = Resources.Load<Sprite>("planetmike_crop");
                break;
        }
        GetComponent<SpriteRenderer>().sprite = planet_spr;

        //randomly set size of planet
        random_flt = Random.Range(.3f, .6f);
        GetComponent<Transform>().localScale = new Vector3(random_flt, random_flt, 1);

        //Set gravity based on size
        Gravity_flt = random_flt * 5;

        //Sets X location of planet
        if (prevplanet_rgb.position.x < 10) vect.x = 11;
        else vect.x = prevplanet_rgb.position.x +  Random.Range(1F, 5F);

        //Set Y location of planet
        //random number from between   -5 to -2.5   or   2.5 to 5
        //Note: Range max is exclusive so Random.Range(0,2) returns 0 or 1
        //     if (Random.Range(0, 2) == 1) vect.y = Random.Range(2.5F, 5F);
        //     else vect.y = -1 * Random.Range(2.5F, 5F);
        vect.y = Random.Range(-5F, 5F);

        //Moves planet to new x,y
        planet_rgb.position = vect;
        //planet_rgb.MovePosition(vect); //don't use to teleport. Use to manually move smoothly
    }

    public void startgame(bool reset,int newlocked_int)
    {
        if (reset)
        {
            //if this changes, also change Planet Trigger
            if (planet_rgb.position.x < 5)
                resetplanet();

            planetlocked_int = newlocked_int;
        }
        Gravity_flt = 1;
    }

    // Use this for initialization
    void Start ()
    {
        Vector2 vect;
        int prevline_int;

        allplanets_rgb = new Rigidbody2D[10];
        numofplanets_int = 10;
        for(int i=1; i<= numofplanets_int; i++)
        {
            allplanets_rgb[i-1] = GameObject.Find("Planet" + i).GetComponent<Rigidbody2D>();
        }
        planet_rgb = GetComponent<Rigidbody2D>();
        player_rgb = GameObject.Find("ThePlayer").GetComponent<Rigidbody2D>();
        vect = planet_rgb.velocity;
        // vect.x = -1;
        planet_rgb.velocity = vect;
        planetlocked_int = 1;
        Gravity_flt = 2;
        switch (name.Substring(6))
        {
            case "1":
                linenum_int = 1;
                break;
            case "2":
                linenum_int = 2;
                break;
            case "3":
                linenum_int = 3;
                break;
            case "4":
                linenum_int = 4;
                break;
            case "5":
                linenum_int = 5;
                break;
            case "6":
                linenum_int = 6;
                break;
            case "7":
                linenum_int = 7;
                break;
            case "8":
                linenum_int = 8;
                break;
            case "9":
                linenum_int = 9;
                break;
            case "10":
                linenum_int = 10;
                break;
            default:
                linenum_int = 0;
                break;
        }
        //if numberinline is 0, the object is not a planet.
        if (linenum_int == 1) prevline_int = numofplanets_int;
        else prevline_int = linenum_int - 1;
        if (linenum_int != 0)
            prevplanet_rgb = GameObject.Find("Planet" + prevline_int).GetComponent<Rigidbody2D>();
    }

    Vector2 SetMag(Vector2 vect, float mag)
    {
        /*
        float ratio = 0;
        ratio= vect.magnitude / mag;
        vect.x = vect.x / ratio;
        vect.y = vect.y / ratio;*/

        float factor = 0;
        float gravity = 0;

        //increases gravity the closer a planet is
        factor = 1 / (vect.magnitude + 1);

        //if the distance to the planet is zero, the factor will be one, mag will equal gravity
        gravity = mag * factor;

        //changes the magnitude to match the new gravity vector
        float ratio = 0;
        ratio = vect.magnitude / gravity;
        vect.x = vect.x / ratio;
        vect.y = vect.y / ratio;

        return vect;
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        //Debug.Log("Planet Update: " + this.name + "Planetlocked " +planetlocked_int);
        Vector2 vect = allplanets_rgb[planetlocked_int-1].velocity; //Gravity Vector between ship and planet

        //calc gravity for all 10 planets
        for (int i = 0; i < numofplanets_int - 1; i++)
        {
            //Set's direction of the vector towards the planet
            vect.x = allplanets_rgb[i].position.x - player_rgb.position.x;
            vect.y = allplanets_rgb[i].position.y - player_rgb.position.y;

            //Sets Magnitude of the vector to be "1". Make sure this value is the same as the value in Planet Class
            vect = SetMag(vect, -Gravity_flt);
            vect.y = 0;

            //adds force to planet locked. If this isn't the planetlocked, then just copy the velocity of the locked planet
            if (linenum_int == 1)
                planet_rgb.AddForce(vect);
            else
                planet_rgb.velocity = allplanets_rgb[0].velocity;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        PlanetReset trigger_plr;

        //Planet has hit the left side and resets
        //if this changes, also change startgame
        if (other.name == "LeftSide")
        {
            resetplanet();
        }
        //Player has hit this object and is now dead. Game Over man, Game Over
        if (other.name == "ThePlayer")
        {
            //Debug.Log(this.name + " just hit " + other.name);
            trigger_plr = GameObject.Find("ScoreTrigger").GetComponent<PlanetReset>();
            trigger_plr.stopgame();
        }
    }

}