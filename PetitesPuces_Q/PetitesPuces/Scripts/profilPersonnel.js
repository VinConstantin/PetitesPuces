$(function () {

    $('#btnModifier').click(function (e) {
        $("#modifierProfil-form").delay(100).fadeIn(100);
        $("#card-form").fadeOut(100);
        e.preventDefault();
      
    });
    $('#btnRetour').click(function (e) {
        $("#card-form").delay(100).fadeIn(100);
        $("#modifierProfil-form").fadeOut(100);
        e.preventDefault();
    });

});