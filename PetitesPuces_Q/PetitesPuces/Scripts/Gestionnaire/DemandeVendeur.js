function afficherDetails(noVendeur) {
    const actionUrl = `/Gestionnaire/DetailsDemande/${noVendeur}`;

    $.ajax(actionUrl, {
        success: gererDetails,
    });
}

function gererDetails(donnees) {
    const modalDiv = $('#contenuDetails');

    modalDiv[0].innerHTML = donnees;
}