function showOptions() {
    const options = document.getElementById('pictureOptions');
    options.style.display = options.style.display === 'none' ? 'block' : 'none';
}

function uploadPicture() {
    document.getElementById('pictureInput').click();
}

document.addEventListener('DOMContentLoaded', function () {
    const pictureInput = document.getElementById('pictureInput');
    if (pictureInput) {
        pictureInput.addEventListener('change', async (e) => {
            const file = e.target.files[0];
            if (file) {
                const formData = new FormData();
                formData.append('Foto', file);
                formData.append('UtenteId', userId);

                try {
                    const response = await fetch('/Utenti/UpdateProfilePicture', {
                        method: 'POST',
                        body: formData,
                        headers: {
                            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
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
