using UnityEngine;       
using UnityEngine.UI;       
using System;
using System.Collections.Generic;  
using UnityEngine.Video;

[System.Serializable]
public class VideoScene
{
    public string sceneName;
    public VideoClip videoClip;
    public bool isLooping;
    public bool waitForChoices;
    public float choiceButtonDelay;
    public List<Choice> choices;
    public string autoNextScene;
    public Action onSceneEnter;
    public bool isEndingScene;
}
