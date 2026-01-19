using System.Collections.Generic;
using UnityEngine;

namespace ProjectClasses
{
    [System.Serializable]
    public class CartInfo
    {

        public string cart_id;
        public float speed;

        public CartInfo()
        {
            this.speed = AppConstants.CartDefaultSpeed;
        }

        public CartInfo(string cart_id) : this()
        {
            this.cart_id = cart_id;
        }

    }
} 
