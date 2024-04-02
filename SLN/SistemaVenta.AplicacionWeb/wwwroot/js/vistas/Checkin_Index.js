let bookings = [];
let bookingSel = null;
let totalVentas = 0;
$(document).ready(function () {  

    $('#toggleCollapse').click(function () {
        $('#collapseEstablishment').collapse('toggle');
        $('#collapseIcon').toggleClass('fa-angle-up fa-angle-down');
    });

    $('#myTab').html(`<li class="nav-item" role="presentation">
                        <button class="nav-link active" id="tab-all" data-bs-toggle="tab" data-bs-target="#tab-content-all" type="button" role="tab" aria-controls="tab-content-all" aria-selected="true" data-level-id="0">Todos</button>
                    </li>`);

    // Cargar todas las habitaciones al inicio
    loadBooks();
  
    // Obtener los métodos de pago disponibles
    fetch("/Movimiento/GetPaymentMethods")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                var payMethods = '';
                var mvtoMethods = '';
                var firstImageUrl = '';
                var firstImageUrlMvto = '';
                responseJson.forEach((item) => {

                    //Separa los metodos de pago de venta y entradas/salidas
                    if (item.idMedioPago != 4 && item.idMedioPago != 5 && item.idMedioPago != 6) {
                        firstImageUrl = firstImageUrl == '' ? item.urlImagen : firstImageUrl;
                        payMethods += `<div class="row">                   
                                        <div class="form-check">
                                                <input class="form-check-input" onchange="changePay('${item.idMedioPago}', '${item.urlImagen}')" data-urlimagen="${item.urlImagen}" data-desc="${item.descripcion}" type="radio" name="payMethod" id="pay${item.idMedioPago}" value="${item.idMedioPago}" ${item.idMedioPago == 1 ? "checked" : ''} >
                                                    <label class="form-check-label" for="selectPay1">${item.descripcion}</label>
                                            </div>
                                        </div>                                         
                                    </div>`;
                    }
                });

                //Setea valores en las modales venta y entrada/salida
                $("#imgPayMethod").attr("src", firstImageUrl);
                $(".payMethods").html('<div>' + payMethods + '</div><br/>');

            }
        });
});
function loadBooks() {

    // Cargar todas las habitaciones en checkout
    $.ajax({
        url: '/Booking/ListarBooksCheckout',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            bookings = response.data;

            var cardBooks = bookings.map(r => {
                return `<div class="col-md-4 col-xl-3" style="cursor: pointer">
                            <div class="card bg-c-green order-card booking-checkout" id='Mvto${r.book?.idMovimiento}' onclick = "selectBook(${r.book?.idMovimiento},'${r.book?.idMovimientoNavigation?.documentoCliente}','${r.book?.idMovimientoNavigation?.nombreCliente}')">
                            <div class="card-block">
                                <h6 class="m-b-20" >${r.room?.categoryName} ${r.book?.idBook}</h6>
                                <h2 class="text-right"><i class="fa fa-bed f-left"></i><span>${r.room?.number}</span></h2>
                                <p class="m-b-0">${r.book?.idMovimientoNavigation?.nombreCliente}<span class="f-right">${r.book?.adults}</span></p>
                                <p class="m-b-0">${r.book?.checkIn.slice(0, -9)}  <span class="f-center"> &nbsp;&nbsp;->  </span>  <span class="f-right"> ${r.book?.checkOut.slice(0, -9)}</span></p>
                            </div>
                            </div>
                        </div>`;
            });

            // Agregar las tarjetas generadas al contenido de la pestaña "Todos"
            $('#myTabContent').html(`<div class="row">${cardBooks.join('')}</div>`);
            $("#txtNombreCliente").val('');
            $("#txtCostoReserva").val(0);
            $("#txtAbonoReserva").val(0);
            $("#txtSaldoReserva").val(0);
            $("#txtValorCaja").val(0);
            $("#txtCostoAdic").val(0);
            $("#txtTotal").val(0);
            $("#tbventa tbody").html("");   
        },
        error: function () {
            // Manejar el error en caso de que falle la solicitud
        }
    });

}
function selectBook(idMovimiento, documentoCliente, nombreCliente) {

    const cardBookings = document.getElementsByClassName("booking-checkout");
    for (let i = 0; i < cardBookings.length; i++) {
        cardBookings[i].className = 'card bg-c-green order-card booking-checkout';
        cardBookings[i].style = "";
    }
    document.getElementById("Mvto" + idMovimiento).style = "border: 3px solid darkslategrey";  

    //Asigna valores reserva
    bookingSel = bookings.find((e) => e.book?.idMovimiento == idMovimiento);
    totalVentas = 0;
    let costoReserva = bookingSel?.book?.idMovimientoNavigation?.total != null ? bookingSel.book.idMovimientoNavigation.total : 0;
    let abonoReserva = bookingSel?.book?.idMovimientoNavigation?.abonoReserva != null ? bookingSel.book.idMovimientoNavigation.abonoReserva : 0;
    let saldoReserva = costoReserva - abonoReserva;
    $("#txtNombreCliente").val(nombreCliente + "-" + documentoCliente);   
    $("#txtCostoReserva").val(costoReserva);
    $("#txtAbonoReserva").val(abonoReserva);
    $("#txtSaldoReserva").val(saldoReserva);
    $("#txtValorCaja").val(0);
    $("#txtCostoAdic").val(0);
    $("#txtTotal").val(0);
    $("#tbventa tbody").html("");   

    fetch(`/Movimiento/GetMovimientosBooking?idMvtoRel=${idMovimiento}`)
        .then(response => {
            $(".card-body").find("div.row").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
           $("#tbventa tbody").html("");
          
           //Filtra los movimientos con tipo "Credito" que haya registrado en caja
           responseJson = responseJson.filter((e) => e.idTipoDocumentoMovimiento == 5);
            if (responseJson != null && responseJson.length > 0) {               
                responseJson.forEach((venta) => {
                    $("#tbventa tbody").append(
                        $("<tr>").append(
                            $("<td>").text(venta.fechaRegistro),
                            $("<td>").text(venta.numeroMovimiento),
                            $("<td>").text(venta.tipoDocumentoMovimiento),
                            $("<td>").text(venta.documentoCliente),
                            $("<td>").text(venta.total),
                        )                   
                    )
                    totalVentas = totalVentas + venta.total;
                })                
            }
            $("#txtValorCaja").val(totalVentas);
            $("#txtTotal").val(costoReserva + totalVentas - abonoReserva);
      })   
}

