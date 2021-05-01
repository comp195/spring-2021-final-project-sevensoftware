using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    public enum PauseState { Play, Pause, Dead };
    public PauseState state = PauseState.Play;
    public GameObject pauseMenu;
    public GameObject endMenu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (state)
            {
                case PauseState.Play:
                    {
                        pause();
                        break;
                    }
                case PauseState.Pause:
                    {
                        resume();
                        break;
                    }
                default: break;
            }
        }
    }

    public void pause()
    {
        state = PauseState.Pause;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
    }

    public void resume()
    {
        state = PauseState.Play;
        pauseMenu.SetActive(false);
        endMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void die()
    {
        state = PauseState.Dead;
        endMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;

        GameObject.Find("ScoreText").GetComponent<UnityEngine.UI.Text>().text = "Score: " + Mathf.Floor(GameObject.Find("PlayerPrefab").transform.position.x).ToString() + " meters";
    }

    public void quit()
    {
        Application.Quit();
    }

    public void restart()
    {
        resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
