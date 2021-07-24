using UnityEngine;
using UnityEngine.EventSystems;


public class ExitGameScript : MonoBehaviour,
    IPointerClickHandler
{//MUST REMOVE FOR APPLE


    public void OnPointerClick(PointerEventData data)
    {
        Application.Quit();
    }


}
