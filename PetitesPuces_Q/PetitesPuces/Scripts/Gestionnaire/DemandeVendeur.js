function afficherDetails(noVendeur) {
    const actionUrl = URLPATH+`/Gestionnaire/DetailsDemande/${noVendeur}`;
    console.log(actionUrl)
    const formData = $('#idForm').serialize();

    $.ajax(actionUrl, {
        method: "GET",
        data: formData,
        success: gererDetails,
        error: genererErreur
    });
}

function gererDetails(donnees) {
    const modalDiv = $('#contenuDetails');

    modalDiv[0].innerHTML = donnees;
}
function genererErreur(donnees) {
    const modalDiv = $('#contenuDetails');

    modalDiv[0].innerHTML = "<P>Une erreure est survenue!</P>";
}