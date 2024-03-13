using CSharpLike;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlatformerMicrogame
{
    /// <summary>
    /// A simple controller for switching between UI panels.
    /// </summary>
    public class MainUIController : LikeBehaviour
    {
        public GameObject[] panels;

        public void SetActivePanel(int index)
        {
            for (var i = 0; i < panels.Length; i++)
            {
                var active = i == index;
                var g = panels[i];
                if (g.activeSelf != active) g.SetActive(active);
            }
        }

        void OnEnable()
        {
            SetActivePanel(0);
        }
        /// <summary>
        /// Back to the main scene
        /// </summary>
        void Back2MainScene()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }
    }
}