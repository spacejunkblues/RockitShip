using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {

	// Use this for initialization
	void Update () {
        GameObject.Find("Rocket").transform.Rotate(0,0,5*Time.deltaTime);
     //  Debug.Log(GameObject.Find("Rocket").transform.eulerAngles.z+","+Mathf.Sin(GameObject.Find("Rocket").transform.eulerAngles.z)+","+ Mathf.Cos(GameObject.Find("Rocket").transform.eulerAngles.z));	
	}
	

}
