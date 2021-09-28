using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MostraUltimaPalavra : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Recupera a palavra oculta finalizada com sucesso 
        GameObject.Find("palavraOculta").GetComponent<Text>().text = PlayerPrefs.GetString("ultimaPalavraOculta");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
