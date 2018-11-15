using Google.Cloud.Datastore.V1;
using HuntingSpots.Model.Domain;
using HuntingSpots.Model.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuntingSpots.Model.Service
{
    public class LocationService : ILocationService
    {
        private DatastoreDb _datastore;
        private KeyFactory _keyFactory;
        public string Kind => nameof(Location);

        public LocationService(string projectId)
        {
            _datastore = DatastoreDb.Create(projectId);
            _keyFactory = _datastore.CreateKeyFactory(Kind);
        }

        public Location Get(long id)
        {
            var entity = _datastore.Lookup(_keyFactory.CreateKey(id));

            var location = entity != null ? Location.From(entity) : null;

            return location;
        }

        public List<Location> GetAllLocations()
        {
            var all = _datastore.RunQuery(new Query(Kind)).Entities.Select(Location.From).ToList();

            return all;
        }

        private void UpsertLocation(DatastoreTransaction transaction, Location location)
        {
            Entity dbLocation = null;

            if (location.Id > 0)
            {
                dbLocation = transaction.Lookup(_keyFactory.CreateKey(location.Id));
            }

            if (dbLocation == null)
            {
                dbLocation = new Entity
                {
                    Key = _keyFactory.CreateIncompleteKey()
                };
            }

            location.Update(dbLocation);
            transaction.Upsert(dbLocation);
        }

        public void DeleteLocation(long id)
        {
            using (var transaction = _datastore.BeginTransaction())
            {
                transaction.Delete(_keyFactory.CreateKey(id));
                transaction.Commit();
            }
        }

        public void UpsertLocation(Location location)
        {
            using (var transaction = _datastore.BeginTransaction())
            {
                UpsertLocation(transaction, location);
                transaction.Commit();
            }
        }

        public void UpsertLocations(List<Location> locations)
        {
            using (var transaction = _datastore.BeginTransaction())
            {
                foreach (var location in locations)
                {
                    UpsertLocation(transaction, location);
                }
                transaction.Commit();
            }
        }
    }
}
