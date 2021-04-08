using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistanseCube : MonoBehaviour
{
    public Transform player;
    public Text posPlayer;

    void Start()
    {
        
    }

    void Update()
    {
        posPlayer.text = "позиция камеры: \n" + player.position.ToString() + "\n";
        posPlayer.text += "дистанция от камеры: \n" + Vector3.Distance(player.position, transform.position) + " м\n";
    }
}
