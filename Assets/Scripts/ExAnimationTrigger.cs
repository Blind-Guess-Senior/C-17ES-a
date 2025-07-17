using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExAnimationTrigger : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ExPlayOver()
    {
        gameObject.SetActive(false);
    }
}
