using UnityEngine;

public class RealityChange : MonoBehaviour
{
    private bool isReality = true;

    private GameObject[] isRealObject;
    private GameObject[] isNotRealObject;

    void Start()
    {
        if (isRealObject == null)
            isRealObject = GameObject.FindGameObjectsWithTag("real");

        if (isNotRealObject == null)
            isNotRealObject = GameObject.FindGameObjectsWithTag("notreal");

        GetReality();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (isReality == true)
                GetUnReality();
            else
                GetReality();
        }
    }

    private void GetReality()
    {
        Time.timeScale = 1f;
        isReality = true;

        for (int i = 0; i < isNotRealObject.Length; i++)
        {
            isRealObject[i].SetActive(true);
            isNotRealObject[i].SetActive(false);
        }
    }

    private void GetUnReality()
    {
        Time.timeScale = 0.5f;
        isReality = false;

        for (int i = 0; i < isRealObject.Length; i++)
        {
            isRealObject[i].SetActive(false);
            isNotRealObject[i].SetActive(true);
        }
    }
}
