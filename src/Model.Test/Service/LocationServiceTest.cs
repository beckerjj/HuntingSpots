using HuntingSpots.Model.Domain;
using HuntingSpots.Model.Service;
using HuntingSpots.Model.Service.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HuntingSpots.Model.Test.Service
{
    [TestClass]
    public class LocationServiceTest
    {
        private ILocationService locationService;
        [TestInitialize]
        public void Init()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"C:\WIP\GCloudSecrets\Hunting Places-eb7de0d7f52b.json");
            locationService = new LocationService("hunting-places-1479526677988");
        }

        [TestMethod]
        public void Test()
        {
            var all = locationService.GetAllLocations();

            Assert.AreNotEqual(0, all.Count);
        }

        public class FileLocation
        {
            public string name { get; set; }
            public decimal latitude { get; set; }
            public decimal? latitudeMinutes { get; set; }
            public decimal longitude { get; set; }
            public decimal? longitudeMinutes { get; set; }

            public Location ToLocation()
            {
                return new Location
                {
                    Name = name,
                    Longitude = longitude + (longitudeMinutes.HasValue ? longitudeMinutes.Value/60 : 0),
                    Latitude = latitude + (latitudeMinutes.HasValue ? latitudeMinutes.Value / 60 : 0),
                };
            }
        }

        [TestMethod, Ignore]
        public void UpsertLocations()
        {
            var fileText = File.ReadAllText(@"C:\WIP\Site\HuntingPlaces\HuntingPlaces.js").Substring("var Places = ".Length).TrimEnd(';');

            var fileLocations = JsonConvert.DeserializeObject<List<FileLocation>>(fileText);

            var locations = fileLocations.Select(l => l.ToLocation()).ToList();

            locationService.UpsertLocations(locations);
        }
    }
}
