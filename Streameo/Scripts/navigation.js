$(document).ready(function () {
    if (window.location.hash.length > 0) {
        //var url = decodeURIComponent(window.location.hash.replace(/#/g, ""));

        var url = window.location.hash;

        if (url != '#_=_') {
            url = decodeURIComponent(url.replace(/#/g, ""));
        }

        $.ajax({
            url: url,
            type: 'GET',
            success: function (data) {
                document.title = $(data).filter('title').html();

                //html = $(data).filter('.page');
                //$('#content').html(html.find('#content').html());
                $('#main').html($(data).filter('#main').html());
            },
            error: function (error) {
                alert('Wystąpił błąd!!');
            }
        });
    }
});

$('.ajax').live('click', function (e) {
    e.preventDefault();

    var url = $(this).attr('href');

    window.location.hash = encodeURIComponent(url);

    //$('#content').html('<center><img src="Content/gfx/load.gif" /></center>');

    $.ajax({
        url: url,
        type: 'GET',
        success: function (data) {
            document.title = $(data).filter('title').html();

            //html = $(data).filter('.page');
            //$('#content').html(html.find('#content').html());
            $('#main').html($(data).filter('#main').html());
        },
        error: function (error) {
            alert('Wystąpił błąd!!');
        }
    });
});