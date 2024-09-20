using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ShadowLib.ChatLog
{
    public class ChatLogItem : MonoBehaviour
    {
        public Text text;

        public void Start()
        {
            StartCoroutine(destroy());
        }

        public IEnumerator destroy()
        {
            yield return new WaitForSeconds(4);
            Destroy(gameObject);
        }
    }
}
