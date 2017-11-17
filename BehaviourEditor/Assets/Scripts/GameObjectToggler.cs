using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectToggler : MonoBehaviour {

    public GameObject toggleObject;
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("CanvasToggle"))
            toggleObject.SetActive(!toggleObject.activeSelf);  
	}
}
