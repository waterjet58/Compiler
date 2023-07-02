using System;
using System.Collections.Generic;

namespace CompilerProject
{
    class GenerateCode
    {
        public static List<string> codeSegment;
        public static void generate()
        {
            codeSegment = new List<string>();
            int i = 0;
            while( i < PDA.endOfQuadList)
            {
                string[] quad = PDA.quadList[i,0].Split(',');
                if(Int32.TryParse(quad[1].Trim(), out int result))
                {
                    quad[1] = "Lit" + result;
                }
                if (Int32.TryParse(quad[2].Trim(), out result))
                {
                    quad[2] = "Lit" + result;
                }
                if (Int32.TryParse(quad[3].Trim(), out result))
                {
                    quad[3] = "Lit" + result;
                }
                switch (quad[0])
                {
                    case "CALL":
                        if (quad[1].Trim().Equals("print"))
                            codeSegment.Add("mov eax, "+quad[2].Trim() + "\n"+
                                            "call ConvertIntegerToString\n" +
                                            "mov eax, offset buffer\n"+
                                            "mov ebx, 10\n"+
                                            "call PrintLine");

                        if (quad[1].Trim().Equals("getInt"))
                            codeSegment.Add("call ReadInteger\n" +
                                            "mov "+quad[2].Trim() + ", eax");
                        break;

                    case "=":
                        codeSegment.Add("mov eax, [" + quad[2].Trim() + "]\n" +
                                        "mov [" + quad[1].Trim() + "], eax");
                        break;

                    case "+":
                        codeSegment.Add("mov eax, [" + quad[1].Trim() + "]\n" +
                                        "add eax, [" + quad[2].Trim() + "]\n" +
                                        "mov [" + quad[3].Trim() + "], eax");
                        break;

                    case "-":
                        codeSegment.Add("mov eax, [" + quad[1].Trim() + "]\n" +
                                        "sub eax, [" + quad[2].Trim() + "]\n" +
                                        "mov [" + quad[3].Trim() + "], eax");
                        break;

                    case "*":
                        codeSegment.Add("mov eax, [" + quad[1].Trim() + "]\n" +
                                        "imul eax, [" + quad[2].Trim() + "]\n" +
                                        "mov [" + quad[3].Trim() + "], eax");
                        break;

                    case "/":
                        codeSegment.Add("mov edx, 0\n"+
                                        "mov eax, [" + quad[1].Trim() + "]\n" +
                                        "mov ebx, [" + quad[2].Trim() + "]\n" +
                                        "div ebx\n" +
                                        "mov [" + quad[3].Trim() + "], eax");
                        break;

                    case ">":
                        codeSegment.Add("mov eax, [" + quad[1].Trim() + "]\n" +
                                        "cmp eax, [" + quad[2].Trim() + "]");
                        break;

                    case ">=":
                        codeSegment.Add("mov eax, [" + quad[1].Trim() + "]\n" +
                                        "cmp eax, [" + quad[2].Trim() + "]");
                        break;

                    case "<":
                        codeSegment.Add("mov eax, [" + quad[1].Trim() + "]\n" +
                                        "cmp eax, [" + quad[2].Trim() + "]");
                        break;

                    case "<=":
                        codeSegment.Add("mov eax, [" + quad[1].Trim() + "]\n" +
                                        "cmp eax, [" + quad[2].Trim() + "]");
                        break;

                    case "==":
                        codeSegment.Add("mov eax, [" + quad[1].Trim() + "]\n" +
                                        "cmp eax, [" + quad[2].Trim() + "]");
                        break;

                    case "THEN":
                        codeSegment.Add("J"+ quad[2].Trim() + " " + quad[1].Trim());
                        break;

                    case "DO":
                        codeSegment.Add("J" + quad[2].Trim() + " " + quad[1].Trim());
                        break;

                    case "L":
                        codeSegment.Add( quad[1].Trim() + ":" + " nop");
                        break;

                    case "J":
                        codeSegment.Add("jmp " + quad[1].Trim());
                        break;

                    case "WHILE":
                        codeSegment.Add(quad[1].Trim()+": ");
                        break;

                }
                i++;  //Increment to the next segment         
            }
        }
    }
}

/*
 * 32 NESTED IF
 * 41 MAIN
 * 42 LEX
 * 52 PDA
 * 61 
 * 62
 * 64
 */