$(document).ready(function () { 


    var systemRootUrl = $('#SystemRootUrl').val();



    $('.ItemSerialDetail').focusout(function (e) {
        e.stopImmediatePropagation();
        console.log('checkingCalled')
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
                    curr.find('.itemPartNumber').text(data.PartNumber);
                    curr.find('.ItemSerialDetail').text(data.SerialNumber);
                    curr.find('.ItemKeyDetail').text(data.ItemKeyDetail);
                    curr.find('.ItemPrice').text(data.Price);
                    curr.find('.ItemRemarks').text(data.ItemRemarks);

                    curr.find('.ItemDescription').val(data.ItemDescription);
                    curr.find('.itemPartNumber').val(data.PartNumber);
                    curr.find('.ItemSerialDetail').val(data.SerialNumber);
                    curr.find('.ItemKeyDetail').val(data.ItemKeyDetail);
                    curr.find('.ItemRemarks').val(data.ItemRemarks);
                    curr.find('.ItemPrice').val(data.Price);
                    curr.find('.AssetItemId').val(data.ItemId);
              

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
                
                }

            }
        });
    }


    $('.ItemTypeSelect').selectmenu();


    $('.ItemDetailDescription').each(function () {

        addAutoComplete($(this), $(this).closest('td').prev().find('.ItemTypeSelect'), 'AssetItemDetail');
    });

    $('.ItemDetailPartNumber').each(function () {
      
        addAutoComplete($(this), $(this).closest('td').prev().prev().find('.ItemTypeSelect'), 'AssetItemPartNumber');
    });


    $('.AssetItemDescription').each(function () {
 
        addAutoComplete($(this), $(this).parent().parent().parent().parent().prev().find('.ItemTypeSelect'), 'AssetItemDetail');
    });

    
    $('.AssetItemPartNumber').each(function () {

        addAutoComplete($(this), $(this).parent().parent().parent().parent().prev().find('.ItemTypeSelect'), 'AssetItemPartNumber');
    });


    $('.AssetItemVendor').each(function () {

        addAutoComplete($(this), undefined, 'Vendor');


    });


    function getValueofElement(elem) {

        if (elem === undefined) {
            return undefined
        } else {
            console.log('getValueofElement')
            return elem.val()
        }
    }

    function addAutoComplete(elem,Description,itemGroup) {

        $(elem).autocomplete({

            source: function (request, response) {
                $.ajax({
                    url: systemRootUrl + "/Asset/GetAutoCompleteLookUp",
                    data: { groupOf: itemGroup, keyword: $(elem).val(), Description: getValueofElement(Description) },
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


    //function addautocomplete(elem, Description,itemGroup) {
      
    //    $(elem).autocomplete({

    //        source: function (request, response) {
    //            $.ajax({
    //                url: systemRootUrl + "/Asset/GetAutoCompleteLookUp",
    //                data: { groupOf: itemGroup, keyword: $(elem).val(), Description: Description },
    //                type: "POST",
    //                dataType: "json",

    //                success: function (data) {

    //                    response($.map(data, function (item) {
    //                        return { label: item.Name, value: item.Name };
    //                    }));

    //                }
    //            });
    //        },
    //        messages: {
    //            noResults: '', results: function (resultsCount) { }
    //        }
    //    });
    //}

})