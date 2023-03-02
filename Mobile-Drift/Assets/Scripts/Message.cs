using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Message : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Say(InputAction.CallbackContext context){ //respond to input
        if(context.performed){
            Debug.Log(context.ReadValue<Vector2>());
        }
    }
}
