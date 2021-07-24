//Score keeping class. Starts, and stops game
//Resets the game
//sets pace for the game
//V3 Tyson

using UnityEngine;
using System.Collections;

public class PlanetReset : MonoBehaviour
{
    GameObject[] allplanets_obj;
    GameObject ThePlayer_obj;
    Rigidbody2D[] allplanets_rgb;
    private int score_int;
    Sprite[] numbers_spr;
    GameObject[] allasteroids_obj;
    private int maxasteroids_int;
    GameObject replay_obj;
    GameObject deathscreen_obj;
    GameObject exit_obj;
    private string lastplanetscored_str;//used to make sure each planet only counts as 1

    // Use this for initialization
    void Start ()
    {
        ThePlayer_obj = GameObject.Find("ThePlayer");
        score_int = 0;

        //get rid of death splash screen
        replay_obj = GameObject.Find("Replay");
        deathscreen_obj = GameObject.Find("deathscreen");
        exit_obj = GameObject.Find("Exit");
        replay_obj.SetActive(false);
        deathscreen_obj.SetActive(false);
        exit_obj.SetActive(false);

        //init numbers
        numbers_spr = new Sprite[10];
        for (int i = 0; i < 10; i++)
        {
            numbers_spr[i] = Resources.Load<Sprite>(""+i);
        }
        
        //inits planets
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
    }
	
	// Update is called once per frame
	void Update () {
	
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
        SpriteRenderer score_srr;
        AudioSource background_aud;

        //get rid of death splash screen
        replay_obj.SetActive(false);
        deathscreen_obj.SetActive(false);
        exit_obj.SetActive(false);

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
            if (allplanets_rgb[currentplanet_int-1].position.x>=5)
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
            current_plt.startgame(true, newlocked_int);//firstplanet should be pointless, the planet should know it's number
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

        //resets score to 0
        score_int = 0;
        score_srr = GameObject.Find("score1").GetComponent<SpriteRenderer>();
        score_srr.sprite = numbers_spr[0];
        score_srr = GameObject.Find("score2").GetComponent<SpriteRenderer>();
        score_srr.sprite = numbers_spr[0];
        score_srr = GameObject.Find("score3").GetComponent<SpriteRenderer>();
        score_srr.sprite = numbers_spr[0];

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
        replay_obj.SetActive(true);
        deathscreen_obj.SetActive(true);
        exit_obj.SetActive(true);

        //pause the game
        player_plc = ThePlayer_obj.GetComponent<PlayerController>();
        player_plc.stopgame();
        for (int i = 1; i <= 10; i++)
        {
            current_plt = allplanets_obj[i - 1].GetComponent<Planet>();
            current_plt.stopgame();
        }

        //play crash sound
        GetComponent<AudioSource>().Play();

        //pause music
        background_aud = GameObject.Find("backgroundsound").GetComponent<AudioSource>();
        background_aud.Stop();
    }

    //planet has gone through a trigger
    public void OnTriggerEnter2D(Collider2D other)
    {
        Vector2 vect;
        Rigidbody2D asteroid_rgb;
        SpriteRenderer score_srr;
        int temp1_int=0;
        int temp2_int = 0;
        int temp3_int = 0;
        int empty_int = maxasteroids_int;

        //some object has passed by the score trigger
        if (this.name == "ScoreTrigger")
        {
            //Planet has passed by the Player. Increase score
            if (other.name.Contains("Planet")&&other.name!=lastplanetscored_str)
            {
                lastplanetscored_str = other.name;
                if (score_int < 1000) score_int++;
                else score_int = 0;
                temp1_int = score_int / 100;
                score_srr = GameObject.Find("score1").GetComponent<SpriteRenderer>();
                score_srr.sprite = numbers_spr[temp1_int];

                temp2_int = (score_int - temp1_int * 100) / 10;
                score_srr = GameObject.Find("score2").GetComponent<SpriteRenderer>();
                score_srr.sprite = numbers_spr[temp2_int];

                temp3_int = score_int - temp1_int * 100 - temp2_int * 10;
                score_srr = GameObject.Find("score3").GetComponent<SpriteRenderer>();
                score_srr.sprite = numbers_spr[temp3_int];

                //Game Progression. Inscreases speed limit
                ThePlayer_obj.GetComponent<PlayerController>().setspeedlimit(temp1_int + 5);
            }
            //create 1 new asteroid
            for(int i=1;i< maxasteroids_int; i++)
            {
                if (allasteroids_obj[i] == null)
                {
                    empty_int = i;
                    break;
                }
            }
            //checks to make sure asteroids aren't maxed. And they max at score 90
            if (empty_int < maxasteroids_int&& Random.Range(score_int, 100) > 90)
            {
                allasteroids_obj[empty_int] = Instantiate<GameObject>(allasteroids_obj[0]);
                asteroid_rgb = allasteroids_obj[empty_int].GetComponent<Rigidbody2D>();
                vect.x = 15 + 2 * Random.Range(0, 5);
                vect.y = Random.Range(-4, 4);
                asteroid_rgb.position = vect;
                //asteroid_rgb.AddTorque(Random.Range(0, 10));
            }
        }
    }
}