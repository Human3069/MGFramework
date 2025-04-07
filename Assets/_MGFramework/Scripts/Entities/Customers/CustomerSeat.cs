using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGFramework
{
    public class CustomerSeat : MonoBehaviour
    {
        [ReadOnly]
        [SerializeField]
        private Customer _occupiedCustomer = null;
        public Customer OccupiedCustomer
        {

            get
            {
                return _occupiedCustomer;
            }
            set
            {
                if (_occupiedCustomer != value)
                {
                    _occupiedCustomer = value;
                }
            }
        }
    }
}