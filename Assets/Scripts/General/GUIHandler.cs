using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIHandler : MonoBehaviour {

    public static GUIHandler instance
    {
        get;
        private set;
    }

    private GameObject flightCanvas;
    private GameObject characterCanvas;

	// Use this for initialization
	void Awake () {
        instance = this;
        flightCanvas = transform.Find("Flight Canvas").gameObject;
        characterCanvas = transform.Find("Character Canvas").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ToggleGUI()
    {
        if (flightCanvas.activeInHierarchy)
        {
            flightCanvas.SetActive(false);
            characterCanvas.SetActive(true); 
        }
        else 
        {
            flightCanvas.SetActive(true);
            characterCanvas.SetActive(false);
        }
           

    }

    public void Hello()
    {
        Debug.Log("Sup");   
    }
}
