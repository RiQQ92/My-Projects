using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static bool gamePaused = true;
    public static bool menuOpen = false;
    private bool wasGamePaused = false;

    private float gameSpeed = 1.0f;


	// Use this for initialization
	void Start ()
    {
        Time.timeScale = 0f;

        MyEvents.AddEventListener(togglePause, MyEventTypes.GAME_TOGGLE_PAUSE_EVENT);
        MyEvents.AddEventListener(pauseGame, MyEventTypes.GAME_PAUSE_EVENT);
        MyEvents.AddEventListener(playGame, MyEventTypes.GAME_PLAY_EVENT);
    }

    public void OpenMenu()
    {
        menuOpen = true;
        wasGamePaused = gamePaused;
        pauseGame();
    }

    public void CloseMenu()
    {
        menuOpen = false;
        if (!wasGamePaused)
            playGame();
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    private void togglePause()
    {
        if (gamePaused)
            playGame();
        else
            pauseGame();
    }

    private void pauseGame()
    {
        gamePaused = true;
        Time.timeScale = 0f;
    }

    private void playGame()
    {
        gamePaused = false;
        Time.timeScale = gameSpeed;
    }
}
