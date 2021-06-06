using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class DistanseObject : MonoBehaviour
{
    [Tooltip("Transform камеры")]
    public Transform player;
    [Tooltip("Имя фигнуры, например 'Cube'")]
    public string figureName;
    [Tooltip("Задаем начальный угл по часовой стрелке от 0 до 360")]
    [Range(0, 360)]
    public int startAngleObj;

    private bool visible = false;
    private int count;
    private string curTime;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && VisibleCenterObj.visible)
        {
            curTime = System.DateTime.Now.ToString("dd-mm-yyyy hh-mm-ss");

            string text = "";
            text += this.transform.position.x.ToString() + " ";
            text += this.transform.position.y.ToString() + " ";
            text += this.transform.position.z.ToString() + " ";
            text += this.transform.localScale.x.ToString() + " ";
            text += this.transform.localScale.y.ToString() + " ";
            text += this.transform.localScale.z.ToString() + " ";
            text += figureName + " ";
            text += "u1 w1 u1 w2";
            text += getRotateObj();

            File.WriteAllText("Data/obj_" + count + ".txt", text);
            count++;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            print(getRotateObj());
        }
    }

    // функция возвращает куда смотрит фигура
    private string getRotateObj()
    {
        double rObj = transform.rotation.eulerAngles.y;

        // если задан начальный угл
        if (rObj + startAngleObj > 360)
        {
            rObj = (rObj + startAngleObj) % 360;
        }
        else
        {
            rObj += startAngleObj;
        }

        // определяем куда смотрит объект
        if (rObj >= 45 && rObj < 135)
        {
            return "<-";
        }
        else if (rObj >= 135 && rObj <= 225)
        {
            return "^";
        }
        else if (rObj > 225 && rObj <= 315)
        {
            return ("->");
        }
        else
        {
            return ("на камеру");
        }
    }
}

// var rObj = Vector3.Angle(Vector3.forward, gameObject.transform.forward); // модуль угла