function afficherDetails(noVendeur) {
    const actionUrl = `/Client/Inscription`// + noVendeur//`/Gestionnaire/DetailsDemande/${noVendeur}`;

    const formData = $('#idForm').serialize();

    $.post(actionUrl, {
        data: formData,
        success: gererDetails,
    });
}

function gererDetails(donnees) {
    const modalDiv = $('#contenuDetails');

    $('#idForm')[0].innerHTML = donnees;
    modalDiv[0].innerHTML = donnees;
}