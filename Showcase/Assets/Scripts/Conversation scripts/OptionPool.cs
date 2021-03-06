﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConversationStore;

/// <summary>
/// Class to store options for use in
/// </summary>
public class OptionPool
{
    OptionData[] m_options;
    InterviewUIPopUp m_popUp;

    public OptionPool(int _size, Vector3 _pos, QuestionManager _manager)
    {
        m_options = new OptionData[_size];

        OptionData temp = Resources.Load<GameObject>("Prefabs/Option")
            .GetComponentInChildren<OptionData>();
        Debug.Assert(temp, "Option was not loaded correctly");

        m_popUp = Object.Instantiate(Resources.Load<GameObject>
            ("Prefabs/InterviewPopUp")).GetComponent<InterviewUIPopUp>();

        OptionData.Register(_manager, m_popUp);

        // loop through and create appropriate amount of options
        Debug.Log("Creating options");
        float yOrigin = 0;
        for (int index = 0; index < _size; index++)
        {
            // assume correct position
            switch (index)
            {
                case 0:
                    _pos = new Vector3(_pos.x, (_pos.y + 1.5f), _pos.z);
                    break;
                default:
                    switch (index % 2)
                    {
                        case 0:
                            _pos = new Vector3(_pos.x, _pos.y - 0.5f, _pos.z);
                            break;
                        default:
                            switch (index)
                            {
                                case 1:
                                    _pos = new Vector3(_pos.x, _pos.y - 0.5f,
                                        _pos.z - 1);
                                    yOrigin = _pos.y;
                                    break;
                                default:
                                    _pos = new Vector3(_pos.x, yOrigin,
                                        _pos.z + 2);
                                    break;
                            }
                            break;
                    }
                    break;
            }

            GameObject hold =
                Object.Instantiate(temp.transform.parent.gameObject);
            hold.transform.position = _pos;
            m_options[index] = hold.GetComponentInChildren<OptionData>();
            Debug.Log("Created option " + (index + 1) + " of " +
                m_options.Length);
        }

        Debug.Log("All options have been created");
    }

    /// <summary>
    /// Set the details of the options
    /// </summary>
    /// <param name="_response">possible user reponses</param>
    /// <param name="_question">possible questions</param>
    public void Set(List<Questionresponse> _response, QuestionData
        _question)
    {
        bool lockedFlag = false;

        for (int index = 0; index < m_options.Length; index++)
        {
            bool locked = !CheckHasFlag(_response[index].unlockCriteria);
            if (locked) { lockedFlag = true; }
            m_options[index].SetLocked(locked);
            m_options[index].SetValue(_response[index], _question.tip);
        }

        if (lockedFlag)
        {
            m_popUp.Display("Some responses are locked due to a " +
                "lack of research prior");
        }
    }

    /// <summary>
    /// Set the details of the options
    /// </summary>
    /// <param name="_questions">questions player can ask</param>
    public void Set(List<PlayerQuestion> _questions)
    {
        bool lockedFlag = false;

        for (int index = 0; index < m_options.Length; index++)
        {
            bool locked = !CheckHasFlag(_questions[index].flag);
            if (locked) { lockedFlag = true; }
            m_options[index].SetLocked(locked);
            Questionresponse temp = new Questionresponse
            {
                rating = e_rating.GREAT,
                response = _questions[index].question
            };
            m_options[index].SetValue(temp, e_tipCategories.NOTASKING,
                index);
        }

        if (lockedFlag)
        {
            m_popUp.Display("Some responses are locked due to a " +
                "lack of research prior");
        }
    }

    /// <summary>
    /// Fade out all of the options
    /// </summary>
    public void TurnOffOptions()
    {
        foreach (OptionData option in m_options)
        {
            if (option.enabled)
            {
                option.StartCoroutine(option.setInactive());
            }
        }
    }

    /// <summary>
    /// Clear out all of the options
    /// </summary>
    /// <returns>yield return null</returns>
    public IEnumerator Clear()
    {
        for (int index = 0; index < m_options.Length; index++)
        {
            Object.Destroy(m_options[index].transform.root.gameObject);
            yield return null;
        }
    }
}
