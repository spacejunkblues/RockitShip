//Score keeping class. Starts, and stops game
//Resets the game
//sets pace for the game
//V4 Louie

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System;

public class PlanetReset : MonoBehaviour
{
    GameObject[] allplanets_obj;
    GameObject ThePlayer_obj;
    Rigidbody2D[] allplanets_rgb;
    private int score_int;
    private int HighScore_int;
    Sprite[] numbers_spr;
    GameObject[] allasteroids_obj;
    private int maxasteroids_int;
    GameObject replay_obj;
    GameObject deathscreen_obj;
    GameObject exit_obj;
    private string lastplanetscored_str;//used to make sure each planet only counts as 1
   // private int planetsonfield_int; //when this number is 0, gravity turns off
    private float delay_flt; //used to create asteroids
    private float progression_flt; //used to create asteroids
    bool dead_bol;
    bool startmessage_bol;
    bool planetmessage_bol;

    // Use this for initialization
    void Start ()
    {
        ThePlayer_obj = GameObject.Find("ThePlayer");

        //checks if this is first play if so makes directory if not sets high score and moves on
        if (Directory.Exists(Application.persistentDataPath + @"\AppDataStorage")==false)
        {
            Debug.Log("am i in");
            Directory.CreateDirectory(Application.persistentDataPath + @"\AppDataStorage");
            File.WriteAllText(Application.persistentDataPath + @"\AppDataStorage\HighScore.txt", "0");
            HighScore_int = 0;
        }
        else
        {
            HighScore_int = Convert.ToInt32(File.ReadAllText(Application.persistentDataPath + @"\AppDataStorage\HighScore.txt"));
        }
        score_int = 0;
        GameObject.Find("High Score").GetComponent<Text>().text = string.Format("{0}", HighScore_int);

        //get rid of death splash screen
        replay_obj = GameObject.Find("Replay");
        deathscreen_obj = GameObject.Find("deathscreen");
        exit_obj = GameObject.Find("Exit");
        replay_obj.SetActive(false);
        deathscreen_obj.SetActive(false);
        exit_obj.SetActive(false);
        dead_bol = false;

        //init numbers
        numbers_spr = new Sprite[10];
        for (int i = 0; i < 10; i++)
        {
            numbers_spr[i] = Resources.Load<Sprite>(""+i);
        }

        //inits planets
        //planetsonfield_int = 0;
        allplanets_obj = new GameObject[10];
        for (int i = 1; i <= 10; i++)
        {
            allplanets_obj[i - 1] = GameObject.Find("Planet" + i);
        }
        allplanets_rgb = new Rigidbody2D[10];
        for (int i = 1; i <= 10; i++)
        {
            allplanets_rgb[i - 1] = GameObject.Find("Planet" + i).GetComponent<Rigidbody2D>();
        }

        //init asteroids
        maxasteroids_int = 20;
        allasteroids_obj = new GameObject[maxasteroids_int];
        allasteroids_obj[0] = GameObject.Find("Asteroid1"); //Prime asteroid is in Zero spot
        progression_flt = 5;    //spawns an asteroid every 30 seconds
        delay_flt = 0;

    }
    
