using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class YatzyUtil
    {
        public static int GetScore(int[] dices, int type)
        {
            if (dices.Length != 5) return 0;

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

        public static int boolArrayToInt(bool[] booleans)
        {
            int result = 0;

            return result;
        }

        // 봇 : 잠글 주사위 결정 (주사위 인덱스 2진법 반환, 미잠금 시 -1)
        public static bool[] GetBotWantLockDice(int[] dices, int[] scoreBoard, int remainDice)
        {
            bool[] locks = new bool[5];
            int[] diceNums = new int[6];
            int remainScores = 0;

            foreach (var diceNum in dices)
                diceNums[diceNum - 1]++;

            foreach (var score in scoreBoard)
                if (score < 0) remainScores++;

            // 3개 이상인 주사위 잠금 (해당하는 마이너 족보 미기입 or 야추 미기입 or 4이상이면서 초이스 및 포카드 미기입 or 풀하우스 미기입)
            for (int i = 0; i < 6; i++)
            {
                if (diceNums[i] >= 3)
                {
                    if (scoreBoard[i] < 0 || scoreBoard[11] < 0 ||
                        (i + 1 >= 4 && (scoreBoard[6] < 0 || scoreBoard[7] < 0 || scoreBoard[8] < 0)))
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            if (dices[j] == i + 1)
                                locks[j] = true;
                        }
                        return locks;
                    }
                }
            }

            // 스트레이트 미정인 경우 - 2,3,4,5 중에 3개 이상 있으면 하나씩만 잠금
            if (scoreBoard[9] < 0)
            {
                int needNumCount = 0;
                for (int i = 1; i <= 4; i++)
                {
                    if (diceNums[i] > 0)
                        needNumCount++;
                }
                if (needNumCount >= 3)
                {
                    bool[] lockNums = new bool[6]; // 해당 숫자 이미 잠궜는지 체크
                    for (int i = 0; i < 5; i++)
                    {
                        if (dices[i] == 2 || dices[i] == 3 || dices[i] == 4 || dices[i] == 5)
                        {
                            if (lockNums[dices[i] - 1] == false)
                            {
                                locks[i] = true;
                                lockNums[dices[i] - 1] = true;
                            }
                        }
                    }
                    return locks;
                }
            }

            // 2개 이상인 수가 두개이고 풀하우스 미정이면 잠금
            if (scoreBoard[8] < 0)
            {
                int largerThanTwo = -1;
                for (int i = 0; i < 6; i++)
                {
                    if (diceNums[i] >= 2)
                    {
                        // 2개 이상인 수가 2개
                        if (largerThanTwo >= 0)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                if (dices[j] == largerThanTwo + 1 || dices[j] == i + 1)
                                    locks[j] = true;
                            }
                            return locks;
                        }
                        else
                            largerThanTwo = i;
                    }
                }
            }

            // 2개 이상인 주사위 잠금, 해당 수 or 초이스, 4카인드, 풀하우스, 야추 미달성 시 해당 수 잠금
            for (int i = 5; i >= 0; i--)
            {
                if (diceNums[i] >= 2)
                {
                    if (scoreBoard[i] < 0 || scoreBoard[11] < 0 ||
                        (i + 1 >= 4 && (scoreBoard[6] < 0 || scoreBoard[7] < 0 || scoreBoard[8] < 0)))
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            if (dices[j] == i + 1)
                                locks[j] = true;
                        }
                        return locks;
                    }
                }
            }

            // 숫자가 1개만 남은 상황이면 무조건 그거만 잠금
            if (remainScores == 1)
            {
                int notScored = 0;
                for (int i = 0; i < scoreBoard.Length; i++)
                {
                    if (scoreBoard[i] < 0) notScored = i;
                }

                for (int i = 0; i < 5; i++)
                    if (dices[i] == notScored) 
                        locks[i] = true;
            }

            // 해당 수나 야추 미등록 혹은 눈이 4 이상인데 초이스, 포카인드, 풀하우스 미등록이면 그거 잠금
            for (int i = 5; i >= 0; i--)
            {
                if (diceNums[i] >= 1)
                {
                    if (scoreBoard[i] < 0 || scoreBoard[11] < 0 ||
                        (i + 1 >= 4 && (scoreBoard[6] < 0 || scoreBoard[7] < 0 || scoreBoard[8] < 0)))
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            if (dices[j] == i + 1)
                                locks[j] = true;
                        }
                        return locks;
                    }
                }
            }

            return locks;
        }

        // 봇 : 등록할 숫자 결정 (족보 인덱스 반환, 미등록 -1)
        public static int GetBotWantWriteScore(int[] dices, int[] scoreBoard, int remainDice)
        {
            int[] previewScore = new int[12];
            int notScoredCount = 0; // 남은 족보 수

            for (int i = 0; i < 12; i++)
            {
                if (scoreBoard[i] < 0)
                {
                    previewScore[i] = GetScore(dices, i);
                    notScoredCount++;
                }
                else
                    previewScore[i] = -1; // 이미 등록된 족보는 -1
            }

            if (remainDice == 0)
            {
                // 족보 한개만 남으면 그거 등록
                if (notScoredCount == 1)
                {
                    for (int i = 0; i < 12; i++)
                        if (previewScore[i] >= 0) return i;
                }

                // 마이너 체크. 5,6이 4개 이상인 경우 등록
                for (int i = 4; i <= 5; i++)
                {
                    if (previewScore[i] >= (i + 1) * 4) return i;
                }

                // 메이저 되는거 있는지 체크. 초이스는 20점 이상인 경우
                for (int i = 11; i >= 7; i--)
                {
                    if (previewScore[i] > 0) return i;
                }
                if (previewScore[6] >= 20) return 6;

                // 마이너 체크. 3개 이상인 경우 등록
                for (int i = 0; i <= 5; i++)
                {
                    if (previewScore[i] >= (i + 1) * 3) return i;
                }

                // 마이너 보너스 가능여부 체크 (메이저 or 마이너 어떤거 포기할지 결정)
                int subTotal = 0;
                for (int i = 0; i < 6; i++)
                {
                    if (scoreBoard[i] > 0) subTotal += scoreBoard[i];
                    else subTotal += (i + 1) * 3; // 없는건 3개씩 나온다고 가정
                }

                // 보너스 가능한 경우
                if (subTotal >= 63)
                {
                    // 기대 점수가 낮은것부터 버린다.
                    if (scoreBoard[11] < 0) return 11; // 야추 버림 2.3
                    if (scoreBoard[7] < 0) return 7; // 포카인드 버림 5.61
                    if (scoreBoard[8] < 0) return 8; // 풀하우스 버림 9.15
                    if (scoreBoard[10] < 0) return 10; // 라지 스트레이트 버림 10.61
                    if (scoreBoard[9] < 0) return 9; // 스몰 스트레이트 버림 18.48
                    if (scoreBoard[6] < 0) return 6; // 초이스 버림 23.33
                }

                // 마이너 체크. 2개 이상인 경우 등록
                for (int i = 0; i <= 5; i++)
                {
                    if (previewScore[i] >= (i + 1) * 2) return i;
                }

                // 마이너 체크. 1개 이상인 경우 등록
                for (int i = 0; i <= 5; i++)
                {
                    if (previewScore[i] >= (i + 1) * 2) return i;
                }

                // 마이너 체크. 0개 이상인 경우 등록
                for (int i = 0; i <= 5; i++)
                {
                    if (previewScore[i] >= 0) return i;
                }

                // 기대 점수가 낮은것부터 버린다.
                if (scoreBoard[11] < 0) return 11; // 야추 버림 2.3
                if (scoreBoard[7] < 0) return 7; // 포카인드 버림 5.61
                if (scoreBoard[8] < 0) return 8; // 풀하우스 버림 9.15
                if (scoreBoard[10] < 0) return 10; // 라지 스트레이트 버림 10.61
                if (scoreBoard[9] < 0) return 9; // 스몰 스트레이트 버림 18.48
                if (scoreBoard[6] < 0) return 6; // 초이스 버림 23.33

                // 최종 방어처리
                for (int i = 0; i < 12; i++)
                    if (previewScore[i] >= 0) return i;
            }
            else if (remainDice >= 1)
            {
                if (previewScore[11] > 0) return 11; // 야추
                if (previewScore[10] > 0) return 10; // 라지스트레이트
                if (previewScore[8] > 0) return 8; // 풀하우스
                if (previewScore[10] < 0 && previewScore[9] > 0) return 9; // 스몰스트레이트
            }

            return -1;
        }
    }
}
