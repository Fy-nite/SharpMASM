using SharpMASM;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpMASM
{
    public class MappedMemoryFile : IDisposable
    {
        private MemoryMappedFile _mappedFile;
        private MemoryMappedViewAccessor _accessor;
        private long _fileSize;
        private bool _disposed = false;
        public static MappedMemoryFile Instance { get; set; }

        public static MappedMemoryFile GetInstance(string filename)
        {
            if (Instance == null)
            {
                Instance = new MappedMemoryFile(filename);
            }
            return Instance;
        }

        // memory map a file to act as a long array
        public MappedMemoryFile(string filename)
        {
            // Define a minimum size for the memory-mapped file (1MB should be plenty)
            long minSize = 1024 * 1024; // 1MB

            // open file and get its size
            using (var fileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                _fileSize = fileStream.Length;
                if (_fileSize < minSize)
                {
                    // Resize the file to ensure it's large enough
                    fileStream.SetLength(minSize);
                    _fileSize = minSize;
                }

                // Create a memory mapped file with the same size
                _mappedFile = MemoryMappedFile.CreateFromFile(
                    fileStream,
                    null, // No name
                    _fileSize,
                    MemoryMappedFileAccess.ReadWrite,
                    HandleInheritability.None,
                    false); // Leave the file open
            }

            // Create an accessor to read/write
            _accessor = _mappedFile.CreateViewAccessor(0, _fileSize);
            
            if (CmdArgs.GetInstance().VeryVerbose)
            {
                Console.WriteLine($"Memory mapped file initialized with size: {_fileSize} bytes");
            }
        }

        // Read a long at the specified position
        public long ReadLong(long position)
        {
            if (position < 0 || position + sizeof(long) > _fileSize)
                throw new ArgumentOutOfRangeException(nameof(position));

            _accessor.Read(position, out long value);
            return value;
        }

        // Write a long at the specified position
        public void WriteLong(long position, long value)
        {
            if (position < 0 || position + sizeof(long) > _fileSize)
                throw new ArgumentOutOfRangeException(nameof(position));

            _accessor.Write(position, value);
        }

        public static void WriteMemory(long position, long value)
        {
            Instance.WriteLong(position, value);
        }

        public static long ReadMemory(long position)
        {
            return Instance.ReadLong(position);
        }
   

        // Improve Read method to handle non-register cases better
        public long Read(string register)
        {
            if (Common.Registers.Contains(register))
            {
                return Instance.ReadLong(Array.IndexOf(Common.Registers, register) * sizeof(long));
            }
            else if (register.StartsWith("$"))
            {
                // Parse memory address without the $ sign
                if (long.TryParse(register.Substring(1), out long address))
                {
                    // Ensure address is valid (convert to byte position)
                    long bytePosition = address * sizeof(long);
                    if (bytePosition >= 0 && bytePosition < _fileSize)
                    {
                        return Instance.ReadLong(bytePosition);
                    }
                    else
                    {
                        throw new MASMException($"Memory address out of range: {register} (byte position {bytePosition}, file size {_fileSize})");
                    }
                }
                else
                {
                    throw new MASMException($"Invalid memory address format: {register}");
                }
            }
            else if (long.TryParse(register, out long value))
            {
                return value;
            }
            else
            {
                throw new MASMException($"Invalid register or memory address: {register}");
            }
        }

        // Improve Write method to handle memory addresses properly
        public void Write(string register, long value)
        {
            if (Common.Registers.Contains(register))
            {
                long position = Array.IndexOf(Common.Registers, register) * sizeof(long);
                Instance.WriteLong(position, value);
            }
            else if (register.StartsWith("$"))
            {
                // Parse memory address without the $ sign
                if (long.TryParse(register.Substring(1), out long address))
                {
                    // Ensure address is valid (convert to byte position)
                    long bytePosition = address * sizeof(long);
                    if (bytePosition >= 0 && bytePosition < _fileSize)
                    {
                        Instance.WriteLong(bytePosition, value);
                    }
                    else
                    {
                        throw new MASMException($"Memory address out of range: {register} (byte position {bytePosition}, file size {_fileSize})");
                    }
                }
                else
                {
                    throw new MASMException($"Invalid memory address format: {register}");
                }
            }
            else
            {
                throw new MASMException($"Invalid register or memory address: {register}");
            }
        }

        public void Write_String(string startingRegister, string value)
        {
            if (Common.Registers.Contains(startingRegister))
            {
                long startingIndex = Array.IndexOf(Common.Registers, startingRegister);
                for (int i = 0; i < value.Length; i++)
                {
                    Instance.WriteLong(startingIndex * sizeof(long) + i, value[i]);
                }
            }
            else
            {
                throw new MASMException("Invalid register: " + startingRegister);
            }
        }
        public string Read_String(string startingRegister)
        {
            // read a string from memory till the first 0 or \0, which ever we set for our null terminator
            if (Common.Registers.Contains(startingRegister))
            {
                long startingIndex = Array.IndexOf(Common.Registers, startingRegister);
                StringBuilder sb = new StringBuilder();
                while (Instance.ReadLong(startingIndex * sizeof(long)) != 0)
                {
                    sb.Append((char)Instance.ReadLong(startingIndex * sizeof(long)));
                    startingIndex++;
                }
                return sb.ToString();

            }
            // could not be a register and instead a number or memory address
            else if (startingRegister.StartsWith("$"))
            {
                // Parse memory address without the $ sign
                if (long.TryParse(startingRegister.Substring(1), out long address))
                {
                    // Ensure address is valid (convert to byte position)
                    long bytePosition = address * sizeof(long);
                    if (bytePosition >= 0 && bytePosition < _fileSize)
                    {
                        StringBuilder sb = new StringBuilder();
                        while (Instance.ReadLong(bytePosition) != 0)
                        {
                            sb.Append((char)Instance.ReadLong(bytePosition));
                            bytePosition++;
                        }
                        return sb.ToString();
                    }
                    else
                    {
                        throw new MASMException($"Memory address out of range: {startingRegister} (byte position {bytePosition}, file size {_fileSize})");
                    }
                }
                else
                {
                    throw new MASMException($"Invalid memory address format: {startingRegister}");
                }
            }
            else if (long.TryParse(startingRegister, out long value))
            {
                return value.ToString();
            }
            else
            {
                throw new MASMException($"Invalid register or memory address: {startingRegister}");
            }
           
            
        }

        // Indexer to use the file as an array
        public long this[long index]
        {
            get => ReadLong(index * sizeof(long));
            set => WriteLong(index * sizeof(long), value);
        }

        // Get number of longs that can fit in the file
        public long Length => _fileSize / sizeof(long);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _accessor?.Dispose();
                    _mappedFile?.Dispose();
                }
                _disposed = true;
            }
        }

        ~MappedMemoryFile()
        {
            Dispose(false);
        }
    }
}
