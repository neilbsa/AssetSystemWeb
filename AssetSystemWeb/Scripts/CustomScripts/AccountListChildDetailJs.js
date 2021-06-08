$(document).ready(function () { 
    var systemRootUrl = $('#SystemRootUrl').val();
    function AddRole(e) {

        $('#IncludedRoles').append('<option value="' + e + '">' + e + '</option>');

    };

    function RemoveRole(e) {
        $('#SystemRoles').append('<option value="' + e + '">' + e + '</option>');

    };


    function AddUserCompany(e) {

        $('#AssignedCompanies').append('<option value="' + e.val() + '">' + e.text() + '</option>');

    };

    function RemoveCompany(e) {
        $('#Companies').append('<option value="' + e.val() + '">' + e.text() + '</option>');

    };

    $('#btnAddCompany').on('click', function () {

        $('#Companies option:selected').each(function () {
            AddUserCompany($(this));
            $(this).remove();
        });
    });


    $('#btnRemoveCompany').on('click', function () {

        $('#AssignedCompanies option:selected').each(function () {
            RemoveCompany($(this));
            $(this).remove();
        });
    });







    $('#newAccessbtnSubmit').click(function () {
        var strUn = $('#UserName').val();
        validateUsername(strUn);

        //alert(strUn);
        if ($('#RegisterSystemAccess').valid()) {
            var IsExist = $('#ValidationErrorUsername').hasClass('text-success');
            if (IsExist) {
                alert('NotExist')

                $("select > option").prop("selected", true);

                $('#RegisterSystemAccess').submit();
            } else {
                alert('Exist')
            }
        }
    });



 







    $('#UpdateAccessbtnSubmit').click(function () {
        var strUn = $('#UserName').val();
        var oldUser = $('#oldUsername').val();

        alert("old" + oldUser + "New" + strUn);

        if (strUn !== oldUser) {
            validateUsername(strUn);
        } else {
            $('#ValidationErrorUsername').val('Username Not Changed').addClass('text-success');
        }

        var pw = $('#Password').val();
        


        if (pw === '') {
            $('#IsChangePassword').val('false');
            $('#Password').val('Undefined');
            $('#ConfirmPassword').val('Undefined');
        } else {
            $('#IsChangePassword').val('true');
        }

        if ($('#RegisterSystemAccess').valid()) {
            var IsExist = $('#ValidationErrorUsername').hasClass('text-success');

            if (IsExist) {
                alert('NotExist')
                $("select > option").prop("selected", true);
                $('#RegisterSystemAccess').submit();
            } else {
                alert('Exist')
            }


        }


        //alert('cloick')
    });


    $('#oldUsername').val($('#UserName').val());


    $('#UserName').focusout(function () {
        var strUn = $(this).val();
        var olUn = $('#oldUsername').val();
        if (olUn !== '') {

            if (strUn !== olUn) {
                validateUsername(strUn);
            } else {
                $('#ValidationErrorUsername').text('Username not changed').removeClass('text-danger').addClass('text-success');
            }

        } else {
            if (strUn !== '') {
                validateUsername(strUn);
            }

        }
    });
    $('#AddRoles').on('click', function () {

        $('#SystemRoles option:selected').each(function () {
            AddRole($(this).text());
            $(this).remove();
        });
    });




    $('#RemoveRole').on('click', function () {
        $('#IncludedRoles option:selected').each(function () {

            RemoveRole($(this).text());
            $(this).remove();
        });
    });



    function validateUsername(strUn) {

        $.ajax({
            url: systemRootUrl + "/Account/CheckUsername",
            method: "POST",
            dataType: 'JSON',
            data: { username: strUn },
            beforeSend: function UsernameVerificationb4send() {

            },
            success: function UsernameVerificationSuccess(response) {
                if (response.IsExist) {
                    $('#ValidationErrorUsername').text('Username already used').removeClass('text-success').addClass('text-danger');
                    return false;
                } else {
                    $('#ValidationErrorUsername').text('Username is valid').removeClass('text-danger').addClass('text-success');
                    return true;
                }
            },
            error: function UsernameVerificationFailed() {

            }
        });
    }







})



