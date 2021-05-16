using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class Luch : MonoBehaviour
{
    public int SIZE_X = 100;
    public int SIZE_Y = 100;
    public int deltaRotation = 1000;

    private RaycastHit hit;
    private Ray[,] ray;
    private Ray rayTest;
    private double dX;
    private double dY;
    private double positionStartX;
    private double positionStartY;
    private string curTime;
    private List<LuchPosition> luchPositions = new List<LuchPosition>();
    private Texture2D tex;
    private Color32 color;

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
            Debug.Log("Загрузка ...");

            curTime = System.DateTime.Now.ToString("dd-mm-yyyy hh-mm-ss");
            getScreenShot(curTime);
            //ScreenCapture.CaptureScreenshot("Data/Screenshot_" + curTime + ".png");

            Camera camera = GetComponent<Camera>();
            int w = camera.pixelWidth / 2 - SIZE_X / 2;
            int h = camera.pixelHeight / 2 - SIZE_Y / 2;
            camera.targetTexture = RenderTexture.GetTemporary(SIZE_X, SIZE_Y, 16);

            RenderTexture renderTexture = camera.targetTexture;

            tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(w, h, renderTexture.width, renderTexture.height), 0, 0);

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
                        color = tex.GetPixel((int)hit.point.x, (int)hit.point.y);
                        luchPositions.Add(new LuchPosition((float)hit.distance, (float)difference(ray[i, j].origin.x, hit.point.x), (float)difference(ray[i, j].origin.y, hit.point.y), color.r, color.g, color.b));
                    }
                    else
                    {
                        luchPositions.Add(new LuchPosition(100000, 100000, 100000, 100000, 100000, 100000));
                    }
                }
            }
            string t = JsonConvert.SerializeObject(luchPositions);
            File.WriteAllText("Data/json_"+ curTime + ".json", t);
            Debug.Log("Успешно");
        }

        if (Input.GetKey(KeyCode.T))
        {
            for (int i = 0; i < SIZE_X; i++)
            {
                for (int j = 0; j < SIZE_Y; j++)
                {
                    dX = (double)SIZE_X / deltaRotation;
                    dY = (double)SIZE_Y / deltaRotation;
                    Vector3 rotation = new Vector3(transform.forward.x - (float)(positionStartX * dX) + (float)dX * i, transform.forward.y - (float)(positionStartY * dY) + (float)dY * j, transform.forward.z);
                    ray[i, j] = new Ray(this.transform.position, rotation);
                    Debug.DrawRay(this.transform.position, rotation * 50f, Color.blue);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Camera camera = GetComponent<Camera>();
            int w = camera.pixelWidth / 2 - SIZE_X / 2;
            int h = camera.pixelHeight / 2 - SIZE_Y / 2;
            camera.targetTexture = RenderTexture.GetTemporary(SIZE_X, SIZE_Y, 16);

            RenderTexture renderTexture = camera.targetTexture;

            tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(w, h, renderTexture.width, renderTexture.height), 0, 0);

            Vector3 rotation = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z);
            rayTest = new Ray(this.transform.position, rotation);
            Debug.DrawRay(this.transform.position, rotation * 50f, Color.red);

            if (Physics.Raycast(rayTest, out hit))
            {
                print(hit.point.x);
                color = tex.GetPixel((int)hit.point.x, (int)hit.point.y);
                print(color);
            }

            RenderTexture.ReleaseTemporary(renderTexture);
            camera.targetTexture = null;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            curTime = System.DateTime.Now.ToString("dd-mm-yyyy hh-mm-ss");
            getScreenShot(curTime);


            Camera camera = GetComponent<Camera>();
            int w = camera.pixelWidth / 2 - SIZE_X / 2;
            int h = camera.pixelHeight / 2 - SIZE_Y / 2;
            camera.targetTexture = RenderTexture.GetTemporary(SIZE_X, SIZE_Y, 16);

            RenderTexture renderTexture = camera.targetTexture;

            tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(w, h, renderTexture.width, renderTexture.height), 0, 0);

            Vector3 rotation = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z);
            rayTest = new Ray(this.transform.position, rotation);
            Debug.DrawRay(this.transform.position, rotation * 50f, Color.red);

            if (Physics.Raycast(rayTest, out hit))
            {
                print(hit.point.x);
                color = tex.GetPixel((int)hit.point.x, (int)hit.point.y);
                print(color);
            }

            RenderTexture.ReleaseTemporary(renderTexture);
            camera.targetTexture = null;
        }
    }

    private void getScreenShot(string name)
    {
        Camera camera = GetComponent<Camera>();
        int w = camera.pixelWidth/2 - SIZE_X/2;
        int h = camera.pixelHeight/2 - SIZE_Y/2;
        camera.targetTexture = RenderTexture.GetTemporary(SIZE_X, SIZE_Y, 16);

        RenderTexture renderTexture = camera.targetTexture;
        Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(w, h, renderTexture.width, renderTexture.height);
        texture2D.ReadPixels(rect, 0, 0);

        byte[] byteArray = texture2D.EncodeToPNG();
        File.WriteAllBytes("Data/Screenshot_" + name + ".png", byteArray);

        RenderTexture.ReleaseTemporary(renderTexture);
        camera.targetTexture = null;
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
    public int r;
    public int g;
    public int b;

    public LuchPosition(float x, float y, float z, int r, int g, int b)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.r = r;
        this.g = g;
        this.b = b;
    }
}

public class ColorObj
{
    public int r;
    public int g;
    public int b;

    public ColorObj(int r, int g, int b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
    }
}