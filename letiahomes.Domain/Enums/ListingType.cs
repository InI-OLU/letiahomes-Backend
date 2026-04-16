using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace letiahomes.Domain.Enums
{
    public enum ListingType
    {
        EntirePlace = 1,    // tenant gets the whole property
        PrivateRoom = 2,    // tenant gets a room, shares common areas
        SharedRoom = 3      // tenant shares a room
    }
}
