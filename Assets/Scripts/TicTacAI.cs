
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

    class TicTacAI
    {
        private int[,,] gameField = new int[10, 3, 3];
        private int[] metaGameField = new int[9];
        private int myPreviousBoardIndex = -1;
   
        private const int UNDEF = 0;
        private const int CMP = 1;
        private const int PLY = 2;
        private const int DRAW = 3;

        public TicTacAI()
        {
            ResetBoards();
        }

        public void ResetBoards()
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                        gameField[i, j, k] = 0;

            for (int j = 0; j < 9; j++)
                metaGameField[j] = 0;
			myPreviousBoardIndex = -1;
        }

        public void Display()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    string s = "";
                    for (int k = 0; k < 3; k++)
                    {
                        string sp = ".";
                        switch (gameField[i * 3 + k, j, 0])
                        {
                            case 1: sp = "X"; break;
                            case 2: sp = "o"; break;
                            default: sp = "."; break;
                        }
                        s = s + sp;

                        switch (gameField[i * 3 + k, j, 1])
                        {
                            case 1: sp = "X"; break;
                            case 2: sp = "o"; break;
                            default: sp = "."; break;
                        }
                        s = s + sp;

                        switch (gameField[i * 3 + k, j, 2])
                        {
                            case 1: sp = "X"; break;
                            case 2: sp = "o"; break;
                            default: sp = "."; break;
                        }
                        s = s + sp;
                        s = s + ' ';
                    }
                    //Debug.Log(s);
                }
               // Debug.Log("\n");
            }
           // Debug.Log("\n");
        }


		public bool ForceComputerMove(int boardIndex, int posIndex)
		{
			if (gameField [boardIndex, posIndex / 3, posIndex % 3] != UNDEF || metaGameField [boardIndex] != UNDEF)
				return false;

			gameField [boardIndex, posIndex / 3, posIndex % 3] = CMP;
			return true;
		}

        public bool MakeMove(int lastBoardIndex, int lastPosIndex, out int moveBoardIndex, out int movePosIndex, int levelStrength, int followChance=100, int switchBoardChance = 0)
        {
            bool bSwitchBoard = false;
            int result = -1;
            moveBoardIndex = movePosIndex = 0;
            bool bFollower = (Random.Range(0,100) < followChance);

            if (lastBoardIndex >= 0)
            {
                if (gameField[lastBoardIndex, lastPosIndex / 3, lastPosIndex % 3] != UNDEF || metaGameField[lastBoardIndex] != UNDEF)
                    return false;

                gameField[lastBoardIndex, lastPosIndex / 3, lastPosIndex % 3] = PLY;
                result = checkBoardWinner(lastBoardIndex);
            }

            bool allDone = true;
            for (int i = 0; i < 9; i++)
                if (metaGameField[i] == UNDEF) allDone = false;
            if (allDone)
                return true;

			if (Random.Range(0,100) < switchBoardChance) bSwitchBoard = true;
            if (result != 0) bSwitchBoard = true;

            do {
                if (bSwitchBoard)
                {
					if (Random.Range(0,100) >= levelStrength)
                    {
                        do {
                            moveBoardIndex = Random.Range(0,9);
                        } while (!hasMoves(moveBoardIndex));
                    }
                    else
                    {
                        bool bFound = false;
                        for (int i = 0; i < 9; i++)
                            if (metaGameField[i] == UNDEF && countMarks(i, CMP) > countMarks(i, PLY))
                            {
                                bFound = true;
                                moveBoardIndex = i;
                            }

                        if (!bFound)
                        {
                            for (int i = 0; i < 9; i++)
                                gameField[9, i / 3, i % 3] = metaGameField[i];
                            moveBoardIndex = getBoardPosition(9, levelStrength);
                        }
                    }
                }
                else
                {
                    if (bFollower) {
                        moveBoardIndex = lastBoardIndex;
                    }
                    else
                    {
                        if (myPreviousBoardIndex != -1 && metaGameField[myPreviousBoardIndex] == UNDEF)
                            moveBoardIndex = myPreviousBoardIndex;
                        else
                            moveBoardIndex = lastBoardIndex;
                    }

					if (Random.Range(0,100) < levelStrength) {
						for (int i = 0; i < 9; i++)
							gameField[9, i / 3, i % 3] = metaGameField[i];
						int winI = checkIfBlockNeeded(9, CMP);
						if (winI != -1 && metaGameField[winI] == UNDEF)
							moveBoardIndex = winI;
					}
					
                }
                movePosIndex = getBoardPosition(moveBoardIndex, levelStrength);
            } while (gameField[moveBoardIndex, movePosIndex / 3, movePosIndex % 3] != UNDEF);
            gameField[moveBoardIndex, movePosIndex / 3, movePosIndex % 3] = CMP;

            checkBoardWinner(moveBoardIndex);

            myPreviousBoardIndex = moveBoardIndex;

            return true;
        }

        private int lineMarkCount(int bI, int v, int lineIndex, bool bClean)
        {
            int count = 0, av = CMP;
            string[] indices = new string[] { "000102", "101112", "202122", "001020", "011121", "021222", "001122", "021120" };
            char[] s = indices[lineIndex].ToCharArray();
            if (v == CMP) av = PLY;

            int ch1 = gameField[bI, s[0] - '0', s[1] - '0'];
            int ch2 = gameField[bI, s[2] - '0', s[3] - '0'];
            int ch3 = gameField[bI, s[4] - '0', s[5] - '0'];
            count = ((ch1 == v || ch1 == DRAW)? 1 : 0) + ((ch2 == v || ch2 == DRAW)? 1 : 0) + ((ch3 == v || ch3 == DRAW)? 1 : 0);
            if (bClean)
                if (ch1 == av || ch2 == av || ch3 == av) count = 0;

            return count;
        }

        private int blockImmediate(int bI, int lineIndex)
        {
            string[] indices = new string[] { "000102", "101112", "202122", "001020", "011121", "021222", "001122", "021120" };
            char[] s = indices[lineIndex].ToCharArray();

            if (gameField[bI, s[0] - '0', s[1] - '0'] == UNDEF) return (s[0] - '0') * 3 + s[1] - '0';
            if (gameField[bI, s[2] - '0', s[3] - '0'] == UNDEF) return (s[2] - '0') * 3 + s[3] - '0';
            if (gameField[bI, s[4] - '0', s[5] - '0'] == UNDEF) return (s[4] - '0') * 3 + s[5] - '0';
            return -1;
        }

        private int checkIfBlockNeeded(int bI, int v)
        {
            for (int i = 0; i < 8; i++)
            {
                if (lineMarkCount(bI, v, i, true) == 2)
                    return blockImmediate(bI, i);
            }
            return -1;
        }

        private int countMarks(int bI, int v)
        {
            int ret = 0;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (gameField[bI, i, j] == v || gameField[bI, i, j] == DRAW)
                        ret++;
            return ret;
        }

        private bool hasMoves(int bI)
        {
            bool bHasMoves = false;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (gameField[bI, i, j] == UNDEF)
                        bHasMoves = true;
            return bHasMoves;
        }

        private int checkBoardWinner(int bI)
        {
            bool line = false;
            for (int i = 0; i < 2; i++)
            {
                int v = (i == 0) ? CMP : PLY;
                for (int j = 0; j < 8; j++)
                    if (lineMarkCount(bI, v, j, false) == 3) line = true;

                if (line)
                {
                    for (int j = 0; j < 9; j++)
                        gameField[bI, j / 3, j % 3] = v;
                    metaGameField[bI] = v;
                    return v;
                }
            }

            if (!hasMoves(bI)) {
                for (int j = 0; j < 9; j++)
                    gameField[bI, j / 3, j % 3] = j / 3 == 1 ? CMP : PLY;
                metaGameField[bI] = DRAW;
                return DRAW;
            }

            return UNDEF;
        }

        private int getBoardPosition(int bI, int levelStrength)
        {
			if (Random.Range (0, 100) >= levelStrength) {

				if (Random.Range (0, 100) < levelStrength + 10) {
					int blockI = checkIfBlockNeeded (bI, PLY);
					int winI = checkIfBlockNeeded (bI, CMP);

					if (winI != -1) return winI;
					if (blockI != -1) return blockI;
				}

				return Random.Range (0, 9);
			}

            return getRuleBasedPosition(bI, levelStrength);
        }

        private int randomCorner(int bI, bool bNextTo = false)
        {
            int[] indices = new int[] { 1, 3, 7, 9 };
            int[] indicesNext = new int[] { 2, 4, 2, 6, 8, 4, 8, 6 };
            do
            {
                int randVal = Random.Range(0,4);
                int i = indices[randVal] - 1;
                if (gameField[bI, i / 3, i % 3] == UNDEF)
                {
                    if (bNextTo == false) return i;
                    int ch1 = indicesNext[randVal * 2] - 1;
                    int ch2 = indicesNext[randVal * 2 + 1] - 1;
                    if (gameField[bI, ch1 / 3, ch1 % 3] != UNDEF || gameField[bI, ch2 / 3, ch2 % 3] != UNDEF) return i;
                }
            } while (true);
        }

        private int randomEdge(int bI, bool bNextTo = false)
        {
            int[] indices = new int[] { 2, 4, 6, 8 };
            do
            {
				int randVal = Random.Range(0,4);
                int i = indices[randVal] - 1;
                if (gameField[bI, i / 3, i % 3] == UNDEF)
                    return i;
            } while (true);
        }

        private int countCorners(int bI, int v)
        {
            int count = 0, i;
            int[] indices = new int[] { 1, 3, 7, 9 };

            for (i = 0; i < 4; i++)
            {
                int ch = gameField[bI, (indices[i] - 1) / 3, (indices[i] - 1) % 3];
                if (ch == v || ch == DRAW)
                    count++;
            }
            return count;
        }
        private int countEdges(int bI, int v)
        {
            int count = 0, i;
            int[] indices = new int[] { 2, 4, 6, 8 };

            for (i = 0; i < 4; i++)
            {
                int ch = gameField[bI, (indices[i] - 1) / 3, (indices[i] - 1) % 3];
                if (ch == v || ch == DRAW)
                    count++;
            }
            return count;
        }

        private int makeTwo(int bI, int v)
        {
            int ret = -1;
            int bestTwos = 0;

            for (int i = 0; i < 9; i++)
            {
                if (gameField[bI, i / 3, i % 3] == UNDEF)
                {
                    gameField[bI, i / 3, i % 3] = v;
                    int nofTwos = 0;
                    for (int j = 0; j < 8; j++)
                    {
                        int cnt = lineMarkCount(bI, v, j, true);
                        if (cnt == 2) nofTwos++;
                    }
                    gameField[bI, i / 3, i % 3] = UNDEF;
                    if (nofTwos > bestTwos)
                    {
                        bestTwos = nofTwos;
                        ret = i;
                    }
                }
            }
            return ret;
        }

        private int oppositeCorner(int bI, int v)
        {
            if (gameField[bI, 0, 0] == v || gameField[bI, 0, 0] == DRAW) return 9 - 1;
            if (gameField[bI, 0, 2] == v || gameField[bI, 0, 2] == DRAW) return 7 - 1;
            if (gameField[bI, 2, 0] == v || gameField[bI, 2, 0] == DRAW) return 3 - 1;
            if (gameField[bI, 2, 2] == v || gameField[bI, 2, 2] == DRAW) return 1 - 1;
            return -1;
        }

        private int getRuleBasedPosition(int bI, int levelStrength)
        {
            int plyC = countMarks(bI, PLY);
            int cmpC = countMarks(bI, CMP);
            int totalC = plyC + cmpC;
            int cornersPly = countCorners(bI, PLY);
            int edgesPly = countEdges(bI, PLY);
            bool midPly = gameField[bI, 1, 1] == PLY;
            int cornersCmp = countCorners(bI, CMP);
            int edgesCmp = countEdges(bI, CMP);
            bool midCmp = gameField[bI, 1, 1] == CMP;

            int blockI = checkIfBlockNeeded(bI, PLY);
            int winI = checkIfBlockNeeded(bI, CMP);

            if (winI != -1) { return winI; }
            if (blockI != -1) return blockI;

            if (totalC == 0)
            {
				if ( Random.Range(0,100) < 30)
                    return 5 - 1;
                else
                    return randomCorner(bI);
            }
            else if (totalC == 1 && plyC == 1)
            {
                if (cornersPly == 1)
                    return 5 - 1;
                if (edgesPly == 1)
                {
                    return randomCorner(bI, true);
                }
                return randomCorner(bI);
            }
            else if (totalC == 3 && plyC == 2)
            {
                if (cornersPly == 2)
                {
                    return randomEdge(bI);
                }
                if (cornersPly == 1 && midPly)
                {
                    return randomCorner(bI);
                }
                if (!midPly && !midCmp)
                    return 5 - 1;

                return randomEdge(bI);
            }
            else if (totalC == 2 && plyC == 1)
            {
                if (cornersCmp == 1)
                {
                    if (cornersPly == 1)
                        return randomCorner(bI);
                    else if (edgesPly == 1)
                        return 5-1;
                    else
                    {
                        int opp = oppositeCorner(bI, CMP);
                        if (opp != -1) return opp;
                    }
                }
                else if (midCmp)
                {
                    if (edgesPly == 1)
                        return randomCorner(bI);
                    int opp = oppositeCorner(bI, PLY);
                    if (opp != -1) return opp;
                }
            }

            int hasTwo = makeTwo(bI, CMP);
            if (hasTwo >= 0) return hasTwo;

			return Random.Range(0,9);
        }
    
}
