$(function () {
    //hang on event of form with id=myform
    $("#register-form").submit(function (e) {

        //prevent Default functionality
        e.preventDefault();

        //get the action-url of the form
        var actionurl = e.currentTarget.action;

        //do your own request an handle the results
        $.ajax({
            url: actionurl,
            type: 'post',

            data: $("#register-form").serialize(),
            success: function (data) {

            }
        });

    });

});

$(function () {
    //hang on event of form with id=myform
    $("#register-form-vendeur").submit(function (e) {

        //prevent Default functionality
        e.preventDefault();

        //get the action-url of the form
        var actionurl = e.currentTarget.action;

        //do your own request an handle the results
        $.ajax({
            url: actionurl,
            type: 'post',

            data: $("#register-form").serialize(),
            success: function (data) {

            }
        });

    });

});