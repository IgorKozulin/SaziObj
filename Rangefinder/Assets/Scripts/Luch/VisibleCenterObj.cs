using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// класс для определение видимости
public class VisibleCenterObj : MonoBehaviour
{
    [Tooltip("Transform родителя у которого нужно определять видимость по центру")]
    public Transform perentObj;

    public static bool visible;

    void Start()
    {
        transform.position = perentObj.position;
        visible = false;
    }

    private void OnBecameVisible()
    {
        visible = true;
    }

    private void OnBecameInvisible()
    {
        visible = false;
    }
}
