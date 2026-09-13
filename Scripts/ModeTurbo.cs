using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModeTurbo : MonoBehaviour
{
    private void Update()
    {
        if (Time.timeScale == 0)
            return;

        Time.timeScale = 1;

        if (Keyboard.current.tabKey.isPressed)
        {
            // Accélère le temps qui passe dans le jeu
            Time.timeScale = 10;
        }
        else if (Keyboard.current.pKey.isPressed)
        {
            // Ralenti le temps du jeu
            Time.timeScale = 0.3f;
        }
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }
}