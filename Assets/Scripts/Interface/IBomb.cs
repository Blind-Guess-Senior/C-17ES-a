using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBomb
{
    float power { get; }
    float explodeTime { get; }
    
    // Need kill itself. And do visual effects
    void Explode();
}
