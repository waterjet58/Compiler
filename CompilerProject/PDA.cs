using System;
using System.Collections.Generic;
using System.Linq;

namespace CompilerProject
{
    class PDA
    {
        private static string[] terminalList = {
            ";", "=", "+", "-", "(", ")", "*", "/", "IF", "THEN", "==", "!=", ">", "<", ">=", "<=", "{", "}", "WHILE", "DO"
        };
        //Precedence Function F
        private static Dictionary<string, int> operatorPrecedenceF = new Dictionary<string, int>(){
            {";", 1}, {"=", 6}, {"+", 16}, {"-", 16}, {"(", 2}, {")", 18}, {"*", 18}, {"/", 18}, {"IF", 2}, {"THEN", 1}, 
            {"==", 5}, {"!=", 5}, {">", 5}, {"<", 5}, {">=", 5}, {"<=", 5}, {"{", 2}, {"}", 1}, {"CALL", 1}, {"WHILE", 2}, {"DO", 1}
        };
        //Precedence Function G
        private static Dictionary<string, int> operatorPrecedenceG = new Dictionary<string, int>(){
            {";", 1}, {"=", 4}, {"+", 14}, {"-", 14}, {"(", 19}, {")", 2}, {"*", 17}, {"/", 17}, {"IF", 2}, {"THEN", 2},
            {"==", 7}, {"!=", 7}, {">", 7}, {"<", 7}, {">=", 7}, {"<=", 7}, {"{", 5}, {"}", 2}, {"CALL", 5}, {"WHILE", 2}, {"DO", 2}
        }; // =, X, 5, ~
        //Setting up public/private variables used
        public static string[,] quadList = new string[30, 2];
        public static int endOfQuadList = 0;
        private static Stack<string> stack = new Stack<string>();
        private static Stack<string> fixupStack = new Stack<string>();
        private static Stack<string> whileStartStack = new Stack<string>();
        private static string temp;
        public static int tempIndex = 1;
        private static int labelIndex = 1;
        private static string lastTerminal = "";
        private static int state = 0;
        private static int i = 0;
        private static string _token = "";
        public static string quad = "";
        private static string rightSide = "";
        public static void generateQuads()
        {
            bool finished = false;
            while ( !finished )
            {
                switch(state) //State machine to remove unneccassary bits at the front of the program
                {
                    case 0:
                        _token = LexicalAnalysis.tokenArray[i, 0];
                        if (_token.Equals("CLASS"))
                        {
                            state = 1;
                        }
                        else if (_token.Equals("VAR"))
                        {
                            state = 2;
                        }
                        else if (_token.Equals("CONST"))
                        {
                            state = 3;
                        }
                        else
                        {
                            state = 4;
                        }

                        break;

                    case 1:
                        while(!_token.Equals("{")) //Remove CLASS and its name
                        {
                            i++;
                            _token = LexicalAnalysis.tokenArray[i, 0]; 
                        }
                        i++; //Remove the {
                        state = 0;
                        break;

                    case 2:
                        while(!_token.Equals(";")) 
                        {
                            i++;
                            _token = LexicalAnalysis.tokenArray[i, 0];
                        }
                        i++;
                        state = 0;
                        break;

                    case 3:
                        while(!_token.Equals(";"))
                        {
                            i++; 
                            _token = LexicalAnalysis.tokenArray[i, 0];
                        }
                        i++;
                        state = 0;
                        break;

                    case 4:
                        generate(); //Start generating the Quads
                        finished = true;
                        break;
                }

            }

        }

