using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class YatzyUtil
    {
        public static int GetScore(List<int> dices, int type)
        {
            if (dices.Count != 5) return 0;

            int score = 0;
            int[] counts = new int[6];
            int sum = 0;

            foreach (var dice in dices)
            {
                if (dice > 6) return 0;
                sum += dice;
                counts[dice - 1]++;
            }

            switch (type)
            {
                case 0:
                    score = counts[0];
                    break;
                case 1:
                    score = counts[1] * 2;
                    break;
                case 2:
                    score = counts[2] * 3;
                    break;
                case 3:
                    score = counts[3] * 4;
                    break;
                case 4:
                    score = counts[4] * 5;
                    break;
                case 5:
                    score = counts[5] * 6;
                    break;
                case 6:
                    score = sum;
                    break;
                case 7:
                    if (counts.Contains(4) || counts.Contains(5)) score = sum;
                    break;
                case 8:
                    if (counts.Contains(2) && counts.Contains(3)) score = sum;
                    else if (counts.Contains(5)) score = sum;
                    break;
                case 9:
                    int continuous = 0;
                    foreach (var count in counts)
                    {
                        if (count > 0) continuous++;
                        else continuous = 0;

                        if (continuous >= 4) score = 15;
                    }
                    break;
                case 10:
                    int continuous2 = 0;
                    foreach (var count in counts)
                    {
                        if (count > 0) continuous2++;
                        else continuous2 = 0;

                        if (continuous2 >= 5) score = 30;
                    }
                    break;
                case 11:
                    if (counts.Contains(5)) score = 50;
                    break;
            }

            return score;
        }

        static void GetFourKind(List<int> dices)
        {
            

        }
    }
}
