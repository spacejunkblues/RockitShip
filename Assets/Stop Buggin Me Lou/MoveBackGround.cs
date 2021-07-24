using UnityEngine;
using System.Collections;

public class MoveBackGround : MonoBehaviour {
    public float speed = 10;
    float Traverse;
    Vector2 Reset;
    // Use this for initialization
	void Start () {
        Traverse = GameObject.Find("Background").transform.position.x-(GameObject.Find("BackGroundTwo").transform.position.x - GameObject.Find("Background").transform.position.x);
        Reset = GameObject.Find("BackGroundTwo").transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        if (transform.position.x<=Traverse)
        {
            gameObject.transform.position = Reset;
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
	}
}
