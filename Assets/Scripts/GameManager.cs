using PHC.Environment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { private set; get; }

        /// <summary>
        /// The current Map.
        /// </summary>
        public Map TheMap { private set; get; }

        private void Start()
        {
            if (Instance != null)
                throw new Exception("More than one GameManager!");

            Instance = this;
            TheMap = new Map();
        }

    }
}
