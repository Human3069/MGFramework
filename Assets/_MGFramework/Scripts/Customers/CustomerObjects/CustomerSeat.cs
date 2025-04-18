using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class CustomerSeat : MonoBehaviour
    {
        [ReadOnly]
        public Customer OccupiedCustomer = null;
    }
}
