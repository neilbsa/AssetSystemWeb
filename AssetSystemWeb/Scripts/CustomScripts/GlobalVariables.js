$(document).ready(function () {

    $('input.inputUsername').attr('maxlength', '25');
    $('input.inputPassword').attr('maxlength', '50');

    $('input.inputFirstname').attr('maxlength', '50');
    $('input.inputMiddlename').attr('maxlength', '50');
    $('input.inputSurname').attr('maxlength', '50');
    $('input.inputEmailAddress').attr('maxlength', '150');
    $('input.inputAddress').attr('maxlength', '200');

    $('input.inputAmount').attr('maxlength', '9');
    $('input.inputDescription').attr('maxlength', '200');
    $('input.inputSerialnumber').attr('maxlength', '200');
    $('input.inputKeydetail').attr('maxlength', '200');
    $('input.inputRemarks').attr('maxlength', '200');
    $('input.inputYears').attr('maxlength', '2');

    $('input.inputCompanyName').attr('maxlength', '200');
    $('input.inputContactNumber').attr('maxlength', '100');
    $('input.inputReferenceNumber').attr('maxlength', '200');

    $('input.inputItemId').attr('maxlength', '15');


    $('input.inputDescription,input.inputCompanyName').bind('paste', function (e) {
        e.preventDefault();

    });


    //$(document).bind("ajaxSend", function () {
    //    console.log('ajax send')
    //    $('#lodingModal').modal('show');
    //}).bind("ajaxComplete", function () {
    //    $('#lodingModal').modal('hide');
    //});




})



var CustomglobalEvents = (function () {



    var openUniversalModal = function (title, body, footer) {

        var _body = $('#univModal > .modal-dialog > .modal-content > .modal-body')
        var _title = $('#univModal > .modal-dialog > .modal-content > .modal-header > .modal-title')
        var _footer = $('#univModal > .modal-dialog > .modal-content > .modal-footer')

        //console.log(_body)
        //console.log(_title)
        //console.log(_footer)

        if (title != null) {
            _title.html('');
            _title.append(title);
        }

        if (body != null) {
            _body.html('');
            _body.append(body);
        }

        if (footer != null) {
            _footer.html('');
            _footer.append(footer);
        }

        $('#univModal').modal('show');
    };


    return {
        OpenUniversalModal: openUniversalModal
    }


})();




