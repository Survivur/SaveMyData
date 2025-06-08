using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;

public class PlayerParry : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] protected Counter counter = new Counter(1);
    [SerializeField] protected float coolDown = 2f;
    [SerializeField] protected float delay = 0.2f;

    [Header("Information")]
    [SerializeField, ReadOnly] protected bool goRight = false;
    [SerializeField, ReadOnly] protected bool parryFlag = false;
    [SerializeField, ReadOnly] protected Coroutine resetCoroutine;

    [Header("Object & Components", order = 0)]    
    [SerializeField, ReadOnly(true)] private GameObject _parrychild = null;
    public GameObject ParryChild => CodeExtensions.SetIfUnityNull(
        ref _parrychild,
        GameObjectRegistry.GetOrRegister(ObjectPath.Player_Parry, gameObject)
    );


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ParryChild.SetActive(false);
        Reset();
    }

    public void Parry()
    {
        if (!parryFlag)
        {
            ParryChild.SetActive(true);
            parryFlag = true;
            Counting();
            Invoke(nameof(ParryEnd), delay);
            Invoke(nameof(ParryAble), coolDown);
        }
    }

    public void Reset()
    {
        counter.Reset();
    }

    private void Counting()
    {
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }

        resetCoroutine = StartCoroutine(counter.CountResetCoroutine(coolDown));
        counter.Counting();
    }

    public void ParryEnd()
    {
        ParryChild.SetActive(false);
    }

    public void ParryAble()
    {
        parryFlag = false;
    }    
}
