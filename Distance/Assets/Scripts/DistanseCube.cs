using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DistanseCube : MonoBehaviour
{
    public Transform player;
    public Text posBlock;
    public Text posPlayer;

    private bool visible = false;
    private int curTime;

    void Start()
    {
        
    }

    void Update()
    {
        posBlock.text = "позиция блока: \n" + this.transform.position.ToString() + "\n";
        posBlock.text += "длина = " + this.transform.localScale.x.ToString() + "\n";
        posBlock.text += "ширина = " + this.transform.localScale.y.ToString() + "\n";
        posBlock.text += "высота = " + this.transform.localScale.z.ToString() + "\n";

        posPlayer.text = "позиция камеры: \n" + player.position.ToString() + "\n";
        posPlayer.text += "дистанция от камеры: \n" + Vector3.Distance(player.position, transform.position) + " м\n";
        posPlayer.text += visible ? "объект виден" : "объект не виден";

        if (Input.GetKeyDown(KeyCode.R))
        {
            string text = "";
            text += visible ? "объект виден \n" : "объект не виден \n";
            text += "позиция блока: \n" + this.transform.position.ToString() + "\n";
            text += "длина объекта = " + this.transform.localScale.x.ToString() + "\n";
            text += "ширина объекта = " + this.transform.localScale.y.ToString() + "\n";
            text += "высота объекта = " + this.transform.localScale.z.ToString() + "\n";

            curTime = (int)Time.time;

            File.WriteAllText("Data/obj_" + curTime + ".txt", text);
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
}