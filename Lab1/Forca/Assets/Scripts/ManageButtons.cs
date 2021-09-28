using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageButtons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Inicializa o score com -1 para que na primeira atualização da cena fique zero
        PlayerPrefs.SetInt("score", -1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Carrega a cena principal do jogo
    public void StartMundoGame()
    {
        SceneManager.LoadScene("Lab1");
    }
}
