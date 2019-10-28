﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ValueTool : Tool {



  void Awake() {
    id = "V";
  }

  override public void Interact() {
    player.targetCell = null;
    player.basePointer = null;

    GameObject cell = player.GetOnCell();
    if (cell != null) {
      player.targetCell = cell.GetComponent<CellController>();
    }
    GameObject pointer = player.GetOnPointer();
    if (pointer != null) {
      player.basePointer = pointer.GetComponent<PointerController>(); 
      player.targetCell = player.basePointer.Target;
    }

    StartCoroutine(TransmitAndUnlock());
  }

  protected IEnumerator TransmitAndUnlock() {
    if (player.IsTransmittable()) {
      yield return StartCoroutine(Transmit());
    }

    player.UnLock();
  }

  public IEnumerator Transmit() {
    if (player.LootText == "" || player.targetCell.immutable) {
      return GetValue();
    }
    else {
      return PutValue();
    }
  }


  protected IEnumerator PutValue() {
    GameObject payload = Instantiate(player.Loot);
    GameObject cell = player.targetCell.GetLootObject();

    payload.transform.SetParent(cell.transform);
    payload.transform.position = player.LootPosition;
    string value = player.LootText;
    player.LootText = "";

    Vector3 startPosition = player.LootPosition;
    Vector3 endPosition = cell.gameObject.transform.position;

    float startTime = Time.time;
    float targetTime = ((Vector2)endPosition-(Vector2)startPosition).magnitude / 5.0f;
    float elapsedTime = 0.0f;

    while (elapsedTime <= targetTime) {
      payload.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / targetTime);
      yield return null;
      elapsedTime = Time.time - startTime;
    }
	SoundManager.instance.PlayRandomizeSfx(servePlate);
    Destroy(payload);
    player.targetCell.SetLoot(value);
    player.levelController.OnTransmit(player.targetCell, player.basePointer, player, false);


  }

  protected IEnumerator GetValue() {
    GameObject cell = player.targetCell.gameObject.transform.Find("loot").gameObject;
    GameObject payload = Instantiate(cell);
    payload.transform.SetParent(cell.transform);
    payload.transform.position = cell.transform.position;

    Vector3 startPosition = payload.transform.position;
    Vector3 endPosition = player.LootPosition;

    float startTime = Time.time;
    float targetTime = ((Vector2)endPosition-(Vector2)startPosition).magnitude / 5.0f;
    float elapsedTime = 0.0f;

    while (elapsedTime <= targetTime) {
      payload.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / targetTime);
      yield return null;
      elapsedTime = Time.time - startTime;
    }
	SoundManager.instance.PlayRandomizeSfx(prepPlate);
    player.Acquire(player.targetCell.GetLoot());
    player.levelController.OnTransmit(player.targetCell, player.basePointer, player, true);

    Destroy(payload);

  }

  override public void InActive() {
    player.LootText = "";
  }
}

