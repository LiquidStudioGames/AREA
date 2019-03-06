using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    interface IDamageable
    {
    /// <summary>
    /// Any object that will interact with bullets will implement this interface
    /// </summary>


    void OnHit(int damage);

    }
