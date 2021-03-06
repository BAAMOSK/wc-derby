﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScatterSpawn : MonoBehaviour {
	public GameObject[] prefabList;
	public int howMany;

	private List<GameObject> spawnedList = new List<GameObject>();

	public Text counterUI; // optional
	private int lastShownSize = 0;
	public Image radarArea;
	public GameObject prefabRadar;
	private List<Image> allRadarPt = new List<Image>();
	public Image playerRadarPt;

	Vector3 domeCenter;
	float domeRadius;

	// Use this for initialization
	void Start () {
		GameObject theDome = GameObject.Find("domeMeasure");
		domeCenter = theDome.transform.position;
		domeRadius = theDome.transform.localScale.y * 0.5f;

		for(int i = 0; i < howMany; i++) {
			int randPick = Mathf.FloorToInt(Random.Range(0, prefabList.Length));
			float randAng = Random.Range(0.0f, 2.0f * Mathf.PI);
			float randScatterDist = Random.Range(0.0f, domeRadius);
			GameObject nextSpawned = GameObject.Instantiate(prefabList[randPick],
				domeCenter
				+ Vector3.right * Mathf.Cos(randAng)*randScatterDist
				+ Vector3.forward * Mathf.Sin(randAng)*randScatterDist, 
				Quaternion.AngleAxis(randAng*Mathf.Rad2Deg+180.0f,Vector3.up)); // point inward at first
			spawnedList.Add(nextSpawned);

			if(playerRadarPt == null && prefabRadar != null) {
				GameObject nextRadarPt = GameObject.Instantiate(prefabRadar);
				nextRadarPt.transform.SetParent(radarArea.transform);
				allRadarPt.Add(nextRadarPt.GetComponent<Image>());

				PlayerDrive.instance = nextSpawned.GetComponent<PlayerDrive>();
			}
		}
	}

	void Update() {
		spawnedList.RemoveAll(delegate (GameObject o) { return o == null; });

		if(counterUI && lastShownSize != spawnedList.Count) {
			lastShownSize = spawnedList.Count;
			counterUI.text = lastShownSize + "/" + howMany;
		}

		if(radarArea) {
			if(playerRadarPt) {
				if(spawnedList.Count>0 && spawnedList[0] != null) {
					playerRadarPt.rectTransform.rotation = Quaternion.AngleAxis(
						-spawnedList[0].transform.eulerAngles.y, Vector3.forward);
					playerRadarPt.rectTransform.localPosition = WorldToRadarCoord(spawnedList[0]);
				}
			} else {
				while(allRadarPt.Count > spawnedList.Count) {
					Destroy(allRadarPt[0]);
					allRadarPt.RemoveAt(0);
				}
				for(int i = 0; i < spawnedList.Count; i++) {
					allRadarPt[i].rectTransform.localPosition = WorldToRadarCoord(spawnedList[i]);
				}
			}
		}
	}

	public Vector3 WorldToRadarCoord(GameObject forGO) {
		Vector3 scaledPos = (radarArea.rectTransform.rect.width*0.5f)*
			(-1.0f * Vector3.one + (forGO.transform.position - domeCenter) / domeRadius);
		scaledPos.y = scaledPos.z;
		return scaledPos;
	}

	public HoverCraftBase nearestAheadOf(Transform relativeTo) {
		spawnedList.RemoveAll(delegate (GameObject o) { return o == null; });

		HoverCraftBase toRet = null;
		float coneAhead = 35.0f;
		float nearestDistance = 300.0f;
		foreach(GameObject craft in spawnedList) {
			float angleTo = Quaternion.Angle(Quaternion.LookRotation(relativeTo.forward),
				Quaternion.LookRotation(craft.transform.position - relativeTo.position));
			float distInFront = relativeTo.transform.InverseTransformPoint(craft.transform.position).z;
			if(distInFront > 5.0f && distInFront < nearestDistance && angleTo < coneAhead) {
				nearestDistance = distInFront;
				toRet = (HoverCraftBase)craft.GetComponent<PlayerDrive>();
				if(toRet == null) {
					toRet = (HoverCraftBase)craft.GetComponent<EnemyDrive>();
				}
			}
		}
		return toRet;
	}
}
