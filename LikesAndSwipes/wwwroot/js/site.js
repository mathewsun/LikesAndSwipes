// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const steps = document.querySelectorAll(".form-step");
const progressBars = document.querySelectorAll(".progress-bar");
const nextBtns = document.querySelectorAll(".next-btn");
const backBtns = document.querySelectorAll(".back-btn");
const currentStepText = document.getElementById("currentStep");

let currentStep = 1;

function updateUI() {
    steps.forEach(step => {
        step.classList.remove("active");
        if (parseInt(step.dataset.step) === currentStep) {
            step.classList.add("active");
        }
    });

    progressBars.forEach(bar => {
        bar.classList.remove("active");
        if (parseInt(bar.dataset.step) <= currentStep) {
            bar.classList.add("active");
        }
    });

    currentStepText.textContent = currentStep;
}

nextBtns.forEach(btn => {
    btn.addEventListener("click", () => {
        if (currentStep === 1) {
            const name = document.getElementById("firstName").value.trim();
            if (name.length < 2) {
                alert("Please enter a valid name.");
                return;
            }
        }

        if (currentStep === 2) {
            const age = document.getElementById("age").value;
            if (age < 18 || age > 100) {
                alert("Age must be between 18 and 100.");
                return;
            }
        }

        if (currentStep < 4) {
            currentStep++;
            updateUI();
        }
    });
});

backBtns.forEach(btn => {
    btn.addEventListener("click", () => {
        if (currentStep > 1) {
            currentStep--;
            updateUI();
        }
    });
});

updateUI();

// PHOTO STEP FUNCTIONALITY
const photoSlots = document.querySelectorAll(".photo-slot input");
const photoCounter = document.getElementById("photoCounter");
const photoContinueBtn = document.getElementById("photoContinue");

let photoCount = 0;

photoSlots.forEach(input => {
    input.addEventListener("change", function () {
        if (this.files && this.files[0]) {
            const reader = new FileReader();

            reader.onload = e => {
                const img = document.createElement("img");
                img.src = e.target.result;

                const parent = this.parentElement;
                parent.innerHTML = "";
                parent.appendChild(img);

                updatePhotoCount();
            };

            reader.readAsDataURL(this.files[0]);
        }
    });
});

function updatePhotoCount() {
    photoCount = document.querySelectorAll(".photo-slot img").length;
    photoCounter.textContent = photoCount;
}

photoContinueBtn.addEventListener("click", () => {
    if (photoCount < 2) {
        alert("Please upload at least 2 photos.");
        return;
    }
});