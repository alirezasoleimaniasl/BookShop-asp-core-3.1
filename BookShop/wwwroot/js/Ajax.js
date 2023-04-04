$(function () {
    var placeholder = $("#modal-placeholder");
    //Get the Details from Action Controller by button ajax - modal codes must be at Details.cshtml
    $("button[data-toggle='ajax-modal']").click(function () {
        var placeholder = $("#modal-placeholder");
        var url = $(this).data('url');
        $.get(url).done(function (result) {
            placeholder.html(result);
            placeholder.find('.modal').modal('show');
        });
    });

    placeholder.on('click', 'button[data-save="modal"]', function () {
        var form = $(this).parents(".modal").find('form');
        var actionUrl = form.attr('action');
        var dataToSend = new FormData(form.get(0));

        $.ajax({ url: actionUrl, type: "post", data: dataToSend, processData: false, contentType: false }).done(function (data) {
            var newBody = $(".modal-body", data);
            placeholder.find(".modal-body").replaceWith(newBody);

            var IsValid = newBody.find("input[name='IsValid']").val() === "True";
            if (IsValid) {
                alert("Inserted data to database");
            }
            
        });
    });
});

