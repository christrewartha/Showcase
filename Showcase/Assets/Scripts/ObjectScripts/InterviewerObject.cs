﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterviewerObject : InteractableObjectBase
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact()
    {
        m_playerscript.m_islookingatinterviewer = true;
        
    }
}
