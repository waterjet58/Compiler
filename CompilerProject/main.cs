using System;

namespace CompilerProject
{
    class main
    {
        static void Main(string[] args)
        {
            Console.Write("Enter program path: "); 
            String path = "program2.txt"; //Comment out for user input
            Console.WriteLine();
            //String path = Console.ReadLine(); //remove comment for user input

            //Generate Symbol table based off the program given
            LexicalAnalysis.generateSymbolTable("C:\\Users\\Zach\\source\\repos\\CompilerProject\\CompilerProject\\" + path);
            //Display the given Symbol table
            Console.WriteLine(SymbolTable.toString());
            //Generate the quads based off the Tokens generated
            PDA.generateQuads();

            //Generate the MASM code from the given quads
            GenerateCode.generate();

            //Print out the code segments available
            foreach(string s in GenerateCode.codeSegment)
            {
                Console.WriteLine(s);
            }
            //Compile all of the data into one file
            GenerateMASM.compile();
            //Wait for user input to end program
            Console.ReadLine();
        }
    }
}

