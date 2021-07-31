//Player controller the shipships movement. Using both user input and gravity fields
//V4 Louie

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour//, IPointerClickHandler
{
    Rigidbody2D player_rgb;
    Rigidbody2D[] allplanets_rgb;
    GameObject[] allplanets_obj;
   // GameObject[] allmissiles_obj;
    int tilt_int; // tilt of the spaceship      2: points upward at a 40 angle
                  //                            1: Points upward at a 20 angle
                  //                            0: Points right
                  //                            -1: Points down at a 20 angle
                  //                            -2: Points downward at a 40 angle

    int planetlocked_int; //This is the planet that the ship is currently orbiting around.
    private int numofplanets_int; //Number of planets
    private float Gravity_flt;
    private bool paused_bol;
   // private int maxmissiles_int; //maximum allow amount of missiles currently flying through space (NOT AMMO)
    private int nogravnum_int; //the planet that was just "unlocked"
    private int speedlimit_int;
    AudioSource boost_aud;
    private bool boosting_bol;

    public void setspeedlimit(int speed)
    {
        speedlimit_int = speed;
    }
    //can be called by user
    //Adds a force in the direction of the ship
    public void Boost()
    {
        Boost(200);
        /*Vector2 vect;

        if (!paused_bol)
        {
            vect.x = 0;
            vect.y = 200 * Mathf.Sin(tilt_int * 20 / 57.33f);//dividing by 57.33 turns degrees into radians
            player_rgb.AddForce(vect);
            allplanets_obj[0].GetComponent<Planet>().Boost(tilt_int * 20, 200);
            boost_aud.Play();
        }*/
    }
    public void SetBoostflag(bool boosting)
    {
        boosting_bol = boosting;
    }

    public void Boost(int power)
    {
        Vector2 vect;

        if (!paused_bol)
        {
            vect.x = 0;
            vect.y = power * Mathf.Sin(tilt_int * 20 / 57.33f);//dividing by 57.33 turns degrees into radians
            player_rgb.AddForce(vect);
            allplanets_obj[0].GetComponent<Planet>().Boost(tilt_int * 20, power);
            boost_aud.Play();
        }
    }

    //this can be used in place of Boost
    //turns off the gravity for the current planet being orbitted around
    public void unlockplanet()
    {
        nogravnum_int = planetlocked_int;
        stopgravity();
       // Debug.Log("Velocity: " + allplanets_rgb[planetlocked_int - 1].velocity);
       // Boost(-50);
    }

    //turns off the gravitational field
    public void stopgravity()
    {
        Gravity_flt = 0;
        allplanets_obj[planetlocked_int - 1].GetComponent<Planet>().stopgravity();
    }

    //turns on the gravitational field
    public void startgravity()
    {
        Gravity_flt = 1;
        allplanets_obj[planetlocked_int - 1].GetComponent<Planet>().startgravity();
    }

   /* //called by user input
    public void Fire()
    {
        int emptyspot = 1;
        Missile current_msl;

        //finds first empty spot in the array
        for (; emptyspot < maxmissiles_int; emptyspot++)
        {
            if (allmissiles_obj[emptyspot] == null)
                break;
        }

        //if emptyspot equals max missiles then the array is full, don't pass go, don't collect 200 dollars
        if (emptyspot < maxmissiles_int)
        {
            //creats a new missile
            allmissiles_obj[emptyspot] = Instantiate<GameObject>(allmissiles_obj[0]);

            //calls firemissile command from within the missile
            current_msl = allmissiles_obj[emptyspot].GetComponent<Missile>();
            current_msl.FireMissile(player_rgb.position.x+1.2f, player_rgb.position.y+tilt_int/2, tilt_int * 20);//add 1.2 to ensure infront of ship, #toolazytocomputeactualnoseofship
            Debug.Log("Phew phew");
        }
    }*/

    private void settilt(int newtilt_int)
    {
        tilt_int = newtilt_int;
        player_rgb.MoveRotation(tilt_int * 20);
    }

    //pauses the game. Used for when player dies
    public void stopgame()
    {
        Vector2 vect;

        vect.x = 0;
        vect.y = 0;
        player_rgb.velocity = vect;
        Gravity_flt = 0;
        paused_bol = true;
    }

    //resumes the game from a pause
    public void startgame(bool reset,int newlocked_int)
    {
        Vector2 vect;

        if (reset)
        {
            //reset the player to starting position
            vect.x = -4.388f;
            vect.y = 2.58F;
            player_rgb.MovePosition(vect);

            //stops the rotation of the player and sets the tilt to 0
            player_rgb.freezeRotation = true;
            player_rgb.rotation = 0;
            player_rgb.freezeRotation = false;
            settilt(0);
            boosting_bol = false;

            //zero's out any velocitys that may be been received from a crash
            vect.x = 0;
            vect.y = 0;
            player_rgb.velocity = vect;

            planetlocked_int = newlocked_int;
        }
        Gravity_flt = 1;
        paused_bol = false;
    }

    // Use this for initialization
    void Start ()
    {
        player_rgb = GetComponent<Rigidbody2D>();
        settilt(0);
        Gravity_flt = 1;
        paused_bol = false;
        planetlocked_int = 1;
        nogravnum_int = 0; //zero means all gravity is working properly
        speedlimit_int = 20; //5?
        boost_aud = GetComponent<AudioSource>();
        boosting_bol = false;

        //init missiles
        // maxmissiles_int = 6;//6 gives us 5 missiles since index 0 is reserved for the primary missile off screen
        // allmissiles_obj = new GameObject[6];
        // allmissiles_obj[0] = GameObject.Find("Missile");

        //init planets
        numofplanets_int = 10;
        allplanets_rgb = new Rigidbody2D[10];
        for (int i = 1; i <= numofplanets_int; i++)
        {
            allplanets_rgb[i-1] = GameObject.Find("Planet" + i).GetComponent<Rigidbody2D>();
        }
        allplanets_obj = new GameObject[10];
        for (int i = 1; i <= numofplanets_int; i++)
        {
            allplanets_obj[i-1] = GameObject.Find("Planet" + i);
        }
    }

    //Vector direction stays the same
    //Vector magnitude changes to "mag" value
    Vector2 SetMag(Vector2 vect,float mag)
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

    //checks to see if the planetlocked is still the closest
    //switchs to next planet at the midpoint
    void checkplanet()
    {
        Rigidbody2D current_rgb = allplanets_rgb[planetlocked_int-1]; //planet that the ship is currently orbiting around
        Rigidbody2D next_rgb;   //rigidbody of the next planet
        int next_int;   //will be new planetlocked index if switching
        float distocurrent_flt; //distance along x-axis
        float distonext_flt;
        Planet planet_plt;

        //find out which planet is next to be considered for gravity hand off
        if (planetlocked_int < numofplanets_int) next_int = planetlocked_int + 1;
        else next_int = 1;
        next_rgb = allplanets_rgb[next_int-1];

        //find distance to both the current planet, and the next one in line.
        distocurrent_flt = player_rgb.position.x - current_rgb.position.x;
        distonext_flt = player_rgb.position.x - next_rgb.position.x;
        if (distocurrent_flt < 0) distocurrent_flt = distocurrent_flt * -1; //ensures all distances are positive
        if (distonext_flt < 0) distonext_flt = distonext_flt * -1;

        //if next planet is closer along the x-axis, then change the planetlocked variable in all objects
        if (distonext_flt < distocurrent_flt)
        {
           // Debug.Log("Planet locked " + planetlocked_int);
            planetlocked_int = next_int;
            for (int i = 1; i <= numofplanets_int; i++)
            {
                planet_plt = allplanets_obj[i-1].GetComponent<Planet>();
                planet_plt.planetlocked_int = planetlocked_int;
               // Debug.Log("Planet "+ i+ " velocity " + allplanets_rgb[i-1].velocity);
            }
        }
    }

    // Update is called once per frame
    // Used to simulate gravity
    void Update()
    {
        PointerEventData data1 = null;
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            MoveUp();
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            MoveDown();
        if (Input.GetKeyDown(KeyCode.Z))
            GameObject.Find("Boost").GetComponent<Boost>().OnPointerClick(data1);// Boost();
        if (Input.GetKeyDown(KeyCode.X))
            GameObject.Find("Fire").GetComponent<Blaster>().OnPointerClick(data1);
    }

    void FixedUpdate()
    { 
        //Debug.Log("Input Update: " + this.name);
        Vector2 vect = player_rgb.velocity; //Gravity Vector between ship and planet
        Vector2 planetvect; //used to pass vector to planet

        //calc gravity for all 10 planets
        for (int i = 0; i < numofplanets_int - 1; i++)
        {
            //Set's direction of the vector towards the planet
            vect.x = allplanets_rgb[i].position.x - player_rgb.position.x;
            vect.y = allplanets_rgb[i].position.y - player_rgb.position.y;

            //Sets Magnitude of the vector to be "1". Make sure this value is the same as the value in Planet Class
            vect = SetMag(vect, allplanets_obj[i].GetComponent<Planet>().Gravity_flt);
           // Debug.Log("Grav: " + allplanets_obj[i].GetComponent<Planet>().Gravity_flt);
            vect.x = 0; //Gravity along the x-axis is acheived by moving all other objects the oppisite direction

            //Applies gravity
            player_rgb.AddForce(vect);
        }

        //gets the current velocity vector of the player for speed control
        vect.y = player_rgb.velocity.y;
        vect.x = allplanets_rgb[0].velocity.x;
        
        //Applies the speed limit
        if (vect.magnitude > speedlimit_int )//No limit!!!
        {
            //standardizes the vect with a constant magnitude, and divides forces
            vect = SetMag(vect, 50);
            planetvect.x = vect.x;
            planetvect.y = 0;
            vect.x = 0;

            //applies the force vector in the oppisite direction
            player_rgb.AddForce(-vect);
            allplanets_rgb[0].AddForce(-planetvect);
           // Debug.Log("Velocity : "+ allplanets_rgb[planetlocked_int - 1].velocity.x +"* *********Slow Down!********");
        }

        //Changes which planet's gravity field the ship is in. Switchs to next planet at halfway point.
        checkplanet();

        //The player is under the influence of a new planet, turn gravity back on
        //used with "unlockplanet" function
        if (nogravnum_int != planetlocked_int && nogravnum_int != 0)
        {
            nogravnum_int = 0;
            startgravity();
        }
    }

    //public void OnPointerClick(PointerEventData data)
    //{
    //    Debug.Log("OnPointerClick: " + this.name)
    //}

    public void MoveUp()
    {
        if (!paused_bol)
        {
            Move('u');
            //boost_aud.Play();
        }
    }

    public void MoveDown()
    {
        if (!paused_bol)
        {
            Move('d');
            //boost_aud.Play();
        }
    }

    //sets new velocity based on current tilt
    //tilt 2: velocity increase by 5
    //tilt 1: velocity increase by 1
    //tilt 0: no increase
    public void Move(char dir_chr)
    {
        //Debug.Log("Input Move: " + this.name);
        Vector2 vect = player_rgb.velocity;

        //This will increase the veloctiy upward
        if (dir_chr == 'u' && tilt_int < 3)//**Change back to 2
        {
            //computes the new velocity based on current tilt
          //  if (tilt_int == 1 || tilt_int == -2)
                vect.y = vect.y +1;
          //  else if (tilt_int == -1 || tilt_int == 0)
          //      vect.y = vect.y + 1;

            player_rgb.velocity = vect;     //pushs the new velocity into the actual spaceship
            settilt(tilt_int + 1);        //records the new tilt position
            
        }
        //This will increase the velocity downward
        else if (dir_chr == 'd' && tilt_int>-3) //**Change back to 2
        {
            //computes the new velocity based on current tilt
            //if (tilt_int == 2 || tilt_int == -1)
                vect.y = vect.y - 1;
            //else if (tilt_int == 1 || tilt_int == 0)
            //    vect.y = vect.y - 1;

            player_rgb.velocity = vect;     //pushs the new velocity into the actual spaceship 
            settilt(tilt_int - 1);        //records the new tilt position
        }
    }
}
