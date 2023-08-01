using CSharpLike;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microgame
{
    /// <summary>
    /// A simple controller for switching between UI panels.
    /// </summary>
    public class MainUIController : LikeBehaviour
    {
        public GameObject[] panels;
        void Awake()
        {
            int count = GetInt("count");
            panels = new GameObject[count];
            for (int i = 0; i < count; i++)
                panels[i] = GetGameObject("panels"+i);
        }
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
            Time.timeScale = 1f;//Time scale reset to 1
            SceneManager.LoadScene(0);
        }
    }
}