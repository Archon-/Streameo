function success(result) {
    $(function () { $('div.rateit').rateit(); });
}


$('.rateit').live('rated', function (e) {
    var ri = $(this);

    //if the use pressed reset, it will get value: 0 (to be compatible with the HTML range control), we could check if e.type == 'reset', and then set the value to  null .
    var value = ri.rateit('value');
    var productID = ri.data('productid'); // if the product id was in some hidden field: ri.closest('li').find('input[name="productid"]').val()

    //maybe we want to disable voting?
    ri.rateit('readonly', true);

    $.ajax({
        url: "/Browse/Rate?id=" + productID + "&rating=" + value, //your server side script
        data: { id: productID, value: value }, //our data
        type: 'POST',
        success: function (data) {
            ri.rateit('value', data)
            //$('#response').append('<li>' + data + '</li>');

        },
        error: function (jxhr, msg, err) {
            $('#response').append('<li style="color:red">' + msg + '</li>');
        }
    });
});



