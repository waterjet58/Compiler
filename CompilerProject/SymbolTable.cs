using System;

namespace CompilerProject
{
    public class Symbol
    {
        public string Name { get; private set; }
        public string Class { get; private set; }
        public object Value { get; private set; }
        public int Address { get; private set; }
        public string Segment { get; private set; }
        public Symbol(string Name, string Class, object Value, int Address, string Segment)
        {
            this.Name = Name;
            this.Class = Class;
            this.Value = Value;
            this.Address = Address;
            this.Segment = Segment;
        }
    }
    public static class SymbolTable
    {
        public static Symbol[] symbolTable = new Symbol[100];
        public static int endOfTable = 0;
        public static void addElement(Symbol newSymbol)
        {
            symbolTable[endOfTable++] = newSymbol; 
        }
        public static Symbol getElement(String Name)
        {
            for(int i = 0; i < endOfTable; i++)
            {
                if(symbolTable[i].Name.Equals(Name))
                {
                    return symbolTable[i];
                }
            }
            return null;
        }
        public static string toString()
        {
            string s = "";

            for(int i = 0; i < endOfTable; i++)
            {
                s += "Name: " + symbolTable[i].Name.PadRight(15) + " | Class: " + symbolTable[i].Class.PadRight(15) + (" | value: " + symbolTable[i].Value).PadRight(15)
                    + " | address: " + symbolTable[i].Address + " | Segment: " + symbolTable[i].Segment + "\n";
            }
            return s;
        }

    }
}
