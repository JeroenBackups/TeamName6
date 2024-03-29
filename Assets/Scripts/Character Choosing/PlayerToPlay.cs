﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToPlay : MonoBehaviour
{
    [HideInInspector] private PlayerVariants _playerScript;
    [HideInInspector] private GameObject _prefab;

    private void Awake()
    {
        _playerScript = GameObject.FindGameObjectWithTag("PlayerCustomization").GetComponent<PlayerVariants>();
        _prefab = _playerScript.Players[PlayerPrefs.GetInt("playerToPlay")].Looks;
    }

    void Start()
    {
        var instantiatedPlayer = Instantiate(_prefab);

        instantiatedPlayer.transform.SetParent(GameObject.FindGameObjectWithTag("Player").transform, false);

        //InstantiatedPlayer.transform.position = new Vector3(0, 0, 0);
        instantiatedPlayer.transform.rotation = new Quaternion(0, GameObject.FindGameObjectWithTag("Player").transform.position.y - 90, 0, 1);
    }

}
