// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    $("#messageForm").submit(function (e) {
        e.preventDefault();
        
        var message = $("#message").val();

        // Відправка повідомлення на сервер
        $.ajax({
            url: "/messages",
            type: "post",
            data: { message: message },
            success: function (result) {
                // Refresh the messages list
                $("#messages").load("/messages");
            }
        });
    });
});