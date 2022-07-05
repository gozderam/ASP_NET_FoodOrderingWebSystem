
$(

    $('#searchDiv').keypress(
    function (event) {
        if (event.which == '13') {
            event.preventDefault();
        }
    }),


    $("#searchByUserId").click(function (e) {
        var id = $("#userIdInput").val();
        window.location.href = '/Complaints/UserComplaints/' + id;
    }),

    $("#userIdInput").keyup(function (e) {

        if (e.keyCode == 13) {
            var id = $("#userIdInput").val();
            window.location.href = '/Complaints/UserComplaints/' + id;
        }
    }),
   
);
