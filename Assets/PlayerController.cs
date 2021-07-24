//Player controller the shipships movement. Using both user input and gravity fields
//V4 Louie

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;// need to use joystick

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
    float tilt_flt; //tilt of ship when hold controls are being used

    int planetlocked_int; //This is the planet that the ship is currently orbiting around.
    private int numofplanets_int; //Number of planets
    private float Gravity_flt;
    private bool paused_bol;
   // private int maxmissiles_int; //maximum allow amount of missiles currently flying through space (NOT AMMO)
    private int speedlimit_int;
    //AudioSource boost_aud;
    private bool rotatingup_bol;
    private bool rotatingdown_bol;
    private float addedvelocity_flt; //used to track how much velocity change is inputed from user
    private int planetsonfield_int; //when this number is 0, gravity turns off
    //   public bool[] planetgravity_bol;       //true if the planet is on screen. then it gives off a gravity field
    private bool boosting_bol;

    // Use this for initialization
    void Start ()
    {
        player_rgb = GetComponent<Rigidbody2D>();
        settilt(0);
        Gravity_flt = 1f;
        paused_bol = false;
        planetlocked_int = 1;
        speedlimit_int = 5;
        //boost_aud = GetComponent<AudioSource>();
        rotatingdown_bol = false;
        rotatingup_bol = false;
        tilt_flt = 0;
        addedvelocity_flt = 0;
        boosting_bol = false;

        //init missiles
        // maxmissiles_int = 6;//6 gives us 5 missiles since index 0 is reserved for the primary missile off screen
        // allmissiles_obj = new GameObject[6];
        // allmissiles_obj[0] = GameObject.Find("Missile");

        //init planets
        numofplanets_int = 10;
        allplanets_rgb = new Rigidbody2D[10];
        planetsonfield_int = 0;
        for (int i = 1; i <= numofplanets_int; i++)
        {
            allplanets_rgb[i-1] = GameObject.Find("Planet" + i).GetComponent<Rigidbody2D>();
        }
        allplanets_obj = new GameObject[10];
        for (int i = 1; i <= numofplanets_int; i++)
        {
            allplanets_obj[i-1] = GameObject.Find("Planet" + i);
        }
        /*planetgravity_bol = new bool[10];
        for (int i = 1; i <= numofplanets_int; i++)// used for A03
        {
            planetgravity_bol[i-1] = false;
        }*/
        Boost();//to start game
        GameObject.Find("Hold").GetComponent<alpha>().flag_bol = true;
    }

    //turns off "boost" when constant speed is being used
    public void gravboostoff()
    {
        if (Gravity_flt==0&& planetsonfield_int > 0)
            startgravity();
    }

    //a planet has entered the field
    public void planetin()
    {
        if (planetsonfield_int == 0)
            startgravity();
        planetsonfield_int++;
        Debug.Log("Num: " + planetsonfield_int);
    }

    //a planet has left
    public void planetout()
    {
        planetsonfield_int--;
        if (planetsonfield_int == 0)
            stopgravity();
        Debug.Log("Num: " + planetsonfield_int);
    }

    public void setspeedlimit(int speed)
    {
        speedlimit_int = speed;
    }
    //can be called by user
    //Adds a force in the direction of the ship
    public void Boost()
    {
        Vector2 vect;

        if (!paused_bol)
        {
            vect.x = 0;
            vect.y = 50 * Mathf.Sin(tilt_int * 20 / 57.33f);//dividing by 57.33 turns degrees into radians
            player_rgb.AddForce(vect);
            allplanets_obj[planetlocked_int - 1].GetComponent<Planet>().Boost(tilt_int * 20, 200);
            //boost_aud.Play();
        }
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
            //Debug.Log("Boost Player");
            vect.x = 0;
            vect.y = power * Mathf.Sin(tilt_int * 20 / 57.33f);//dividing by 57.33 turns degrees into radians
            player_rgb.AddForce(vect);
            allplanets_obj[planetlocked_int - 1].GetComponent<Planet>().Boost(tilt_int * 20, power);
            //boost_aud.Play();
        }
    }

    public void Turn(float power, bool up)
    {
        Vector2 vect;
        int dir;
        Vector2 forwardvelocity_vct;
        Vector2 displace;
        Vector2 newhdg;
       // int power = (int)power_flt;

        if (up)
            dir = 90;
        else
            dir = -90;

        if (!paused_bol)
        {
            Debug.Log("Turn Player");
            vect.x = 0;
            vect.y = power * Mathf.Sin((tilt_flt+dir) / 57.33f);//dividing by 57.33 turns degrees into radians
            player_rgb.AddForce(vect);
            allplanets_obj[planetlocked_int - 1].GetComponent<Planet>().Boost(tilt_flt + dir, power);
            //boost_aud.Play();

            //get relative forward velocity
            forwardvelocity_vct = new Vector2(-1* allplanets_rgb[planetlocked_int - 1].velocity.x * Mathf.Cos(tilt_flt / 57.33f), player_rgb.velocity.y*Mathf.Sin(tilt_flt/57.33f));
            Debug.Log("Forward vel " + forwardvelocity_vct+ " displace? "+ power * Mathf.Sin((tilt_flt + dir) / 57.33f));
            displace = new Vector2(power * Mathf.Cos((tilt_flt + dir) / 57.33f), power * Mathf.Sin((tilt_flt + dir) / 57.33f));
            Debug.Log("displace " + displace);
            newhdg = forwardvelocity_vct + displace;
            Debug.Log("tilt " + tilt_flt+" new hdg: "+newhdg);
            //tilt_flt = Mathf.Atan2(newhdg.y, newhdg.x)*57.33f;
            if (up)
                tilt_flt += .1f;
            else
                tilt_flt -= .1f;
            Debug.Log("new tilt " + tilt_flt);
            player_rgb.MoveRotation(tilt_flt);
            tilt_int = (int)tilt_flt / 20;
        }

    }

    //turns off the gravitational field
    public void stopgravity()
    {
        Gravity_flt = 0;
        allplanets_obj[planetlocked_int - 1].GetComponent<Planet>().stopgravity();//so that the planet won't pull you back
        Debug.Log("velocity: " + player_rgb.velocity + " and mass: "+player_rgb.mass);
        player_rgb.AddForce(new Vector2(0, -(player_rgb.velocity.y - addedvelocity_flt)*player_rgb.mass/Time.deltaTime));// * Time.deltaTime*2500)); //takes out all veritcal velocity from the player
        //player_rgb.AddForce(new Vector2(0,100));
        Debug.Log("velocity: " + player_rgb.velocity + " and mass: " + player_rgb.mass);
    }

   /* //turns off the gravitational field to a spefic planet used with A03
    public void stopgravity(int planetnum_int)
    {
        planetgravity_bol[planetnum_int - 1] = false;
    }
    //turns off the gravitational field to a spefic planet used with A03
    public void startgravity(int planetnum_int)
    {
        planetgravity_bol[planetnum_int - 1] = true;
    }*/

    //turns on the gravitational field
    public void startgravity()
    {
        Gravity_flt = 1;
        allplanets_obj[planetlocked_int - 1].GetComponent<Planet>().startgravity();
        Debug.Log("Planetlocked: " + planetlocked_int);
    }

    private void settilt(int newtilt_int)
    {
        tilt_int = newtilt_int;
        tilt_flt = tilt_int;
        player_rgb.MoveRotation(tilt_int * 20);
        //Debug.Log("move rotation: " + tilt_int * 20);
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
        rotatingdown_bol = false;
        rotatingup_bol = false;
    }

    //resumes the game from a pause
    public void startgame(bool reset,int newlocked_int)
    {
        Vector2 vect;

        paused_bol = false;
        if (reset)
        {
            //reset the player to starting position
            vect.x = -5f;
            vect.y = 2.58F;
            //player_rgb.MovePosition(vect);
            player_rgb.position = vect;
            Debug.Log("start game");
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
            Debug.Log("player velocity: " + vect+"planetlocked: "+newlocked_int);
            planetlocked_int = newlocked_int;
            addedvelocity_flt = 0;
            Gravity_flt = 1f;
            Boost();//to start game
            //Resets gravity on the field; must be done after player is activated
            planetsonfield_int = 1;
            planetout();
        }
    }

    //Vector direction stays the same
    //Vector magnitude changes to "mag" value
    Vector2 SetMag(Vector2 vect,float mag)
    {
        float ratio = 0;
        if(mag!=0)
            ratio= vect.magnitude / mag;
        if (ratio != 0)
        {
            vect.x = vect.x / ratio;
            vect.y = vect.y / ratio;
        }
        else
            vect = new Vector2(0, 0);

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

        //get planet script to see if planetlocked is in play
        planet_plt = allplanets_obj[planetlocked_int - 1].GetComponent<Planet>();

       // Debug.Log("name " + this.name + " inplay=" + planet_plt.inplay_bol + " planetlocked: Planet" + planetlocked_int);
        //if next planet is closer along the x-axis or current planet has left field, then change the planetlocked variable in all objects
        if (distonext_flt <= distocurrent_flt&&next_rgb.position.x>-10||!planet_plt.inplay_bol)
        {
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

        // Debug.Log("planetlocked: " + planetlocked_int);
        //Get keyboard inputs
        //*******DELETE after Alpha testing**********
        if (GameObject.Find("Hold").GetComponent<alpha>().flag_bol)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                MoveUpHold();
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                MoveDownHold();
            if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
                StopRotatingUp();
            if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.RightArrow))
                StopRotatingDown();
            if (Input.GetKeyDown(KeyCode.Z))
                GameObject.Find("Boost").GetComponent<Boost>().BoostOn();// Boost();
            if (Input.GetKeyUp(KeyCode.Z))
                GameObject.Find("Boost").GetComponent<Boost>().BoostOff();// Boost();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                MoveUp();
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                MoveDown();
            if (Input.GetKeyDown(KeyCode.Z))
                GameObject.Find("Boost").GetComponent<Boost>().OnPointerClick(data1);// Boost();
        }
        if (Input.GetKeyDown(KeyCode.X))
            GameObject.Find("Fire").GetComponent<Blaster>().OnPointerClick(data1);
        /*if (CrossPlatformInputManager.GetAxis("Vertical") > 0)
             MoveUpHold();
         if (CrossPlatformInputManager.GetAxis("Vertical") < 0)
             MoveDownHold();
         if (CrossPlatformInputManager.GetAxis("Vertical") == 0)
             StopRotating();//this is the shitty joystick, save for ref*/

        //set the ship into rotation
        /* if (player_rgb.rotation > 60 || player_rgb.rotation < -60)
             StopRotating();
         if (rotatingup_bol)
             player_rgb.AddTorque(10);
         if (rotatingdown_bol)
             player_rgb.AddTorque(-10);*/

        // Debug.Log("velocity: " + player_rgb.velocity + " and mass: " + player_rgb.mass+" and planetlocked: "+planetlocked_int);
    }

    void FixedUpdate()
    {
        Vector2 vect = player_rgb.velocity; //Gravity Vector between ship and planet
        Vector2 planetvect; //used to pass vector to planet

        //*******DELETE after Alpha testing**********
       /* if (GameObject.Find("Hold").GetComponent<alpha>().flag_bol && (rotatingup_bol || rotatingdown_bol))
        {
            float tilttemp;
            Vector2 vect2;
            vect2 = new Vector2(-1 * allplanets_rgb[planetlocked_int - 1].velocity.x, player_rgb.velocity.y);

            tilttemp = 57.32484076433121f * Mathf.Atan2(vect2.y, vect2.x);
            if (rotatingup_bol && tilt_flt < 60)
            {
                tilt_flt += 30 * Time.deltaTime;
                tilttemp += 2;
                //player_rgb.AddForce(new Vector2(0,6));
                player_rgb.velocity = new Vector2(0, player_rgb.velocity.y + .07f); //.07f for A01 A02   //.1 for A03
                addedvelocity_flt += .07f;
            }
            if (rotatingdown_bol && tilt_flt > -60)
            {
                tilt_flt -= 30 * Time.deltaTime;
                tilttemp -= 2;
                //player_rgb.AddForce(new Vector2(0, -6));
                player_rgb.velocity = new Vector2(0, player_rgb.velocity.y - .07f);
                addedvelocity_flt -= .07f;
            }
            //*******DELETE after Alpha testing**********
            if (GameObject.Find("tiltlocked").GetComponent<alpha>().flag_bol)
                tilt_flt = tilttemp;

            player_rgb.MoveRotation(tilt_flt);
            tilt_int = (int)tilt_flt / 20;

            //player_rgb.velocity = new Vector2(0, vect2.magnitude * Mathf.Sin(tilttemp / 57.32484076433121f));
            //allplanets_rgb[planetlocked_int - 1].velocity = new Vector2(-1 * vect2.magnitude * Mathf.Cos(tilttemp / 57.32484076433121f), 0);
        }*/
             //use with joystick v2
           //  Debug.Log("Start: vertical: " + CrossPlatformInputManager.GetAxis("Vertical")+ " addedvelocity going in" + addedvelocity_flt+" velocity going in "+player_rgb.velocity);
             tilt_flt = CrossPlatformInputManager.GetAxis("Vertical") * 30;
             player_rgb.velocity = new Vector2(0, player_rgb.velocity.y - addedvelocity_flt);
           //  Debug.Log("Non input corrected velocity " + player_rgb.velocity+ " with gravity at: "+Gravity_flt);
             addedvelocity_flt = CrossPlatformInputManager.GetAxis("Vertical") * speedlimit_int * Mathf.Tan(30 / 57.32484076433121f);//7f;//sets the vertical speed, givig a higher "dodging speed" relative to the speedlimit.
            // Debug.Log("New addedvelocity " + addedvelocity_flt);
             player_rgb.velocity = new Vector2(0, player_rgb.velocity.y + addedvelocity_flt);
            // Debug.Log("New Velocity" + player_rgb.velocity);
             player_rgb.MoveRotation(tilt_flt);
             tilt_int = (int)tilt_flt / 20;

        //test
        // tilt_flt = CrossPlatformInputManager.GetAxis("Vertical") * 60;
        //  player_rgb.MoveRotation(tilt_flt);
        // tilt_int = (int)tilt_flt / 20;
        /*if (CrossPlatformInputManager.GetAxis("Vertical") > 0)
            Turn(CrossPlatformInputManager.GetAxis("Vertical")*2, true);
        else if (CrossPlatformInputManager.GetAxis("Vertical") < 0)
            Turn(-1*CrossPlatformInputManager.GetAxis("Vertical")*2, false);
        Boost(2);*/

        //Set's direction of the vector towards the planet   A01 A02
        vect.x = allplanets_rgb[planetlocked_int-1].position.x-player_rgb.position.x ;
        vect.y = allplanets_rgb[planetlocked_int-1].position.y-player_rgb.position.y;

        //Sets Magnitude of the vector to be "1". Make sure this value is the same as the value in Planet Class
        vect =SetMag(vect, Gravity_flt);
        vect.x = 0; //Gravity along the x-axis is acheived by moving all other objects the oppisite direction
        
        //Applies gravity
        player_rgb.AddForce(vect);

       /* //A03
        for (int i = 1; i <= numofplanets_int; i++)
        {
            //Set's direction of the vector towards the planet
            vect.x = allplanets_rgb[i - 1].position.x - player_rgb.position.x;
            vect.y = allplanets_rgb[i - 1].position.y - player_rgb.position.y;

            //Sets Magnitude of the vector to be "1". Make sure this value is the same as the value in Planet Class
            vect = SetMag(vect, Gravity_flt);
            vect.x = 0; //Gravity along the x-axis is acheived by moving all other objects the oppisite direction

            //Applies gravity
            if(planetgravity_bol[i-1])
                player_rgb.AddForce(vect);
        }*/

        //gets the current velocity vector of the player for speed control
        vect.y = player_rgb.velocity.y;
        vect.x = allplanets_rgb[planetlocked_int - 1].velocity.x;
        //Sets a speed limit
        //**********Alpha testing*******************************************
        //if (vect.magnitude > speedlimit_int&&!GameObject.Find("constantspeed").GetComponent<alpha>().flag_bol)
        if (vect.x < -1* speedlimit_int&&!boosting_bol&&!GameObject.Find("constantspeed").GetComponent<alpha>().flag_bol)
        {
            //standardizes the vect with a constant magnitude, and divides forces
            vect = SetMag(vect, 8f);
            planetvect.x = vect.x;
            planetvect.y = 0;
            vect.x = 0;

            //applies the force vector in the oppisite direction
            player_rgb.AddForce(-vect);
            allplanets_rgb[planetlocked_int - 1].AddForce(-planetvect);
            //Debug.Log("Velocity : "+ allplanets_rgb[planetlocked_int - 1].velocity.x +"* *********Slow Down!********");
        }
        else if (vect.x > -1)//Sets a speed minimum by making sure you can't go backwards
        {
            //standardizes the vect with a constant magnitude, and divides forces
            vect = SetMag(vect, 1);
            planetvect.x = vect.x;
            planetvect.y = 0;
            vect.x = 0;

            //applies the force vector in the oppisite direction if moving backwards
            if (planetvect.x > 0)
            {
               //player_rgb.AddForce(vect);
                allplanets_rgb[planetlocked_int - 1].AddForce(-planetvect);
            }
            else
            {
                //Debug.Log("vect :" + vect + " and planet :" + planetvect);
                //player_rgb.AddForce(vect);
                allplanets_rgb[planetlocked_int - 1].AddForce(planetvect);
            }
            //Debug.Log("Velocity : " + allplanets_rgb[planetlocked_int - 1].velocity.x + "* *********Speed Up!********");
        }

        //Changes which planet's gravity field the ship is in. Switchs to next planet at halfway point.
        checkplanet();
    }

    //public void OnPointerClick(PointerEventData data)
    //{
    //    Debug.Log("OnPointerClick: " + this.name)
    //}

    public void MoveUpHold()
    {
        //******** DELETE after ALPHA Testing**********************
        StopRotating();
        if (GameObject.Find("Hold").GetComponent<alpha>().flag_bol&&!paused_bol)
            rotatingup_bol = true;
    }

    public void MoveDownHold()
    {
        //******** DELETE after ALPHA Testing**********************
        StopRotating();
        if (GameObject.Find("Hold").GetComponent<alpha>().flag_bol&&!paused_bol)
            rotatingdown_bol = true;
    }

    public void StopRotating()
    {
        rotatingdown_bol = false;
        rotatingup_bol = false;
        player_rgb.freezeRotation = true;
        player_rgb.freezeRotation = false;
    }
    public void StopRotatingUp()
    {
        rotatingup_bol = false;
    }
    public void StopRotatingDown()
    {
        rotatingdown_bol = false;
    }

    public void MoveUp()
    {
        //******** DELETE after ALPHA Testing**********************
        if (!GameObject.Find("Hold").GetComponent<alpha>().flag_bol)
        {
            if (!paused_bol)
            {
                Move('u');
                //boost_aud.Play();
            }
        }
    }

    public void MoveDown()
    {
        //******** DELETE after ALPHA Testing**********************
        if (!GameObject.Find("Hold").GetComponent<alpha>().flag_bol)
        {
            if (!paused_bol)
            {
                Move('d');
                //boost_aud.Play();
            }
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
        if (dir_chr == 'u' && tilt_int < 2)
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
        else if (dir_chr == 'd' && tilt_int>-2)
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
