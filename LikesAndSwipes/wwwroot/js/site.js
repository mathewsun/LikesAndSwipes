let currentStep = 1;
const totalSteps = 8;

const steps = document.querySelectorAll(".form-step");
const progressBars = document.querySelectorAll(".progress-bar");
const currentStepText = document.getElementById("currentStep");
const sexInput = document.getElementById("sexInput");
const genderOptions = document.querySelectorAll(".gender-option");
const romanticPreferenceGroup = document.getElementById("romanticPreferenceGroup");
const romanticMenOption = document.getElementById("romanticMenOption");
const romanticWomenOption = document.getElementById("romanticWomenOption");
const birthDayInput = document.getElementById("birthDayInput");
const addressInput = document.getElementById("addressInput");
const useCurrentLocationBtn = document.getElementById("useCurrentLocationBtn");
let hasPromptedForGeolocation = false;
let hasInitializedAddressAutocomplete = false;
let pendingGeolocationCoordinates = null;

function updateRomanticPreferenceOrder(selectedValue) {
    if (!romanticPreferenceGroup || !romanticMenOption || !romanticWomenOption) {
        return;
    }

    if (selectedValue == "true") {
        romanticPreferenceGroup.appendChild(romanticWomenOption);
        romanticPreferenceGroup.appendChild(romanticMenOption);
        return;
    }

    romanticPreferenceGroup.appendChild(romanticMenOption);
    romanticPreferenceGroup.appendChild(romanticWomenOption);
}

function updateUI() {
    steps.forEach(step => {
        step.classList.remove("active");
        if (parseInt(step.dataset.step) === currentStep) {
            step.classList.add("active");
        }
    });

    progressBars.forEach((bar, index) => {
        bar.classList.remove("active");
        if (index < currentStep) {
            bar.classList.add("active");
        }
    });

    currentStepText.textContent = currentStep;

    if (currentStep === 7) {
        promptAddressFromGeolocation();
    }
}

document.querySelectorAll(".next-btn").forEach(btn => {
    btn.addEventListener("click", () => {

        if (currentStep === 1) {
            const name = document.getElementById("nameInput").value.trim();
            if (!name) {
                alert("Please enter your name.");
                return;
            }
        }

        if (currentStep === 2) {
            const selectedSex = document.querySelector('input[name="Input.Sex"]:checked');
            if (!selectedSex) {
                alert("Please select your gender.");
                return;
            }
        }

        if (currentStep === 3) {
            const hasPreference = document.querySelectorAll('.preference-option input:checked').length > 0;
            if (!hasPreference) {
                alert("Please select at least one preference.");
                return;
            }
        }

        if (currentStep === 4) {
            const birthDayValue = document.getElementById("birthDayInput").value;
            if (!birthDayValue) {
                alert("Please enter your birthday.");
                return;
            }

            const birthDay = new Date(birthDayValue);
            const now = new Date();
            let age = now.getFullYear() - birthDay.getFullYear();
            const monthDiff = now.getMonth() - birthDay.getMonth();

            if (monthDiff < 0 || (monthDiff === 0 && now.getDate() < birthDay.getDate())) {
                age--;
            }

            //if (age < 18) {
            //    alert("You must be at least 18 years old.");
            //    return;
            //}
        }

        if (currentStep === 5) {
            const uploaded = document.querySelectorAll(".photo-box img").length;
            if (uploaded < 0) {
                alert("Please upload at least 2 photos.");
                return;
            }
        }

        if (currentStep === 7) {
            const addressValue = addressInput?.value?.trim();
            if (!addressValue) {
                alert("Please enter your address.");
                return;
            }
        }

        currentStep++;
        updateUI();
    });
});

document.querySelectorAll(".back-btn").forEach(btn => {
    btn.addEventListener("click", () => {
        currentStep--;
        updateUI();
    });
});

/* 2 step gender */
genderOptions.forEach(option => {
    option.addEventListener("click", () => {
        genderOptions.forEach(btn => btn.classList.remove("selected"));
        option.classList.add("selected");

        var selectedValue = $('input[name="Input.Sex"]:checked').val();

        /* 3 step prefer to meet */

        if (selectedValue == "true") {
            document.getElementById("Input_RomanticWomen").checked = true;
            document.getElementById("Input_RomanticMen").checked = false;
        }
        else {
            document.getElementById("Input_RomanticMen").checked = true;
            document.getElementById("Input_RomanticWomen").checked = false;
        }

        updateRomanticPreferenceOrder(selectedValue);
    });
});

/* Photo preview */
const photoInputs = document.querySelectorAll(".photo-input");
const photoCount = document.getElementById("photoCount");

photoInputs.forEach(input => {
    input.addEventListener("change", function () {
        const file = this.files[0];
        const parent = this.parentElement;
        const existingImage = parent.querySelector("img");
        const placeholder = parent.querySelector("span");

        if (!file) {
            existingImage?.remove();
            if (placeholder) {
                placeholder.style.display = "";
            }

            updatePhotoCount();
            return;
        }

        const reader = new FileReader();
        reader.onload = e => {
            if (!existingImage) {
                const img = document.createElement("img");
                img.src = e.target.result;
                parent.appendChild(img);
            }
            else {
                existingImage.src = e.target.result;
            }

            if (placeholder) {
                placeholder.style.display = "none";
            }

            updatePhotoCount();
        };
        reader.readAsDataURL(file);
    });
});

