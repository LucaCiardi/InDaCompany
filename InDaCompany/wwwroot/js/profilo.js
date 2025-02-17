document.addEventListener('DOMContentLoaded', function () {
    // Navigation functionality
    const navLinks = document.querySelectorAll('.nav-link');
    const contentSections = document.querySelectorAll('.content-section');

    navLinks.forEach(link => {
        link.addEventListener('click', function (event) {
            event.preventDefault();
            navLinks.forEach(nav => nav.classList.remove('active'));
            this.classList.add('active');
            contentSections.forEach(section => section.classList.add('d-none'));
            const targetId = this.getAttribute('href').substring(1);
            document.getElementById(`content-${targetId}`).classList.remove('d-none');
        });
    });

    // Profile picture functionality
    const pictureInput = document.getElementById('pictureInput');
    if (pictureInput) {
        pictureInput.addEventListener('change', async (e) => {
            const file = e.target.files[0];
            if (file) {
                const formData = new FormData();
                formData.append('Foto', file);
                formData.append('UtenteId', userId);
                formData.append('__RequestVerificationToken', document.querySelector('input[name="__RequestVerificationToken"]').value);

                try {
                    const response = await fetch('/Utenti/UpdateProfilePicture', {
                        method: 'POST',
                        body: formData,
                        credentials: 'include',
                        headers: {
                            'X-CSRF-TOKEN': document.querySelector('input[name="__RequestVerificationToken"]').value
                        }
                    });
                    const result = await response.json();
                    if (result.success) {
                        location.reload();
                    } else {
                        alert(result.message || 'Error uploading picture');
                    }
                } catch (error) {
                    console.error('Error:', error);
                    alert('Error uploading picture');
                }
            }
        });
    }
});

// Profile picture options
function showOptions() {
    const options = document.getElementById('pictureOptions');
    options.style.display = options.style.display === 'none' ? 'block' : 'none';
}

function uploadPicture() {
    document.getElementById('pictureInput').click();
}
