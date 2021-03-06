﻿using UnityEngine;
using System;
using FMODUnity;
using UnityEngine.Assertions;

public class QuestionAudio
{
    // Fmod Instance for this GameObject
    FMODUnity.StudioEventEmitter m_FMODInstance;

    public QuestionAudio(FMODUnity.StudioEventEmitter _emitter)
    {
        Assert.IsNotNull(_emitter);
        m_FMODInstance = _emitter;
    }

    public void PlayNewQuestion(int _questionID, e_rating _rating)
    {
        CheckIfDone();

        string newEvent = "Q" + _questionID + ParseContext(_rating);

        m_FMODInstance.Event = "event:/Dialogue/Interviewer/Questions/" +
            newEvent;
        LookUpAndPlay("question Q" + _questionID + ParseContext(_rating));
        
    }

    /// <summary>
    /// Is audio event done playing?
    /// </summary>
    /// <returns>if done</returns>
    public bool IsDonePlaying() => !m_FMODInstance.IsPlaying();

    /// <summary>
    /// Play the event
    /// </summary>
    public void PlayAudio()
    {
        CheckIfDone();
        m_FMODInstance.Play();
    }

    /// <summary>
    /// Stop the event
    /// </summary>
    public void StopAudio() => m_FMODInstance.Stop();

    /// <summary>
    /// Play a specific event from the questions directory
    /// </summary>
    /// <param name="_event">the events name</param>
    public void PlayEventFromQuestions(string _event)
    {
        if (!IsDonePlaying())
        {
            Debug.LogWarning("Audio was playing when this was called");
        }
        m_FMODInstance.Event = "event:/Dialogue/Interviewer/Questions/"
            + _event;

        LookUpAndPlay("event " + _event);
    }

    /// <summary>
    /// Play the response to a player question
    /// </summary>
    /// <param name="ID"></param>
    public void PlayResponseToPlayerQuestion(int _ID)
    {
        CheckIfDone();

        if (_ID != 0)
        {
            m_FMODInstance.Event = "event:/Dialogue/Interviewer/Answers/answer_"
                + _ID;
        }
        else
        {
            m_FMODInstance.Event = "event:/Dialogue/Interviewer/Questions" +
                "/nothing_ok_then";
        }
        LookUpAndPlay("player response " + _ID);
        
    }

    /// <summary>
    /// Function to parse the audio context
    /// </summary>
    /// <param name="_context">the past rating</param>
    /// <returns>a char that corrosponds</returns>
    char ParseContext(e_rating _context)
    {
        switch (_context)
        {
            case e_rating.NONE:
                return 'A';
            case e_rating.GREAT:
                return 'B';
            case e_rating.GOOD:
                return 'C';
            case e_rating.OK:
                return 'D';
            case e_rating.BAD:
                return 'E';
            case e_rating.AWFUL:
                return 'F';
        }

        throw new Exception("An illegal value has been passed to the " +
            "context parser");
    }

    public void PlayIntro(int _line)
    {
        CheckIfDone();

        m_FMODInstance.Event = "event:/Dialogue/Interviewer/Extras/intro_" +
            _line;

        Debug.Log("Playing intro line: " + _line);

        LookUpAndPlay("intro line  " + _line);  
    }

    public void PlayOutro(int _line)
    {
        CheckIfDone();

        m_FMODInstance.Event = "event:/Dialogue/Interviewer/Extras/outro" +
            _line;

        Debug.Log("Playing outro line: " + _line);

        LookUpAndPlay("outro line " + _line);        
    }

    void CheckIfDone()
    {
        if (PauseMenu.IsPaused())
        {

        }
        if (!IsDonePlaying())
        {
            Debug.LogWarning("Audio was playing when this was called");
            StopAudio();
        }
    }

    void LookUpAndPlay(string _audioText)
    {
        try
        {
            m_FMODInstance.Lookup();
            PlayAudio();
        }
        catch (EventNotFoundException)
        {
            Debug.Log("Requested Audio event: " + _audioText +
                " Could not be found");
        }
    }
}
