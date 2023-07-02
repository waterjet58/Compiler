using System;
using System.IO;

namespace CompilerProject
{
    class GenerateMASM
    {
        public static string asmFile;
        private static string fileName = "C:\\Users\\Zach\\source\\repos\\CompilerProject\\CompilerProject\\Assembly.asm";
        private static string topAppendPath = "C:\\Users\\Zach\\source\\repos\\CompilerProject\\CompilerProject\\AppendTop.txt";
        private static string dataAppendPath = "C:\\Users\\Zach\\source\\repos\\CompilerProject\\CompilerProject\\AppendData.txt";
        private static string dataVarAppendPath = "C:\\Users\\Zach\\source\\repos\\CompilerProject\\CompilerProject\\AppendVarData.txt";
        private static string IOAppendPath = "C:\\Users\\Zach\\source\\repos\\CompilerProject\\CompilerProject\\AppendIO.txt";
        public static void compile()
        {
            createTopSegment();
            createDataSegment();
            addCodeSegment();
            addIO();
            File.WriteAllText(fileName, asmFile);
        }
        private static void createTopSegment()
        {
            asmFile = File.ReadAllText(topAppendPath);
        }
        private static void createDataSegment()
        {
            asmFile += ".data\n";
            foreach(Symbol s in SymbolTable.symbolTable)
            {
                if(s != null)
                {
                    if ((s.Class.Equals("$VAR") || s.Class.Equals("$CONST") || s.Class.Equals("<integer>")) && !s.Value.Equals("?"))
                    {
                        if(s.Class.Equals("<integer>") && !asmFile.Contains("Lit" + s.Name))
                            asmFile += "Lit" + s.Name + " dd " + s.Value + "\n";
                        else if(!s.Class.Equals("<integer>"))
                            asmFile += s.Name + " dd " + s.Value + "\n";
                    }
                }
            }

            asmFile += File.ReadAllText(dataAppendPath);
            asmFile += ".data?\n";

            foreach(Symbol s in SymbolTable.symbolTable)
            {
                if (s != null)
                {
                    if (s.Class.Equals("<var>") && s.Value.Equals("?"))
                    {
                        asmFile += s.Name + " dd " + s.Value + "\n";
                    }
                }
            }
            for(int i = 1; i < PDA.tempIndex; i++)
            {
                asmFile += "T" + i + " dd ?\n";
            }
            asmFile += File.ReadAllText(dataVarAppendPath);
        }
        private static void addCodeSegment()
        {
            asmFile += ".code\n";
            asmFile += "main proc\n";
            foreach (String s in GenerateCode.codeSegment)
            {
                asmFile += s + "\n";
            }
        }
        private static void addIO()
        {
            asmFile += File.ReadAllText(IOAppendPath);
        }
    }
}
