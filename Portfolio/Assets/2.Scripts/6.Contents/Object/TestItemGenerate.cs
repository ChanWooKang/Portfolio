using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItemGenerate : MonoBehaviour
{
    public SODropTable _dropTable;

    private void Start()
    {
        
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            GenerateItem();
    }

    void GenerateItem()
    {
        _dropTable.ItemDrop(transform);
    }
}
