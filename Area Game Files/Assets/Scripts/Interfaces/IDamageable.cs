using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    interface IDamageable
    {
    //Any object that will interact with bullets will implement this interface
    void OnHit(int damage);

    }
