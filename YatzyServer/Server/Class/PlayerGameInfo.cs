using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class PlayerGameInfo
    {
        public int index = -1;
        public bool ready = false;
        public int[] scoreBoard = new int[12] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        public void RestartGame()
        {
            ready = false;
            for (int i = 0; i < scoreBoard.Length; i++)
                scoreBoard[i] = -1;
        }

        public int GetScoreSum()
        {
            int sum = 0;
            for (int i = 0; i < 6; i++)
            {
                if (scoreBoard[i] > 0)
                    sum += scoreBoard[i];
            }

            if (sum >= 63) sum += 35;

            for (int i = 6; i < 12; i++)
            {
                if (scoreBoard[i] > 0)
                    sum += scoreBoard[i];
            }

            return sum;
        }
    }

}
