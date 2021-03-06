﻿using UnityEngine;

public class InteractableObjectBase : MonoBehaviour
{
    GameObject ig_Player;

    protected PlayerController m_playerscript;

    protected GameManagerScript m_gmscript;

    Outline m_outline;

    bool m_shouldGlow = true;

    /// <summary>
    /// if the 
    /// </summary>
    [SerializeField] MeshRenderer m_alternateGlowObject;

    // Start is called before the first frame update
    public void Start()
    {
        /*
        ig_GameManager = GameObject.Find("GameManager");
        m_gmscript = ig_GameManager.GetComponent<GameManagerScript>();

        ig_Player = GameObject.Find("Player");
        m_playerscript = ig_Player.GetComponent<PlayerController>();
        */

        m_playerscript = FindObjectOfType<PlayerController>();

        m_gmscript = FindObjectOfType<GameManagerScript>();
        if (m_alternateGlowObject == null)
        {
            m_outline = gameObject.AddComponent<Outline>();
        }
        else
        {
            m_outline = m_alternateGlowObject.gameObject.AddComponent<Outline>();
        }

        Debug.Assert(!m_outline.Equals(null), "object: " + gameObject.name +
            " does not have a specified reference for the object " +
            "to attach the outline object to, provide a reference to the " +
            "'m_alternateGlowObject' variable in the editor");
        m_outline.OutlineColor = Color.blue;
        m_outline.OutlineWidth = 10.0f;
        m_outline.enabled = false;
        gameObject.layer = 8;

        //AddToList();
    }

    /*
    void AddToList()
    {
        m_playerscript.ig_interactable.Add(this);
    }*/

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Interact()
    {
        //TODO - Add code for whatever you want the user to do once they click on an object   
    }

    /// <summary>
    /// Function to return the outline component
    /// </summary>
    /// <returns> the outline component </returns>
    public Outline GetObjectOutline() => m_outline;

    public void SetShouldGlow(bool _newValue) => m_shouldGlow = _newValue;

    public bool GetShouldGlow() => m_shouldGlow;
}
