﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerCol : MonoBehaviour
{

    private WaitingRoomManager roomManager;

    private GameManagerScript m_gmScript;
    // Start is called before the first frame update
    void Start()
    {
        roomManager = GameObject.FindGameObjectWithTag("WaitingRoomManager").GetComponent<WaitingRoomManager>();
        m_gmScript = FindObjectOfType<GameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
       if(other.CompareTag("Player"))
        {
            roomManager.DoorClose();
            m_gmScript.SetTaskTrue(4);
            this.gameObject.SetActive(false);
        }
    }

}