        private static void generate()
        {
            stack.Push("~"); //Push any non terminal
            lastTerminal = "~";

            while (i < LexicalAnalysis.endOfTokenArray || stack.Count != 0)
            {
                int f = 0;
                int g = 0;
                if(_token != null) //Get the precedence function values
                {
                    operatorPrecedenceF.TryGetValue(lastTerminal, out f);
                    operatorPrecedenceG.TryGetValue(_token, out g);
                }
                else //If the token next token is null check and see if the terminal is null too
                {
                    operatorPrecedenceF.TryGetValue(lastTerminal, out f);
                    if (stack.Peek() == null) break; //if the terminal is null end loop
                }

                if(lastTerminal.Equals("{") && _token.Equals("}"))
                {
                    stack.Pop();
                    i++;
                    string[] thenArgs = { "", "THEN", "" };
                    checkOperator(thenArgs);
                    lastTerminal = stack.Peek();
                    _token = LexicalAnalysis.tokenArray[i, 0];   
                }
                else if(lastTerminal.Equals("THEN") && _token.Equals("}"))
                {
                    stack.Pop();
                    i++;
                    string[] thenArgs = { "", "THEN", "" };
                    checkOperator(thenArgs);
                    lastTerminal = stack.Peek();
                    _token = LexicalAnalysis.tokenArray[i, 0];
                }
                else if (lastTerminal.Equals("DO") && _token.Equals("}"))
                {
                    i++;
                    string[] thenArgs = { "", "DO", "" };
                    checkOperator(thenArgs);
                    lastTerminal = stack.Peek();
                    _token = LexicalAnalysis.tokenArray[i, 0];
                }

                if (f <= g && terminalList.Contains(_token) && !_token.Equals("IF") && !_token.Equals("WHILE") && !(lastTerminal.Equals("{") && _token.Equals("}")))
                {
                    i++;
                    if (!_token.Equals(";"))
                        stack.Push(_token);
                    else
                        _token = stack.Peek();
                    lastTerminal = _token;
                    _token = LexicalAnalysis.tokenArray[i, 0];
                }
                else if ((f > g && terminalList.Contains(_token) || _token == null))
                {
                    string[] args = new string[3];
                    int argIndex = 0;
                    rightSide = _token;
                    while (f > g || (rightSide == null && stack.Peek() != null))
                    {
                        if (argIndex == 3 && !stack.Peek().Equals("("))
                        {
                            lastTerminal = stack.Peek();
                            break;
                        }else if (argIndex == 3 && stack.Peek().Equals("(") && rightSide.Equals(")"))
                        {
                            stack.Pop();
                            break;
                        }                        
                        if (stack.Count == 0)
                        {
                            i++;
                            stack.Push(LexicalAnalysis.tokenArray[i, 0]);
                            i++;
                            lastTerminal = "";
                            break;
                        }
                        if (terminalList.Contains(stack.Peek()) && rightSide != null)
                        {
                            operatorPrecedenceF.TryGetValue(stack.Peek(), out f);
                            operatorPrecedenceG.TryGetValue(rightSide, out g);
                        }
                        else if(stack.Peek().Equals("THEN"))
                        {
                            args[1] = "THEN";
                            checkOperator(args);
                            break;
                        }
                        else if (stack.Peek().Equals("DO"))
                        {
                            args[1] = "DO";
                            checkOperator(args);
                            break;
                        }
                        if (f > g)
                        {
                            args[argIndex] = stack.Pop();
                            argIndex++;
                        }
                        else if(stack.Peek().Equals("(") && rightSide.Equals(")"))
                        {
                            stack.Pop();
                        }
                    }
                    if (rightSide != null && rightSide.Equals(")"))
                    {
                        lastTerminal = stack.Peek();
                        checkOperator(args);
                    }
                    else if (rightSide != null && rightSide.Equals("}") && stack.Peek().Equals("DO"))
                    {
                        Console.Read();
                    }
                    else if (rightSide != null && !rightSide.Equals(";") && !rightSide.Equals("}") && !rightSide.Equals("IF"))
                    {
                        checkOperator(args);
                        lastTerminal = rightSide;
                        stack.Push(rightSide);
                    }
                    else if (rightSide != null && rightSide.Equals("IF"))
                    {
                        string[] ifArg = { "", "IF", "" };
                        checkOperator(ifArg);
                    }
                    else if (rightSide != null && _token.Equals("WHILE"))
                    {
                        string[] whileArgs = { "", "WHILE", "" };
                        checkOperator(whileArgs);
                        i++;
                        _token = LexicalAnalysis.tokenArray[i, 0];
                    }
                    else if (rightSide != null)
                    {
                        checkOperator(args);
                        i--;
                    }
                    if(rightSide != null)
                    {
                        i++;
                        _token = LexicalAnalysis.tokenArray[i, 0];
                    }                                        
                }
                else if(_token.Equals("IF"))
                {
                    string[] args = { "", _token, "" };
                    checkOperator(args);
                    i++;
                    _token = LexicalAnalysis.tokenArray[i, 0];
                }
                else if (_token.Equals("WHILE"))
                {
                    string[] args = { "", _token, "" };
                    checkOperator(args);
                    i++;
                    _token = LexicalAnalysis.tokenArray[i, 0];
                }
                else if (_token.Equals("CALL"))
                {
                    string[] args = { "", _token, "" };
                    checkOperator(args);
                    _token = LexicalAnalysis.tokenArray[i, 0];
                }
                else if (!(lastTerminal.Equals("{") && _token.Equals("}")))
                {
                    stack.Push(_token);
                    i++;
                    _token = LexicalAnalysis.tokenArray[i, 0];
                }
            }
            for(int i = 0; i < endOfQuadList; i++)
            {
                Console.WriteLine(quadList[i,0].PadRight(20) + " | address: " + quadList[i,1]);
            }
            Console.WriteLine("-----------------------------------------------------");
        }
        