function updatePhotoCount() {
    const count = document.querySelectorAll(".photo-box img").length;
    photoCount.textContent = `${count}/6 photos added`;
}

updateUI();
updateRomanticPreferenceOrder($('input[name="Input.Sex"]:checked').val());
initGoogleAddressAutocomplete();

if (birthDayInput) {
    const minAgeDate = new Date();
    minAgeDate.setFullYear(minAgeDate.getFullYear());
    birthDayInput.max = minAgeDate.toISOString().split("T")[0];
}

/* Interests logic */
const interests = document.querySelectorAll(".interest");
const interestCount = document.getElementById("interestCount");
const selectedInterestsContainer = document.getElementById("selectedInterestsContainer");

interests.forEach(btn => {
    btn.addEventListener("click", () => {
        btn.classList.toggle("selected");

        const selected = document.querySelectorAll(".interest.selected");

        if (selected.length > 0) {
            btn.classList.remove("selected");
            return;
        }

        syncSelectedInterests();
        interestCount.textContent = `${selected.length}/5 selected`;
    });
});

function syncSelectedInterests() {
    if (!selectedInterestsContainer) {
        return;
    }

    selectedInterestsContainer.innerHTML = "";

    document.querySelectorAll(".interest.selected").forEach((button, index) => {
        const hiddenInput = document.createElement("input");
        hiddenInput.type = "hidden";
        hiddenInput.name = `Input.SelectedInterests[${index}]`;
        hiddenInput.value = button.dataset.interest || button.textContent.trim();
        selectedInterestsContainer.appendChild(hiddenInput);
    });
}

/* Registration validation */
document.getElementById("registerBtn").addEventListener("click", () => {
    const email = document.getElementById("email").value.trim();
    const password = document.getElementById("password").value;
    const confirmPassword = document.getElementById("confirmPassword").value;

    if (!email || !password || !confirmPassword) {
        alert("Please fill all fields.");
        return;
    }

    if (password !== confirmPassword) {
        alert("Passwords do not match.");
        return;
    }

    //alert("Registration complete 🎉");
});

function initGoogleAddressAutocomplete() {
    if (!addressInput) {
        return;
    }

    const apiKey = window.googleMapsApiKey;
    if (!apiKey) {
        return;
    }

    const script = document.getElementById("googlePlacesScript");
    if (!script) {
        return;
    }

    if (window.google?.maps?.places) {
        attachGoogleAutocomplete();
        return;
    }

    if (!script.src) {
        script.src = `https://maps.googleapis.com/maps/api/js?key=${encodeURIComponent(apiKey)}&libraries=places&loading=async&callback=onGoogleMapsReady`;
    }

    script.addEventListener("load", attachGoogleAutocomplete, { once: true });
}

function attachGoogleAutocomplete() {
    if (hasInitializedAddressAutocomplete || !window.google?.maps?.places || !addressInput) {
        return;
    }
    hasInitializedAddressAutocomplete = true;

    const autocomplete = new google.maps.places.Autocomplete(addressInput, {
        fields: ["formatted_address"],
        types: ["address"]
    });

    autocomplete.addListener("place_changed", () => {
        const place = autocomplete.getPlace();
        if (place?.formatted_address) {
            addressInput.value = place.formatted_address;
        }
    });

    if (pendingGeolocationCoordinates) {
        fillAddressFromCoordinates(pendingGeolocationCoordinates.latitude, pendingGeolocationCoordinates.longitude);
        pendingGeolocationCoordinates = null;
    }
}

window.onGoogleMapsReady = function () {
    attachGoogleAutocomplete();
};

function promptAddressFromGeolocation() {
    if (hasPromptedForGeolocation || !addressInput || addressInput.value.trim()) {
        return;
    }

    hasPromptedForGeolocation = true;

    if (!navigator.geolocation) {
        return;
    }

    const shouldDetectAddress = window.confirm("Allow browser geolocation to auto-fill your address?");
    if (!shouldDetectAddress) {
        return;
    }

    navigator.geolocation.getCurrentPosition(
        position => {
            fillAddressFromCoordinates(position.coords.latitude, position.coords.longitude);
        },
        () => {
            // User denied permission or location is unavailable.
        },
        { enableHighAccuracy: true, timeout: 10000, maximumAge: 0 }
    );
}

if (useCurrentLocationBtn) {
    useCurrentLocationBtn.addEventListener("click", () => {
        hasPromptedForGeolocation = false;
        promptAddressFromGeolocation();
    });
}

function fillAddressFromCoordinates(latitude, longitude) {
    if (!addressInput) {
        return;
    }

    if (!window.google?.maps?.Geocoder) {
        pendingGeolocationCoordinates = { latitude, longitude };
        return;
    }

    const geocoder = new google.maps.Geocoder();
    geocoder
        .geocode({ location: { lat: latitude, lng: longitude } })
        .then(response => {
            const result = response.results?.[0];
            if (result?.formatted_address) {
                addressInput.value = result.formatted_address;
            }
        })
        .catch(() => {
            // Ignore geocoding errors.
        });
}
