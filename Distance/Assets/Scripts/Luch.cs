using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class Luch : MonoBehaviour
{
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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            luchPositions.Clear();
            for (int i = 0; i < SIZE_X; i++)
            {
                for (int j = 0; j < SIZE_Y; j++)
                {
                    dX = (double)SIZE_X / deltaRotation;
                    dY = (double)SIZE_Y / deltaRotation;
                    Vector3 rotation = new Vector3(transform.forward.x - (float)(positionStartX*dX) + (float)dX*i, transform.forward.y - (float)(positionStartY * dY) + (float)dY*j, transform.forward.z);
                    ray[i,j] = new Ray(this.transform.position, rotation);
                    Debug.DrawRay(this.transform.position, rotation * 50f, Color.red);

                    if (Physics.Raycast(ray[i, j], out hit))
                    {
                        luchPositions.Add(new LuchPosition((float)hit.distance, (float)difference(ray[i, j].origin.x, hit.point.x), (float)difference(ray[i, j].origin.y, hit.point.y)));
                    }
                    else
                    {
                        luchPositions.Add(new LuchPosition(100000, 100000, 100000));
                    }
                }
            }

            string t = JsonConvert.SerializeObject(luchPositions);
            File.WriteAllText("Data/json.json", t);
            ScreenCapture.CaptureScreenshot("Data/Screenshot.png");
            Debug.Log("Успешно");
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