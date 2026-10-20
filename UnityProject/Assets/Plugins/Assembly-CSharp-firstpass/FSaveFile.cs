using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (FSaveFile @ 0x16644ec..0x1667760, Ghidra vaddr).
public class FSaveFile
{
	public byte CycleCount = 3;

	private static byte[] cSalt;

	private static byte[] cXor;

	[CompilerGenerated]
	private static Action m_BadHashEvent;

	public static string PersistentDataPath;

	protected byte[] mSalt;

	protected byte[] mXor;

	protected string mBaseName;

	protected string mName;

	protected string mCycleFilename;

	protected string mFileName;

	private string mBackupPlayerPrefsKey;

	protected string mBinFileName;

	protected string mSignatureFileName;

	protected MemoryStream mDataStream;

	protected bool mCompress = true;

	protected bool mObfuscate = true;

	protected bool mPreferKeychain = true;

	protected bool mFileBackup = true;

	protected bool mUseKeychain = true;

	protected bool mUseFile = true;

	protected byte mCyclePosition;

	protected byte[] mCompressedBytes;

	public bool FromKeychain;

	protected object mLock = new object();

	protected bool mSaveInProgress;

	public byte CyclePosition => mCyclePosition;

	public bool SaveInProgess => mSaveInProgress;

	public MemoryStream DataStream => mDataStream;

	public string DataAsUTF8String
	{
		get
		{
			lock (mLock)
			{
				if (mDataStream == null)
				{
					return null;
				}
				return Encoding.UTF8.GetString(mDataStream.ToArray());
			}
		}
	}

	public static event Action BadHashEvent;

	static FSaveFile()
	{
		cSalt = Encoding.ASCII.GetBytes("tWDhcKr5l2zOtZnevE/QZ/X3RR9Dkr2LJo888rrsLOLrs4KU+lQHJsNadIREqmoWjZTy7FrBHEgWZloc67vxevXRqH2wmOkVJ3tUAfNihTAFD8L0gfTX0Q+4m55AeXliBa13IVNJbFytsqc+WoJzFuy/70oxh2HZj38C7jFHhZw=");
		cXor = Encoding.ASCII.GetBytes("PWiJLfmulLIe9zdlcdIyRrS6T2iRB5RwQRD+FuL9XUr+/8Pka9vEPlsOqMsteIQhwfgAv/72nE/H4MZMaYfC9Z4RNpiNaaguuDeOiiarguTXtGZN5RkLZ08YbDUXPW4/HH6ulxAYgW/M4/xxYyKfZyNG5cydLKfoJlfrNl7Rg30=");
		PersistentDataPath = FUtil.PersistentDataPath;
	}

	public FSaveFile(string name, bool compress = true, bool obfuscate = true, bool preferKeychain = true, bool fileBackup = true, bool cycle = true, string salt = null, string xor = null)
	{
		mBaseName = name;
		mCycleFilename = Path.Combine(PersistentDataPath, mBaseName + ".cyc");
		mCompress = compress;
		mObfuscate = obfuscate;
		mPreferKeychain = preferKeychain;
		mFileBackup = fileBackup;
		if (!preferKeychain)
		{
			mUseKeychain = false;
			mUseFile = true;
		}
		else
		{
			mUseKeychain = FKeychain.Available;
			mUseFile = !mUseKeychain || mFileBackup;
		}
		mSalt = (byte[])cSalt.Clone();
		byte[] saltKey = Encoding.ASCII.GetBytes(salt ?? mBaseName);
		for (int i = 0; i < saltKey.Length; i++)
		{
			mSalt[i] ^= saltKey[i % saltKey.Length];
		}
		mXor = (byte[])cXor.Clone();
		byte[] xorKey = Encoding.ASCII.GetBytes(xor ?? mBaseName);
		for (int j = 0; j < xorKey.Length; j++)
		{
			mXor[j] ^= xorKey[j % xorKey.Length];
		}
		if (!cycle)
		{
			CycleCount = 1;
		}
		DetermineCyclePosition();
		UpdateFilenames(mCyclePosition);
	}

	protected void DetermineCyclePosition()
	{
		FLog.UnityLog("FSaveFile.DetermineCyclePosition: " + mBaseName);
		byte[] array;
		if (!mUseKeychain)
		{
			array = null;
		}
		else
		{
			array = FKeychain.GetBytes(mBaseName + ".cyc");
			FLog.UnityLog("FSaveFile.DetermineCyclePosition: " + mBaseName + ", from keychain, " + ((array != null) ? "success" : "failed"));
			if (array != null && array.Length == 1)
			{
				mCyclePosition = array[0];
				FLog.UnityLog("FSaveFile.DetermineCyclePosition: " + mBaseName + ", got value, " + mCyclePosition);
				return;
			}
		}
		if (mUseFile && File.Exists(mCycleFilename))
		{
			array = File.ReadAllBytes(mCycleFilename);
			FLog.UnityLog("FSaveFile.DetermineCyclePosition: " + mBaseName + ", from file: " + ((array != null) ? "success" : "failed"));
		}
		if (array != null && array.Length == 1)
		{
			mCyclePosition = array[0];
			FLog.UnityLog("FSaveFile.DetermineCyclePosition: " + mBaseName + ", got value, " + mCyclePosition);
			return;
		}
		mCyclePosition = 0;
		FLog.UnityLog("FSaveFile.DetermineCyclePosition: " + mBaseName + ", defaulting to 0");
	}

