﻿using UnityEngine;
using System.Collections;
using System;

public class Grid : MonoBehaviour
{
    public EmptyField[,] grid;
    int idToGive = 1;
    [SerializeField]
    EmptyField emptyField;
    [SerializeField]
    GameObject allFields, gradient;
    public int maxGridSize = 8;
    [SerializeField]
    BuildingButtons buildings;

    public void MakeGrid()
    {
        grid = new EmptyField[maxGridSize, maxGridSize];

        GameObject allFieldsTemp = Instantiate(allFields);
        
        for (int x = 0; x < maxGridSize; x++)
        {
            for (int y = 0; y < maxGridSize; y++)
            {
                Vector3 newPos = new Vector3((x - y) * 2.09f, (x + y) * 1.21f, 0);
                EmptyField tempField = (EmptyField)Instantiate(emptyField, newPos, emptyField.transform.rotation);
                tempField.transform.SetParent(allFieldsTemp.transform);
                tempField.ID = idToGive;
                tempField.buildMenu = buildings;
                idToGive++;
                tempField.gridPosition = new Vector2(x, y);
                grid[x, y] = tempField;
            }
        }
        GameObject tempGra = (GameObject)Instantiate(gradient, new Vector3(0,28.6f), emptyField.transform.rotation);
        tempGra.GetComponent<SpriteRenderer>().sortingOrder = -1;
        tempGra.transform.localScale = new Vector3(3.25f,3.2f);
        tempGra.transform.SetParent(allFieldsTemp.transform);
        //tempGra.transform.localScale = new Vector3(maxGridSize, maxGridSize);
        allFieldsTemp.transform.position = new Vector3(-8.6f, 6.4f, 6.4f);
    }
}
