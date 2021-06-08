$(document).ready(function () { 

    $('.ItemStatusSelect').selectmenu();
    var systemRootUrl = $('#SystemRootUrl').val();

    console.log(systemRootUrl)


    $('.editAssetDetail').click(function () {

        var id = $(this).data('myid');
        console.log(id);
        $.ajax({
            url: systemRootUrl + '/AssetItem/Update',
            data: { Id: id },
            dataType: 'html',
            method:'GET',
            success: function (data) {
                console.log(data);

                CustomglobalEvents.OpenUniversalModal('Asset Item Update', data, ' ');
            }
        });
    });



    $(document).on('click', '.viewDocument', function () {

       
        var id = $(this).data('documentid');
        console.log(id);
        $.ajax({

            url: systemRootUrl + '/Asset/ViewDocument',
            data: { Id: id },
            dataType: 'html',
            method: 'GET',
            success: function (data) {
                console.log(data);

                CustomglobalEvents.OpenUniversalModal('View Document', data, ' ');
            }
        });





        //CustomglobalEvents.OpenUniversalModal('Asset Item Update', 'test', ' ');
    })









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

    $(".supersededItem").click(function () {
        //windows.location = $(this).linkUrl;
        alert('lipat')

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










    $('#btnCreateNewAsset').click(function ()
    {
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
                        $(this).hide()
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
                    console.log(data);
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


    $('#addNewDocument').click(function () {
        $.ajax({
            url: systemRootUrl + "/Asset/AddDocuments",
            type: "GET",
            dataType: "html",
            success: function (response) {
                $('#documentTable').append(response);
                //$("#addNewAssetFrm").removeData("validator");
                //$("#addNewAssetFrm").removeData("unobtrusiveValidation");
                //$.validator.unobtrusive.parse("#addNewAssetFrm");
            },
            error: function () {

            }
        });

    


        //alert("no implementation yet");
    });


    $(document).on('click','.btnDelete',function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        var curr = $(this).closest('tr');
        curr.remove();
    });



    $(document).on("change", ".fileUpload", function (evt) {
        evt.stopImmediatePropagation()
        var dom = $(this);
        var file = dom[0].files[0];
        var fileName = file.name;
        var parentdom = dom.parent();
        var nameDom = parentdom.next();

    
        nameDom.text(fileName);



    })

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
            url: systemRootUrl + "/Asset/AddNewAssetDetails",
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
                success: function (data)
                {
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
