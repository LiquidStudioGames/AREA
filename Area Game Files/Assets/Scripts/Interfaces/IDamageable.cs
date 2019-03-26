using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Any object that will interact with bullets will implement this interface
/// </summary>
interface IDamageable
{
    void OnHit(int damage);

}
