$(function () {

    $(".clickable").children("ul").click(function () {
        $(this).parent().children(".clickable").slideToggle();
    });
});