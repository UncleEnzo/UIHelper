using System.Collections;
using UnityEngine;

namespace Nevelson.UIHelper
{
    public class UIDestroyer : MonoBehaviour
    {
        IEnumerator Start()
        {
            yield return null;
            Destroy(gameObject);
        }
    }
}