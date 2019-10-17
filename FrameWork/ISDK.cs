using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  interface ISDK
{
    string Name { get; }  
    void Init();
    void Disable();
    
}
