using PHC.Environment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PHC.Pawns
{
    public class EggHole : Pawn
    {
        [SerializeField]
        private GameObject m_monsterPrefab;

        private long m_spawnAmount = 1;

        private long m_spawned = 0;

        private float m_lastSpawnTime;

        private float m_waitFor;

        private void Start()
        {
            base.Start();

            // 5 Seconds
            m_waitFor = 5;// + UnityEngine.Random.Range(0f, 40f);

            m_lastSpawnTime = Time.time;
        }

        public void Update()
        {
            // If time has elapsed
            if(Time.time - m_lastSpawnTime >= m_waitFor)
            {
                // Reset timer.
                m_lastSpawnTime = Time.time;

                // Spawn Monster.
                GameObject newMonsterGO = Instantiate(m_monsterPrefab, gameObject.transform);
                Monster monster = newMonsterGO.GetComponent<Monster>();
                monster.Position = Position;

                // Increment the spawn count.
                m_spawned++;
                m_waitFor = 3 + UnityEngine.Random.Range(0f, 5f);
            }

            if (m_spawned >= m_spawnAmount)
                enabled = false;
        }

        public void SpawnAnotherIn(float time)
        {
            // Reset the timer
            if (!enabled)
            {
                m_lastSpawnTime = Time.time;
                m_waitFor = time;
            }

            m_spawnAmount++;
            enabled = true;
        }
    }
}