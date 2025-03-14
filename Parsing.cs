using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMASM
{

    /*
     * Parsing.cs
     * 
     * Description: Parsing class for the SharpMASM project.
     *
     * (C) Finite, 2025
     */


    /*
     * Micro-Assembly has some great features like labels, comments, and instructions.
     * this class helps parse out the instructions and labels from the source code.
     * 
     * labels aare defined by 
     * lbl <name>
     * 
     * and are added to the label map.
     * these are preprocess steps before the actual execution of each instruction.
     * 
     * we also have include statements
     * #include "file"
     * 
     * lets say we want a stdio function right?
     * #include "stdio.print"
     * this will include the <root>/stdio/print.masm file from Common.root
     * if common.root is not set, it will look in the current directory that the program is running in.
     * 
     */

    // instruction class
    public class Parsing
    {
        // label map
        public static Dictionary<string, int> labelMap = new Dictionary<string, int>();

        // include map
        public static Dictionary<string, string> includeMap = new Dictionary<string, string>();

        // parse the source code
        public static void Parse(string source)
        {
            // split the source code by new line
            string[] lines = source.Split('\n');

            // iterate through each line
            for (int i = 0; i < lines.Length; i++)
            {
                // get the line
                string line = lines[i];

                // check if the line is a label
                if (line.StartsWith("lbl"))
                {
                    // get the label name
                    string label = line.Split(' ')[1];

                    // add the label to the label map
                    labelMap.Add(label, i);
                }

                // check if the line is an include
                if (line.StartsWith("#include"))
                {
                    // get the include file
                    string include = line.Split(' ')[1];

                    // add the include to the include map
                    includeMap.Add(include, include);
                }
            }
        }
        public static string GetLabel(string label)
        {
            // check if the label exists in the label map
            if (labelMap.ContainsKey(label))
            {
                // return the label
                return labelMap[label].ToString();
            }

            // return null if the label does not exist
            return null;
        }
    

    }
}
