let currentStep = 1;
const totalSteps = 5;

const steps = document.querySelectorAll(".form-step");
const progressBars = document.querySelectorAll(".progress-bar");
const currentStepText = document.getElementById("currentStep");

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
            const age = parseInt(document.getElementById("ageInput").value);
            if (!age || age < 18) {
                alert("You must be at least 18 years old.");
                return;
            }
        }

        if (currentStep === 3) {
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

/* Photo preview */
const photoInputs = document.querySelectorAll(".photo-input");
const photoCount = document.getElementById("photoCount");

photoInputs.forEach(input => {
    input.addEventListener("change", function () {
        const file = this.files[0];
        if (!file) return;

        const reader = new FileReader();
        reader.onload = e => {
            const img = document.createElement("img");
            img.src = e.target.result;

            const parent = this.parentElement;
            parent.innerHTML = "";
            parent.appendChild(img);

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

/* Interests logic */
const interests = document.querySelectorAll(".interest");
const interestCount = document.getElementById("interestCount");

interests.forEach(btn => {
    btn.addEventListener("click", () => {
        btn.classList.toggle("selected");

        const selected = document.querySelectorAll(".interest.selected");

        if (selected.length > 5) {
            btn.classList.remove("selected");
            return;
        }

        interestCount.textContent = `${selected.length}/5 selected`;
    });
});

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