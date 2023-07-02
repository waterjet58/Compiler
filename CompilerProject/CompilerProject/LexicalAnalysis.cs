using System;
using System.IO;

namespace CompilerProject
{
    public static class LexicalAnalysis
    {
        
        public static String[,] tokenArray = new String[200, 2]; //Array that holds the Tokens
        public static int endOfTokenArray = 0; //Index inside of the token array

        public static void generateSymbolTable(String path)
        {
            
            StreamReader reader; //Open a stream reader
            reader = new StreamReader(path);

            bool finished = false; //True when program pass completed
            char curCharacter; //Current character selected
            String token = ""; //Current token string
            bool readyForNext = false; //When the FSA is ready for the next character
            int state = 0; //Current state of the FSA

            //Setting up Symbol production
            string symbolName = "";
            string symbolClass = "";
            int value = 0;
            int address = 0;

            curCharacter = (char)reader.Read(); //Read character for first while loop pass

            do //FSA
            {
                while(curCharacter.Equals('\n') || curCharacter.Equals('\r') || curCharacter.Equals('\t'))
                {
                    curCharacter = (char)reader.Read();
                } //If the current char is a newline skip or tab

                switch (state)
                {
                   case 0: //State 0 Looking for the CLASS classification
                        if (!Char.IsLetter(curCharacter)) { //Set the CLASS Symbol
                            state = 1;
                            //Create Token
                            createToken(token.Trim(), "$CLASS");
                            token = "";
                            readyForNext = true;
                        }
                        else //Generate token
                        {
                            token += curCharacter;
                            readyForNext = true;
                        }
                        break;

                    case 1: //State 1 Looking for the name of the class

                        //Getting the name of the program
                        if (!Char.IsLetter(curCharacter) && !Char.IsDigit(curCharacter)) 
                        {
                            state = 2; 
                            symbolClass = createToken(token.Trim(), "<classname>");
                            symbolName = token.Trim();
                            token = "";
                            //Create Class Symbol
                            SymbolTable.addElement(new Symbol(symbolName, symbolClass, null, address, "DS"));
                        }
                        else //Generate Token
                        {
                            token += curCharacter;
                            readyForNext = true;
                        }
                        break;

                    case 2: //State 2 looking for the { delimiter
                        if (curCharacter.Equals('{'))
                        {
                            state = 3;
                            tokenArray[endOfTokenArray, 0] = curCharacter.ToString();
                            tokenArray[endOfTokenArray, 1] = "$LB";
                            endOfTokenArray++;

                            readyForNext = true;
                        }
                        else //Goto next character
                        {
                            readyForNext = true;
                        }

                        break;

                    case 3: //Finding first line that has a delimit space, checking for which state to goto

                        if (!Char.IsLetter(curCharacter) && !token.Trim().Equals(""))
                        {
                            if (token.Trim().Equals("CONST"))
                            {
                                state = 4;
                                symbolClass = createToken(token, "$CONST");
                                token = "";
                                readyForNext = true;
                            }
                            else if (token.Trim().Equals("VAR"))
                            {
                                state = 7;
                                symbolClass = createToken(token, "$VAR");
                                token = "";
                                readyForNext = true;
                            }
                            else if (token.Trim().Equals("IF"))
                            {
                                state = 11;
                                symbolClass = createToken(token, "$IF");
                                token = "";
                                readyForNext = true;
                            }
                            else if(token.Trim().Equals("WHILE"))
                            {
                                state = 14;
                                symbolClass = createToken(token, "$WHILE");
                                token = "";
                                readyForNext = true;
                            }
                            else if (token.Trim().Equals("CALL"))
                            {
                                state = 8;
                                symbolClass = createToken(token, "$CALL");
                                token = "";
                                readyForNext = true;
                            }
                            else //If not a reserved word continue
                            {
                                state = 8;
                            }
                        }
                        else if(curCharacter.Equals('}')) //Checking for brackets in IF or WHILE statements
                        {
                            createToken(curCharacter.ToString(), "$RB");
                            token = "";
                            readyForNext = true;
                        }
                        else if (curCharacter.Equals('{')) //Checking for brackets in IF or WHILE statements
                        {
                            createToken(curCharacter.ToString(), "$LB");
                            token = "";
                            readyForNext = true;
                        }
                        else //Generate Token
                        {
                            token += curCharacter;
                            readyForNext = true;
                        }
                        break;
                    case 4: //Looking for CONST name

                        if (!Char.IsLetter(curCharacter) && !curCharacter.Equals(' ')) //Getting the name
                        {
                            state = 5;
                            symbolClass = createToken(token.Trim(), "<constvar>");
                            symbolName = token.Trim();
                            token = "";
                            endOfTokenArray++;
                        }
                        else //Generating Token
                        {
                            token += curCharacter;
                            readyForNext = true;
                        }
                            break;

                    case 5: //Looking for assignment operator
                        if (curCharacter.Equals('='))
                        {
                            state = 6;
                            createToken(curCharacter.ToString(), "<assign>");
                            token = "";
                            readyForNext = true;
                        }
                        else //Generating Token
                        {
                            token += curCharacter;
                            readyForNext = true;
                        }
                        break;

                    case 6: //Looking for integers and commas
                        if (curCharacter.Equals(',')) //If a comma, set current Const to the int then goto next CONST
                        {
                            symbolClass = createToken(token.Trim(), "<int>"); //Get the int
                            createToken(",", "<constvar>"); //Add a comma
                            token = "";
                            state = 4;
                            readyForNext = true;

                            //Create Symbol
                            SymbolTable.addElement(new Symbol(symbolName, symbolClass, value, address, "CS"));
                            address += 2;
                        }
                        else if(curCharacter.Equals(';')) //If a semi then set current CONST, Go back to State 3
                        {
                            symbolClass = createToken(token.Trim(), "<int>"); //Get the int
                            createToken(token.Trim(), "<constvar>"); //Add the semi
                            token = "";
                            state = 3;
                            readyForNext = true;

                            //Create Symbol
                            SymbolTable.addElement(new Symbol(symbolName, symbolClass, value, address, "CS"));
                            address += 2;
                        }
                        else //Generate Token
                        {
                            token += curCharacter;
                            readyForNext = true;
                        }
                        
                        break;

                    case 7:

                        if (curCharacter.Equals(',')) { //If a comma, set current VAR then move to the next VAR
                            state = 7;
                            symbolClass = createToken(token.Trim(), "<var>"); //Get the var
                            symbolName = token.Trim();
                            createToken(",", "<comma>"); //Add comma
                            token = "";
                            readyForNext = true;
                            //Creating Symbol
                            SymbolTable.addElement(new Symbol(symbolName, symbolClass, "?", address, "CS"));
                            address += 2;
                        }
                        else if (curCharacter.Equals(';')) //If a semi then set current VAR go back to state 3
                        {
                            state = 3;
                            symbolClass = createToken(token.Trim(), "<var>"); //Get the var
                            symbolName = token.Trim();
                            createToken(";", "<constvar>"); //Add the semi
                            token = "";
                            readyForNext = true;
                            //Creating Symbol
                            SymbolTable.addElement(new Symbol(symbolName, symbolClass, "?", address, "CS"));
                            address += 2;
                        }
                        else //Generating Token
                        {
                            token += curCharacter;
                            readyForNext = true;
                        }
                        break;

                    case 8:

                        if (!Char.IsLetter(curCharacter) && !token.Trim().Equals("") && !token.Trim().Equals("{")) 
                        {
                            state = 9;

                            tokenArray[endOfTokenArray, 0] = token.Trim();

                            //Checking for Brackets after reserved words are passed
                            if (token.Trim().Equals("(")) { tokenArray[endOfTokenArray, 1] = "<lb>"; } 
                            else if (token.Trim().Equals(")")) { tokenArray[endOfTokenArray, 1] = "<rb>"; }
                            else if (token.Trim().Equals("{")) { tokenArray[endOfTokenArray, 1] = "$LB"; }
                            else if (token.Trim().Equals("}")) { tokenArray[endOfTokenArray, 1] = "$RB"; }
                            else { tokenArray[endOfTokenArray, 1] = "<var>"; }

                            token = "";
                            endOfTokenArray++;
                        }
                        else if(Char.IsDigit(curCharacter)) //If the current char is a digit then move to state 10 to set integer
                        {
                            state = 10;
                        }
                        else if(curCharacter.Equals(';')) //If the cur character is a semi move to state 3 for next token
                        {
                            state = 3;
                            createToken(curCharacter.ToString(), "<semi>");
                            token = "";
                            readyForNext = true;
                        }
                        else if (curCharacter.Equals('{'))
                        {
                            state = 3;
                            symbolClass = createToken(curCharacter.ToString(), "$LB");
                            token = "";
                            readyForNext = true;
                        }
                        else //Generate Token
                        {
                            token += curCharacter;
                            readyForNext = true;
                        }
                        break;

                    case 9:

                        if (curCharacter.Equals(-1)) //If the current character is null you hit the end of the file end FSA
                        {
                            finished = true;
                        }

                        if (!Char.IsLetter(curCharacter)) //Set operators for statements
                        {
                            if (curCharacter.Equals('=')) 
                            {
                                state = 8;
                                createToken(curCharacter.ToString(), "<assign>");
                                token = "";
                                readyForNext = true;
                            }
                            else if (curCharacter.Equals('+'))
                            {
                                state = 8;
                                createToken(curCharacter.ToString(), "<addop>");
                                token = "";
                                readyForNext = true;
                            }
                            else if (curCharacter.Equals('-'))
                            {
                                state = 8;
                                createToken(curCharacter.ToString(), "<addop>");
                                token = "";
                                readyForNext = true;
                            }
                            else if (curCharacter.Equals('*'))
                            {
                                state = 8;
                                createToken(curCharacter.ToString(), "<mop>");
                                token = "";
                                readyForNext = true;
                            }
                            else if (curCharacter.Equals('/'))
                            {
                                state = 8;
                                createToken(curCharacter.ToString(), "<mop>");
                                token = "";
                                readyForNext = true;
                            }
                            else if (curCharacter.Equals('('))
                            {
                                state = 8;
                                createToken(curCharacter.ToString(), "<lb>");
                                token = "";
                                readyForNext = true;
                            }
                            else if (curCharacter.Equals(')'))
                            {
                                state = 8;
                                createToken(curCharacter.ToString(), "<rb>");
                                token = "";
                                readyForNext = true;
                            }
                            else if (Char.IsDigit(curCharacter))
                            {
                                state = 10;
                            }
                            else if (curCharacter.Equals(';'))
                            {
                                state = 3;
                                createToken(curCharacter.ToString(), "<semi>");
                                token = "";
                                readyForNext = true;
                            }
                            else if(curCharacter.Equals('{'))
                            {
                                state = 8;
                                createToken(curCharacter.ToString(), "$LB");
                                token = "";
                            }
                            else if (curCharacter.Equals('}'))
                            {
                                state = 8;
                            }
                            else
                            {
                                token += curCharacter;
                                readyForNext = true;
                            }
                        }
                        else
                        {
                            token += curCharacter;
                            readyForNext = true;
                        }

                        break;

                    case 10:

                        if (!Char.IsDigit(curCharacter)) //If you find a non digit, set the int
                        {
                            tokenArray[endOfTokenArray, 0] = token.Trim();
                            tokenArray[endOfTokenArray, 1] = "<integer>";
                            symbolName = token.Trim();
                            symbolClass = tokenArray[endOfTokenArray, 1];
                            value = Int32.Parse(token.Trim());
                            token = "";
                            endOfTokenArray++;
                            state = 9;

                            SymbolTable.addElement(new Symbol(symbolName, symbolClass, value, address, "CS"));
                            address += 2;
                        }
                        else //Generate token for the integer
                        {
                            token += curCharacter;
                            readyForNext = true;
                        }
                            
                        break;

                    case 11:

                        if (!Char.IsLetterOrDigit(curCharacter)) //Looking for the different relational operators
                        {
                            if (curCharacter.Equals('<'))
                            {
                                if (reader.Peek().Equals('=')) //Check for <=
                                {
                                    reader.Read(); //Read next character to move forward
                                    createToken("<=", "<relop>");
                                    token = "";
                                    readyForNext = true;
                                }
                                else //only if its just <
                                {
                                    createToken("<", "<relop>");
                                    token = "";
                                    readyForNext = true;
                                }
                            }
                            else if (curCharacter.Equals('>'))
                            {
                                if (reader.Peek().Equals('=')) //Check for >=
                                {
                                    reader.Read(); //Read next character to move forward
                                    createToken(">=", "<relop>");
                                    token = "";
                                    readyForNext = true;
                                }
                                else //only if its just >
                                {
                                    createToken(">", "<relop>");
                                    token = "";
                                    readyForNext = true;
                                }
                            }
                            else if (curCharacter.Equals('='))
                            {
                                if (reader.Peek().Equals('=')) //Check for ==
                                {
                                    reader.Read(); //Read next character to move forward
                                    createToken("==", "<relop>");
                                    token = "";
                                    readyForNext = true;
                                }
                            }
                            else //Generate Token
                            {
                                token += curCharacter;
                                readyForNext = true;
                            }

                        }
                        else
                        {
                            if (Char.IsDigit(curCharacter)) //If a digit find the rest
                            {
                                state = 12;
                            }
                            else if (Char.IsLetter(curCharacter)) //If a letter find the rest
                            {
                                state = 13;
                            }
                        }
                        break;

                    case 12:

                        if(!Char.IsDigit(curCharacter)) //Once you find a non digit set the integer
                        {
                            state = 11;
                            createToken(token.Trim(), "<integer>");
                            token = "";
                            readyForNext = true;
                        }
                        else //Generate Token
                        {
                            token += curCharacter;
                            readyForNext = true;
                        }

                        break;

                    case 13:
                        if (!Char.IsLetter(curCharacter)) //Finding the next non letter
                        {
                            if (token.Trim().Equals("THEN")) //Setting the THEN token
                            {
                                state = 8;
                                createToken(token.Trim(), "$THEN");
                                token = "";
                                break;
                            } //If not THEN set the var

                            state = 11;
                            createToken(token.Trim(), "<var>");
                            token = "";
                            readyForNext = true;
                        }
                        else //Generate Token
                        {
                            token += curCharacter;
                            readyForNext = true;
                        }
                        break;

                    case 14:

                        if (!Char.IsLetterOrDigit(curCharacter)) //Generating relational operators
                        {
                            if (curCharacter.Equals('<'))
                            {
                                if (reader.Peek().Equals('=')) //Check for <=
                                {
                                    reader.Read(); //Read next character to move forward
                                    createToken("<=", "<relop>");
                                    token = "";
                                    readyForNext = true;
                                }
                                else //only if its just <
                                {
                                    createToken("<", "<relop>");
                                    readyForNext = true;
                                }
                            }
                            else if (curCharacter.Equals('>'))
                            {
                                if (reader.Peek().Equals('=')) //Check for >=
                                {
                                    reader.Read(); //Read next character to move forward
                                    createToken(">=", "<relop>");
                                    token = "";
                                    readyForNext = true;
                                }
                                else //only if its just >
                                {
                                    createToken(">", "<relop>");
                                    readyForNext = true;
                                }
                            }
                            else if (curCharacter.Equals('='))
                            {
                                if (reader.Peek().Equals('=')) //Check for ==
                                {
                                    reader.Read(); //Read next character to move forward
                                    createToken("==", "<relop>");
                                    token = "";
                                    readyForNext = true;
                                }
                            }
                            else //Generate Token
                            {
                                token += curCharacter;
                                readyForNext = true;
                            }
                        }
                        else
                        {
                            if (Char.IsDigit(curCharacter)) //If a digit get the rest
                            {
                                state = 15;
                            }
                            else if (Char.IsLetter(curCharacter)) //If a digit get the rest
                            {
                                state = 16;
                            }
                        }
                        break;

                    case 15:
                        if (!Char.IsDigit(curCharacter)) //Setting integer
                        {
                            state = 14;
                            createToken(token.Trim(), "<integer>");
                            token = "";
                            readyForNext = true;
                        }
                        else //Generating Tokens
                        {
                            token += curCharacter;
                            readyForNext = true;
                        }
                        break;

                    case 16:
                        if (!Char.IsLetter(curCharacter)) //Generating tokens for WHILE
                        {
                            if (token.Trim().Equals("DO"))
                            {
                                state = 3;
                                createToken(token.Trim(), "$DO");
                                token = "";
                                createToken(curCharacter.ToString(), "$LB");
                                token = "";
                                readyForNext = true;
                                break;
                            }
                            state = 14;
                            createToken(token.Trim(), "<var>");
                            token = "";
                            readyForNext = true;
                        }
                        else
                        {
                            token += curCharacter;
                            readyForNext = true;
                        }
                        break;

                }

                if(readyForNext) //When the FSA is ready for another character
                {
                    int readByte = reader.Read(); //Reading the byte to get the integer value
                    if (readByte == -1) finished = true; //If the end of the file is found end
                    curCharacter = (char)readByte; //Setting the byte to a char for reading
                    readyForNext = false; //Reseting so the FSA knows we don't want a new token
                }

            } while (!finished); //Continue till finished
            //Print all tokens 3 per line
            int inc = (int)Math.Ceiling(endOfTokenArray / 3.0);
            Console.WriteLine("-------------------------------------------------------------------------------------------------");
            for (int i = 0; i < inc; i++)
            {
                string s = ("Token " + i + ": " + tokenArray[i, 0] + " " + tokenArray[i, 1]).PadRight(30) + " | ";
                Console.Write(s);
                s = ("Token " + (i+inc) + ": " + tokenArray[i + inc, 0] + " " + tokenArray[i + inc, 1]).PadRight(30) + " | ";
                Console.Write(s);
                s = ("Token " + (i + inc * 2) + ": " + tokenArray[i + inc * 2, 0] + " " + tokenArray[i + inc * 2, 1]).PadRight(30) + " | ";
                Console.WriteLine(s);
            }
            Console.WriteLine("--------------------------------------------------------------------------------------------------");
        }

        private static string createToken(string token, string tokenClass)
        {
            tokenArray[endOfTokenArray, 0] = token.Trim(); //Trim token set to the array
            tokenArray[endOfTokenArray, 1] = tokenClass; //Set variable type
            string symbolClass = tokenArray[endOfTokenArray, 1];
            token = "";
            endOfTokenArray++;
            return symbolClass;
        }
    }
}
