//Used to click above and below ship
//V1 Louie

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class screeninput : MonoBehaviour, IPointerClickHandler
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //returns true if mouse location is above the ship location
    private bool clickaboveship(Vector2 cursorpos)
    {
        Vector2 pos;    //position of the ship from camera view
        Vector2 canvpos; //position of the ship from the canvas view

        //get's ships position
        pos = GameObject.Find("ThePlayer").GetComponent<Rigidbody2D>().position;

        //converts that position to the "canvas standard"
        canvpos.x = (pos.x + 10) * Screen.width / 20;
        canvpos.y = (pos.y + 5) * Screen.height / 10;

        if (cursorpos.y > canvpos.y)
            return true;
        else
            return false;
    }

    public void OnPointerClick(PointerEventData data)
    {
        PlayerController ThePlayer_pcl;

        ThePlayer_pcl = GameObject.Find("ThePlayer").GetComponent<PlayerController>();

        if(clickaboveship(data.pressPosition))
            ThePlayer_pcl.MoveUp();
        else
            ThePlayer_pcl.MoveDown();
    }

    public void OnPointerUp(PointerEventData data)
    {
        Debug.Log("up click");
    }

    public void clickdown()
    {
        PlayerController ThePlayer_pcl;

        ThePlayer_pcl = GameObject.Find("ThePlayer").GetComponent<PlayerController>();

        if (clickaboveship(Input.mousePosition))
            ThePlayer_pcl.MoveUpHold();
        else
            ThePlayer_pcl.MoveDownHold();
    }

    public void clickup()
    {
        GameObject.Find("ThePlayer").GetComponent<PlayerController>().StopRotating();
    }
}
