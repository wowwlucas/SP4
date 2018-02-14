﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    bool freecam = false;
    bool isPanning = false;
    int cameraMode = 0;
    Vector3 anchorPos;
    private UnitManager unitmanager;
    // Use this for initialization
    void Start () {
        unitmanager = UnitManager.instance;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown("b"))
        {
            cameraMode = 0;
        }
        if (Input.GetKeyDown("n"))
        {
            cameraMode = 1;
        }

        if (!freecam)
        {
            if (Input.GetKeyDown("c"))
            {
                freecam = true;
            }

        }
        if (freecam)
        {
            if (Input.GetKeyDown("v"))
            {
                freecam = false;
                transform.position = new Vector3(unitmanager.GetUnitToDoActions().transform.position.x, transform.position.y, unitmanager.GetUnitToDoActions().transform.position.z);
            }

            if (cameraMode == 0)
            {
                if (Input.GetKey("w"))
                {
                    transform.position += transform.up * 10 * Time.deltaTime;
                }
                if (Input.GetKey("a"))
                {
                    transform.position -= transform.right * 10 * Time.deltaTime;
                }
                if (Input.GetKey("s"))
                {
                    transform.position -= transform.up * 10 * Time.deltaTime;
                }
                if (Input.GetKey("d"))
                {
                    transform.position += transform.right * 10 * Time.deltaTime;
                }
            }

            if(cameraMode == 1)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isPanning = true;
                    anchorPos = Input.mousePosition;

                }

                if(!Input.GetMouseButton(0))
                {
                    isPanning = false;
                }

                if(isPanning)
                {
                    Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - anchorPos);

                    Vector3 move = new Vector3(pos.x * 2, pos.y * 2, 0);
                    transform.Translate(move, Space.Self);
                }
  
            }

        }
        
    }
}