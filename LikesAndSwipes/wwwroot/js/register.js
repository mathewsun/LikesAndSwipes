function initAutocomplete() {

    var ac = new google.maps.places.Autocomplete(
        (document.getElementById('addressInput')), {
        types: ['geocode']
    });

    ac.addListener('place_changed', function () {

        var place = ac.getPlace();

        if (!place.geometry) {
            // User entered the name of a Place that was not suggested and
            // pressed the Enter key, or the Place Details request failed.
            console.log("No details available for input: '" + place.name + "'");
            return;
        }

        document.getElementById('addressLatitude').value = place.geometry.location.lat();
        document.getElementById('addressLongitude').value = place.geometry.location.lng();
    });
}

function onClickRegister(e) {
    e.preventDefault();
    grecaptcha.enterprise.ready(async () => {
        const token = await grecaptcha.enterprise.execute('6LfTSrQsAAAAALo1Ru9UdeHOwDzKqswse4xLvi02', { action: 'login' });
    });
}