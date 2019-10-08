using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
	//Path to save file
	public static string pathPlayerSave = Application.persistentDataPath + "/playerData.sav";
	public static string pathOptionSave = Application.persistentDataPath + "/options.sav";

	//Save player game clear data
	public static void SaveGame(PlayerData data)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream stream = new FileStream(pathPlayerSave, FileMode.Create);

		PlayerData save = new PlayerData(data);

		formatter.Serialize(stream, save);

		stream.Close();
	}

	//Save options data
	public static void SaveOptions(OptionsData data)
	{
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream stream = new FileStream(pathOptionSave, FileMode.Create);

		OptionsData save = new OptionsData(data);

		formatter.Serialize(stream, save);

		stream.Close();
	}

	//Load player game clear data
	public static PlayerData LoadGame()
	{
		if (File.Exists(pathPlayerSave))
		{
            BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(pathPlayerSave, FileMode.Open);

			PlayerData data = formatter.Deserialize(stream) as PlayerData;

			stream.Close();

			return data;
		}
		else
		{
			return new PlayerData(); ;
		}
	}

	//Load options data
	public static OptionsData LoadOptions()
	{
		if (File.Exists(pathOptionSave))
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(pathOptionSave, FileMode.Open);

			OptionsData data = formatter.Deserialize(stream) as OptionsData;

			stream.Close();

			return data;
		}
		else
		{
			return new OptionsData(); ;
		}
	}
}
