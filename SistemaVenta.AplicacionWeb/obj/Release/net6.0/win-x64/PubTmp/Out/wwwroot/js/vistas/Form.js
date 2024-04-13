
$(document).ready(function () {

    function numberWithCommas(x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }

    // Valor total
    const priceSelectedRoom = $("#priceSelectedRoom").val();
    const totalToPay = $("#totalToPay").val();
    const totalServices = $("#totalPriceServices").val();
    $("#totalSpan").text('$ ' + numberWithCommas(totalToPay) + ' COP');
    $("#totalRoomSpan").text('$ ' + numberWithCommas(priceSelectedRoom) + ' COP');
    $("#serviceValueSpan").text('$ ' + numberWithCommas(totalServices) + ' COP');

    /*
    $("form").submit(function (e) {
        e.preventDefault(e);

        let idestabl = $('#idestabl').val();
        let idroom = $('#idroom').val();
        let checkin = $('#checkin').val();
        let checkout = $('#checkout').val();
        let nrooms = $('#nrooms').val();
        let nadults = $('#nadults').val();
        let nchildren = $('#nchildren').val('numchildren') === '' ? 0 : $(this).val('numchildren');
        let priceroom = $('#priceroom').val('priceroom');
        let services = $('#services64').val('services64');

        // Parametros
        let url = '/MotorReservas/ValidarFormulario?idestablishment=' + idestabl + '&idroom=' + idroom + '&checkin=' +
            checkin + '&checkout=' + checkout + '&nrooms=' + nrooms + '&nadults=' + nadults + '&nchildren=' +
            nchildren + '&priceroom=' + priceroom + '&services=' + services;

        console.log(url)

        window.location.href = url;
    });
    */

});