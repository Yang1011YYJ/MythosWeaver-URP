using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMenu : MonoBehaviour
{
    public GameObject Circle;
    public GameObject Circle1;
    public Animator Circleanimator;
    public Animator Circle1animator;
    private void Start()
    {
        Circleanimator = Circle.GetComponent<Animator>();
        Circle1animator = Circle1.GetComponent<Animator>();
    }
    // Start is called before the first frame update
    public void CircleOnC()
    {
        Circleanimator.SetBool("open", true);
        Circle1animator.SetBool("open",true);
    }
}
