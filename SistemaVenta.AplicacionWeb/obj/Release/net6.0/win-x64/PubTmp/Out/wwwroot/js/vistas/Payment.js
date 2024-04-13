
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
    $("#totalToPaySpan").text('Total a pagar: $ ' + numberWithCommas(totalToPay) + ' COP');
    $("#serviceValueSpan").text('$ ' + numberWithCommas(totalServices) + ' COP');


    $("input[type=radio]").click(function (event) {
        var val = $("input[name=payMethod]:checked").val();
        $("#selectPay1Btn").hide();
        $("#selectPay2Btn").hide();
        $("#selectPay3Btn").hide();
        $("#selectPay" + $("input[name=payMethod]:checked").val() + "Btn").show();
    });

    $("#btnPayPartial").click(function (event) {
        var handler = ePayco.checkout.configure({
            key: 'f214f116508a0cb81f432f160acb81b1',
            test: true
        });
        var data = {

            //Parametros compra (obligatorio)
            name: $("#nameServiceToPay").val(),
            description: $("#nameServiceToPay").val(),
            currency: "cop",
            amount: $("#newPay").val(),
            country: "co",
            external: "true",
            methodconfirmation: "get",

            //Atributos opcionales
            extra1: $("#numdoc").val(),
            extra2: $("#IdEstablishment").val(),
            extra3: $("#idBook").val(),
            extra4: $("#emailGuest").val(),
            confirmation: $("#urlConfirmPay").val(),
            response: $("#urlResponsePay").val(),
        }
        handler.open(data);
    });
});