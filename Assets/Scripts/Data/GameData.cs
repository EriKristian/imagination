using System;
using System.Collections.Generic;

namespace Imagination.Data
{
    [Serializable]
    public class GameData
    {
        public bool hasPlayed = false;
        public bool unlockedFighting = false;
        public bool unlockedRacing = false;
        public bool unlockedPlatformer = false;

        public int fightingLevel = 0;
        public int racingLevel = 0;
        public int platformerLevel = 0;

        public float masterVolume = 1f;
        public float musicVolume = 0.8f;
        public float sfxVolume = 1f;

        public List<string> unlockedArt = new List<string>();
    }
}