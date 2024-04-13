
$(document).ready(function () {

    var objetoGuardado = {
        totalPorServicio: 0,
        total: 0,
        totalValue: 0,
        servicios: [],
    };

    var totalServices = 0;

    // Valor total
    const priceSelectedRoom = $("#priceSelectedRoom").val();
    $("#totalSpan").text('$ ' + numberWithCommas(priceSelectedRoom) + ' COP');
    $("#totalRoomSpan").text('$ ' + numberWithCommas(priceSelectedRoom) + ' COP');

    /*
        var objetoGuardado = {
            total: 0,
            servicios: [
                {
                    id: 1,
                    cont: 1
                },
                {
                    id: 2,
                    cont: 1
                }
            ],
        };
    */

    $('#countServicesInput').val(0);
    $('#countAdultsInput').val(0);
    $('#countChildrenInput').val(0);

    function numberWithCommas(x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }

    function actualizarObjetoServicios(id, operacion, price) {
        const servicioEncontrado = objetoGuardado.servicios.find(servicio => servicio.id === id);

        if (operacion === "add") {
            if (servicioEncontrado) {
                servicioEncontrado.cont++;
                servicioEncontrado.value = parseFloat(servicioEncontrado.value) + parseFloat(price);
            } else {
                objetoGuardado.servicios.push({
                    id: id,
                    cont: 1,
                    value: price
                });
            }
        } else if (operacion === "substract") {
            if (servicioEncontrado) {

                if (servicioEncontrado.cont === 1) {
                    // Si la propiedad 'cont' es 1, quitar el objeto del array 'servicios'
                    objetoGuardado.servicios = objetoGuardado.servicios.filter(servicio => servicio.id !== id);
                } else {
                    // Restar en 1 la propiedad 'cont' del servicio encontrado
                    servicioEncontrado.cont--;
                }
            }
        }

        const totalPorServicio = objetoGuardado.servicios.length;
        objetoGuardado.totalPorServicio = totalPorServicio;

        let total = 0;
        let totalValue = 0;
        objetoGuardado.servicios.forEach(element => {
            if (element.cont) {
                total = total + element.cont;
            }
            if (element.value) {
                totalValue = totalValue + parseFloat(element.value);
            }
        });
        objetoGuardado.total = total;
        objetoGuardado.totalValue = totalValue;

        const priceSelectedRoom = $("#priceSelectedRoom").val();
        const totalToPay = totalValue + parseFloat(priceSelectedRoom);

        $("#serviceValueSpan").text('$ ' + totalValue + ' COP');
        $("#totalPriceServices").val(totalValue);
        $("#totalSpan").text('$ ' + numberWithCommas(totalToPay) + ' COP');
        totalServices = totalValue;
    }
 
    $(".minusBtn").on("click", function () {

        var input = $(this).siblings(".countInput");
        var contador = parseInt(input.val());
        if (contador > 0) {
            input.val(contador - 1);
        }

        let idBuscado = $(this).data('idservice');
        let price = $(this).data('price');

        actualizarObjetoServicios(idBuscado, 'substract', price);
    });



    // Evento para el botón Restar
    $(".plusBtn").on("click", function () {

        var input = $(this).siblings(".countInput");
        var contador = parseInt(input.val());
        let maximumService = $(this).data('maxservice');

        if (contador < maximumService) {
            input.val(contador + 1);

            let idBuscado = $(this).data('idservice');
            let price = $(this).data('price');
 
            actualizarObjetoServicios(idBuscado, 'add', price);
        }

    });

    // Seleccion de servicio / continuar
    $('.dynamic-button-service').click(function () {

        console.log(objetoGuardado);

        // Proceso para convertir objeto a JSON
        var jsonString = JSON.stringify(objetoGuardado);
        const base64String = btoa(jsonString);

        let idestabl = $(this).data('idestabl');
        let idroom = $(this).data('idroom');
        let checkin = $(this).data('checkin');
        let checkout = $(this).data('checkout');
        let nrooms = $(this).data('numrooms');
        let nadults = $(this).data('numadults');
        let nchildren = $(this).data('numchildren') === '' ? 0 : $(this).data('numchildren');
        let priceroom = $(this).data('priceroom');

        // Parametros
        var url = '/MotorReservas/Form?idestablishment=' + idestabl + '&idroom=' + idroom + '&checkin=' +
            checkin + '&checkout=' + checkout + '&nrooms=' + nrooms + '&nadults=' + nadults + '&nchildren=' +
            nchildren + '&services=' + base64String + '&priceroom=' + priceroom + '&totalservices=' + totalServices;

        window.location.href = url;
    });

});