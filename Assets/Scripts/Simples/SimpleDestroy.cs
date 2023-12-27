using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDestroy : MonoBehaviour
{
    [SerializeField] private float _delay;

    private void Start()
    {
        Co_DelayedDestroy();
    }

    private IEnumerator Co_DelayedDestroy()
    {
        yield return new WaitForSeconds(_delay);

        Destroy(gameObject);
    }
}
