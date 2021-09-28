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

        [SerializeField]
        GameObject m_mapDebugPrefab;

        /// <summary>
        /// The current Map.
        /// </summary>
        public Map TheMap
        {
            private set => m_map = value;
            get
            {
                if (m_map == null)
                    m_map = new Map(m_mapDebugPrefab);
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

        private int m_currentScene = 0;

        [SerializeField]
        private InputActionAsset m_inputActionSet;

        private bool m_instanceOneTime = true;

        private void Start()
        {
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

            if (!m_isTitleScreen)
            {
                GameboiTheme.s_defaultGameboyTheme.ApplyColorTheme(3);

                MapLoaded();
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

        public void NextLevel()
        {
            // Unload current scene.
            // Use this to make sure we don't unload it.
            Scene gameManagerScene = gameObject.scene;

            m_currentScene++;
            if (m_currentScene >= SceneManager.sceneCountInBuildSettings)
            {
                m_currentScene = 0;
            }

            m_isTitleScreen = m_currentScene == 0;

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

        private void MapLoaded()
        {
            TheMap = null;
            SetMusic(m_gameMusic);
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