$("#btnTerminar").click(function () {

    // Validar si se requieren datos de cliente
    if ($("#txtNombreCliente").val() == ''){
        toastr.warning("", "Debe seleccionar un Cliente");
        return;
    }
    $("#totalCobro").html($("#txtTotal").val());

    $('#modalPay').on('shown.bs.modal', function () {
        $('#totalPayWith').trigger('focus')
    });
    $('#modalPay').modal('show');

});
$("#txtCostoAdic").on("input", function () {
    var costoAdic = parseFloat($(this).val());

    //Asigna valores reserva
    if (bookingSel != null) {
        let costoReserva = bookingSel?.book?.idMovimientoNavigation?.total != null ? bookingSel.book.idMovimientoNavigation.total : 0;
        let abonoReserva = bookingSel?.book?.idMovimientoNavigation?.abonoReserva != null ? bookingSel.book.idMovimientoNavigation.abonoReserva : 0;
        let saldoReserva = costoReserva - abonoReserva;      
        $("#txtTotal").val(costoReserva + totalVentas + costoAdic - abonoReserva);
    }  
});
$("#totalPayWith").on("input", function () {

    var selectPayMethod = $("input[type='radio'][name='payMethod']:checked");
    var inputValue = $(this).val();
    var total = selectPayMethod.val() != 3 ? parseFloat($("#txtTotal").val()) : parseFloat($("#totalPay1").val());
    var pago = parseFloat(inputValue);

    if (!isNaN(pago)) {
        var cambio = pago - total;
        $("#cambio").text(cambio);
    } else {
        $("#cambio").text("");
    }

    if (selectPayMethod == 3) {
        $("#totalPay2").val(total - pay1);
    }

});

function changePay(id, img) {
    $("#imgPayMethod").attr("src", img);
    $("#cambio").text("0");
    $("#totalPayWith").val(0);

    //Mixto
    if (parseInt(id) == 3) {
        $(".payWith").css("display", "");
        $(".payMixto").css("display", "");
    }
    else { //Otros
        $(".payWith").css("display", parseInt(id) != 1 && parseInt(id) != 3 ? "none" : "");
        $(".payMixto").css("display", "none");
    }
}

function registerCheckout(print) {

    var selectPayMethod = $("input[type='radio'][name='payMethod']:checked");

    // Valida el medio pago seleccionado y registra la venta
    if (selectPayMethod) {

        const Movimiento = {
            idTipoDocumentoMovimiento: 1,//Recibo,
            documentoCliente: bookingSel?.book?.idMovimientoNavigation?.documentoCliente,
            nombreCliente: bookingSel?.book?.idMovimientoNavigation?.nombreCliente,
            subTotal: 0,
            impuestoTotal: 0,
            total: $("#txtTotal").val(),
            IdMovimientoRel: bookingSel?.book?.idMovimientoNavigation.idMovimiento,
            IdMedioPago: selectPayMethod.val(),
            TotalEfectivoMixto: selectPayMethod.val() == 3 ? $("#totalPay1").val() : 0,
            TotalRecibido: selectPayMethod.val() == 1 || selectPayMethod.val() == 3 ? $("#totalPayWith").val() : 0,
            TotalCambio: selectPayMethod.val() == 1 || selectPayMethod.val() == 3 ? $("#cambio").html() : 0,
            Observacion: $("#observacion").val(),
            PrintTicket: print,
            IdBookCheckout: bookingSel?.book?.idBook,
            SaldoReserva: $("#txtSaldoReserva").val(),
            ValorCaja: $("#txtValorCaja").val(),
            CostoAdic: $("#txtCostoAdic").val(),
        }
        $('#modalPay').modal('hide');
        $(".viewMvto").LoadingOverlay("show");

        //Registrar el movimiento
        fetch("/Movimiento/RegistrarMovimiento", {
            method: "POST",
            headers: { "Content-type": "application/json; charset=utf-8" },
            body: JSON.stringify(Movimiento),
        })
            .then(response => {
                $(".viewMvto").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    bookingSel = null;
                    totalVentas = 0;
                    loadBooks();
                    $("#observacion").val("")
                    $("#tbventa tbody").html("");
                    $("#totalPay1").val("");
                    $("#totalPay2").val("");
                    $("#totalPayWith").val("");
                    $("#cambio").html("0");
                    swal("Checkout Registrado", `Numero de Movimiento:${responseJson.objeto.numeroMovimiento}  `, "success");
                } else {
                    swal("Error", responseJson.mensaje, "error");
                }
            });
       }    
 }


    

