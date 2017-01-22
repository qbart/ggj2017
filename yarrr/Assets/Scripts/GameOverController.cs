using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    void onPlayClicked()
    {
        SceneManager.LoadScene("Game");
    }
}
