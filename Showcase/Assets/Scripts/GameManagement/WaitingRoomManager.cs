﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaitingRoomManager : MonoBehaviour
{
    [SerializeField]
    private float m_waitTimer;
    [SerializeField]
    private float m_hourStart;
    private Vector3 m_rotHour;
    [SerializeField]
    private bool m_chatAvailable;
    [SerializeField]
    private List<DialogSO> m_availableDialogs;

    [Header("UI")]
    [SerializeField]
    private GameObject ig_tinyClockHandle;
    [SerializeField]
    private GameObject ig_normalClockHandle;


    [Header("UI")]
    [SerializeField]
    private GameObject ig_textRepresentation;
    [SerializeField]
    private GameObject ig_textBox;
    [SerializeField]
    private GameObject ig_nextText;


    [Header("DEBUG ONLY")]
    [SerializeField]
    private float m_minutes;
    [SerializeField]
    private float m_seconds;
    [SerializeField]
    private DialogSO m_currentDialog;


    List<DialogSO> m_usedDialogs;
    [SerializeField]
    int m_currentSentence;
    float m_dialogTimer;
    bool m_nextTextShown;
    bool m_activeChat = true;

    protected PlayerController m_playerscript;
    


    // Start is called before the first frame updateS
    void Start()
    {
        m_playerscript = FindObjectOfType<PlayerController>();
        m_playerscript.SetCanPlayerMove(false);
        PickNextDialog();
        m_usedDialogs = new List<DialogSO>();
        m_dialogTimer = Random.Range(5, 15);//random wait timer between 2 dialogs


        m_rotHour = new Vector3(0, 0, (m_hourStart * 360) / 60);

    }

    // Update is called once per frame
    void Update()
    {
   
        if (m_activeChat)
        {
            DialogTick();
        }

        TimerTick();
        TimerInClock();
        StopDialog();
    }


 


    //----------------------------------WAIT TIMER----------------------------------
    /// <summary>
    /// Simple tick for the wait timer
    /// </summary>
    void TimerTick()
    {
        if(m_waitTimer > 0)
        {
            m_waitTimer -= Time.deltaTime;
        }
        else
        {
            if(!m_nextTextShown)
            {
                StartCoroutine(NextText());
                m_nextTextShown = true;
            }
            
            m_playerscript.SetCanPlayerMove(true);
            //end scene
        }
    }

    /// <summary>
    /// Uses the timer value to calculate mins and secs
    /// </summary>
    void TimerInClock()
    {
        m_minutes = Mathf.Floor(m_waitTimer / 60);
        m_seconds = Mathf.RoundToInt(m_waitTimer % 60);

        Vector3 _newRot = new Vector3(0, 90, (m_seconds * 360) / 60);
        ig_tinyClockHandle.transform.rotation = Quaternion.Euler(_newRot);

        Vector3 _newRotH = new Vector3(0, 90, (m_minutes * 360) / 60);
        ig_normalClockHandle.transform.rotation = Quaternion.Euler(m_rotHour+_newRotH);
    }

    IEnumerator NextText()
    {
        ig_nextText.SetActive(true);
        yield return new WaitForSeconds(2);
        ig_nextText.SetActive(false);
    }


    

    //----------------------------------DIALOG CONTROLLER----------------------------------

    /// <summary>
    /// Picks a random dialog to run from the available dialog list
    /// </summary>
    void PickNextDialog()
    {
        if(m_currentDialog == null)
        {
            int r = Random.Range(0, m_availableDialogs.Count);
            m_currentDialog = m_availableDialogs[r];
        }
    }

    /// <summary>
    /// Moves the last used dialog into the used list to prevent repeats
    /// </summary>
    /// <param name="_dialog"></param>
    void RemoveDialog(DialogSO _dialog)
    {
        m_availableDialogs.Remove(_dialog);
        m_usedDialogs.Add(_dialog);
        m_currentDialog = null;

    }

    /// <summary>
    /// Updates the UI representation of the dialog text
    /// </summary>
    /// <param name="_newText"></param>
    void UpdateTextDisplay(string _newText)
    {

        ig_textRepresentation.GetComponent<TextMeshProUGUI>().text = _newText;
        
    }

    /// <summary>
    /// reads the next sentence in the dialog scriptable obj
    /// if the current sentence is above the available sentences in the dialog it removes and resets the dialog
    /// </summary>
    void ReadNextSentence()
    {

        if (m_currentSentence >= m_currentDialog.sentences.Count)
        {
            RemoveDialog(m_currentDialog);
            m_dialogTimer = Random.Range(10, 40);//random wait timer between 2 dialogs
            ig_textBox.SetActive(false);
            m_currentSentence = 0;
            PickNextDialog();
        }
        else
        {
            ig_textBox.SetActive(true);
            UpdateTextDisplay(m_currentDialog.sentences[m_currentSentence].text);
            m_dialogTimer = m_currentDialog.sentences[m_currentSentence].waitTimer;
            m_currentSentence += 1;
        }
    }


    /// <summary>
    /// Simple tick for the dialog timer
    /// </summary>
    void DialogTick()
    {
        if(m_dialogTimer>0)
        {
            m_dialogTimer -= Time.deltaTime;
        }
        else
        {
            ReadNextSentence();
        }
    }


    /// <summary>
    /// Stops the dialog when the timer runs out
    /// </summary>
    void StopDialog()
    {
        if(m_waitTimer <= 0 && m_activeChat)
        {
            Debug.Log("i stoped dialog");
            ig_textBox.SetActive(false);
            m_activeChat = false;
        }
    }


}
