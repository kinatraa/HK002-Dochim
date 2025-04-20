using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationEvents : MonoBehaviour
{
    public void ResetState()
    {
        int currentTurn = GamePlayManager.Instance.GameTurnController.GetTurn();
        if (currentTurn == 0)
        {
            GamePlayManager.Instance.State = GameState.PlayerTurn;
        }
        else // Opponent's turn
        {
            GamePlayManager.Instance.State = GameState.OpponentTurn;
        }
    }

    public void ToggleCutsceneImage()
    {
        Image _skillImage = GetComponent<Image>();
        if (_skillImage.enabled)
        {
            _skillImage.enabled = false;
        }
        else
        {
            _skillImage.enabled = true;
        }
    }
}
