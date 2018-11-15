// Write your Javascript code.

var mapView = function (config) {

    config = $.extend(true, {
        createUrl: null,
    }, config);

    var bigMap,
        bigMapOptions = {
            zoom: 12,
            center: new google.maps.LatLng(44.293392, -90.698946),
            mapTypeId: google.maps.MapTypeId.HYBRID
        };

    ////Sort by name
    //Places.sort(function (a, b) {
    //    if (a.name < b.name)
    //        return -1;
    //    if (a.name > b.name)
    //        return 1;
    //    // a must be equal to b
    //    return 0;
    //});

    //Construct map addresses
    $.each(Places, function (index, place) {
        var temp = 'https://maps.google.com/?q={latitude},{longitude}&t=h&z={zoom}';
        
        temp = temp.replace('{latitude}', place.Latitude || 0);
        temp = temp.replace('{longitude}', place.Longitude || 0);
        temp = temp.replace('{zoom}', place.zoom || 18);

        place.link = temp;
    });

    bigMap = new google.maps.Map(document.getElementById('bigMap'), bigMapOptions);

    var infowindow = new google.maps.InfoWindow();
    var positionMarker;
    var positionAccuracyCircle;

    $.each(Places, function (index, place) {

        var marker = new google.maps.Marker({
            position: new google.maps.LatLng(place.Latitude, place.Longitude),
            map: bigMap
        });

        google.maps.event.addListener(marker, 'click', function () {
            infowindow.setContent('<div style="display: table;"><a href="' + place.link + '" title="Go to Google Maps"><b>' + place.Name + '</b></a></div>');
            infowindow.open(bigMap, marker);
        });
    });

    GetLocation(function (position) {
        var center = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);

        // $('#lat').text(position.coords.latitude);
        // $('#long').text(position.coords.longitude);
        // $('#updates').text(++positionUpdates);

        //$('#log').append('<tr><td>' + position.coords.latitude + '</td><td>' + position.coords.longitude + '</td><td>' + position.coords.accuracy + '</td><td>' + (position.coords.speed || 'N/A') + '</td><td>' + (position.coords.speed * 2.23694 || 'N/A') + '</td><td>' + (position.coords.heading || 'N/A') + '</td></tr>');

        if (!positionMarker) {
            positionMarker = new google.maps.Marker({
                position: center,
                icon: {
                    fillOpacity: 1,
                    fillColor: '#007bff',
                    strokeColor: 'white',
                    strokeWeight: 2,
                    path: google.maps.SymbolPath.CIRCLE,
                    scale: 6
                },
                map: bigMap
            });
                       
            google.maps.event.addListener(positionMarker, 'click', function () {
                infowindow.setContent('<a href="' + config.createUrl.replace('latV', position.coords.latitude).replace('longV', position.coords.longitude) + '">Save Location</a>');
                infowindow.open(bigMap, positionMarker);
            });

            bigMap.panTo(new google.maps.LatLng(position.coords.latitude, position.coords.longitude));
        } else {
            positionMarker.setPosition(center);
        }

        if (!positionAccuracyCircle) {
            positionAccuracyCircle = new google.maps.Circle({
                strokeColor: '#007bff',
                strokeOpacity: 0.8,
                strokeWeight: 2,
                fillColor: '#007bff',
                fillOpacity: 0.25,
                map: bigMap,
                center: center,
                radius: position.coords.accuracy,
                clickable: false
            });
        } else {
            positionAccuracyCircle.setCenter(center);
            positionAccuracyCircle.setRadius(position.coords.accuracy);
        }
    });

    function GetLocation(success, fail) {
        if (navigator.geolocation) {
            //navigator.geolocation.getCurrentPosition(success, function showError(error) {
            navigator.geolocation.watchPosition(success, function (error) {
                switch (error.code) {
                    case error.PERMISSION_DENIED:
                        alert("User denied the request for Geolocation.");
                        break;
                    case error.POSITION_UNAVAILABLE:
                        alert("Location information is unavailable.");
                        break;
                    case error.TIMEOUT:
                        alert("The request to get user location timed out.");
                        break;
                    case error.UNKNOWN_ERROR:
                    default:
                        alert("An unknown error occurred.");
                        break;
                }
            }, {
                    enableHighAccuracy: true
                });
        } else {
            alert("Geolocation is not supported by this browser.");
        }
    };
};