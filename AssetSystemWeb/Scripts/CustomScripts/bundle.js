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




$(document).ready(function () { 

    $('.btnDelete').click(function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        var curr = $(this).closest('tr');
        curr.remove();
    });
    var systemRootUrl = $('#SystemRootUrl').val();



    $('.ItemSerialDetail').focusout(function (e) {
        e.stopImmediatePropagation();
        var result = CheckDetails('SerialNumber', $(this));
    });




    $('.ItemKeyDetail').focusout(function (e) {
        e.stopImmediatePropagation();
        var result = CheckDetails('ItemKeyDetail', $(this));
    });


    function CheckDetails(type, detail) {

        $.ajax({
            url: systemRootUrl + "/Asset/CheckDetailsIsExist",
            data: { type: type, Details: detail.val() },
            type: "POST",
            dataType: "json",
            success: function (data) {

                if (data !== null) {
                    var border = detail.closest('.errorBorder');
                    var icon = detail.next('.errorIcon');
                    if (data) {


                        border.removeClass('has-error');
                        icon.removeClass('glyphicon-remove');

                        border.addClass('has-warning');
                        icon.addClass('glyphicon-warning-sign');


                    } else {

                        border.removeClass('has-error');
                        icon.removeClass('glyphicon-remove');
                        border.removeClass('has-warning');
                        icon.removeClass('glyphicon-warning-sign');



                    }
                } else {

                }
            }
        });
    }

    $('.ItemTypeId').focusout(function (e) {
        e.stopImmediatePropagation();
        var tt = GetItemDetail($(this));
    });

    function GetItemDetail(detail) {

        $.ajax({
            url: systemRootUrl + "/Asset/GetAssetItemFullDetail",
            data: { Id: detail.val() },
            type: "POST",
            dataType: "json",
            success: function (data) {


                var border = detail.closest('.errorBorder');
                var icon = detail.next('.errorIcon');
                if (data.ItemId !== 0) {
                    var curr = $(detail).closest('tr');

                    border.removeClass('has-error');
                    icon.removeClass('glyphicon-remove');


                    border.removeClass('has-warning');
                    icon.removeClass('glyphicon-warning-sign');


                    curr.find('.ItemDescription').text(data.ItemDescription);
                    curr.find('.ItemSerialDetail').text(data.SerialNumber);
                    curr.find('.ItemKeyDetail').text(data.ItemKeyDetail);
                    curr.find('.ItemPrice').text(data.Price);
                    curr.find('.ItemRemarks').text(data.ItemRemarks);

                    curr.find('.ItemDescription').val(data.ItemDescription);
                    curr.find('.ItemSerialDetail').val(data.SerialNumber);
                    curr.find('.ItemKeyDetail').val(data.ItemKeyDetail);
                    curr.find('.ItemRemarks').val(data.ItemRemarks);
                    curr.find('.ItemPrice').val(data.Price);
                    curr.find('.AssetItemId').val(data.ItemId);
                    console.log(data);

                } else {

                    var curr = $(detail).closest('tr');
                    border.removeClass('has-error');
                    icon.removeClass('glyphicon-remove');

                    border.addClass('has-warning');
                    icon.addClass('glyphicon-warning-sign');

                    curr.find('.ItemDescription').text('');
                    curr.find('.ItemSerialDetail').text('');
                    curr.find('.ItemKeyDetail').text('');
                    curr.find('.ItemRemarks').text('');
                    curr.find('.ItemPrice').text('');


                    curr.find('.ItemDescription').val('');
                    curr.find('.ItemSerialDetail').val('');
                    curr.find('.ItemKeyDetail').val('');

                    curr.find('.ItemRemarks').val('');
                    curr.find('.ItemPrice').val('');
                    curr.find('.AssetItemId').val(0);

                    console.log(data);




                }

            }
        });
    }


    $('.ItemTypeSelect').selectmenu();


    $('.ItemDetailDescription').each(function () {
        addautocomplete($(this));
    });




    function addautocomplete(elem) {
        $(elem).autocomplete({

            source: function (request, response) {
                $.ajax({
                    url: systemRootUrl + "/Asset/GetAutoCompleteLookUp",
                    data: { groupOf: 'AssetItemDetail', keyword: $(elem).val(), Description: $(elem).closest('td').prev().find('.ItemTypeSelect').val() },
                    type: "POST",
                    dataType: "json",

                    success: function (data) {

                        response($.map(data, function (item) {
                            return { label: item.Name, value: item.Name };
                        }));

                    }
                });
            },
            messages: {
                noResults: '', results: function (resultsCount) { }
            }
        });
    }

})
$(document).ready(function () { 

    $('.ItemStatusSelect').selectmenu();
    var systemRootUrl = $('#SystemRootUrl').val();


    var Assettable = $('#AssetTable').DataTable(
        {
            responsive: true,
            "initComplete": function () {
                var api = this.api();
                //api.$('tr').on("dblclick", function (e) {
                //    var uid = $(this).attr('id');
                //    formChild(uid);
                //});
                api.$('tr').hover(function () {
                    $(this).css('cursor', 'pointer');
                }, function () {
                    $(this).css('cursor', 'auto');
                });
            },
            "autoWidth": false
        });



    var Assetitemtable = $('#assetItemTable').DataTable(
        {
            responsive: true,
            "initComplete": function () {
                var api = this.api();
                //api.$('tr').on("dblclick", function (e) {
                //    var uid = $(this).attr('id');
                //    formChild(uid);
                //});
                api.$('tr').hover(function () {
                    $(this).css('cursor', 'pointer');
                }, function () {
                    $(this).css('cursor', 'auto');
                });
            },
            "autoWidth": false
        });

    $('#btnUpgradeAsset').click(function () {



        var str = ' <div id="assetTableError" class="alert alert-danger" role="alert"> ' +
            ' <span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span> ' +
            ' <span class="sr-only">Error:</span> ' +
            ' Please check item details ' +
            ' </div> ';



        if (!$('#upgradeAssetFrm').valid()) {
            $('#AssetSaveModal').modal('show');

        } else {
            CheckFinalUpgradedetail();

            var postData = $('#upgradeAssetFrm').serialize();
            $.ajax({
                type: "POST",
                datatype: "JSON",
                url: systemRootUrl + "/Asset/CheckAssetIsAvailableForUpgrade",
                data: postData,
                success: function (data) {
                    if (!data) {
                        console.log(data);

                        $(str).insertBefore('#upgradeParts');

                        setTimeout(function () {
                            $('#assetTableError').remove();
                        }, 5000);

                    } else {
                        $('#upgradeAssetFrm').submit();
                    }
                }
            });



        }
    });

    function CheckFinalUpgradedetail() {

        $('.AssetItemId').each(function () {
            FinalCheckUpgradeDetail($(this));
        });

    };


    function FinalCheckUpgradeDetail(detail) {

        if (detail.val() !== 0) {

            $.ajax({
                url: systemRootUrl + "/Asset/CheckDetailIsAvailable",
                data: { Details: detail.val() },
                type: "POST",
                dataType: "json",
                success: function (data) {
                    if (data !== null) {
                        var row = detail.closest('tr');
                        var errorInput = row.find('.ItemTypeId');
                        var border = errorInput.closest('.errorBorder');
                        var icon = errorInput.next('.errorIcon');
                        if (data) {

                            border.removeClass('has-warning');
                            icon.removeClass('glyphicon-warning-sign');
                            border.removeClass('has-error');
                            icon.removeClass('glyphicon-remove');
                            icon.css("visibility", "hidden");
                        } else {

                            border.removeClass('has-warning');
                            icon.removeClass('glyphicon-warning-sign');
                            border.addClass('has-error');
                            icon.addClass('glyphicon-remove');
                            icon.css("visibility", "visible");
                        }

                    } else {
                        return alert('NULL');
                    }
                }
            });




        }
    }










    $('#btnCreateNewAsset').click(function () {



        var str = ' <div id="assetTableError" class="alert alert-danger" role="alert"> ' +
            ' <span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span> ' +
            ' <span class="sr-only">Error:</span> ' +
            ' Please item details ' +
            ' </div> ';



        if (!$('#addNewAssetFrm').valid()) {
            $('#AssetSaveModal').modal('show');

        } else {
            CheckFinalassetDetail();


            var postData = $('#addNewAssetFrm').serialize();

            console.log(postData);
            $.ajax({
                type: "POST",
                datatype: "JSON",
                url: systemRootUrl + "/Asset/CheckAssetIsExisting",
                data: postData,
                success: function (data) {
                    if (!data) {


                        $(str).insertBefore('#assetDetails');

                        setTimeout(function () {
                            $('#assetTableError').remove();
                        }, 5000);

                    } else {
                        $('#addNewAssetFrm').submit();
                    }
                }
            });



        }
    });


    $('#UpdateAssetStatus').click(function () {

        $.ajax({
            url: systemRootUrl + "/Asset/UpdateAssetStatus",
            Type: "POST",
            datatype: "html",
            data: { Id: $(this).prop('value') },
            success: function (data) {

                $('#UpdateAssetModal > .modal-dialog > .modal-content > .modal-body').html("");
                $('#UpdateAssetModal > .modal-dialog > .modal-content > .modal-body').append(data);

            }

        });

    });






    $('#updateStatusBtn').click(function () {

        $('#updateStatusForm').submit();

    });






    function CheckFinalassetDetail() {

        $('.ItemSerialDetail').each(function () {

            FinalCheckDetail('SerialNumber', $(this));

        });

        $('.ItemKeyDetail').each(function () {

            FinalCheckDetail('ItemKeyDetail', $(this));

        });

    }






    function FinalCheckDetail(type, detail) {


        $.ajax({
            url: systemRootUrl + "/Asset/CheckDetailsIsExist",
            data: { type: type, Details: detail.val() },
            type: "POST",
            dataType: "json",
            success: function (data) {
                if (data !== null) {
                    var border = detail.closest('.errorBorder');
                    var icon = detail.next('.errorIcon');
                    if (data) {
                        border.removeClass('has-warning');
                        icon.removeClass('glyphicon-warning-sign');
                        border.addClass('has-error');
                        icon.addClass('glyphicon-remove');
                        icon.css("visibility", "visible");

                    } else {
                        border.removeClass('has-warning');
                        icon.removeClass('glyphicon-warning-sign');
                        border.removeClass('has-error');
                        icon.removeClass('glyphicon-remove');
                        icon.css("visibility", "hidden");
                    }
                    return data;
                }
            }
        });
    }


    $('#addNewAssetDetails').click(function () {
        $.ajax({
            url: systemRootUrl + "/Asset/AddNewAssetDetails",
            type: "GET",
            dataType: "html",
            success: function (response) {
                $('#assetDetails').append(response);
                $("#addNewAssetFrm").removeData("validator");
                $("#addNewAssetFrm").removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse("#addNewAssetFrm");
            },
            error: function () {

            }
        });
    });


    $('#addNewAssetItemDetails').click(function () {
        $.ajax({
            url: systemRootUrl + "/AssetItem/NewAssetItem",
            type: "GET",
            dataType: "html",
            success: function (response) {
                $('#assetDetails').append(response);
                $("#addNewAssetFrm").removeData("validator");
                $("#addNewAssetFrm").removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse("#addNewAssetFrm");
            },
            error: function () {

            }
        });
    });



    $('.CompanySelection').selectmenu();

    $("#AssetDescriptionLookUp").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: systemRootUrl + "/Asset/GetAutoCompleteLookUp",
                data: { groupOf: "AssetItemDesc", keyword: $("#AssetDescriptionLookUp").val() },
                type: "POST",
                dataType: "json",

                success: function (data) {

                    response($.map(data, function (item) {
                        return { label: item.Name, value: item.Name };
                    }));

                }
            });
        },
        messages: {
            noResults: '', results: function (resultsCount) { }
        }
    });



    $("#VendorDescriptionLookUp").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: systemRootUrl + "/Asset/GetAutoCompleteLookUp",
                data: { groupOf: "Vendor", keyword: $("#VendorDescriptionLookUp").val() },
                type: "POST",
                dataType: "json",

                success: function (data) {

                    response($.map(data, function (item) {
                        return { label: item.Name, value: item.Name };
                    }));

                }
            });
        },
        messages: {
            noResults: '', results: function (resultsCount) { }
        }
    });


    $("#VendorDescriptionLookUp").bind("paste", function(e) {

        e.preventDefault();


    });


    $('#addBufferUpgradeAssetDetails').click(function () {
        $.ajax({
            url: systemRootUrl + "/Asset/AddNewAssetUpgradeDetail",
            type: "GET",
            dataType: "html",
            success: function (response) {
                $('#upgradeParts').append(response);
                $("#upgradeAsset").removeData("validator");
                $("#upgradeAsset").removeData("unobtrusiveValidation");
                $.validator.unobtrusive.parse("#upgradeAsset");
            },
            error: function () {

            }
        });
    });


    $("#SupplierNameLookUp").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: systemRootUrl + "/Asset/GetFullSupplierDetailsList",
                data: { SupplierName: $("#SupplierNameLookUp").val() },
                type: "POST",
                dataType: "json",

                success: function (data) {

                    response($.map(data, function (item) {
                        return { label: item.Name, value: item.Name };
                    }));

                }
            });
        },
        messages: {
            noResults: '', results: function (resultsCount) { }
        }
    });


    $("#SupplierNameLookUp").focusout(
        function () {

            $.ajax({
                url: systemRootUrl + "/Asset/GetFullSupplierDetails",
                data: { SupplierName: $("#SupplierNameLookUp").val() },
                type: "POST",
                dataType: "json",
                success: function (data) {



                    if (data !== null) {

                        $('#InvoiceDetails_SupplierDetails_Address').val(data.Address);
                        $('#InvoiceDetails_SupplierDetails_ContactNumber').val(data.ContactNumber);
                        $('#InvoiceDetails_SupplierDetails_EmailAdd').val(data.EmailAdd);
                        $('#InvoiceDetails_SupplierDetails_Id').val(data.Id);
                        $('#InvoiceDetails_SupplierDetails_Address').attr('readonly', 'readonly');
                        $('#InvoiceDetails_SupplierDetails_ContactNumber').attr('readonly', 'readonly');
                        $('#InvoiceDetails_SupplierDetails_EmailAdd').attr('readonly', 'readonly');

                        if (data.Id === null) {

                            $('#InvoiceDetails_SupplierDetails_Id').val("");
                        }
                        if (data.Address === null) {
                            $('#InvoiceDetails_SupplierDetails_Address').removeAttr('readonly');
                        }

                        if (data.ContactNumber === null) {
                            $('#InvoiceDetails_SupplierDetails_ContactNumber').removeAttr('readonly');
                        }
                        if (data.EmailAdd === null) {
                            $('#InvoiceDetails_SupplierDetails_EmailAdd').removeAttr('readonly');
                        }
                    }
                }
            });
        });
})


    $('.DeleteAsset').click(function (e) {
        console.log('test click');
        e.preventDefault();
        var curr = $(this).closest('tr');
        curr.remove();
    });


