(function () {
    const form = document.getElementById('step1Form');
    form.addEventListener('submit', (e) => {
        e.preventDefault();
        const input = document.getElementById('firstName');
        const name = input.value.trim();
        if (name === '') {
            alert('👋 Please enter your first name (demo)');
        } else {
            alert(`Thanks, ${name}! (demo — continue to step 2)`);
        }
    });

    // optional: simple demo for top menu links (no navigation)
    document.querySelectorAll('.nav-menu a, .auth-links a, .footer-privacy a').forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            const text = e.target.innerText.trim();
            if (text === 'Home') alert('🏠 Home (demo)');
            else if (text === 'Privacy') alert('🔒 Privacy page (demo)');
            else if (text === 'Register') alert('📝 Register (demo)');
            else if (text === 'Login') alert('🔑 Login (demo)');
        });
    });
})();