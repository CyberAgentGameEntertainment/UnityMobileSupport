// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using MobileSupport;
using UnityEngine;
using UnityEngine.Android;

public class AndroidGameView : MonoBehaviour
{
    private void Start()
    {
        // just to test set GameState, parameters are meaningless
        AndroidGame.SetGameState(true, AndroidGameState.Content);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) return;

        // switch game quality based on game mode when application resumed
        Debug.Log($"AndroidGame.GameMode={AndroidGame.GameMode}");
    }
}