$(document).ready(function () { 



    var ConsignmentTable = $('#consignmentTable').DataTable(
        {
            responsive: true,
            "initComplete": function () {
                var api = this.api();
                api.$('tr').on("dblclick", function (e) {
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


    $("#showLookup").click(function () {

        $("#UserLookUp").modal("show");
    })

    var UserLookupTbl = $('#userLookupTbl').DataTable(
        {
            responsive: true,
            "initComplete": function () {
                var api = this.api();
                api.$('tr').on("dblclick", function (e) {
                    var uid = $(this).attr('id');
                    $('#CompanyId').val(uid);
                    RenderUserInfo(uid);
                    $("#UserLookUp").modal("hide");
                });
                api.$('tr').hover(function () {
                    $(this).css('cursor', 'pointer');
                }, function () {
                    $(this).css('cursor', 'auto');
                });
            },
            "autoWidth": false
        });

    $('#AssetError').hide();
    $('#UserDetail').hide();
    var systemRootUrl = $('#SystemRootUrl').val();
    $('#AssetNumber').focusout(function () {
        var assetnumb = $(this).val();
        var userId = $('#UserId').val();


        if (userId === '') {
            $('#UserDetail').fadeIn().fadeOut(500);
            $("#AssetNumber").val('');
            $("#AssetDescription").text('');
            $('#assetDetails > tbody > tr').remove();
        } else {
            if (assetnumb !== '') {
                $.ajax({
                    url: systemRootUrl + "/Consignment/GetAssetInformation",
                    method: "POST",
                    data: { assetNumber: $('#AssetNumber').val(), userId: $('#UserId').val() },
                    dataType: "JSON",
                    success: function (response) {

                        $('#AssetDescription').text(response);

                    },
                    error: function () {

                        $('#AssetError').fadeIn().fadeOut(500);
                    }
                });
            }
        }
    });




    $('#submitConsigment').click(function () {
        var count = $('#assetDetails tr').length;
        if ($('#UserId').val() !== '' && count > 1) {
            $('#consignmentForm').submit();



        } else {
            alert('please verify inputs');
        }
    });


    $('#CompanyId').focusout(function () {
      
        RenderUserInfo($(this).val());
        
    });


    function RenderUserInfo(id) {
        var compId = id;
 


        if (compId !== '') {
            $.ajax({
                url: systemRootUrl + "/Consignment/GetUserInformation",
                method: "POST",
                data: { IdNum: $('#CompanyId').val() },
                dataType: "JSON",
                success: function (response) {
                    $('.UserFirstname').text(response.Firstname);
                    $('.UserMiddlename').text(response.Middlename);
                    $('.UserSurname').text(response.Surname);
                    $('#ConsigneeCompanyId').val(response.CompanyId);

                    //$('#CurrentUser_Firstname').val(response.Firstname);
                    //$('#CurrentUser_MiddleName').val(response.Middlename);
                    //$('#Current_Surname').val(response.Surname);

                    $('#UserId').val(response.Id);
                    $('#assetDetails > tbody > tr').remove();
                },
                error: function () {

                    $('.UserFirstname').text('');
                    $('.UserMiddlename').text('');
                    $('.UserSurname').text('');
                    $('#UserId').val('');
                    $('#ConsigneeCompanyId').val('');
                    $("#AssetNumber").val('');
                    $("#AssetDescription").text('');
                    $('#assetDetails > tbody > tr').remove();


                    $('#UserDetail').fadeIn().fadeOut(500);

                }

            });
        }

        if ($('#UserId').val() === '') {
            $("#AssetNumber").val('');
            $("#AssetDescription").text('');
            $('#assetDetails > tbody > tr').remove();
        }


    }







    function InsertToDetail(InsAssetNumber) {

        $.ajax({
            url: systemRootUrl + "/Consignment/AddNewAssets",
            method: "POST",
            dataType: "html",
            data: { assetNumber: InsAssetNumber },
            success: function (response) {


                $('#assetDetails > tbody').append(response);
                $("#AssetNumber").val('');
                $("#AssetDescription").text('');

            },
            error: function () {
                console.log('opening child ajax success error');
                console.log('AJAX ERROR');
            }
        });
    }




    $('#AddConsignmentAsset').click(function () {
        var AssetId = $('#AssetNumber').val();
        if (!CheckAssetToTable(AssetId)) {
            InsertToDetail(AssetId);
        }
    });



    function CheckAssetToTable(assetNumber) {
        var duplicate = 0;
        $('.AssetNumberDetails').each(function () {
            var mydata = $(this).val();
            if (mydata.toUpperCase() === assetNumber.toUpperCase()) {
                duplicate = duplicate + 1;
            }
        });

        if (duplicate > 0) {

            return true;
        } else {

            return false;
        }

    }
})
$(document).ready(function () { 
    $('.ItemStatusSelect').selectmenu();
})

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