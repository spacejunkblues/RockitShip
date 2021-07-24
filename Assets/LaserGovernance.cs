using UnityEngine;
using System.Collections;


public class LaserGovernance : MonoBehaviour {
    //this script is attached to each laser, it starts a clock and self destructs
    

    void Start()
    {
        StartCoroutine(Timeout());//starts on spawn and calls the coroutine
    }
    void OnCollisionEnter2D(Collision2D DeleteMe)//gets the object it hit and destroys it too
    {
       // Destroy(DeleteMe.gameObject);//destroys whatevers hit**********NEEDS AN IF STATEMENT TO MAKE SURE ITS AN ASTEROID************
        Destroy(gameObject);//when hit destroy laser
    }
    void OnTriggerEnter2D(Collider2D other)//gets the object it hit and destroys it too
    {
        // Destroy(DeleteMe.gameObject);//destroys whatevers hit**********NEEDS AN IF STATEMENT TO MAKE SURE ITS AN ASTEROID************
        Debug.Log("laser: " + other.name);
        if (!other.name.Contains("Laser")&&!other.name.Equals("ThePlayer"))
            Destroy(gameObject);//when hit destroy laser
    }
    private IEnumerator Timeout()
    {
        yield return new WaitForSeconds(7.0f);//waits 7 seconds here this can be changed to match what makes sense
        Destroy(gameObject);//this destroys the laser
    }

}
