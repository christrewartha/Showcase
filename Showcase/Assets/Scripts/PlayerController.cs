﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float m_playerSpeed = 3.0f;

    //Camera Variables
    Vector2 m_mouseLook;
    Vector2 m_smoothV;
    Vector3 m_campos;
    float m_mouseSensitivity = 2.0f;
    float m_smoothing = 2.0f;
    public float m_Translation;
    public float m_Straffe;

    static FMODUnity.StudioEventEmitter m_eventEmitter;

    public Camera m_camera;

    //Interactable Object - ref
    //public List<InteractableObjectBase> ig_interactable;

    //Float value used for the timer in setting the coroutine to set the value false
    [SerializeField] float m_viewchangetimer;

    //Bool to handle player movement
    bool m_canmove = true;

    //Bool to hangle player camera movement
    bool m_cancameramove = true;

    //Bool to check if the player is able to interact
    bool m_caninteract = true;

    //Bool to check if the player is in the interview
    bool m_isininterview = false;

    //Bool to check if the player is looking at the interviewer
    public bool m_islookingatinterviewer = false;

    // the currently selected interactible
    InteractableObjectBase m_currentlySelected;

    int m_layerMask;

    //Static value for Players mesh
    public static Mesh m_playermesh;

    //Static value for Players material
    public static Material m_playermaterial;

    static bool m_inPlay;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //m_camera = FindObjectOfType<Camera>(); //Find the camera which is a child of the Player 
        m_camera = this.gameObject.GetComponentInChildren<Camera>();
        m_campos = m_camera.transform.position;
        m_layerMask = LayerMask.GetMask("Interact");

        SetPlayerMeshModel(m_playermesh);
        SetPlayerMaterial(m_playermaterial);

        m_eventEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.IsPaused())
        {
            if (m_canmove == true)
            {
                PlayerMovement();
            }
            CameraMovement();
            OnInteract();
        }
    }

    //Seting up the player movements
    void PlayerMovement()
    {
        m_Translation = Input.GetAxis("Vertical") * m_playerSpeed;
        m_Straffe = Input.GetAxis("Horizontal") * m_playerSpeed;
        m_Translation *= Time.deltaTime;
        m_Straffe *= Time.deltaTime;

        if (m_Translation != 0 || m_Straffe != 0)
        {
            if (!m_eventEmitter.IsPlaying() && m_inPlay)
            {
                m_eventEmitter.Play();
            }
        }
        else
        {
            m_eventEmitter.Stop();
        }

        transform.Translate(m_Straffe, 0, m_Translation);
    }

    //Setting up the camera movement to move with the mouse
    void CameraMovement()
    {
        var m_mouseDirection = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        m_mouseDirection = Vector2.Scale(m_mouseDirection, new Vector2(m_mouseSensitivity * m_smoothing, m_mouseSensitivity * m_smoothing));
        m_smoothV.x = Mathf.Lerp(m_smoothV.x, m_mouseDirection.x, 1f / m_smoothing);
        m_smoothV.y = Mathf.Lerp(m_smoothV.y, m_mouseDirection.y, 1f / m_smoothing);
        m_mouseLook += m_smoothV;
        if (m_isininterview == true)
        {
            m_mouseLook.x = Mathf.Clamp(m_mouseLook.x, -90.0f, 90.0f);
            m_mouseLook.y = Mathf.Clamp(m_mouseLook.y, -45.0f, 90.0f);
           
        }
        else
        {
            m_mouseLook.y = Mathf.Clamp(m_mouseLook.y, -90.0f, 90.0f);
        }
        if (m_cancameramove == true)
        {
            m_camera.transform.localRotation = Quaternion.AngleAxis(-m_mouseLook.y, Vector3.right);
            this.transform.localRotation = Quaternion.AngleAxis(m_mouseLook.x, this.transform.up);
        }
        else
        {
            this.transform.localRotation = Quaternion.AngleAxis(m_mouseLook.x, Vector3.zero);
        }
        
    }

    //Setting up the interaction with left mouse button click
    void OnInteract()
    {
        if (m_caninteract == true)
        {
            RaycastHit m_hit;
            Ray m_ray = m_camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(m_ray, out m_hit, 3.0f, m_layerMask))
            {
                InteractableObjectBase hitObject = m_hit.transform.gameObject
                    .GetComponent<InteractableObjectBase>();

                if (m_currentlySelected == null)
                {
                    m_currentlySelected = hitObject;
                   
                }
                else if (m_currentlySelected != hitObject)
                {
                    m_currentlySelected.GetObjectOutline().enabled = false;
                    m_currentlySelected = hitObject;
                }

                if (hitObject.GetShouldGlow())
                {
                    hitObject.GetObjectOutline().enabled = true;
                }
                m_currentlySelected = hitObject;

                if (hitObject.GetComponent<InterviewerObject>())
                {
                    hitObject.GetComponent<InterviewerObject>().Interact();
                    m_currentlySelected.GetObjectOutline().enabled = false;
                }
               
                if (Input.GetMouseButtonDown(0))
                {
                    m_eventEmitter.Stop();
                    hitObject.GetComponent<InteractableObjectBase>().Interact();
                }
            }
            else
            {
                if (m_currentlySelected)
                {
                    m_currentlySelected.GetObjectOutline().enabled = false;
                    m_currentlySelected = null;
                    if (m_islookingatinterviewer.Equals(true))
                    {
                        StartCoroutine(SetPlayerInterviewViewFalse());
                    }
                }
            }

                /*
                Transform m_objectHit = m_hit.transform;
                //Run through all interactable objects within the game
                for (int i = 0; i < ig_interactable.Count; i++)
                {
                    if (m_objectHit.gameObject == ig_interactable[i].gameObject)
                    {
                        m_currentlySelected = ig_interactable[i];
                        if (ig_interactable[i].GetShouldGlow())
                        {
                           // Debug.Log("Should be glowing fam");
                            ig_interactable[i].GetObjectOutline().enabled = true;
                        }
                        if (Input.GetMouseButtonDown(0))
                        {
                            //Call the specific object interact function
                            ig_interactable[i].Interact();
                            ig_interactable[i].GetObjectOutline()
                                .enabled = false;
                        }
                    }
                }
                */

            /*
            // if we hit nothing, remove the current glow
            else
            {
                if (m_currentlySelected)
                {
                    m_currentlySelected.GetObjectOutline().enabled = false;
                }
            }*/
        }
    }

    IEnumerator SetPlayerInterviewViewFalse()
    {
        Debug.Log("Called");
        yield return new WaitForSeconds(m_viewchangetimer);
        m_islookingatinterviewer = false;
        ConversationStore.LookedAway();
    }

    public void SetPlayerMeshModel(Mesh _mesh)
    {
        m_playermesh = _mesh;
        this.GetComponent<MeshFilter>().mesh = m_playermesh;
    }

    public void SetPlayerMaterial(Material _material)
    {
        m_playermaterial = _material;
        this.GetComponent<MeshRenderer>().material = m_playermaterial;
    }

    public bool SetCanInteract(bool _canInteractbool)
    {
        Debug.Log("CanInteractcalled");
        m_caninteract = _canInteractbool;
        return m_caninteract;
        
    }

    public bool SetCanCameraMove(bool _canCamerabool)
    {
        m_cancameramove = _canCamerabool;
        return m_cancameramove;
    }

    public bool SetCanPlayerMove(bool _canPlayerbool)
    {
        m_canmove = _canPlayerbool;
        return m_canmove;
    }

    public bool SetIsInInterview(bool _isininterview)
    {
        m_isininterview = _isininterview;
        return m_isininterview;
    }

    public static void SetInPlay(bool _value)
    {
        if (!_value && m_eventEmitter != null)
        {
            m_eventEmitter.Stop();   
        }
        m_inPlay = _value;
    }

}
