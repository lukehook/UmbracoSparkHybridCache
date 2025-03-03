async function fetchPokemon() {
    const name = document.getElementById('pokemonName').value.toLowerCase();
    let url = `/pokemon/${name}`;

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
