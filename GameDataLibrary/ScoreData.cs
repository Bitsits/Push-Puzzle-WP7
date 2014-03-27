using System;
using System.Collections.Generic;

namespace GameDataLibrary
{
    public class ScoreData
    {
        public int CurrentLevel;
        public int PrevScore;

        public List<int> HighScores;

        string fileName = "score.bin";

        public ScoreData Load()
        {
            ScoreData s = Storage.LoadXml<ScoreData>(fileName);

            if (s == null)
            {
                s = new ScoreData();
                s.LoadDefault();
            }

            return s;
        }

        void LoadDefault()
        {
            CurrentLevel = 0;
            PrevScore = 0;

            HighScores = new List<int>() { 750, 500, 250 };
        }

        public bool SetHighScore(int totalScore)
        {
            if (HighScores.Contains(totalScore)) return true;

            for (int i = 0; i < HighScores.Count; i++)
                if (totalScore > HighScores[i])
                {
                    HighScores.Insert(i, totalScore);
                    HighScores.RemoveAt(HighScores.Count - 1);
                    return true;
                }

            return false;
        }

        public void Save()
        {
            Storage.SaveXml<ScoreData>(fileName, this);
        }
    }
}
