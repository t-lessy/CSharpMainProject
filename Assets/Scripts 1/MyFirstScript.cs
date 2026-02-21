using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MyFirstScript : MonoBehaviour
{
    public MyFirstScript()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        List<string> fruits = new List<string> { "Banana", "Alpple", "Dragonfruit", "Grabe" };
        fruits.Add("Pineapple");
        fruits.Add("Orange");
        fruits.Remove("Dragonfruit");
        fruits.Sort();
       
       

        foreach (string fruit in fruits)
        {
            Debug.Log(fruit);
        }

   
   

       
       
    }
}







    



































