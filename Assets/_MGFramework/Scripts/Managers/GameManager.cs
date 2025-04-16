using _KMH_Framework;
using _KMH_Framework.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class GameManager : MonoBehaviour
    {
        private Inventory playerInventory;

        private void Awake()
        {
            playerInventory = Player.Instance.GetComponent<Inventory>();
        }

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

            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                playerInventory.Push(PoolType.Stackable_RawMeat);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                playerInventory.Push(PoolType.Stackable_Wood);
            }
        }
#endif
    }
}
