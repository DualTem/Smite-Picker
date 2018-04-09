using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class God : MonoBehaviour {
	public Text text;
	public Image winBar;
	public SpriteRenderer portrait;

	public string Name;
	[Range(0.0f,1.0f)]
	public float winRate;
	public int listIndex;
	public int siblingIndex;

	public God(string _name){
		Name = _name;
	}



	void Update(){
		text.text = Name;
		winBar.fillAmount = winRate;
		winBar.color = Color.HSVToRGB(Mathf.Lerp(0.0f, 0.34f, winRate * 2.0f - 0.5f), 1.0f, 1.0f);//Color.Lerp(Color.red, Color.green, winRate * 2f - 0.5f);
	}

	public void Select(){
		MainScript.instance.SelectGod(this);
	}
}
