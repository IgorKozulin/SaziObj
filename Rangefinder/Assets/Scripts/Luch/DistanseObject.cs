using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class DistanseObject : MonoBehaviour
{
    public Transform player;
    public string figureName;

    private bool visible = false;
    private int count;
    private string curTime;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
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


            text += visible ? "объект виден \n" : "объект не виден \n";

            File.WriteAllText("Data/obj_" + count + ".txt", text);
            count++;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            print(getRotateObj());
        }
    }

    private void OnBecameVisible()
    {
        visible = true;
    }

    private void OnBecameInvisible()
    {
        visible = false;
    }

    private string getRotateObj()
    {
        var rObj = Vector3.Angle(Vector3.forward, gameObject.transform.forward);
        var rCamera = Vector3.Angle(Vector3.forward, player.transform.forward);

        if (rObj >= 0)
        {
            if (rObj >= 45 && rObj < 135)
            {
                return "->";
            }
            else if (rObj >= 135 && rObj <= 225)
            {
                return "^";
            }
            else if (rObj > 225 && rObj <= 315)
            {
                return ("<-");
            }
            else
            {
                return ("на камеру");
            }
        }
        else
        {
            if (rObj >= -45 && rObj < -135)
            {
                return "<-";
            }
            else if (rObj >= -135 && rObj <= -225)
            {
                return "^";
            }
            else if (rObj > -225 && rObj <= -315)
            {
                return ("->");
            }
            else
            {
                return ("на камеру");
            }
        }
    }
}
