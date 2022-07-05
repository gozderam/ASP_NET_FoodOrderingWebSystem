$(document).ready(function () {
    $("#sidebar").mCustomScrollbar({
        theme: "minimal"
    });
});

function show() {
        $('#sidebar').addClass('active');
        $('.overlay').addClass('active');
        $('.collapse.in').toggleClass('in');
        $('a[aria-expanded=true]').attr('aria-expanded', 'false');
}

function hide() {
    $('#dismiss, .overlay').on("click", function () {
        $('#sidebar').removeClass('active');
        $('.overlay').removeClass('active');
    });
}