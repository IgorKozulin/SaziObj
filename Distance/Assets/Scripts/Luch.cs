using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Luch : MonoBehaviour
{
    public string fileName;
    public int SIZE_X = 100;
    public int SIZE_Y = 100;
    public int deltaRotation = 500;

    private RaycastHit hit;
    private Ray[,] ray;

    private double dX;
    private double dY;

    private double positionStartX;
    private double positionStartY;
    private List<LuchPosition> luchPositions = new List<LuchPosition>();

    void Start()
    {
        ray = new Ray[SIZE_X, SIZE_Y];
        positionStartX = SIZE_X / 2;
        positionStartY = SIZE_Y / 2;

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
                    Vector3 rotation = new Vector3(transform.forward.x - (float)(positionStartX*dX) + (float)dX*i, transform.forward.y - (float)(positionStartY * dY) + (float)dY*j, transform.forward.z);
                    ray[i,j] = new Ray(this.transform.position, rotation); // создаем лучь в позиции камеры
                    Debug.DrawRay(this.transform.position, rotation * 50f, Color.red); // отрисовка луча начальные кординаты и цвет

                    if (Physics.Raycast(ray[i, j], out hit))
                    {
                        if (i == 0 && j == 0)
                        {
                            print("point  " + hit.point);
                        }
                        text += (float)hit.distance + " " + difference(ray[i, j].origin.x, hit.point.x) + " " + difference(ray[i, j].origin.y, hit.point.y) + " | ";
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
            Vector3 rotation = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z);
            ray1 = new Ray(transform.position, rotation);
            Vector3 rotation2 = new Vector3(transform.forward.x + 0.1f, transform.forward.y, transform.forward.z);

            if (Physics.Raycast(ray1, out hit))
            {
                print("point " + hit.point);
                print("orign " + ray1.origin);
                print("dx=" + (float)hit.distance + " dy=" + difference(ray1.origin.x, hit.point.x) + " dz=" + difference(ray1.origin.y, hit.point.y));
            }
        }
    }

    private float difference(float orign, float point)
    {
        if (orign > point)
        {
            return (Mathf.Abs(orign - point));
        }
        else
        {
            return -(Mathf.Abs(orign - point));
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