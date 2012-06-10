(function ($) {
    $.fn.cascade = function (options) {
        var defaults = {};
        var opts = $.extend(defaults, options);

        return this.each(function () {
            $(this).change(function () {
                var selectedValue = $(this).val();
                var params = {};
                params[opts.paramName] = selectedValue;
                $.getJSON(opts.url, params, function (items) {
                    opts.childSelect.empty();
                    $.each(items, function (index, item) {
                        opts.childSelect.append(
                            $('<option/>')
                                .attr('value', item.Id)
                                .text(item.Name)
                        );
                    });
                });
            });
        });
    };
})(jQuery);