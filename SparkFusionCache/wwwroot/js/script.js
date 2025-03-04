let timerInterval;

async function fetchPokemon() {
    // Reset and start the timer
    clearInterval(timerInterval);
    const timerElement = document.getElementById('timer');
    timerElement.innerText = '0s';
    timerElement.style.display = 'block';
    let seconds = 0;
    timerInterval = setInterval(() => {
        seconds++;
        timerElement.innerText = `${seconds}s`;
    }, 1000);

    const name = document.getElementById('pokemonName').value.toLowerCase();
    const types = Array.from(document.querySelectorAll('input[name="type"]:checked')).map(cb => cb.value.toLowerCase()).join('|');
    const isBreakChecked = document.getElementById('breakCheckbox').checked;
    let url = `/pokemon/${name}`;
    if (types) {
        url += `?types=${types}`;
    }
    if (isBreakChecked) {
        url += (url.includes('?') ? '&' : '?') + 'fake=true';
    }
    const startTime = performance.now();

    try {
        const response = await fetch(url);
        const endTime = performance.now();
        const responseTime = endTime - startTime;

        document.getElementById('statusCode').innerText = response.status;
        document.getElementById('responseTime').innerText = responseTime.toFixed(2);

        // Show status code and response time
        document.getElementById('responseInfo').style.display = 'block';

        if (response.ok) {
            const data = await response.json();
            const pokemonImage = document.getElementById('pokemonImage');
            pokemonImage.src = data.image;
            pokemonImage.style.display = 'block';

            const pokemonDetails = document.getElementById('pokemonDetails');
            pokemonDetails.innerHTML = `
                <p>Name: ${data.name.charAt(0).toUpperCase() + data.name.slice(1)}</p>
                <p>Types: ${data.types.map(type => `<span class="type ${type}">${type}</span>`).join(' ')}</p>
            `;
        } else {
            console.error('Error fetching Pokémon:', response.statusText);
        }
    } catch (error) {
        console.error('Error fetching Pokémon:', error);
        document.getElementById('statusCode').innerText = error.message;
        document.getElementById('responseTime').innerText = 'N/A';

        // Show status code and response time
        document.getElementById('responseInfo').style.display = 'block';
    }
}

function toggleTypes() {
    const typesContainer = document.getElementById('typesContainer');
    if (typesContainer.style.display === 'none' || typesContainer.style.display === '') {
        typesContainer.style.display = 'block';
    } else {
        typesContainer.style.display = 'none';
    }

    // Clear any selected checkboxes
    const checkboxes = document.querySelectorAll('#typesContainer input[type="checkbox"]');
    checkboxes.forEach(checkbox => {
        checkbox.checked = false;
    });
}

function updateClearButtonState() {
    const checkboxes = document.querySelectorAll('#typesContainer input[type="checkbox"]');
    const clearButton = document.getElementById('clearButton');
    clearButton.disabled = !Array.from(checkboxes).some(checkbox => checkbox.checked);
}

async function clearTypes() {
    const types = Array.from(document.querySelectorAll('input[name="type"]:checked')).map(cb => cb.value.toLowerCase()).join('|');
    if (types) {
        const url = `/clear/${types}`;
        try {
            const response = await fetch(url);
            if (response.ok) {
                console.log('Selected types cleared successfully');
            } else {
                console.error('Error clearing selected types:', response.statusText);
            }
        } catch (error) {
            console.error('Error clearing selected types:', error);
        }
    }
}

// Add event listeners to checkboxes to update the clear button state
document.querySelectorAll('#typesContainer input[type="checkbox"]').forEach(checkbox => {
    checkbox.addEventListener('change', updateClearButtonState);
});
