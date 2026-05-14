using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainingControls : MonoBehaviour
{
    [Header("Keybinds")]
    [SerializeField] private KeyCode restartKey = KeyCode.T;
    [SerializeField] private KeyCode menuKey = KeyCode.Escape;

    [Header("Scene Names")]
    [SerializeField] private string trainingSceneName = "TrainingRange";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Update()
    {
        if (Input.GetKeyDown(restartKey))
        {
            Debug.Log("Restart key pressed: " + restartKey);
            SceneManager.LoadScene(trainingSceneName);
        }

        if (Input.GetKeyDown(menuKey))
        {
            Debug.Log("Menu key pressed: " + menuKey);
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}