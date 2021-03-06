﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MigrateEvent : LevelEvent {

    public Transform[] MigratePoints;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private GameObject yakPrefab;

    [SerializeField] private float createTimePeriod;

    private void Start()
    {
        MigratePoints = spawnPoint.GetComponentsInChildren<Transform>();
    }

    override public void OnStart()
    {
        FindSpawnPoint();
        StartCoroutine("CreateYaks");
        base.OnStart();    
    }

    override public void OnEnd()
    {
        StopCoroutine("CreateYaks");
        base.OnEnd();
        AudioManager.Instance.StopSoundEffect("YakMarching", true);
    }

    private void FindSpawnPoint()
    {

    }

    private IEnumerator CreateYaks()
    {
        AudioManager.Instance.PlaySoundEffect("YakMooing");
        yield return new WaitForSecondsRealtime(2f);
        AudioManager.Instance.PlaySoundEffect("YakMooing");
        yield return new WaitForSecondsRealtime(5f);
        AudioManager.Instance.PlaySoundEffect("YakMarching", true, true, 1);
        while (true)
        {
            GameObject go = Instantiate(yakPrefab, MigratePoints[1].transform.position, Quaternion.identity);
            go.GetComponent<Yak>().SetMigrateDirection(MigratePoints[2].position - MigratePoints[1].transform.position);
            //go.transform.LookAt(MigratePoints[1].position - spawnPoint.transform.position);
            go.SetActive(true);
            yield return new WaitForSeconds(createTimePeriod);
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (Transform t in MigratePoints)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(t.position, new Vector3(0.5f, 0.5f, 0.5f));
        }
    }

}
