using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class LuchTest : MonoBehaviour
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

    public GameObject ColorObj;
    public GameObject[] cube;

    void Start()
    {
        ray = new Ray[SIZE_X, SIZE_Y];
        positionStartX = SIZE_X / 2;
        positionStartY = SIZE_Y / 2;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            Camera camera = GetComponent<Camera>();
            int w = camera.pixelWidth / 2 - SIZE_X / 2; // начало отрисовку по ширине
            int h = camera.pixelHeight / 2 - SIZE_Y / 2; // начало отрисовку по высоте
            camera.targetTexture = RenderTexture.GetTemporary(SIZE_X, SIZE_Y, 16); // создаем текстуру от камеры ао разменам

            RenderTexture renderTexture = camera.targetTexture;

            tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(w, h, renderTexture.width, renderTexture.height), 0, 0); // рисуем по цетру

            Vector3 rotation = new Vector3(transform.forward.x - 0.1f, transform.forward.y + 0.1f, transform.forward.z);
            rayTest = new Ray(this.transform.position, rotation);
            Debug.DrawRay(this.transform.position, rotation * 50f, Color.yellow);
            if (Physics.Raycast(rayTest, out hit))
            {
                Vector3 screenPos = camera.WorldToScreenPoint(hit.point);
                print(screenPos.x + " " + screenPos.y);
                color = tex.GetPixel((int)screenPos.x, (int)screenPos.y);
                // print(color);
                cube[0].GetComponent<Renderer>().material.color = color;
            }
            // пробую создать 4 луча

            //rotation = new Vector3(transform.forward.x + 0.1f, transform.forward.y + 0.1f, transform.forward.z);
            //rayTest = new Ray(this.transform.position, rotation);
            //Debug.DrawRay(this.transform.position, rotation * 50f, Color.yellow);

            //if (Physics.Raycast(rayTest, out hit))
            //{
            //    Vector3 screenPos = camera.WorldToScreenPoint(hit.point);
            //    color = tex.GetPixel((int)screenPos.x, (int)screenPos.y);
            //    cube[1].GetComponent<Renderer>().material.color = color;
            //}
            //rotation = new Vector3(transform.forward.x - 0.1f, transform.forward.y - 0.1f, transform.forward.z);
            //rayTest = new Ray(this.transform.position, rotation);
            //Debug.DrawRay(this.transform.position, rotation * 50f, Color.yellow);

            //if (Physics.Raycast(rayTest, out hit))
            //{
            //    Vector3 screenPos = camera.WorldToScreenPoint(hit.point);
            //    color = tex.GetPixel((int)screenPos.x, (int)screenPos.y);
            //    cube[2].GetComponent<Renderer>().material.color = color;
            //}
            //rotation = new Vector3(transform.forward.x + 0.1f, transform.forward.y - 0.1f, transform.forward.z);
            //rayTest = new Ray(this.transform.position, rotation);
            //Debug.DrawRay(this.transform.position, rotation * 50f, Color.yellow);

            //if (Physics.Raycast(rayTest, out hit))
            //{
            //    Vector3 screenPos = camera.WorldToScreenPoint(hit.point);
            //    color = tex.GetPixel((int)screenPos.x, (int)screenPos.y);
            //    cube[3].GetComponent<Renderer>().material.color = color;
            //}

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
    }

    private void getScreenShot(string name)
    {
        Camera camera = GetComponent<Camera>();
        int w = camera.pixelWidth / 2 - SIZE_X / 2; // отпределяет откуда рисовать
        int h = camera.pixelHeight / 2 - SIZE_Y / 2; // отпределяет откуда рисовать
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