	protected void UpdateFilenames(byte cyclePosition)
	{
		mName = mBaseName + ((cyclePosition == 0) ? "" : ("_" + cyclePosition));
		mFileName = Path.Combine(PersistentDataPath, mName);
		mBinFileName = mFileName + ".bin";
		mSignatureFileName = mFileName + "_sig";
		mBackupPlayerPrefsKey = mName + "Backup";
	}

	public void RewindCyclePosition()
	{
		mCyclePosition = (byte)((mCyclePosition == 0) ? (CycleCount - 1) : (mCyclePosition - 1));
		UpdateFilenames(mCyclePosition);
		SaveCyclePosition(mCyclePosition);
	}

	protected bool SaveCyclePosition(byte cyclePosition)
	{
		FLog.UnityLog("FSaveFile.SaveCyclePosition: " + mBaseName + ", " + cyclePosition);
		try
		{
			byte[] array = new byte[1] { cyclePosition };
			if (mUseKeychain)
			{
				FKeychain.SetBytes(mBaseName + ".cyc", array);
			}
			if (mUseFile)
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(File.Create(mCycleFilename)))
				{
					binaryWriter.Write(array);
					binaryWriter.Close();
				}
			}
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogWarning("Open file for writing failed: " + mCycleFilename + " " + ex.Message);
			return false;
		}
		mCyclePosition = cyclePosition;
		return true;
	}

	public bool Load(bool fromPlayerPrefs = false)
	{
		lock (mLock)
		{
			byte[] array = null;
			bool gotBytes = false;
			if (!fromPlayerPrefs)
			{
				if (mUseKeychain)
				{
					array = FKeychain.GetBytes(mName);
					if (array != null)
					{
						FLog.Log("FSaveFile.Load: " + mName + ", from keychain: success");
						FromKeychain = true;
						gotBytes = true;
					}
					else
					{
						FLog.Log("FSaveFile.Load: " + mName + ", from keychain: failed");
						FromKeychain = false;
					}
				}
			}
			else if (PlayerPrefs.HasKey(mBackupPlayerPrefsKey))
			{
				array = Convert.FromBase64String(PlayerPrefs.GetString(mBackupPlayerPrefsKey));
				gotBytes = array != null;
			}
			if (!gotBytes)
			{
				if (mUseFile)
				{
					string path = ((!mCompress && !mObfuscate) ? mFileName : mBinFileName);
					try
					{
						array = File.ReadAllBytes(path);
					}
					catch (Exception)
					{
						array = null;
					}
					FLog.Log("FSaveFile.Load: " + mName + ", from file: " + ((array != null) ? "success" : "failed"));
				}
				else
				{
					array = null;
				}
			}
			if (mObfuscate)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i] ^= mXor[i % mXor.Length];
				}
			}
			SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();
			MemoryStream memoryStream = new MemoryStream(array);
			if (!mCompress)
			{
				mDataStream = memoryStream;
			}
			else
			{
				mDataStream = new MemoryStream();
				byte[] properties = new byte[5];
				memoryStream.Read(properties, 0, 5);
				decoder.SetDecoderProperties(properties);
				byte[] sizeBytes = new byte[8];
				memoryStream.Read(sizeBytes, 0, 8);
				long outSize = BitConverter.ToInt64(sizeBytes, 0);
				decoder.Code(memoryStream, mDataStream, memoryStream.Length, outSize, null);
			}
			mDataStream.Position = 0L;
			string text = FCoreUtil.SHA1HashToString(mDataStream.ToArray().Concat(mSalt));
			string text2 = null;
			if (mUseKeychain)
			{
				text2 = FKeychain.GetString(mName + "_sig", false);
			}
			if (string.IsNullOrEmpty(text2) && mUseFile && File.Exists(mSignatureFileName))
			{
				using (StreamReader streamReader = new StreamReader(mSignatureFileName))
				{
					text2 = streamReader.ReadLine().Trim();
				}
			}
			if (text2 != text && m_BadHashEvent != null)
			{
				m_BadHashEvent();
			}
			return true;
		}
	}

	public bool DoesSaveFileExist()
	{
		if (mUseKeychain && FKeychain.GetBytes(mName) != null)
		{
			return true;
		}
		if (mUseFile)
		{
			return File.Exists((!mCompress && !mObfuscate) ? mFileName : mBinFileName);
		}
		return false;
	}

	public void Save(XmlElement root, bool mainThread = false)
	{
		lock (mLock)
		{
			mSaveInProgress = true;
			if (!mainThread)
			{
				new Thread(SaveWorkerXml).Start(root);
			}
			else
			{
				SaveWorkerXml(root);
			}
		}
	}

	public void SaveWorkerXml(object rootObj)
	{
		lock (mLock)
		{
			XmlElement xmlElement = rootObj as XmlElement;
			if (xmlElement != null)
			{
				SaveWorkerString(xmlElement.ConvertDocToString(true));
				return;
			}
			mSaveInProgress = false;
			FLog.Warning("SAVEDATALOGGING: FSaveFile.SaveWorkerXml: root is null!", true);
		}
	}

	public void Save(string str, bool mainThread = false)
	{
		lock (mLock)
		{
			mSaveInProgress = true;
			if (!mainThread)
			{
				new Thread(SaveWorkerString).Start(str);
			}
			else
			{
				SaveWorkerString(str);
			}
		}
	}

	public void SaveWorkerString(object stringObj)
	{
		lock (mLock)
		{
			string text = stringObj as string;
			if (text == null)
			{
				mSaveInProgress = false;
				FLog.Warning("SAVEDATALOGGING: FSaveFile.SaveWorkerString: str is null!", true);
				return;
			}
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream);
			streamWriter.Write(text);
			streamWriter.Flush();
			memoryStream.Position = 0L;
			UnityEngine.Debug.Log("In SaveWorkerString, written stream");
			SaveWorker(memoryStream);
		}
	}

	public void Save(MemoryStream stream, bool mainThread = false)
	{
		lock (mLock)
		{
			mSaveInProgress = true;
			if (!mainThread)
			{
				new Thread(SaveWorker).Start(stream);
			}
			else
			{
				SaveWorker(stream);
			}
		}
	}

	public void SaveWorker(object streamObj)
	{
		lock (mLock)
		{
			byte b = (byte)((mCyclePosition + 1) % CycleCount);
			UpdateFilenames(b);
			MemoryStream memoryStream = streamObj as MemoryStream;
			if (memoryStream == null)
			{
				mSaveInProgress = false;
				FLog.Warning("SAVEDATALOGGING: FSaveFile.SaveWorker: stream is null!", true);
				return;
			}
			SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
			MemoryStream memoryStream2 = memoryStream;
			if (mCompress)
			{
				memoryStream2 = new MemoryStream();
				encoder.WriteCoderProperties(memoryStream2);
				memoryStream2.Write(BitConverter.GetBytes(memoryStream.Length), 0, 8);
				encoder.Code(memoryStream, memoryStream2, -1L, -1L, null);
			}
			mCompressedBytes = memoryStream2.ToArray();
			if (mObfuscate)
			{
				for (int i = 0; i < mCompressedBytes.Length; i++)
				{
					mCompressedBytes[i] ^= mXor[i % mXor.Length];
				}
			}
			if (mUseKeychain)
			{
				FKeychain.SetBytes(mName, mCompressedBytes);
			}
			if (mUseFile)
			{
				string path = ((!mCompress && !mObfuscate) ? mFileName : mBinFileName);
				try
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(File.Create(path)))
					{
						binaryWriter.Write(mCompressedBytes);
						binaryWriter.Close();
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogWarning("Open file for writing failed: " + mBinFileName + " " + ex.Message);
					mSaveInProgress = false;
					return;
				}
			}
			FLog.Log("saved file: " + mName);
			string text = FCoreUtil.SHA1HashToString(memoryStream.ToArray().Concat(mSalt));
			if (mUseKeychain)
			{
				FKeychain.SetString(mName + "_sig", text, false);
			}
			if (mUseFile)
			{
				using (StreamWriter streamWriter = File.CreateText(mSignatureFileName))
				{
					streamWriter.Write(text);
					streamWriter.Close();
				}
			}
			FLog.Log("saved file sig for " + mName + ": " + text);
			SaveCyclePosition(b);
			FLog.Log("Cycle position for " + mName + " is now " + b);
			mSaveInProgress = false;
		}
	}

	public void SavePlayerPrefsBackup()
	{
		if (mCompressedBytes != null)
		{
			PlayerPrefs.SetString(mBackupPlayerPrefsKey, Convert.ToBase64String(mCompressedBytes));
		}
	}

	public void Delete()
	{
		if (mUseKeychain)
		{
			throw new Exception(string.Format("FSaveFile.Delete doesn't work on the keychain (Filename: {0})", mFileName));
		}
		for (byte b = 0; b < CycleCount; b++)
		{
			UpdateFilenames(b);
			if (File.Exists(mFileName))
			{
				File.Delete(mFileName);
			}
			if (File.Exists(mBinFileName))
			{
				File.Delete(mBinFileName);
			}
			if (File.Exists(mSignatureFileName))
			{
				File.Delete(mSignatureFileName);
			}
		}
		if (File.Exists(mCycleFilename))
		{
			File.Delete(mCycleFilename);
		}
		mCyclePosition = 0;
		UpdateFilenames(0);
	}

	public string GetFileName()
	{
		return mFileName;
	}
}
