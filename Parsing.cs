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
     * this will include the <root>/stdio/print.masm file
     * 
     */

    // instruction class
    public class Parsing
    {

    }
}
