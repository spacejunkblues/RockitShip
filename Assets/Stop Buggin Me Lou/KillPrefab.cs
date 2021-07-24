using UnityEngine;
using System.Collections;


public class KillPrefab : MonoBehaviour
{
    //this script is attached to each laser, it starts a clock and self destructs


    void Start()
    {
        StartCoroutine(Timeout());//starts on spawn and calls the coroutine
    }
    private IEnumerator Timeout()
    {

        yield return new WaitForSeconds(3.0f);//waits 7 seconds here this can be changed to match what makes sense
        Destroy(gameObject);//this destroys the laser
    }
}
