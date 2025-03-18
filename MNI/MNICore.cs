using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpMASM.MNI
{
    // Attributes for marking MNI classes and methods
    [AttributeUsage(AttributeTargets.Class)]
    public class MNIClassAttribute : Attribute
    {
        public string ModuleName { get; }

        public MNIClassAttribute(string moduleName)
        {
            ModuleName = moduleName;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class MNIFunctionAttribute : Attribute
    {
        public string Name { get; }
        public string Module { get; }

        public MNIFunctionAttribute(string name, string module)
        {
            Name = name;
            Module = module;
        }
    }

    // Core method object that contains all context for MNI execution
    public class MNIMethodObject
    {
        // Arguments passed to the MNI function
        public string arg1 { get; set; }
        public string arg2 { get; set; }
        public string arg3 { get; set; }
        public string arg4 { get; set; }

        private MappedMemoryFile _memory;

        public MNIMethodObject(MappedMemoryFile memory)
        {
            _memory = memory;
        }

        // Methods for reading and writing values
        public string readString(string location)
        {
            // Parse location which could be a register or memory address
            if (location.StartsWith("R"))
            {
                // Register - get the value which points to a memory address
                long registerValue = _memory.Read(location);
                return _memory.Read_String(registerValue.ToString());
            }
            else if (location.StartsWith("$"))
            {
                // Direct memory address
                string addressStr = location.Substring(1);
                if (long.TryParse(addressStr, out long address))
                {
                    return _memory.Read_String(address.ToString());
                }
            }
            else if (long.TryParse(location, out long directAddress))
            {
                // Direct number (memory address)
                return _memory.Read_String(directAddress.ToString());
            }

            throw new ArgumentException($"Invalid location format: {location}");
        }

        public int readInteger(string location)
        {
            // Parse location which could be a register or memory address
            if (location.StartsWith("R"))
            {
                // Register - get the value directly
                return (int)_memory.Read(location);
            }
            else if (location.StartsWith("$"))
            {
                // Direct memory address
                string addressStr = location.Substring(1);
                if (long.TryParse(addressStr, out long address))
                {
                    return (int)MappedMemoryFile.ReadMemory(address);
                }
            }
            else if (long.TryParse(location, out long directValue))
            {
                // Direct number
                return (int)directValue;
            }

            throw new ArgumentException($"Invalid location format: {location}");
        }

        public void writeString(string location, string value)
        {
            if (location.StartsWith("R"))
            {
                // Register contains a memory address where to write
                long address = _memory.Read(location);
                _memory.Write_String(address.ToString(), value);
            }
            else if (location.StartsWith("$"))
            {
                // Direct memory address
                string addressStr = location.Substring(1);
                if (long.TryParse(addressStr, out long address))
                {
                    _memory.Write_String(address.ToString(), value);
                }
            }
            else if (long.TryParse(location, out long directAddress))
            {
                // Direct number as address
                _memory.Write_String(directAddress.ToString(), value);
            }
            else
            {
                throw new ArgumentException($"Invalid location format: {location}");
            }
        }

        public void writeInteger(string location, int value)
        {
            if (location.StartsWith("R"))
            {
                // Write to register
                _memory.Write(location, value);
            }
            else if (location.StartsWith("$"))
            {
                // Write to memory address
                string addressStr = location.Substring(1);
                if (long.TryParse(addressStr, out long address))
                {
                    _memory.WriteLong(address, value);
                }
            }
            else
            {
                throw new ArgumentException($"Invalid location format: {location}");
            }
        }

        public void setRegister(string register, long value)
        {
            _memory.Write(register, value);
        }

        public long getRegister(string register)
        {
            return _memory.Read(register);
        }
    }
}
