using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuggingFace.API;

public class DummySend : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DummyCallAPI();
    }

    void DummyCallAPI()

    {
        Debug.Log("DummyCallAPI");
        string[] labels = new string[] { "bad", "mean", "neutral" };
        // We just send a "Dummy" message to the API to activate the API
        HuggingFaceAPI.ZeroShotTextClassification("Hello", outputClassification => {
            // On Success
        }, error => {
            // On Error
            Debug.Log("Error: " + error);
        }, labels);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
