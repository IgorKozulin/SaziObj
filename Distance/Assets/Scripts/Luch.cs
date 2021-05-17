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

    public Texture2D heightmap;
    public Vector3 size = new Vector3(100, 10, 100);
    public GameObject ColorObj;

    void Start()
    {
        ray = new Ray[SIZE_X, SIZE_Y];
        positionStartX = SIZE_X / 2;
        positionStartY = SIZE_Y / 2;
    }

    void Update()
    {
        // фото + json x,y,z,r,g,b
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Загрузка ...");
            
            curTime = System.DateTime.Now.ToString("dd-mm-yyyy hh-mm-ss"); // сохраняем дату для записи файла как имя
            getScreenShot(curTime); // функция возвращая скриншет
            //ScreenCapture.CaptureScreenshot("Data/Screenshot_" + curTime + ".png"); // скриншет всего экрана


            luchPositions.Clear(); // отчистка списка
            for (int i = 0; i < SIZE_X; i++)
            {
                for (int j = 0; j < SIZE_Y; j++)
                {
                    dX = (double)SIZE_X / deltaRotation; // dx
                    dY = (double)SIZE_Y / deltaRotation; // dy
                    Vector3 rotation = new Vector3(transform.forward.x - (float)(positionStartX*dX) + (float)dX*i, transform.forward.y - (float)(positionStartY * dY) + (float)dY*j, transform.forward.z);
                    ray[i,j] = new Ray(this.transform.position, rotation); // создаем луч
                    Debug.DrawRay(this.transform.position, rotation * 50f, Color.red); // отрисовка луча

                    // если пересекся с каким либо объектом
                    if (Physics.Raycast(ray[i, j], out hit))
                    {
                        //color = tex.GetPixel((int)hit.point.x, (int)hit.point.y);
                        //luchPositions.Add(new LuchPosition((float)hit.distance, (float)difference(ray[i, j].origin.x, hit.point.x), (float)difference(ray[i, j].origin.y, hit.point.y), color.r, color.g, color.b))

                        luchPositions.Add(new LuchPosition((float)hit.distance, (float)difference(ray[i, j].origin.x, hit.point.x), (float)difference(ray[i, j].origin.y, hit.point.y), 255, 255, 255));
                    }
                    else
                    {
                        luchPositions.Add(new LuchPosition(100000, 100000, 100000, 100000, 100000, 100000));
                    }
                }
            }
            string t = JsonConvert.SerializeObject(luchPositions); // переводим список в формат json
            File.WriteAllText("Data/json_"+ curTime + ".json", t); // сохроняем json
            Debug.Log("Успешно");
        }

        // простотр лучей (тест)
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

        //
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

        // снимок экрана (тест)
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

        // вывод цвета по левой кнопки мыши
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 mpos = Input.mousePosition;

            tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            tex.Apply();

            Color32 bla = tex.GetPixel((int)mpos.x, (int)mpos.y);
            print(mpos.x.GetType());
            ColorObj.GetComponent<Renderer>().material.color = bla;
        }

        // вывод цвета по левой кнопки мыши
        if (Input.GetKeyDown(KeyCode.Mouse3))
        {

        }
    }

    private void getScreenShot(string name)
    {
        Camera camera = GetComponent<Camera>();
        int w = camera.pixelWidth/2 - SIZE_X/2; // отпределяет откуда рисовать
        int h = camera.pixelHeight/2 - SIZE_Y/2; // отпределяет откуда рисовать
        camera.targetTexture = RenderTexture.GetTemporary(SIZE_X, SIZE_Y, 16);

        RenderTexture renderTexture = camera.targetTexture;
        Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false); // создаем тектуру по скрину
        Rect rect = new Rect(w, h, renderTexture.width, renderTexture.height); // начало отрисовки
        texture2D.ReadPixels(rect, 0, 0);

        byte[] byteArray = texture2D.EncodeToPNG(); // переводим в png формат
        File.WriteAllBytes("Data/Screenshot_" + name + ".png", byteArray); // сохранаяем

        RenderTexture.ReleaseTemporary(renderTexture);
        camera.targetTexture = null;
    }

    // возвращает точку соприкосновения луча
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

// объект для сериализации лучей
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