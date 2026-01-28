// wwwroot/js/cascade-dropdowns.js
// Gestion des listes déroulantes en cascade pour Marque → Modèle → Finition

(function () {
    'use strict';

    // Références vers les éléments HTML
    const marqueSelect = document.getElementById('MarqueId');
    const modeleSelect = document.getElementById('ModeleId');
    const finitionSelect = document.getElementById('FinitionId');

    // Vérifier que les éléments existent (on est bien sur la bonne page)
    if (!marqueSelect || !modeleSelect || !finitionSelect) {
        return;
    }

    // Fonction pour vider une liste déroulante et afficher un message
    function clearDropdown(selectElement, message) {
        selectElement.innerHTML = '';
        const option = document.createElement('option');
        option.value = '';
        option.textContent = message;
        selectElement.appendChild(option);
        selectElement.disabled = true;
    }

    // Fonction pour remplir une liste déroulante avec des données
    function populateDropdown(selectElement, data, selectedValue) {
        selectElement.innerHTML = '';

        // Option par défaut
        const defaultOption = document.createElement('option');
        defaultOption.value = '';
        defaultOption.textContent = '-- Sélectionnez --';
        selectElement.appendChild(defaultOption);

        // Ajouter les données
        data.forEach(item => {
            const option = document.createElement('option');
            option.value = item.id;
            option.textContent = item.nom;

            // Si c'est la valeur sélectionnée (mode Edit)
            if (selectedValue && item.id == selectedValue) {
                option.selected = true;
            }

            selectElement.appendChild(option);
        });

        selectElement.disabled = false;
    }

    // Charger les modèles selon la marque sélectionnée
    function loadModeles(marqueId, selectedModeleId = null) {
        if (!marqueId) {
            clearDropdown(modeleSelect, '-- Sélectionnez d\'abord une marque --');
            clearDropdown(finitionSelect, '-- Sélectionnez d\'abord un modèle --');
            return;
        }

        // Afficher un message de chargement
        clearDropdown(modeleSelect, 'Chargement...');
        clearDropdown(finitionSelect, '-- Sélectionnez d\'abord un modèle --');

        // Appel AJAX vers le serveur
        fetch(`/Voitures/GetModelesByMarque?marqueId=${marqueId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Erreur lors du chargement des modèles');
                }
                return response.json();
            })
            .then(data => {
                populateDropdown(modeleSelect, data, selectedModeleId);

                // Si on est en mode Edit et qu'un modèle est sélectionné, charger ses finitions
                if (selectedModeleId) {
                    const selectedFinitionId = finitionSelect.dataset.selectedValue;
                    loadFinitions(selectedModeleId, selectedFinitionId);
                }
            })
            .catch(error => {
                console.error('Erreur:', error);
                clearDropdown(modeleSelect, 'Erreur de chargement');
            });
    }

    // Charger les finitions selon le modèle sélectionné
    function loadFinitions(modeleId, selectedFinitionId = null) {
        if (!modeleId) {
            clearDropdown(finitionSelect, '-- Sélectionnez d\'abord un modèle --');
            return;
        }

        // Afficher un message de chargement
        clearDropdown(finitionSelect, 'Chargement...');

        // Appel AJAX vers le serveur
        fetch(`/Voitures/GetFinitionsByModele?modeleId=${modeleId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Erreur lors du chargement des finitions');
                }
                return response.json();
            })
            .then(data => {
                populateDropdown(finitionSelect, data, selectedFinitionId);
            })
            .catch(error => {
                console.error('Erreur:', error);
                clearDropdown(finitionSelect, 'Erreur de chargement');
            });
    }

    // Event listener : quand la marque change
    marqueSelect.addEventListener('change', function () {
        const marqueId = this.value;
        loadModeles(marqueId);
    });

    // Event listener : quand le modèle change
    modeleSelect.addEventListener('change', function () {
        const modeleId = this.value;
        loadFinitions(modeleId);
    });

    // Au chargement de la page (mode Edit)
    // Si une marque est déjà sélectionnée, charger ses modèles
    window.addEventListener('DOMContentLoaded', function () {
        const selectedMarqueId = marqueSelect.value;
        const selectedModeleId = modeleSelect.dataset.selectedValue;
        const selectedFinitionId = finitionSelect.dataset.selectedValue;

        if (selectedMarqueId) {
            loadModeles(selectedMarqueId, selectedModeleId);
        } else {
            clearDropdown(modeleSelect, '-- Sélectionnez d\'abord une marque --');
            clearDropdown(finitionSelect, '-- Sélectionnez d\'abord un modèle --');
        }
    });

})();