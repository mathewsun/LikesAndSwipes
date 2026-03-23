let currentStep = 1;
const totalSteps = 7;

const steps = document.querySelectorAll(".form-step");
const progressBars = document.querySelectorAll(".progress-bar");
const currentStepText = document.getElementById("currentStep");
const sexInput = document.getElementById("sexInput");
const genderOptions = document.querySelectorAll(".gender-option");
const romanticPreferenceGroup = document.getElementById("romanticPreferenceGroup");
const romanticMenOption = document.getElementById("romanticMenOption");
const romanticWomenOption = document.getElementById("romanticWomenOption");

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
            const age = parseInt(document.getElementById("ageInput").value);
            if (!age || age < 18) {
                alert("You must be at least 18 years old.");
                return;
            }
        }

        if (currentStep === 5) {
            const uploaded = document.querySelectorAll(".photo-box img").length;
            if (uploaded < 2) {
                alert("Please upload at least 2 photos.");
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

/* Interests logic */
const interests = document.querySelectorAll(".interest");
const interestCount = document.getElementById("interestCount");
const selectedInterestsContainer = document.getElementById("selectedInterestsContainer");

interests.forEach(btn => {
    btn.addEventListener("click", () => {
        btn.classList.toggle("selected");

        const selected = document.querySelectorAll(".interest.selected");

        if (selected.length > 5) {
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
