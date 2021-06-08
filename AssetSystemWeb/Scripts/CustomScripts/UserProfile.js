$(document).ready(function () { 
    var systemRootUrl = $('#SystemRootUrl').val();
    var table = $('#UserTable').DataTable(
        {
            responsive: true,
            "initComplete": function () {
                var api = this.api();
                api.$('tr').on("dblclick", function (e)
                {
                    var uid = $(this).attr('id');
                    formChild(uid);                                              
                });
                api.$('tr').hover(function () {
                    $(this).css('cursor', 'pointer');
                }, function () {
                    $(this).css('cursor', 'auto');
                });
            },
            "autoWidth": false
        });



    var userConsigment = $('#userConsignments').DataTable(
        {
            responsive: true,
            "initComplete": function () {
                var api = this.api();
               
                api.$('tr').hover(function () {
                    $(this).css('cursor', 'pointer');
                }, function () {
                    $(this).css('cursor', 'auto');
                });
            },
            "autoWidth": false
        });





    var tableAccess = $('#UserAccessTable').DataTable(
        {
            responsive: true,
            "initComplete": function () {

                var api = this.api();


                api.$('tr').on("dblclick", function (e) {
                    var uid = $(this).attr('Id');
              
                    updateAccess(uid);
                });


                api.$('tr').hover(function () {
                    $(this).css('cursor', 'pointer');
                }, function () {
                    $(this).css('cursor', 'auto');
                });
            },
            "autoWidth": false
        });




    function updateAccess(uid) {
        $.ajax({
            url: systemRootUrl + "/Account/UpdateUserAccess",
            dataType: "html",
            type: "GET",
            data: {UserId : uid},
            success: function (response) {

                $('#modalAccess').modal('show');
                $('#modalAccess > .modal-dialog > .modal-content > .modal-body').html('');
                $('#modalAccess > .modal-dialog > .modal-content > .modal-body').append(response);
                $('#modalAccess > .modal-dialog > .modal-content > .modal-header > .modal-title').html('Update User');
                $('#modalAccess > .modal-dialog > .modal-content > .modal-footer').html('');
                $('#modalAccess > .modal-dialog > .modal-content > .modal-footer').append(AccessUpdateBtn);

                $("#RegisterSystemAccess").removeData("validator");
                $("#RegisterSystemAccess").removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse("#RegisterSystemAccess");
            },
            error: function () {
                alert('error calling ajax');

            }
        });
    }


    var AccessAddBtn = '<button type="button" class="btn btn-success" id="newAccessbtnSubmit">Register</button>'

    var AccessUpdateBtn = '<button type="button" class="btn btn-success" id="UpdateAccessbtnSubmit">Update</button>'

 




    $('#CreateNewAccess').click(function () {
        $.ajax({
            url: systemRootUrl + "/Account/RegisterUser",
            dataType: "html",
            type: "GET",
            success: function (response) {

                $('#modalAccess').modal('show');
                $('#modalAccess > .modal-dialog > .modal-content > .modal-body').html('');
                $('#modalAccess > .modal-dialog > .modal-content > .modal-body').append(response);
                $('#modalAccess > .modal-dialog > .modal-content > .modal-header > .modal-title').html('Create User');
                $('#modalAccess > .modal-dialog > .modal-content > .modal-footer').html('');
                $('#modalAccess > .modal-dialog > .modal-content > .modal-footer').append(AccessAddBtn);
            
                $("#RegisterSystemAccess").removeData("validator");
                $("#RegisterSystemAccess").removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse("#RegisterSystemAccess");

            },
            error: function () {
                alert('error calling ajax');

            }
        });

    });






    function formChild(id) {
 
        $.ajax({
            url: systemRootUrl + "/SystemUser/ForUpdateData",
            type: "GET",
            dataType: "html",
            data: { Id: id },
            success: function (response) {
    
                //$('#editBody').empty();
                //$('#editBody').append(response);
                //$('#EditModal').modal('show');


                $('#userModel').modal('show');
                $('#userModel > .modal-dialog > .modal-content > .modal-body').html('');
                $('#userModel > .modal-dialog > .modal-content > .modal-body').append(response);
                $('#userModel > .modal-dialog > .modal-content > .modal-header > .modal-title').html('Update User');
                $('#userModel > .modal-dialog > .modal-content > .modal-footer').html('');
                $('#userModel > .modal-dialog > .modal-content > .modal-footer').append(updateBtn);
                $("#UpdateDetailFrm").removeData("validator");
                $("#UpdateDetailFrm").removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse("#UpdateDetailFrm");
          
            },
            error: function () {
                console.log('opening child ajax success error');
                console.log('AJAX ERROR');
            }
        });
    }

    var addBtn = '<button type="button" class="btn btn-default" data-dismiss="modal">Close</button><button type="button" class="btn btn-primary" id="registerUser">Add</button>'

                

    var updateBtn = '<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>' +
                    '<button type="button" class="btn btn-primary" id="updateUser">Update</button>'

    $('#registerNewUser').click(function () {
   

        $.ajax({
            url: systemRootUrl + "/SystemUser/Register",
            type: "GET",
            dataType: "html",
          
            success: function (response) {
                //console.log(response);
                $('#userModel').modal('show');
                $('#userModel > .modal-dialog > .modal-content > .modal-body').html('');
                $('#userModel > .modal-dialog > .modal-content > .modal-body').append(response);
                $('#userModel > .modal-dialog > .modal-content > .modal-header > .modal-title').html('User Registration');
                $('#userModel > .modal-dialog > .modal-content > .modal-footer').html('');
                $('#userModel > .modal-dialog > .modal-content > .modal-footer').append(addBtn);
                $("#RegistrationView").removeData("validator");
                $("#RegistrationView").removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse("#RegistrationView");
                //$('#editBody').empty();
                //$('#editBody').append(response);
                //$('#EditModal').modal('show');

            },
            error: function () {
   
                console.log('AJAX ERROR');
            }
        });

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
                    $('#IdNumValidation').text('Id Number already used').removeClass('text-success').addClass('text-danger');
                } else {
                    $('#IdNumValidation').text('Id Number is valid').removeClass('text-danger').addClass('text-success');
                }
            },
            error: function UsernameVerificationFailed() {

            }
        });


    }




    $('#btnUpdateUserDetails').on("click", function () {
        var id = $('#UpdateUserDetails').find("input[name='UserId']");
        var uid = id.attr('value');

        $('#IncludedRoles option').prop("selected", "selected");
        $('#SystemRoles option').prop("selected", "selected");
        $('#UpdateUserDetails').submit();

    
    });



    $(function () {
        $('.modal').on('hidden.bs.modal', function () {
            $(this).removeData('bs.modal');
        });
    });


 
        
    $('#RegistrationView :input').on("focusout", function () {
        if ($(this).text() !== null) {
            $(this).valid();
        }
    });

    $('#RegistrationView :input').on("focusin", function () {
        $(this).next().text('').removeClass('text-success').addClass('text-danger');
    });
    
    $('#RegistrationModel_UserName').on("focusout", function () {
        if ($(this).val() !== "") {
            validateUsername($(this).val());    
        }
    });


    $('#RegistrationModel_Email').on("focusout", function () {
        if ($('#ValidationErrorEmail').text() === "" || $('#ValidationErrorEmail').hasClass('text-success')) {
            validateEmail($(this).val());
        }
    });

    function validateBeforePosting() {
        var valErrors = 0;
     
        $('.Validators').each(function (index)
        {
            if ($(this).text() !== "" && $(this).hasClass("text-danger")) {
                console.log(index + ":" + $(this).text());
                valErrors = valErrors + 1;
            }
        });
        console.log(valErrors + "found");
        return valErrors;
    }


  

   

  




    //function validateIdNum(IdNumUn) {

    //    $.ajax({
    //        url: systemRootUrl + "/Account/CheckIdNum",
    //        method: "POST",
    //        dataType: 'JSON',
    //        data: { Id: IdNumUn },
    //        beforeSend: function UsernameVerificationb4send() {

    //        },
    //        success: function UsernameVerificationSuccess(response) {
    //            if (response.IsExist) {
    //                $('#IdNumValidation').text('Id Number already used').removeClass('text-success').addClass('text-danger')
    //            } else {
    //                $('#IdNumValidation').text('Id Number is valid').removeClass('text-danger').addClass('text-success')
    //            }
    //        },
    //        error: function UsernameVerificationFailed() {

    //        }
    //    });


    //};




    function validateEmail(strUn) {

        $.ajax({
            url: systemRootUrl + "/Account/CheckEmailAddress",
            method: "POST",
            dataType: 'JSON',
            data: { Email: strUn },
            beforeSend: function EmailVerificationb4send() {
            },
            success: function EmailVerificationSuccess(response) {
                if (response.ValidationError !== "")
                {
                    $('#ValidationErrorEmail').text(response.ValidationError).removeClass('text-success').addClass('text-danger');
                } else {
                    $('#ValidationErrorEmail').text('Email is valid').removeClass('text-danger').addClass('text-success');
                }
            },
            error: function EmailnameVerificationFailed() {
            }
        });


    }

})
