using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


    class Program
    {
        static void Main(string[] args)
        {
            bool bPlayerStart = true, valid = false;
            TicTacAI ttAI = new TicTacAI();
            int outBoard, outPos;
            String s = "Dummy";

            ttAI.Display();
            Debug.Log("\n");

            do
            {
                if (bPlayerStart)
                {
                    s = Console.ReadLine();
					Debug.Log("\n");
                    if (s.Length >= 2)
                    {
                        char[] sa = s.ToCharArray();
                        int v1 = (int)Char.GetNumericValue(sa[0]);
                        int v2 = (int)Char.GetNumericValue(sa[1]);
                        if (v1 >= 1 && v2 >= 1 && v1 <= 9 && v2 <= 9)
                            valid = ttAI.MakeMove(v1 - 1, v2 - 1, out outBoard, out outPos, 80,  100,0);
//                        valid = ttAI.MakeMove(v1 - 1, v2 - 1, out outBoard, out outPos, 75, 85,5);
//                        valid = ttAI.MakeMove(v1 - 1, v2 - 1, out outBoard, out outPos, 0, 50, 100);
//                        valid = ttAI.MakeMove(v1 - 1, v2 - 1, out outBoard, out outPos, 0, 80, 5);
                    }
                } else
                {
                    valid = ttAI.MakeMove(-1, -1, out outBoard, out outPos, 100,  100,0);
                    bPlayerStart = true;
                }

                if (valid)
                    ttAI.Display();

            } while (s.Length >= 2);
        }
    
}
