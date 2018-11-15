using Google.Cloud.Datastore.V1;
using System.ComponentModel.DataAnnotations;

namespace HuntingSpots.Model.Domain
{
    public class Location
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }

        public static Location From(Entity entity)
        {
            return new Location
            {
                Id = entity.Key.Path[0].Id,
                Name = entity[nameof(Name)].StringValue,
                Latitude = entity[nameof(Latitude)].DoubleValue,
                Longitude = entity[nameof(Longitude)].DoubleValue,
            };
        }

        public void Update(Entity entity)
        {
            entity[nameof(Name)] = Name;
            entity[nameof(Latitude)] = new Value { DoubleValue = Latitude };
            entity[nameof(Longitude)] = new Value { DoubleValue = Longitude };
        }
    }
}
