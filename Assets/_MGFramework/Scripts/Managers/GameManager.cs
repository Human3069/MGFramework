using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class GameManager : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                Time.timeScale += 1f;
            }
            else if (Input.GetKeyDown(KeyCode.Delete))
            {
                Time.timeScale = 1f;
            }
        }
#endif
    }
}
