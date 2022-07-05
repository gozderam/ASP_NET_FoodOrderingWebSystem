
$(
    $("#loginBtn").click(function (e) {
        var email = $("#emailInput").val();
        userLoginModel = { "email": email };
        $.ajax({
            type: 'POST',
            url: '/Login/Login',
            data: JSON.stringify(userLoginModel),
            contentType: "application/json",
            error: function (jqxhr, status, exception) {
                alert("Incorrect login data. Try again.");
            },
            success: function () {
                window.location.href = "/Home/Index"
            }
        });
    }),
   
);
