
$(

    $('#searchDiv').keypress(
    function (event) {
        if (event.which == '13') {
            event.preventDefault();
        }
    }),


    $("#searchByRestaurantId").click(function (e) {
        var id = $("#restaurantIdInput").val();
        window.location.href = '/Complaints/RestaurantComplaints/' + id;
    }),

    $("#restaurantIdInput").keyup(function (e) {

        if (e.keyCode == 13) {
            var id = $("#restaurantIdInput").val();
            window.location.href = '/Complaints/RestaurantComplaints/' + id;
        }
    }),
   
);
