$(function () {
    var placeholder = $("#modal-placeholder");
    //Get the Details from Action Controller by button ajax - modal codes must be at Details.cshtml
    //$("button[data-toggle='ajax-modal']").click(function () {
    $(document).on('click','button[data-toggle="ajax-modal"]',function () {
        var placeholder = $("#modal-placeholder");
        var url = $(this).data('url');
        $.ajax({
            url: url,
            beforeSend: function () { $("body").preloader(); },
            complete: function () { $("body").preloader('remove'); },
            error: function () {
                ShowSweetAlert();
            }
        }).done(function (result) {
            placeholder.html(result);
            placeholder.find('.modal').modal('show');
        });
    });

    placeholder.on('click', 'button[data-save="modal"]', function () {
        $("body").preloader();
        var form = $(this).parents(".modal").find('form');
        var actionUrl = form.attr('action');
        var dataToSend = new FormData(form.get(0));

        $.ajax({ url: actionUrl, type: "post", data: dataToSend, processData: false, contentType: false }).done(function (data) {
            var newBody = $(".modal-body", data);
            placeholder.find(".modal-body").replaceWith(newBody);

            var IsValid = newBody.find("input[name='IsValid']").val() === "True";
            if (IsValid) {
                var notificationPlaceholder = $("#notification");
                var notificationUrl = notificationPlaceholder.data('url');
                $.ajax({ url: notificationUrl, error: function () { ShowSweetAlert(); } }).done(function (notification) {
                    notificationPlaceholder.html(notification);
                });
                var tableElement = $("#myTable");
                var tableUrl = tableElement.data('url');
                $.ajax({ url: tableUrl, error: function () { ShowSweetAlert(); } }).done(function (table) {
                    $("#tableContent").html(table);
                });
                placeholder.find(".modal").modal('hide');
            }
            //$("body").preloader('remove');
        });
    });
});

function ShowSweetAlert() {
    Swal.fire({
        type: 'error',
        title: 'خطایی رخ داده است !!!',
        text: 'لطفا تا برطرف شدن خطا شکیبا باشید.',
        confirmButtonText: 'بستن'
    });
}

