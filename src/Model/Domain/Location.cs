using Google.Cloud.Datastore.V1;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HuntingSpots.Model.Domain
{
    public class Location
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Latitude { get; set; }
        [Required)]
        public decimal Longitude { get; set; }

        public static Location From(Entity entity)
        {
            return new Location
            {
                Id = entity.Key.Path[0].Id,
                Name = entity[nameof(Name)].StringValue,
                Latitude = Convert.ToDecimal(entity[nameof(Latitude)].DoubleValue),
                Longitude = Convert.ToDecimal(entity[nameof(Longitude)].DoubleValue),
            };
        }

        public void Update(Entity entity)
        {
            entity[nameof(Name)] = Name;
            entity[nameof(Latitude)] = new Value() { DoubleValue = Convert.ToDouble(Latitude) };
            entity[nameof(Longitude)] = new Value() { DoubleValue = Convert.ToDouble(Longitude) };
        }
    }
}
