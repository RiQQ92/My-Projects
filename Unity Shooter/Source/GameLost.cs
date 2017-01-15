using UnityEngine;
using System.Collections;

public class GameLost : MonoBehaviour
{
    public void restartLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void closeGame()
    {
        Application.Quit();
    }
}
