using System.Collections.Generic;
using System.Text;

namespace System.IO.Compression
{
	public class ZipStorer : IDisposable
	{
		public enum Compression : ushort
		{
			Store = 0,
			Deflate = 8
		}

		public struct ZipFileEntry
		{
			public Compression Method;

			public string FilenameInZip;

			public uint FileSize;

			public uint CompressedSize;

			public uint HeaderOffset;

			public uint FileOffset;

			public uint HeaderSize;

			public uint Crc32;

			public DateTime ModifyTime;

			public string Comment;

			public bool EncodeUTF8;

			public override string ToString()
			{
				return null;
			}
		}

		public bool EncodeUTF8;

		public bool ForceDeflating;

		private List<ZipFileEntry> Files;

		private string FileName;

		private Stream ZipFileStream;

		private string Comment;

		private byte[] CentralDirImage;

		private ushort ExistingFiles;

		private FileAccess Access;

		private static uint[] CrcTable;

		private static Encoding DefaultEncoding;

		static ZipStorer()
		{
		}

		public static ZipStorer Create(string _filename, string _comment)
		{
			return null;
		}

		public static ZipStorer Create(Stream _stream, string _comment)
		{
			return null;
		}

		public static ZipStorer Open(string _filename, FileAccess _access)
		{
			return null;
		}

		public static ZipStorer Open(Stream _stream, FileAccess _access)
		{
			return null;
		}

		public void AddFile(Compression _method, string _pathname, string _filenameInZip, string _comment)
		{
		}

		public void AddStream(Compression _method, string _filenameInZip, Stream _source, DateTime _modTime, string _comment)
		{
		}

		public void Close()
		{
		}

		public List<ZipFileEntry> ReadCentralDir()
		{
			return null;
		}

		public bool ExtractFile(ZipFileEntry _zfe, string _filename)
		{
			return false;
		}

		public bool ExtractFile(ZipFileEntry _zfe, Stream _stream)
		{
			return false;
		}

		public static bool RemoveEntries(ref ZipStorer _zip, List<ZipFileEntry> _zfes)
		{
			return false;
		}

		private uint GetFileOffset(uint _headerOffset)
		{
			return 0u;
		}

		private void WriteLocalHeader(ref ZipFileEntry _zfe)
		{
		}

		private void WriteCentralDirRecord(ZipFileEntry _zfe)
		{
		}

		private void WriteEndRecord(uint _size, uint _offset)
		{
		}

		private void Store(ref ZipFileEntry _zfe, Stream _source)
		{
		}

		private uint DateTimeToDosTime(DateTime _dt)
		{
			return 0u;
		}

		private DateTime DosTimeToDateTime(uint _dt)
		{
			return default;
		}

		private void UpdateCrcAndSizes(ref ZipFileEntry _zfe)
		{
		}

		private string NormalizedFilename(string _filename)
		{
			return null;
		}

		private bool ReadFileInfo()
		{
			return false;
		}

		public void Dispose()
		{
		}
	}
}
