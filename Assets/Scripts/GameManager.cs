using PHC.Art;
using PHC.Environment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace PHC
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { private set; get; }

        private Map m_map;

        /// <summary>
        /// The current Map.
        /// </summary>
        public Map TheMap
        {
            private set => m_map = value;
            get
            {
                if (m_map == null)
                    m_map = new Map();
                return m_map;
            }
        }

        [SerializeField]
        private AudioSource m_musicAudioSource;

        [SerializeField]
        private AudioClip
            m_mainMenuMusic,
            m_gameMusic;

        [SerializeField]
        private bool m_isTitleScreen = false;

        private bool m_isLoseScreen = false;

        private int m_currentScene = 0;

        [SerializeField]
        private InputActionAsset m_inputActionSet;

        private bool m_instanceOneTime = true;

        private void Start()
        {
            // Make sure there's only one instance of GameManager in the game.
            if (m_instanceOneTime)
            {
                if (Instance != null)
                {
                    Destroy(gameObject);
                    return;
                }
                Instance = this;
                m_instanceOneTime = false;
            }

            if(m_isLoseScreen)
            {
                GameboiTheme.s_defaultGameboyTheme.ApplyColorTheme(4);
                SetMusic(m_mainMenuMusic);
            }
            else if (!m_isTitleScreen)
            {
                GameboiTheme.s_defaultGameboyTheme.ApplyColorTheme(3);

                TheMap = null;
                SetMusic(m_gameMusic);
            }
            else
            {
                GameboiTheme.s_defaultGameboyTheme.ApplyColorTheme(4);

                SetMusic(m_mainMenuMusic);

                InputActionMap actionMap = m_inputActionSet.FindActionMap("Player", true);
                actionMap.FindAction("Use", true).performed += StartPressed;
                actionMap.FindAction("Open Inventory", true).performed += StartPressed;
                actionMap.Enable();
            }
        }

        private void StartPressed(InputAction.CallbackContext obj)
        {
            InputActionMap actionMap = m_inputActionSet.FindActionMap("Player", true);
            actionMap.FindAction("Use", true).performed -= StartPressed;
            actionMap.FindAction("Open Inventory", true).performed -= StartPressed;
            //actionMap.FindAction("Use", true).performed -= StartPressed;
            //actionMap.FindAction("Open Inventory", true).performed -= StartPressed;

            NextLevel();
        }

        public void LoseScreen()
        {
            goToLevel = Time.time + 4;
            goToLevelNum = m_currentScene;
            NextLevel(SceneManager.sceneCountInBuildSettings - 1);
        }

        private float goToLevel;
        private int goToLevelNum = -1;

        private void Update()
        {
            if(goToLevelNum != -1 && Time.time > goToLevel)
            {
                NextLevel(goToLevelNum);
                goToLevel = 0;
                goToLevelNum = -1;
            }
        }

        public void NextLevel(int specificLevel = -1)
        {
            // Unload current scene.
            // Use this to make sure we don't unload it.
            Scene gameManagerScene = gameObject.scene;

            if (specificLevel == -1)
                m_currentScene++;
            else
                m_currentScene = specificLevel;

            if (specificLevel == -1 && m_currentScene >= SceneManager.sceneCountInBuildSettings - 1)
            {
                m_currentScene = 0;
            }

            m_isTitleScreen = m_currentScene == 0;
            m_isLoseScreen = m_currentScene == SceneManager.sceneCountInBuildSettings - 1;

            // Set activity of all GameObjects in this Scene other than the GameManger.
            foreach (var go in gameManagerScene.GetRootGameObjects())
                if (go != gameObject)
                    go.SetActive(m_isTitleScreen);

            Scene newLevel;
            if (m_currentScene != 0)
            {
                SceneManager.LoadScene(m_currentScene, LoadSceneMode.Additive);
                newLevel = SceneManager.GetSceneByBuildIndex(m_currentScene);
            }
            else
                newLevel = gameManagerScene;

            // Unload all scenes that aren't the GameManagersScene
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene s = SceneManager.GetSceneAt(i);
                if (s != newLevel && s != gameManagerScene)
                    SceneManager.UnloadSceneAsync(s);
            }

            Start();
        }

        public void SetMusic(AudioClip track)
        {
            if (m_musicAudioSource != null)
            {
                m_musicAudioSource.clip = track;
                if (track != null)
                    m_musicAudioSource.Play();
            }
        }
    }
}
