using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class FadeIn : MonoBehaviour,IPointerClickHandler {
    byte MyAlpha = 0;
    // Use this for initialization
    public void OnPointerClick(PointerEventData data)
    {
        Debug.Log("click");
        SceneManager.LoadScene("MainScreen");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (MyAlpha <= 254)
        {
            MyAlpha += 3;
            Debug.Log(MyAlpha);
            gameObject.GetComponent<RawImage>().color = new Color32(255, 255, 255, MyAlpha);
        }
        if (MyAlpha == 255)
        { 
            StartCoroutine(Timeout());//starts on spawn and calls the coroutine
        }
    }
    private IEnumerator Timeout()
    {
        yield return new WaitForSeconds(3.0f);//waits 7 seconds here this can be changed to match what makes sense
        SceneManager.LoadScene("MainScreen");
    }
}
	

