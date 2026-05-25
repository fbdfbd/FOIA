using UnityEngine;
using UnityEngine.SceneManagement;

namespace OneMoreSpoon.View.UI
{
    public sealed class TitleView : MonoBehaviour
    {
        public void StartTutorial()
        {
            SceneManager.LoadScene("TutorialScene");
        }

        public void StartGame()
        {
            SceneManager.LoadScene("MainScene");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}