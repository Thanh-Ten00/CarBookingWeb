


function initAutocomplete() {
    var originInput = document.getElementById('origin');
    var destinationInput = document.getElementById('destination');

    var originAutocomplete = new google.maps.places.Autocomplete(originInput);
    var destinationAutocomplete = new google.maps.places.Autocomplete(destinationInput);
}

