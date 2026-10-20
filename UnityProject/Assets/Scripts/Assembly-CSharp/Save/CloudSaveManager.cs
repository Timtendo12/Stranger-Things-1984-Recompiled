using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// Rewritten from libil2cpp.so 1.0.605 (CloudSaveManager @ 0x17e4628..0x17e8000, Ghidra vaddr).
// PC port: local-only; the former Netflix cloud blob round-trips are no-ops. The game is local-only:
// nothing is ever loaded from the cloud and there is never a conflict. Encode/decode/serialize helpers are faithful.
public class CloudSaveManager
{
	private const bool SAVE_TO_PLAYER_PREFS = true;

	public const string BLOB_SLOT_PLAYER_DATA = "cs_player_data";

	public const string BLOB_SLOT_GAME_DATA = "cs_game_data";

	private const string BACKUP_PREFS_KEY = "backup_prefs";

	private CSPlayerData _mCSPlayerData;

	private static CloudSaveManager mInstance;

	public CSPlayerData CSPlayerData => _mCSPlayerData;

	public static CloudSaveManager Instance
	{
		get
		{
			if (mInstance == null)
			{
				mInstance = new CloudSaveManager();
			}
			return mInstance;
		}
	}

	public CloudSaveManager()
	{
		_mCSPlayerData = null;
	}

	private void ResetPlayerData()
	{
		_mCSPlayerData = null;
	}

	public Task SavePlayerDataToCloudSaveData(FPlayerData playerData)
	{
		// PC port: no cloud write.
		return Task.CompletedTask;
	}

	public Task PreLoadPlayerData()
	{
		// PC port: nothing to read from the cloud; leaves CSPlayerData null and no conflict.
		ResetPlayerData();
		return Task.CompletedTask;
	}

	private byte[] CompressAndEncodeContent(string content)
	{
		MemoryStream memoryStream = new MemoryStream();
		StreamWriter streamWriter = new StreamWriter(memoryStream);
		streamWriter.Write(content);
		streamWriter.Flush();
		memoryStream.Position = 0L;
		SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
		MemoryStream memoryStream2 = new MemoryStream();
		encoder.WriteCoderProperties(memoryStream2);
		memoryStream2.Write(BitConverter.GetBytes(memoryStream.Length), 0, 8);
		encoder.Code(memoryStream, memoryStream2, -1L, -1L, null);
		return memoryStream2.ToArray();
	}

	private string DecodeBlobDataToContentString(byte[] bytes)
	{
		SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();
		MemoryStream memoryStream = new MemoryStream(bytes);
		MemoryStream memoryStream2 = new MemoryStream();
		byte[] properties = new byte[5];
		memoryStream.Read(properties, 0, 5);
		decoder.SetDecoderProperties(properties);
		byte[] sizeBytes = new byte[8];
		memoryStream.Read(sizeBytes, 0, 8);
		long outSize = BitConverter.ToInt64(sizeBytes, 0);
		decoder.Code(memoryStream, memoryStream2, memoryStream.Length, outSize, null);
		memoryStream2.Position = 0L;
		return Encoding.UTF8.GetString(memoryStream2.ToArray());
	}

	public string SerializePlayerSavedStats(FPlayerStats playerStats)
	{
		List<string> list = new List<string>();
		foreach (FPlayerState savedPlayerState in playerStats.SavedPlayerStates)
		{
			string text = JsonUtility.ToJson(savedPlayerState);
			if (FCoreUtil.Valid(text))
			{
				list.Add(text);
			}
		}
		string text2 = string.Join(": ", list);
		return FCoreUtil.Valid(text2) ? text2 : null;
	}

	public void DeserializePlayerSavedStats(string json)
	{
		string[] array = json.Split(new string[1] { ": " }, StringSplitOptions.None);
		if (array == null || array.Length == 0)
		{
			return;
		}
		List<FPlayerState> list = new List<FPlayerState>();
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			if (FCoreUtil.Valid(text))
			{
				FPlayerState fPlayerState = JsonUtility.FromJson<FPlayerState>(text);
				if (fPlayerState != null && FPlayerStats.cCurrentSaveVersion <= fPlayerState.SaveVersion)
				{
					list.Add(fPlayerState);
				}
			}
		}
		FApp.PlayerStats.SavedPlayerStates = list;
		FApp.PlayerStats.CurrentSaveSlot = 0;
	}

	public void DeletePlayerDataFromCloudSave()
	{
		// PC port: no cloud data to delete.
		ResetPlayerData();
	}
}
