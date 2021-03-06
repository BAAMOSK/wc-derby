﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour {
	void ResetStaticsAndLoadScene(int sceneIndex) {
		EnemyDrive.ResetStatics();
		HoverCraftBase.ResetStatic();
		SceneManager.LoadScene(sceneIndex);
	}
	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1)) {
			ResetStaticsAndLoadScene(0);
		}
		if(Input.GetKeyDown(KeyCode.Alpha2)) {
			ResetStaticsAndLoadScene(1);
		}
		if(Input.GetKeyDown(KeyCode.Alpha3)) {
			ResetStaticsAndLoadScene(2);
		}
		if(Input.GetKeyDown(KeyCode.Alpha4)) {
			ResetStaticsAndLoadScene(3);
		}
	}
}
