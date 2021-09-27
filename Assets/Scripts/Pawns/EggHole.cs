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

        [Tooltip("The amount of mobs this EggHole will spawn.")]
        public long m_spawnAmount;

        private long m_spawned = 0;

        private float m_lastSpawnTime;

        private float m_waitFor;

        private void Start()
        {
            base.Start();

            // 5 Seconds
            m_waitFor = 5;

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
                GameObject newMonsterGO = Instantiate(m_monsterPrefab);
                Monster monster = newMonsterGO.GetComponent<Monster>();
                monster.Position = Position;

                // Increment the spawn count.
#if !DEBUG
                m_spawned++;
                m_waitFor *= 3;
#endif
            }

            if (m_spawned >= m_spawnAmount)
                enabled = false;
        }
    }
}