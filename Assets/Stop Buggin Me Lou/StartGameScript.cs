using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class StartGameScript : MonoBehaviour,
    IPointerClickHandler
{
    public GameObject LaserPrefab;
    public GameObject AsteroidPrefab;
    public GameObject ShipPrefab;
    public GameObject UnicornPrefab;
    public int ForcePublic = 100;

    void OnGUI()
    {
        StartTheGame(GUI.Button(new Rect(Screen.width / 2 - Screen.width / 8, Screen.height / 8, Screen.width / 4, Screen.height / 6), "Start Game"));
    }

    private void StartTheGame(bool v)
    {
        if (v == true)
        {
            SceneManager.LoadScene("Scene1");
        }
    }
    public void OnPointerClick(PointerEventData data)
    {
        GameObject PrefabToMove;
        int SelectPrefab = UnityEngine.Random.Range(1, 1000);
        Vector2 KickThisObject = new Vector2();
        float StartingVert = UnityEngine.Random.Range(0, Screen.height);
        KickThisObject.y = data.position.y-StartingVert;
        if (UnityEngine.Random.Range(1, 3) == 2)     
        {
            if (SelectPrefab < 300)
            {
                PrefabToMove = Instantiate(LaserPrefab, new Vector2(0, StartingVert), Quaternion.Euler(0, 0, 0), GameObject.Find("MainMenuCanvas").transform) as GameObject;
            }
            else if (SelectPrefab < 600)
            {
                PrefabToMove = Instantiate(ShipPrefab, new Vector2(0, StartingVert), Quaternion.Euler(0, 0, 0), GameObject.Find("MainMenuCanvas").transform) as GameObject;
            }
            else if (SelectPrefab < 900)
            {
                PrefabToMove = Instantiate(AsteroidPrefab, new Vector2(0, StartingVert), Quaternion.Euler(0, 0, 0), GameObject.Find("MainMenuCanvas").transform) as GameObject;
            }
            else
            {
                PrefabToMove = Instantiate(UnicornPrefab, new Vector2(0, StartingVert), Quaternion.Euler(0, 180, 0), GameObject.Find("MainMenuCanvas").transform) as GameObject;
            }
            KickThisObject.x = data.position.x;
            PrefabToMove.GetComponent<Rigidbody2D>().MoveRotation(Mathf.Atan2(KickThisObject.y, KickThisObject.x)*Mathf.Rad2Deg);
            PrefabToMove.AddComponent<KillPrefab>();
            PrefabToMove.GetComponent<Rigidbody2D>().AddForce(KickThisObject*ForcePublic);

            
        }
        else
        {
            if (SelectPrefab < 300)
            {
                PrefabToMove = Instantiate(LaserPrefab, new Vector2(Screen.width, StartingVert), Quaternion.Euler(0, 180, 0), GameObject.Find("MainMenuCanvas").transform) as GameObject;
            }
            else if (SelectPrefab < 600)
            {
                PrefabToMove = Instantiate(ShipPrefab, new Vector2(Screen.width, StartingVert), Quaternion.Euler(0, 180, 0), GameObject.Find("MainMenuCanvas").transform) as GameObject;
            }
            else if (SelectPrefab < 900)
            {
                PrefabToMove = Instantiate(AsteroidPrefab, new Vector2(Screen.width, StartingVert), Quaternion.Euler(0, 180, 0), GameObject.Find("MainMenuCanvas").transform) as GameObject;
            }
            else 
            {
                PrefabToMove = Instantiate(UnicornPrefab, new Vector2(Screen.width, StartingVert), Quaternion.Euler(0,0, 0), GameObject.Find("MainMenuCanvas").transform) as GameObject;
            }
            KickThisObject.x = data.position.x - Screen.width;
            PrefabToMove.AddComponent<KillPrefab>();
            PrefabToMove.GetComponent<Rigidbody2D>().AddForce(KickThisObject*ForcePublic);
            PrefabToMove.GetComponent<Rigidbody2D>().MoveRotation(Mathf.Atan2(KickThisObject.y, KickThisObject.x) * Mathf.Rad2Deg+180);
        }
        
    }

}
