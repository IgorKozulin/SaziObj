using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;

public class Luch : MonoBehaviour
{

    public int NUM_X; // число лучей по оси X
    public int NUM_Y; // число лучей по оси Y
    public int WX; // Screen.width;
    public int WY; // Screen.height;
    public float Angle_X;// угол исходящих лучей из камеры по оси X
    public float Angle_Y;// угол исходящих лучей из камеры по оси Y

    private float Length_X;// Ширина разброса лучей
    private float Length_Y;// высота разброса лучей
    private float FocalLength;
    private RaycastHit hit;
    private Ray[,] ray;
    private double dX;
    private double dY;

    private int count; // счетчик числа файлов
    private Color32 color;
    private List<LuchPosition> luchPositions = new List<LuchPosition>();

    //for Intrinsic matrix
    private float cx;
    private float cy;
    private float fx;//Focal length in pixels
    private float fy;//Focal length in pixels
    private int s;

    void Start()
    {
        NUM_X = 320;  // число лучей по оси X
        NUM_Y = 240;  // число лучей по оси Y
        WX=320; // Screen.width;
        WY=240; // Screen.height;
      //  Angle_X = 83.0f; // угол исходящих лучей из камеры по оси горизонтали
      //  Angle_Y = 61.92f; // угол исходящих лучей из камеры по вертикали

        Camera camera = GetComponent<Camera>();
        float filmWidth = camera.sensorSize.x; //по умолчанию 36 in mm;
        float filmHeight = camera.sensorSize.y; //по умолчанию это 24 in mm

        // можно установить ширину и высоту сенсора камеры
        filmWidth = 32; 
        filmHeight = 24;
        FocalLength = camera.focalLength;

        Angle_X = (float)(Mathf.Rad2Deg * 2.0 * Math.Atan(filmWidth / (2.0 * FocalLength)));//Hfov или угол обзора по ширине
        Angle_Y = (float)(Mathf.Rad2Deg * 2.0 * Math.Atan(filmHeight / (2.0 * FocalLength)));//Vfov или угол обзора по высоте
        //print(" FocalLength=" + FocalLength + " Xfov=" + Angle_X + " Yfov=" + Angle_Y);

        Length_X = 2* Mathf.Tan(Angle_X/2* Mathf.PI/180);// пересчитали угол в радианах для тангенса
        Length_Y = 2 * Mathf.Tan(Angle_Y / 2 * Mathf.PI / 180);// пересчитали угол в радианах для тангенса
        //Debug.Log("Length_Y="+ Length_Y);
        dX = (double)Length_X/(NUM_X - 1); // расстояние между лучами на ширине Length_X
        if (NUM_X<=1) { dX = 0; Length_X = 0;  } // просмотр одного луча
        dY = (double)Length_Y / (NUM_Y - 1); // расстояние между лучами на высоте Length_Y
        if (NUM_Y <= 1) { dY = 0; Length_Y = 0; } // просмотр одного луча

        ray = new Ray[NUM_X, NUM_Y];
        count = 0; // счетчик числа файлов

    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Загрузка ...");
      
            Camera camera = GetComponent<Camera>();
            int resWidth = WX;// Screen.width;
            int resHeight = WY;//Screen.height;
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            camera.targetTexture = rt; //Create new renderTexture and assign to camera
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false); //Create new texture
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0); //Apply pixels from camera onto Texture2D
            byte[] byteArray = screenShot.EncodeToPNG();
            File.WriteAllBytes("Data/Screenshot_" + count + ".png", byteArray);

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
                    }
                    else
                    {
                        luchPositions.Add(new LuchPosition(100000, 100000, 100000, 10000, 10000, 10000));
                    }
                }
            }
            camera.targetTexture = null;
            RenderTexture.active = null; //Clean
            Destroy(rt); //Free memory


            /////// INTRINSIC MATRIX (K)
            cx = WX / 2;
            cy = WY / 2;
            fx = cx / (Mathf.Tan(Angle_X / 2 * Mathf.PI / 180));//
            fy = cy / (Mathf.Tan(Angle_Y / 2 * Mathf.PI / 180));//
            s = 0;// нет искажения
            //print("cx=" + cx + " cy=" + cy + " fx=" + fx + " fy=" + fy);
            //print(fx+" "+0f+" "+0f+" "    +0f+" "+fy+" "+0f+" "      +cx+" "+cy+" "+1f);// матрица K

            ///////ROTATION MATRIX (R)
            float alfa_X = Camera.main.transform.eulerAngles.x;
            float cos_alfa_X = Mathf.Cos(alfa_X * Mathf.PI / 180);
            float sin_alfa_X = Mathf.Sin(alfa_X * Mathf.PI / 180);

            float beta_Y = Camera.main.transform.eulerAngles.y;
            float gamma_Z = Camera.main.transform.eulerAngles.z;
            //print("xRot=" + alfa_X + " cos=" + cos_alfa_X + " sin=" + sin_alfa_X);// cos и sin
            //print(1 + " " + 0f + " " + 0f + " " + 0f + " " + cos_alfa_X + " " + -sin_alfa_X + " " + 0f + " " + sin_alfa_X + " " + cos_alfa_X);// матрица R


            /////// ЗАПИСЬ ДАННЫХ
            string text = "";
            text += 1 + " " + 0f + " " + 0f + " " + 0f + " " + cos_alfa_X + " " + -sin_alfa_X + " " + 0f + " " + sin_alfa_X + " " + cos_alfa_X + "\n";// матрица R
            text += fx + " " + 0f + " " + 0f + " " + 0f + " " + fy + " " + 0f + " " + cx + " " + cy + " " + 1f + "\n";//K матрица
            text += "FocalLength=" + FocalLength + " Xfov=" + Angle_X + " Yfov=" + Angle_Y + "\n";
            File.WriteAllText("Data/Matrix_" + count + ".txt", text);// запись K и R матрицы

            string t = JsonConvert.SerializeObject(luchPositions);
            File.WriteAllText("Data/json_" + count + ".json", t);
            //File.WriteAllText("Data/txt_" + count + ".txt", t);
            Debug.Log("File="+count+" Успешно");
            count++; //счетчик числа файлов
        }

        // просмотр лучей (тест)
        if (Input.GetKey(KeyCode.T))
        {
            for (int i = 0; i < NUM_X; i++) // лучи по горизонтали
            {
                for (int j = 0; j < NUM_Y; j++) // лучи по вертикали
                {
                    Vector3 rotation = new Vector3(transform.forward.x + (float)(-Length_X/2+i*dX), transform.forward.y + (float)(-Length_Y/2 + j*dY), transform.forward.z);
                    //Debug.Log("j="+j+" transform="+dY+" points="+(float)(-Length_Y/2+j*dY));
                    ray[i, j] = new Ray(this.transform.position, rotation);
                    Debug.DrawRay(this.transform.position, rotation * 100f, Color.blue);// здесь 100f - это длина лучей
                }
            }
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
