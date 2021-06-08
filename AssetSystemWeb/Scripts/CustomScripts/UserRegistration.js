$(document).ready(function () { 

    var systemRootUrl = $('#SystemRootUrl').val();
    $('#CompEmployeeNum').focusout(function () {    
        if ($(this).val() != "") {
            validateIdNum($(this).val());
        }
    });

    $('#updateUser').click(function () {
        if ($('#UpdateDetailFrm').valid()) {
            $('#UpdateDetailFrm').submit();

        } else {
            alert('Input Not Valid');
        }
    });


    //$('#AddRoles').click(function () {
    //    alert(' add role click')


    //});


    $('#registerUser').click(function () {

        validateIdNum($('#CompEmployeeNum').val());

        if ($('#IdNumValidation').hasClass('text-success')) {
            if ($('#RegistrationView').valid()) {
                $('#RegistrationView').submit();

            } else {
                //alert('notvalid');
            }
        } else { /*alert('not Valid'); */}

    });




    function validateIdNum(IdNumUn) {

        $.ajax({
            url: systemRootUrl + "/SystemUser/CheckIdNum",
            method: "POST",
            dataType: 'JSON',
            data: { Id: IdNumUn },
            beforeSend: function UsernameVerificationb4send() {

            },
            success: function UsernameVerificationSuccess(response) {
                if (response.IsExist) {
                    $('#IdNumValidation').text('Id Number already used').removeClass('text-success').addClass('text-danger')
                } else {
                    $('#IdNumValidation').text('Id Number is valid').removeClass('text-danger').addClass('text-success')
                }
            },
            error: function UsernameVerificationFailed() {

            }
        });


    };

})