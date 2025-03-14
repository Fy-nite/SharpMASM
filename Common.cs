/*
 * Common.cs
 * 
 * Description: Common class for the SharpMASM project.
 *
 * (C) Finite, 2025
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.IO.MemoryMappedFiles;
namespace SharpMASM
{


    public class instruction
    {
        public string name;
        public string[] args;
        public instruction(string name, string[] args)
        {
            this.name = name;
            this.args = args;
        }
    }

    public class Instructions
    {
        public static MappedMemoryFile Memory = MappedMemoryFile.GetInstance(Common.MappedFile);
        public static Dictionary<string, long> Labels = new Dictionary<string, long>();
        public static List<instruction> instructions = new List<instruction>();
        public static void AddInstruction(string name, string[] args)
        {
            instructions.Add(new instruction(name, args));
        }
        public static void ClearInstructions()
        {
            instructions.Clear();
        }

    }

    public class Common
    {
        public static string Version = "1.0.0";
        public static string Author = "Finite";
        // for now, 64mib is enough
        public static ulong MemorySize = 64 * 1024 * 1024;
        public static ulong MemoryUsed = 0;
        public static ulong MemoryFree = 0;
        public static bool IsMemoryFull = false;
        public static bool exitOnHLT = true;

        public static string[] Registers = new string[] { "RAX", "RBX", "RCX", "RDX", "RBP", "RSP", "RDI", "RSI", "R0", "R1", "R2", "R3", "R4", "R5", "R6", "R7", "R8", "R9", "R10", "R11", "R12", "R13", "R14", "R15" };
        public static string[] Instructions = new string[] { "MOV", "ADD", "SUB", "MUL", "DIV", "AND", "OR", "XOR", "NOT", "SHL", "SHR", "CMP", "JMP", "JE", "JNE", "CALL", "RET", "HLT", "NOP", "PUSH", "POP", "INC", "DEC", "calle", "callne", "ENTER", "LEAVE" };
        public static string MappedFile = "FiniteSharpMasmMemory";
        public static MappedMemoryFile Memory = MappedMemoryFile.GetInstance(MappedFile);
        public static Dictionary<string, long> Labels = new Dictionary<string, long>();
        public static List<instruction> instructions = new List<instruction>();
        public static void AddInstruction(string name, string[] args)
        {
            instructions.Add(new instruction(name, args));
        }
        public static Common? Instance { get; set; }

        public static Common GetInstance()
        {
            if (Instance == null)
            {
                Instance = new Common();
            }
            return Instance;
        }

        public static string[] Split(string input)
        {
            return input.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
