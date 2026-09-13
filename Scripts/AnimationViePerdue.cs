using System;
using System.Collections;
using UnityEngine;

public class AnimationViePerdue : MonoBehaviour
{
    private Coroutine coroutineAnimationViePerdue;
    [SerializeField] private GameObject lumiereViePerdue;

    private void Start()
    {
        lumiereViePerdue.SetActive(false);
    }

    public void Demarrer()
    {
        if(coroutineAnimationViePerdue != null)
            StopCoroutine(coroutineAnimationViePerdue);
        
        coroutineAnimationViePerdue = StartCoroutine(ViePerdue());
    }


    private IEnumerator ViePerdue()
    {
        lumiereViePerdue.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        lumiereViePerdue.SetActive(false);
    }
}
