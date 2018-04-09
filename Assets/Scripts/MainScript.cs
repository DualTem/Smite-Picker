using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour {

	// *****Singleton Stuff*****
	private static MainScript _instance;

	private void SetInstance(MainScript ms){
		if(_instance != null){
			Debug.LogError("Attempting to create another MainScript, that shouldn't happen!");
			GameObject.Destroy(this);
			return;
		}
		_instance = ms;
	}

	public static MainScript instance{
		get{
			if(_instance == null)
				_instance = new MainScript();
			return _instance;
		}
	}// *****End of singleton stuff*****


	// this is the teams' icon list, 0-4 is friendly team, 5-9 is enemy team
	// (initialized in the Unity editor)
	public List<TeamIcon> SelectedGods;

	public Transform GodContainer;
	public GameObject GodPrefab;
	public Toggle AZToggle;
	public Sprite blankPortrait;
	int selectedIcon = -1;

	// List of all the god icons
	public List<God> GodList;
	public List<Sprite> PortraitList;
	private float[,] winrateTable;
	private float[,] winrateTableE;

	void Awake(){
		SetInstance(this);
		

		// Initialization here
		
		LoadGods();
		Refresh();
	}

	MainScript(){
		if(_instance != null){
			Debug.LogError("Attempting to create another MainScript, that shouldn't happen!");
			GameObject.Destroy(this);
			return;
		}
	}

	private void LoadGods(){
		TextAsset Tfile = Resources.Load("TeamWinLose") as TextAsset;
        TextAsset Efile = Resources.Load("EnemyWinLose") as TextAsset;
        TextAsset Gfile = Resources.Load("GodsList") as TextAsset;
        string fileData = Tfile.text;
		string[] lines = fileData.Split('\n');
		string[] line1 = lines[0].Trim().Split(',');

		// setup the god list
		for(int i = 1; i < line1.Length; i++){
			if(line1[i] == "")
			 continue;
			GameObject newGodObj = Instantiate(GodPrefab, GodContainer);
			God newGodComponent = newGodObj.GetComponent<God>();
			
			PortraitList.Add(Resources.Load<Sprite>(line1[i]));
			if(PortraitList[PortraitList.Count-1])
				newGodComponent.portrait.sprite = PortraitList[PortraitList.Count-1];
			else
			{
				newGodComponent.portrait.sprite = blankPortrait;
				Debug.LogError("No portrait sprite found for \"" + line1[i] + "\"");
			}

			newGodComponent.Name = line1[i];
			newGodComponent.listIndex = i - 1;
			GodList.Add(newGodComponent);
		}
		
		// setup the win rate table
		winrateTable = new float[GodList.Count, GodList.Count];
		for(int i = 1; i < lines.Length; i++){
			string[] line = lines[i].Trim().Split(',');
			for(int j = 1; j < line.Length; j++){
				winrateTable[j-1, i-1] = float.Parse(line[j]);
			}
		}


		fileData = Efile.text;
		lines = fileData.Split('\n');
		winrateTableE = new float[GodList.Count, GodList.Count];
		for(int i = 1; i < lines.Length; i++){
			string[] line = lines[i].Split(',');
			for(int j = 1; j < line.Length; j++){
				winrateTableE[j-1, i-1] = float.Parse(line[j]);
			}
		}

        fileData = Gfile.text;
        lines = fileData.Split('\n');
        for(int i = 0; i < lines.Length && i < GodList.Count; i++)
        {
            string[] line = lines[i].Split(',');
            GodList[i].SetClass(line[1]);
        }
	}

	// this function is called when the user clicks on a player's Icon
	public void SelectIcon(int i){
		if(selectedIcon == i)
		{
			SelectedGods[i].Name = "";
			SelectedGods[i].winRate = -1f;
			SelectedGods[i].listIndex = -1;
			SelectedGods[i].portrait.sprite = blankPortrait;
			selectedIcon = -1;
			Refresh();
		}
		else
		{
			selectedIcon = i;
		}
	}

	// this function is called when the user clicks on a god's icon
	public void SelectGod(God selection){
		if(selectedIcon > -1 && selectedIcon < SelectedGods.Count)
		{
			SelectedGods[selectedIcon].Name = selection.Name;
			SelectedGods[selectedIcon].listIndex = selection.listIndex;
			SelectedGods[selectedIcon].portrait.sprite = selection.portrait.sprite;
			Refresh();
		}
		selectedIcon = -1;
	}

	public void Sort(){
		if(AZToggle.isOn){
			// sort alphabetically
			for(int i = 0; i < GodList.Count; i++){
				GodList[i].transform.SetSiblingIndex(i);
			}
		}else{
			// sort by win rate
			List<God> tempList = new List<God>(GodList);
			
			tempList.Sort((godA, godB) => godB.winRate.CompareTo(godA.winRate));
			for(int i = 0; i < tempList.Count; i++){
				tempList[i].transform.SetSiblingIndex(i);
				tempList[i].siblingIndex = i;
			}
		}
	}

    public void Filter(int classFilter)
    {
        foreach(God god in GodList)
        {
            if (classFilter > 0 && god.godClass != (GodClass)classFilter)
            {
                god.gameObject.SetActive(false);
            }
            else
            {
                god.gameObject.SetActive(true);
            }
        }
    }

	// refreshes the calculated winrates for each god based on the team selections
	public void Refresh(){
		int SelectionCount = 0;
		for(int i = 0; i < 10; i++){
			if(SelectedGods[i].Name != ""){
				SelectionCount++;
			}
		}
		
		if(SelectionCount == 0){
			for(int i = 0; i < GodList.Count; i++){
				GodList[i].winRate = 0.0f;
				SelectionCount = 0;
				for(int j = 0; j < 10; j++){
					if(winrateTable[i,j] >= 0f){
						SelectionCount++;
						GodList[i].winRate += winrateTable[i,j];
					}
				}
				if(SelectionCount > 0){
					GodList[i].winRate /= (float)(SelectionCount);
				}
				else{
					GodList[i].winRate = -1f;
				}
			}
		}
		else{
			for(int i = 0; i < GodList.Count; i++){
				SelectionCount = 0;
				GodList[i].winRate = 0.0f;
				for(int j = 0; j < 10; j++){
					if(SelectedGods[j].Name != "") if((j < 5)?
					winrateTable[i,SelectedGods[j].listIndex] >= 0f :
					winrateTableE[i,SelectedGods[j].listIndex] >= 0f
					){
						SelectionCount++;

						GodList[i].winRate += (j < 5)?
							winrateTable[i,SelectedGods[j].listIndex] :
							winrateTableE[i,SelectedGods[j].listIndex];
					}
				}
				if(SelectionCount > 0){
					GodList[i].winRate /= (float)(SelectionCount);
				}
				else{
					GodList[i].winRate = -1f;
				}
			}
		}


		Sort();
	}
	
}