    // Update is called once per frame
    void Update ()
    {
        delay_flt += Time.deltaTime;
        if(delay_flt>progression_flt)   //iniately should create an asteroid after a "progression" amount of seconds.
        {
            CreateAsteroid();
            delay_flt = 0;
        }
    }
    void OnGUI()
    {
        GUI.skin.box.wordWrap = true;
        if (Screen.currentResolution.width <= 800)
            GUI.skin.box.fontSize = 20;
        else if (Screen.currentResolution.width < 1280)
            GUI.skin.box.fontSize = 26;
        else
            GUI.skin.box.fontSize = 36;
        GUI.skin.box.fontSize = Screen.currentResolution.width / 36;

        //Don't die message
        if (startmessage_bol)
        {
            GUI.Box(new Rect(Screen.width / 2 - Screen.width / 4, 3 * Screen.height / 8, Screen.width / 2, Screen.height / 8), "Dodge for Points");
        }

        //Planets have gravity message
        if (planetmessage_bol)
        {
            GUI.Box(new Rect(Screen.width / 2 - Screen.width / 4, 3 * Screen.height / 8, Screen.width / 2, Screen.height / 8), "Don't let the planets suck you in!");
        }

        //This is the Game Over pop up
        if (dead_bol)
        {
            GUI.skin.box.wordWrap = true;
            GUI.skin.button.fontSize = 20;
            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 8, Screen.height / 2, Screen.width / 4, Screen.height / 6), "Replay Game") && Input.touchCount < 2)
                StartGame();
            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 8, Screen.height / 2 + Screen.height / 6 + Screen.height / 24, Screen.width / 4, Screen.height / 6), "Exit Game") && Input.touchCount < 2)
                Application.Quit();//remove for APPLE
        }
    }

    public void StartGame()
    {
        PlayerController player_plc;
        Planet current_plt;
        int firstplanet_int=1;
        int currentplanet_int = 1;
        float lowest_flt= allplanets_rgb[0].position.x;
        int i = 1;
        int newlocked_int = 1;
        AudioSource background_aud;
        progression_flt = 5;    //spawns an asteroid every 30 seconds

        //get rid of death splash screen
        //replay_obj.SetActive(false);
        deathscreen_obj.SetActive(false);
        //exit_obj.SetActive(false);
        dead_bol = false;

        //finds the left most planet and dubs it "first planet"
        for (i = 1; i <= 10; i++)
        {
            if(allplanets_rgb[i-1].position.x<lowest_flt)
            {
                lowest_flt = allplanets_rgb[i - 1].position.x;
                firstplanet_int = i;
                newlocked_int = firstplanet_int;
            }
        }

        //starting at the "first planet", finds which planet will be the left on the screen
        currentplanet_int = firstplanet_int;
        for (i = 1; i <= 10; i++)
        {
            if (allplanets_rgb[currentplanet_int-1].position.x>13)//makes sure planets appear to the right of "planetin"
            {
                newlocked_int = currentplanet_int;
                break;
            }
            if (currentplanet_int == 10) currentplanet_int = 1;
            else currentplanet_int = currentplanet_int + 1;
        }

        //starting at the "first planet", activates each planet, one at a time
        //planets must be activated inorder from left most to right.
        currentplanet_int = firstplanet_int;
        for (i = 1; i <= 10; i++)
        {
            current_plt = allplanets_obj[currentplanet_int - 1].GetComponent<Planet>();
            current_plt.startgame(true, newlocked_int);
            if (currentplanet_int == 10) currentplanet_int = 1;
            else currentplanet_int = currentplanet_int + 1;
        }

        //destroy all asteriods
        for(i=1;i< maxasteroids_int; i++)
        {
            //Destroy(GameObject.Find("Asteroid1(Clone)"));
            Destroy(allasteroids_obj[i]);
        }

        //activates player
        player_plc = ThePlayer_obj.GetComponent<PlayerController>();
        player_plc.startgame(true,newlocked_int);

        //Resets gravity on the field; must be done after player is activated
        //planetsonfield_int = 1;
        //planetout();

        //activates the laser
        GameObject.Find("Fire").GetComponent<Blaster>().turnonlaser();

        //Loads Previous High Score        
        HighScore_int = int.Parse((File.ReadAllText(Application.persistentDataPath + @"\AppDataStorage\HighScore.txt")));
        Debug.Log(HighScore_int);
        
        //resets score to 0
        score_int = 0;
        GameObject.Find("Current Score").GetComponent<Text>().text = string.Format("{0}", score_int);
        GameObject.Find("High Score").GetComponent<Text>().text = string.Format("{0}", HighScore_int);

        //play backgroud music
        background_aud = GameObject.Find("backgroundsound").GetComponent<AudioSource>();
        background_aud.Play();
    }

    public void stopgame()
    {
        AudioSource background_aud;
        PlayerController player_plc;
        Planet current_plt;

        //bring the death screen into view
        //replay_obj.SetActive(true);
        deathscreen_obj.SetActive(true);
        //exit_obj.SetActive(true);
        dead_bol = true;

        //pause the game
        player_plc = ThePlayer_obj.GetComponent<PlayerController>();
        player_plc.stopgame();
        for (int i = 1; i <= 10; i++)
        {
            current_plt = allplanets_obj[i - 1].GetComponent<Planet>();
            current_plt.stopgame();
        }
        GameObject.Find("Fire").GetComponent<Blaster>().turnofflaser();

        //play crash sound
        GetComponent<AudioSource>().Play();

        //pause music
        background_aud = GameObject.Find("backgroundsound").GetComponent<AudioSource>();
        background_aud.Stop();

        //Store High Score
        if (score_int > HighScore_int)
        {
            File.WriteAllText(Application.persistentDataPath + @"\AppDataStorage\HighScore.txt", string.Format("{0}", score_int));
        }
    }

    //creates a single asteroid
    //will appear to the right of the screen on a random y-axis
    private void CreateAsteroid()
    {
        Vector2 vect;
        Rigidbody2D asteroid_rgb;
        int empty_int = maxasteroids_int;

        //create 1 new asteroid
        for (int i = 1; i < maxasteroids_int; i++)
        {
            if (allasteroids_obj[i] == null)
            {
                empty_int = i;
                break;
            }
        }
        //checks to make sure asteroids aren't maxed. And they max at score 90
        //if (empty_int < maxasteroids_int&& UnityEngine.Random.Range(score_int, 100) > 90)  //A01
        if (empty_int < maxasteroids_int)// && UnityEngine.Random.Range(score_int, 200) > 150) //A02
        {
            allasteroids_obj[empty_int] = Instantiate<GameObject>(allasteroids_obj[0]);
            asteroid_rgb = allasteroids_obj[empty_int].GetComponent<Rigidbody2D>();
            vect.x = 15 + 2 * UnityEngine.Random.Range(0, 5);
            vect.y = UnityEngine.Random.Range(-4, 4);
            asteroid_rgb.position = vect;
            //asteroid_rgb.AddTorque(Random.Range(0, 10));
        }
    }

    //planet has gone through a trigger
    public void OnTriggerEnter2D(Collider2D other)
    {
        //some object has passed by the score trigger
        if (this.name == "ScoreTrigger" && !dead_bol)
        {
            //Planet has passed by the Player. Increase score
            if (other.name.Contains("Planet")&&other.name!=lastplanetscored_str||other.name.Contains("Asteroid"))
            {
                lastplanetscored_str = other.name;
                if (score_int < 1000) score_int++;
                else score_int = 0;
                GameObject.Find("Current Score").GetComponent<Text>().text = string.Format("{0}", score_int);

                //Game Progression. Inscreases speed limit
                ThePlayer_obj.GetComponent<PlayerController>().setspeedlimit(score_int/100 + 5);//A03 use 4, others use 5
            }
            if (score_int%100 == 0)
                progression_flt = progression_flt * .8f; //creates more asteriods faster each 100 pionts
           // if (score_int == 200)
           //     progression_flt = progression_flt * .5f;
            // CreateAsteroid();
        }
    }
}