        private static void checkOperator(string[] args)
        {
            int address = 0;
            switch (args[1])
            {
                case "CALL": //We know there will be 5 tokens in front of call Ex: Call print(x);
                    i++; //Get the procedure name
                    quad = "CALL, " + LexicalAnalysis.tokenArray[i,0] + ", "; // Call, procName, 
                    i+=2; //Skip the bracket to the inside token
                    quad += LexicalAnalysis.tokenArray[i, 0] + ", ~"; // Call, procName, var, ~
                    i+=3; //Skip past the right bracket and semi
                    quadList[endOfQuadList, 0] = quad;
                    quadList[endOfQuadList, 1] = "" + endOfQuadList * 2;
                    endOfQuadList++;
                    break;
                case "=":
                    quad = (args[1] + ", " + args[2] + ", " + args[0] + ", ~");
                    quadList[endOfQuadList,0] = quad;
                    quadList[endOfQuadList, 1] = "" + endOfQuadList * 2;
                    endOfQuadList++;
                    break;

                case "+":
                    temp = "T" + tempIndex;
                    quad = (args[1] + ", " + args[2] + ", " + args[0] + ", " + temp);
                    quadList[endOfQuadList, 0] = quad;
                    quadList[endOfQuadList, 1] = "" + endOfQuadList * 2;
                    endOfQuadList++;
                    tempIndex++;
                    lastTerminal = stack.Peek();
                    stack.Push(temp);
                    break;

                case "-":
                    temp = "T" + tempIndex;
                    quad = (args[1] + ", " + args[2] + ", " + args[0] + ", " + temp);
                    quadList[endOfQuadList, 0] = quad;
                    quadList[endOfQuadList, 1] = "" + endOfQuadList * 2;
                    endOfQuadList++;
                    tempIndex++;
                    lastTerminal = stack.Peek();
                    stack.Push(temp);

                    break;

                case "*":
                    temp = "T" + tempIndex;
                    quad = (args[1] + ", " + args[2] + ", " + args[0] + ", " + temp);
                    quadList[endOfQuadList, 0] = quad;
                    quadList[endOfQuadList, 1] = "" + endOfQuadList * 2;
                    endOfQuadList++;
                    tempIndex++;
                    lastTerminal = stack.Peek();
                    stack.Push(temp);

                    break;

                case "/":
                    temp = "T" + tempIndex;
                    quad = (args[1] + ", " + args[2] + ", " + args[0] + ", " + temp);
                    quadList[endOfQuadList, 0] = quad;
                    quadList[endOfQuadList, 1] = "" + endOfQuadList * 2;
                    endOfQuadList++;
                    tempIndex++;
                    lastTerminal = stack.Peek();
                    stack.Push(temp);
                    
                    break;

                case "IF":
                    quad = (args[1] + ", ~, ~, ~");
                    quadList[endOfQuadList, 0] = quad;
                    quadList[endOfQuadList, 1] = "" + endOfQuadList * 2;
                    endOfQuadList++;
                    stack.Push(_token);
                    break;

                case "THEN":
                    stack.Pop(); //Popping THEN
                    stack.Pop(); //Popping IF
                    string Label = fixupStack.Pop();
                    string[] s = Label.Split(',');
                    quad = "L, " + s[0] + ", ~, ~";
                    quadList[endOfQuadList, 0] = quad;
                    quadList[endOfQuadList, 1] = "" + endOfQuadList * 2;
                    endOfQuadList++;
                    break;

                case "DO":
                    stack.Pop(); //Popping DO
                    stack.Pop(); //Popping WHILE               

                    Label = whileStartStack.Pop(); //Generating While Label
                    s = Label.Split(',');
                    quad = "J, " + s[0] + ", ~, ~";
                    quadList[endOfQuadList, 0] = quad;
                    quadList[endOfQuadList, 1] = "" + endOfQuadList * 2;
                    endOfQuadList++;

                    Label = fixupStack.Pop(); //Generating While Label
                    s = Label.Split(',');
                    quad = "L, " + s[0] + ", ~, ~";
                    quadList[endOfQuadList, 0] = quad;
                    quadList[endOfQuadList, 1] = "" + endOfQuadList * 2;
                    endOfQuadList++;
                    break;

                case "WHILE":
                    string whileLabel = "W" + labelIndex;
                    quad = (args[1] + ", "+whileLabel+", ~, ~");
                    quadList[endOfQuadList, 0] = quad;
                    quadList[endOfQuadList, 1] = "" + endOfQuadList * 2;
                    endOfQuadList++;
                    stack.Push(_token);
                    break;

                case ">":
                    quad = (args[1] + ", " + args[2] + ", " + args[0] + ", ~");
                    quadList[endOfQuadList, 0] = quad;
                    quadList[endOfQuadList, 1] = "" + endOfQuadList * 2;
                    endOfQuadList++;
                    lastTerminal = stack.Peek();
                    
                    if(rightSide.Equals("THEN"))
                    {
                        quadList[endOfQuadList, 0] = ("THEN, L" + labelIndex + ",LE , ~");
                        address = endOfQuadList * 2;
                        quadList[endOfQuadList, 1] = "" + address;
                        endOfQuadList++;
                        fixupStack.Push("L" + labelIndex + ", " + address);
                        labelIndex++;
                    }
                    else if (rightSide.Equals("DO"))
                    {
                        whileStartStack.Push("W" + labelIndex + ", " + endOfQuadList * 2);
                        quadList[endOfQuadList, 0] = ("DO, L" + labelIndex + ",LE , ~");
                        address = endOfQuadList * 2;
                        quadList[endOfQuadList, 1] = "" + address;
                        endOfQuadList++;
                        fixupStack.Push("L" + labelIndex + ", " + address);
                        labelIndex++;
                    }
                    break;

                case ">=":
                    quad = (args[1] + ", " + args[2] + ", " + args[0] + ", ~");
                    quadList[endOfQuadList, 0] = quad;
                    quadList[endOfQuadList, 1] = "" + endOfQuadList * 2;
                    endOfQuadList++;
                    lastTerminal = stack.Peek();

                    if (rightSide.Equals("THEN"))
                    {
                        quadList[endOfQuadList, 0] = ("THEN, L" + labelIndex + ",L , ~");
                        address = endOfQuadList * 2;
                        quadList[endOfQuadList, 1] = "" + address;
                        endOfQuadList++;
                        fixupStack.Push("L" + labelIndex + ", " + address);
                        labelIndex++;
                    }
                    else if (rightSide.Equals("DO"))
                    {
                        whileStartStack.Push("W" + labelIndex + ", " + endOfQuadList * 2);
                        quadList[endOfQuadList, 0] = ("DO, L" + labelIndex + ",L , ~");
                        address = endOfQuadList * 2;
                        quadList[endOfQuadList, 1] = "" + address;
                        endOfQuadList++;
                        fixupStack.Push("L" + labelIndex + ", " + address);
                        labelIndex++;
                    }

                    break;

                case "<":
                    quad = (args[1] + ", " + args[2] + ", " + args[0] + ", ~");
                    quadList[endOfQuadList, 0] = quad;
                    quadList[endOfQuadList, 1] = "" + endOfQuadList * 2;
                    endOfQuadList++;
                    lastTerminal = stack.Peek();

                    if (rightSide.Equals("THEN"))
                    {
                        quadList[endOfQuadList, 0] = ("THEN, L" + labelIndex + ",GE , ~");
                        address = endOfQuadList * 2;
                        quadList[endOfQuadList, 1] = "" + address;
                        endOfQuadList++;
                        fixupStack.Push("L" + labelIndex + ", " + address);
                        labelIndex++;
                    }
                    else if (rightSide.Equals("DO"))
                    {
                        whileStartStack.Push("W" + labelIndex + ", " + endOfQuadList * 2);
                        quadList[endOfQuadList, 0] = ("DO, L" + labelIndex + ",GE , ~");
                        address = endOfQuadList * 2;
                        quadList[endOfQuadList, 1] = "" + address;
                        endOfQuadList++;
                        fixupStack.Push("L" + labelIndex + ", " + address);
                        labelIndex++;
                    }
                    break;

                case "<=":
                    quad = (args[1] + ", " + args[2] + ", " + args[0] + ", ~");
                    quadList[endOfQuadList, 0] = quad;
                    quadList[endOfQuadList, 1] = "" + endOfQuadList * 2;
                    endOfQuadList++;
                    lastTerminal = stack.Peek();

                    if (rightSide.Equals("THEN"))
                    {
                        quadList[endOfQuadList, 0] = ("THEN, L" + labelIndex + ",G , ~");
                        address = endOfQuadList * 2;
                        quadList[endOfQuadList, 1] = "" + address;
                        endOfQuadList++;
                        fixupStack.Push("L" + labelIndex + ", " + address);
                        labelIndex++;
                    }
                    else if (rightSide.Equals("DO"))
                    {
                        whileStartStack.Push("W" + labelIndex + ", " + endOfQuadList * 2);
                        quadList[endOfQuadList, 0] = ("DO, L" + labelIndex + ",G , ~");
                        address = endOfQuadList * 2;
                        quadList[endOfQuadList, 1] = "" + address;
                        endOfQuadList++;
                        fixupStack.Push("L" + labelIndex + ", " + address);
                        labelIndex++;
                    }

                    break;

                case "==":
                    quad = (args[1] + ", " + args[2] + ", " + args[0] + ", ~");
                    quadList[endOfQuadList, 0] = quad;
                    quadList[endOfQuadList, 1] = "" + endOfQuadList * 2;
                    endOfQuadList++;
                    lastTerminal = stack.Peek();

                    if (rightSide.Equals("THEN"))
                    {
                        quadList[endOfQuadList, 0] = ("THEN, L" + labelIndex + ",NE , ~");
                        address = endOfQuadList * 2;
                        quadList[endOfQuadList, 1] = "" + address;
                        endOfQuadList++;
                        fixupStack.Push("L" + labelIndex + ", " + address);
                        labelIndex++;
                    } 
                    else if(rightSide.Equals("DO"))
                    {
                        whileStartStack.Push("W" + labelIndex + ", " + endOfQuadList * 2);
                        quadList[endOfQuadList, 0] = ("DO, L" + labelIndex + ",NE , ~");
                        address = endOfQuadList * 2;
                        quadList[endOfQuadList, 1] = "" + address;
                        endOfQuadList++;
                        fixupStack.Push("L" + labelIndex + ", " + address);
                        labelIndex++;
                    }

                    break;

                default:
                    lastTerminal = stack.Peek();
                    i++;
                    break;
            }
            quad = "";
        }
    }
}
