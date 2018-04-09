using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamIcon : MonoBehaviour {

	public Text text;
	public Image portrait;
	public string Name;
	public int listIndex;
	[Range(0.0f,1.0f)]
	public float winRate;

	void Start(){
		text = GetComponentInChildren<Text>();
	}

	void Update(){
		text.text = Name;
	}
}
