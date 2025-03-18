using System;
using System.IO;
using SharpMASM.MNI;

namespace SharpMASM.MNI.Modules
{
    [MNIClass("FileOperations")]
    public class FileOperationsModule
    {
        [MNIFunction(name: "readFile", module: "FileOperations")]
        public static void ReadFile(MNIMethodObject obj)
        {
            try
            {
                // Get filename from memory
                string filename = obj.readString(obj.arg1);
                
                // Destination memory address
                string destination = obj.arg2;
                
                // Read the file content
                string content = File.ReadAllText(filename);
                
                // Write to memory
                obj.writeString(destination, content);
                
                // Set success flag
                obj.setRegister("RFLAGS", 1);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading file: {ex.Message}");
                obj.setRegister("RFLAGS", 0);
            }
        }
        
        [MNIFunction(name: "writeFile", module: "FileOperations")]
        public static void WriteFile(MNIMethodObject obj)
        {
            try
            {
                // Get filename from memory
                string filename = obj.readString(obj.arg1);
                
                // Get content to write
                string content = obj.readString(obj.arg2);
                
                // Write the content to file
                File.WriteAllText(filename, content);
                
                // Set success flag
                obj.setRegister("RFLAGS", 1);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error writing file: {ex.Message}");
                obj.setRegister("RFLAGS", 0);
            }
        }
        
        [MNIFunction(name: "fileExists", module: "FileOperations")]
        public static void FileExists(MNIMethodObject obj)
        {
            try
            {
                // Get filename from memory
                string filename = obj.readString(obj.arg1);
                
                // Check if file exists
                bool exists = File.Exists(filename);
                
                // Set result flag
                obj.setRegister("RFLAGS", exists ? 1 : 0);
            }
            catch (Exception)
            {
                obj.setRegister("RFLAGS", 0);
            }
        }
    }
}
