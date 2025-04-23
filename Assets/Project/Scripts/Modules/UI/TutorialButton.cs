using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialButton : MonoBehaviour
{
    public void TurnOffTutorial(GameObject tutorial)
    {
        tutorial.SetActive(false);    
    }
}
