$(document).ready(function () { 

    $('#AssetError').hide();
    $('#UserDetail').hide();

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


    $('#uploadConsignmentBtn').on('click', function () {

        var dom = $(this);
        var id = dom.data("myid")
        $.ajax({
            url: systemRootUrl + '/Consignment/UploadConsignment',
            data: {Id : id},
            dataType: 'html',
            method: 'GET',
            success: function (data) {
           

                CustomglobalEvents.OpenUniversalModal('Update Consignment', data, ' ');
            }
        });
    })

    $(document).on('click','#consFileUpload', function (evt) {
        evt.stopImmediatePropagation();

        var dom = $(this);
        console.log(dom);


    })

    var UserLookupTbl = $('#userLookupTbl').DataTable(
        {
            responsive: true,
            "initComplete": function () {
                var api = this.api();
                api.$('tr').on("dblclick", function (e) {
                    var uid = $(this).attr('id');
                    //var trData = $(this).data();
                    //console.log(trData);
                    //var usertype = trData.usertype;
                    //    alert(usertype);
                 
                    $('#CompanyIdNum').val(uid);
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
            console.log($('#consignmentForm').serialize());


        } else {
            alert('please verify inputs');
        }
    });


    $('#CompanyIdNum').focusout(function () {
      
        RenderUserInfo($(this).val());
        
    });

    function createUserHtml(data) {
     
        var finalHtml =   ' <div class="col-md-4"> '+
                     '   <div class="col-md-5"> '+
                  
        '         <label>Firstname:</label> ' +
        '      </div> '+
        '       <div class="col-md-7"> '+

        '           <label class="UserFirstname">' + data.Firstname + '</label> ' +
        '        </div> '+
        '     </div> '+
               '     <div class="col-md-4"> '+
               '         <div class="col-md-5"> '+
        '            <label>Middlename:</label> ' +
        '         </div> '+
        '          <div class="col-md-7"> '+

        '              <label class="UserMiddlename">' + data.Middlename + '</label> ' +
        '          </div> '+
        '      </div> '+
        '      <div class="col-md-4"> '+
        '          <div class="col-md-5"> '+
        '            <label>Surname:</label> '+
        '         </div> '+
        '          <div class="col-md-7"> '+

             '               <label class="UserSurname">' + data.Surname + '</label> ' +
           '             </div> '+
              '      </div>';


        return finalHtml;
    }



    function createDepartmentHtml(data) {

        var finalHtml = ' <div class="col-md-4"> ' +
                     '   <div class="col-md-5"> ' +

        '         <label>Depatment:</label> ' +
        '      </div> ' +
        '       <div class="col-md-7"> ' +

        '           <label class="UserFirstname">' + data.Fullname + '</label> ' +
        '        </div> ' +
        '     </div> ';
        return finalHtml;
    }

    function RenderUserInfo(id) {
        var compId = id;
        if (compId !== '') {
            $.ajax({
                url: systemRootUrl + "/Consignment/GetUserInformation",
                method: "POST",
                data: { IdNum: $('#CompanyIdNum').val() },
                dataType: "JSON",
                success: function (response) {
               
                    let detailDom = $('#rowUserDetails');
                   detailDom.html('');
                    if (response.UserType === "Users") {
                        //detailDom.html(createUserHtml(response));
                        detailDom.prepend(createUserHtml(response))
                    } else {
                       // detailDom.html(createDepartmentHtml(response));
                        detailDom.prepend(createDepartmentHtml(response))
                      //  alert("dept");
                    }
                    //$('#CurrentUser_Firstname').val(response.Firstname);
                    //$('#CurrentUser_MiddleName').val(response.Middlename);
                    //$('#Current_Surname').val(response.Surname);
                    $('#ConsigneeCompanyIdNum').val(response.CompanyIdNum);
                    $('#UserId').val(response.Id);
                    $('#assetDetails > tbody > tr').remove();
                },
                error: function () {
                //    alert('error');
                    $('.UserFirstname').text('');
                    $('.UserMiddlename').text('');
                    $('.UserSurname').text('');
                    $('#UserId').val('');
                    $('#ConsigneeCompanyIdNum').val('');
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
             
                console.log(response)
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