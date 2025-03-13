using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    public Button buttonClick;

    public void ButtonOnClick()
    {
        buttonClick.onClick.Invoke();
    }
}
