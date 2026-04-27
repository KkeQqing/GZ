using UnityEngine;

public class UIRegister : MonoBehaviour
{
    void Awake()
    {
        UIManager.Instance.Register(GetComponent<BaseUI>());
    }
}