using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Luch : MonoBehaviour
{
    public string fileName;
    public int SIZE_X = 5;
    public int SIZE_Y = 5;
    public int deltaRotation = 200;

    private RaycastHit hit;
    private Ray[,] ray;

    private double dX;
    private double dY;

    private double positionStart;
    private List<LuchPosition> luchPositions = new List<LuchPosition>();

    void Start()
    {
        ray = new Ray[SIZE_X, SIZE_Y];

        if (fileName == "")
        {
            fileName = "Data/Luch.json";
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StreamWriter sw = new StreamWriter(fileName);
            string text;
            for (int i = 0; i < SIZE_X; i++)
            {
                text = "";
                for (int j = 0; j < SIZE_Y; j++)
                {
                    dX = (double)SIZE_X / deltaRotation;
                    dY = (double)SIZE_Y / deltaRotation;
                    Vector3 rotation = new Vector3(transform.forward.x + (float)dX*i, transform.forward.y + (float)dY*j, transform.forward.z);
                    ray[i,j] = new Ray(this.transform.position, rotation); // создаем лучь в позиции камеры
                    Debug.DrawRay(this.transform.position, rotation * 50f, Color.red); // отрисовка луча начальные кординаты и цвет

                    if (Physics.Raycast(ray[i, j], out hit))
                    {
                        // point точка куда попала
                        // normal нормали
                        // distance дистанция
                        // например вычислить точку столкновения
                        if (i == 0 && j == 0)
                        {
                            print("point  " + hit.point);
                        }
                        text += hit.distance + " " + ((transform.position.x - hit.point.x))+ " " + ((transform.position.y - hit.point.y)) + " | ";
                    }
                    else
                    {
                        text += "null | ";
                    }
                }
                sw.WriteLineAsync(text);
            }
            sw.Close();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Ray ray1;
            Ray ray2;
            Ray ray3;
            Vector3 rotation = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z);
            ray1 = new Ray(this.transform.position, rotation); // создаем лучь в позиции камеры
            Debug.DrawRay(this.transform.position, rotation * 50f, Color.green); // отрисовка луча начальные кординаты и цвет

            if (Physics.Raycast(ray1, out hit))
            {
                print("debug  " + hit.point);
                print(ray1.origin);
                print("=  " + (ray1.origin.x - hit.point.x) + " " + (ray1.origin.y - hit.point.y) + " " + (ray1.origin.z - hit.point.z));
            }
        }

        if (Input.GetKey(KeyCode.P))
        {
            print("------------00");
        }
    }
}


public class LuchPosition
{
    public float x;
    public float y;
    public float z;

    public LuchPosition(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}