let systemRootUrl = $('#SystemRootUrl').val();

var donutChart = (function () {
    //let systemRootUrl = $('#SystemRootUrl').val();

    let donutData = [{ "value": "", "label": "" }];

    updatePage();

    let StatusDonut = Morris.Donut({
        element: 'statusDataDonutChart', //ELEMENT NAME
        data: donutData,
        resize: true
    });



    StatusDonut.on('click', function (i, row) {
       
        detailTable.setDataOff(row.label);
    });

    function updatePage() {
        $.ajax({
            url: systemRootUrl + "/AssetItem/GetUpdatedData",
            method: "GET",
            dataType: "JSON",
            success: function plotData(data) {
                plotDataPage(data);
            }
        });
    }

    function plotDataPage(data) {
        ticker.setData(data);
        StatusDonut.setData(data);
    }
})();


let ticker = (function () {
    let availableTicker = $('#availableCount');
    let onUsedTicker = $('#onUsedCount');
    let defectiveTicker = $('#defectiveCount');


    function postTicker(data) {
        availableTicker.text(getCurrent("Available", data));
        onUsedTicker.text(getCurrent("OnUsed", data));
        defectiveTicker.text(getCurrent("Defective", data));

    }

    function getCurrent(labelKey, arr) {
        for (let i = 0; i < arr.length; i += 1) {
            if (arr[i].label === labelKey) {
                return arr[i].value;
            }
        }
    }

    return {
        setData: postTicker
    }
})();



let detailTable = (function () {

    let table = $('#statusDetailRow');
    let loading = $('#lodingModal');
    let dataTable = table.DataTable(
        {
            responsive: false,
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

    let getData = function (type) {
            $.ajax({
                url: systemRootUrl + "/AssetItem/getItemByStatus",
                beforeSend: function () {
                    $('#lodingModal').modal('show');

                },
                method: "GET",
                data: { status: type },
                dataType: "JSON",
                success: function plotData(data) {
                   
                    plotDataPage(data);
                }
            });
        }
         
            
    
        function plotDataPage(type) {
            dataTable.clear();



             for (let i = 0; i < type.length; i += 1) {
                 //let currentDate = new Date(parseInt(type[i].PurchaseDate.substr(6)));
                 let currentData = [type[i].AssetNumber, type[i].ItemId, type[i].ItemDescription ];
                 dataTable.row.add(currentData).draw(false);
             }
             $('#lodingModal').modal('hide')

         };
         return {
             setDataOff: getData
         };
             

})();