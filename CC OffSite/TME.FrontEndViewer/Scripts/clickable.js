$(function () {

    $(".clickable").children("table").click(function () {
        $(this).parent().children(".clickable").slideToggle();
    });
});