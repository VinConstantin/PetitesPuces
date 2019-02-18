$(function () {

    $('#oublieMDPCourriel').submit(function (e) {
      
        $("#recuperationMDP").delay(100).fadeIn(100);
        $("#oublieMDPCourriel").fadeOut(100);
        e.preventDefault();

    });
    

});