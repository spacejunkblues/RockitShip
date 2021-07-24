using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class alpha : MonoBehaviour, IPointerClickHandler
{
    public bool flag_bol;

	// Use this for initialization
	void Start ()
    {
        flag_bol = false;	
	}

    public void OnPointerClick(PointerEventData data)
    {
        if (flag_bol)
            flag_bol = false;
        else
            flag_bol = true;
    }
}
