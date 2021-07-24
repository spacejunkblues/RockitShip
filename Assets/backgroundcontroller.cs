using UnityEngine;
using System.Collections;

public class backgroundcontroller : MonoBehaviour
{
    Rigidbody2D mainplanet_rgb;
    Rigidbody2D background_rgb;

	// Use this for initialization
	void Start ()
    {
        mainplanet_rgb = GameObject.Find("Planet1").GetComponent<Rigidbody2D>();
        background_rgb = GetComponent<Rigidbody2D>();

    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector2 vect;

        vect=mainplanet_rgb.velocity;
        vect.y = 0;
        vect.x = vect.x / 16;

        background_rgb.velocity = vect;
        if (background_rgb.position.x < -26)
            background_rgb.position = new Vector2(31, 0);

	}
}
