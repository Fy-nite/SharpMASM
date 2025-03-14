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
            // open file and get its size
            using (var fileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                _fileSize = fileStream.Length;
                // Create a memory mapped file with the same size
                _mappedFile = MemoryMappedFile.CreateFromFile(
                    fileStream,
                    null, // No name
                    _fileSize > 0 ? _fileSize : 4096, // Use at least 4KB if file is empty
                    MemoryMappedFileAccess.ReadWrite,
                    HandleInheritability.None,
                    false); // Leave the file open
            }

            // Create an accessor to read/write
            _accessor = _mappedFile.CreateViewAccessor(0, _fileSize);
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
