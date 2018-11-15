using HuntingSpots.Model.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace HuntingSpots.Model.Service.Interface
{
    public interface ILocationService
    {
        Location Get(long id);
        List<Location> GetAllLocations();
        void DeleteLocation(long id);
        void UpsertLocation(Location location);
        void UpsertLocations(List<Location> locations);
    }
}
