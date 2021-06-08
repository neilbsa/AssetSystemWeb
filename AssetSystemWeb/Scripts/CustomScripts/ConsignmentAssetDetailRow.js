
    $('.DeleteAsset').click(function (e) {
      
        e.preventDefault();
        var curr = $(this).closest('tr');
        curr.remove();
    });

