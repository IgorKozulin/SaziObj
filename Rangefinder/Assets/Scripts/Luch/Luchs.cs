using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class Luchs : MonoBehaviour
{
    public int NUM_X; // число лучей по оси X
    public int NUM_Y; // число лучей по оси Y
    public float Angle_X;// угол исходящих лучей из камеры по оси X
    public float Angle_Y;// угол исходящих лучей из камеры по оси Y

    private float Length_X;// Ширина разброса лучей
    private float Length_Y;// высота разброса лучей
    private RaycastHit hit;
    private Ray[,] ray;
    private double dX;
    private double dY;

    // private int count; // счетчик числа файлов
    private string curTime;
    private Color32 color;
    private List<LuchPosition> luchPositions = new List<LuchPosition>();
    private string textJson;
    
    void Start()
    {
        NUM_X = 10;  // число лучей по оси X
        NUM_Y = 10;  // число лучей по оси Y
        Angle_X = 45; // угол исходящих лучей из камеры по оси X
        Angle_Y = 45; // угол исходящих лучей из камеры по оси X

        Length_X = 2 * Mathf.Tan(Angle_X / 2 * Mathf.PI / 180);// пересчитали угол в радианах для тангенса
        Length_Y = 2 * Mathf.Tan(Angle_Y / 2 * Mathf.PI / 180);// пересчитали угол в радианах для тангенса
        dX = (double)Length_X / (NUM_X - 1); // расстояние между лучами на ширине Length_X
        if (NUM_X <= 1) { dX = 0; Length_X = 0; } // просмотр одного луча
        dY = (double)Length_Y / (NUM_Y - 1); // расстояние между лучами на высоте Length_Y
        if (NUM_Y <= 1) { dY = 0; Length_Y = 0; } // просмотр одного луча

        ray = new Ray[NUM_X, NUM_Y];
        // count = 0;
    }

    void Update()
    {
        // фото + json x,y,z,r,g,b
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Загрузка ...");

            textJson = "";
            curTime = System.DateTime.Now.ToString("dd-mm-yyyy hh-mm-ss"); // сохраняем дату для записи файла как имя
            //getScreenShot(curTime); // функция возвращая скриншет
            //ScreenCapture.CaptureScreenshot("Data/Screenshot_" + curTime + ".png"); // скриншет всего экрана

            Camera camera = GetComponent<Camera>();
            int resWidth = Screen.width;
            int resHeight = Screen.height;
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            camera.targetTexture = rt; //Create new renderTexture and assign to camera
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false); //Create new texture
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0); //Apply pixels from camera onto Texture2D
            byte[] byteArray = screenShot.EncodeToPNG();
            File.WriteAllBytes("Data/Screenshot_" + curTime + ".png", byteArray);


            luchPositions.Clear();
            for (int i = 0; i < NUM_X; i++) // лучи по горизонтали последовательность слева направо
            {
                for (int j = 0; j < NUM_Y; j++) // лучи по вертикали последовательность сснизу вверх
                {
                    Vector3 rotation = new Vector3(transform.forward.x + (float)(-Length_X / 2 + i * dX), transform.forward.y + (float)(-Length_Y / 2 + j * dY), transform.forward.z);
                    ray[i, j] = new Ray(this.transform.position, rotation);
                    // Debug.DrawRay(this.transform.position, rotation * 50f, Color.red);

                    if (Physics.Raycast(ray[i, j], out hit))
                    {
                        Vector3 screenPos = camera.WorldToScreenPoint(hit.point);
                        color = screenShot.GetPixel((int)screenPos.x, (int)screenPos.y);
                        //Debug.Log("color=" + color + " х(" + screenPos.x + ") y(" + screenPos.y + ") - Позиция точки попадания луча из центра камеры на экране");
                        luchPositions.Add(new LuchPosition((float)hit.distance, (float)(ray[i, j].origin.x - hit.point.x), (float)(hit.point.y - ray[i, j].origin.y), color.r, color.g, color.b));
                        textJson += (float)hit.distance + " " + (ray[i, j].origin.x - hit.point.x) + " " + (float)(hit.point.y - ray[i, j].origin.y) + " " + color.r + " " + color.g + " " + color.b + ", ";
                    }
                    else
                    {
                        luchPositions.Add(new LuchPosition(100000, 100000, 100000, 10000, 10000, 10000));
                        textJson += 1000 + " " + 1000 + " " + 1000 + " " + 1000 + " " + 1000 + " " + 1000 + ", ";
                    }
                }
            }
            camera.targetTexture = null;
            RenderTexture.active = null; //Clean
            Destroy(rt); //Free memory

            string t = JsonConvert.SerializeObject(luchPositions);
            File.WriteAllText("Data/json_" + curTime + ".json", t);
            File.WriteAllText("Data/txt_" + curTime + ".txt", textJson);
            Debug.Log("Успешно");
            //count++; //счетчик числа файлов    
        }

        // просмотр лучей (тест)
        if (Input.GetKey(KeyCode.T))
        {
            for (int i = 0; i < NUM_X; i++) // лучи по горизонтали
            {
                for (int j = 0; j < NUM_Y; j++) // лучи по вертикали
                {
                    Vector3 rotation = new Vector3(transform.forward.x + (float)(-Length_X / 2 + i * dX), transform.forward.y + (float)(-Length_Y / 2 + j * dY), transform.forward.z);
                    //Debug.Log("j="+j+" transform="+dY+" points="+(float)(-Length_Y/2+j*dY));
                    ray[i, j] = new Ray(this.transform.position, rotation);
                    Debug.DrawRay(this.transform.position, rotation * 100f, Color.red);// здесь 100f - это длина лучей
                }
            }
        }
    }

    // сделать скриншет по размеру
    private void getScreenShot(string name)
    {
        Camera camera = GetComponent<Camera>();
        int w = camera.pixelWidth / 2 - NUM_X / 2; // отпределяет откуда рисовать
        int h = camera.pixelHeight / 2 - NUM_Y / 2; // отпределяет откуда рисовать
        camera.targetTexture = RenderTexture.GetTemporary(NUM_X, NUM_Y, 16);

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