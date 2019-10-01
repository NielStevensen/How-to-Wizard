[System.Serializable]
public class PlayerData
{
	//Story mode clear data, best clear times and least spells used
	public bool[]  storyClearData = new bool[15];
	public float[] storyClearTime = new float[15];
	public int[]   storyBestSpell = new int[15];

	//Story mode clear data, best clear times and least spells used
	public bool[]  nGameClearData = new bool[15];
	public float[] nGameClearTime = new float[15];
	public int[]   nGameBestSpell = new int[15];

	//Challenge mode clear data, best clear times and least spells used
	public bool[]  extraClearData = new bool[15];
	public float[] extraClearTime = new float[15];
	public int[]   extraBestSpell = new int[15];
	
	//Create empty player data
	public PlayerData()
	{
		storyClearData = new bool[15] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
		storyClearTime = new float[15] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
		storyBestSpell = new int[15] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

		nGameClearData = new bool[15] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
		nGameClearTime = new float[15] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
		nGameBestSpell = new int[15] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

		extraClearData = new bool[15] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
		extraClearTime = new float[15] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
		extraBestSpell = new int[15] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
	}

	//Copy player data
	public PlayerData(PlayerData data)
	{
		storyClearData = data.storyClearData;
		storyClearTime = data.storyClearTime;
		storyBestSpell = data.storyBestSpell;

		nGameClearData = data.nGameClearData;
		nGameClearTime = data.nGameClearTime;
		nGameBestSpell = data.nGameBestSpell;

		extraClearData = data.extraClearData;
		extraClearTime = data.extraClearTime;
		extraBestSpell = data.extraBestSpell;
	}
}

[System.Serializable]
public class OptionsData
{
	//VR options
	public bool useTeleportation = true;

	//PC options
	public float xSens = 3.75f;
	public float ySens = 3.75f;
	public bool yInversion = false;

	//Common options
	public float sfxLevel = 0.8f;
	public float bgmLevel = 0.8f;

	//Default options
	public OptionsData()
	{
		useTeleportation = true;
		
		xSens = 3.75f;
		ySens = 3.75f;
		yInversion = false;

		sfxLevel = 0.8f;
		bgmLevel = 0.8f;
	}

	//Copy options
	public OptionsData(OptionsData data)
	{
		useTeleportation = data.useTeleportation;

		xSens = data.xSens;
		ySens = data.ySens;

		sfxLevel = data.sfxLevel;
		bgmLevel = data.bgmLevel;
	}
}
