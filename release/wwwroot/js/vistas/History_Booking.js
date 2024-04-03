const VISTA_BUSQUEDA = {
    busquedaFecha: () => {
        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")
        $("#txtNumeroReserva").val("")

        $(".busqueda-fecha").show()
        $(".busqueda-venta").hide()
    },
    busquedaMovimiento: () => {
        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")
        $("#txtNumeroReserva").val("")

        $(".busqueda-fecha").hide()
        $(".busqueda-venta").show()
    }
}

$(document).ready(function () {
    VISTA_BUSQUEDA["busquedaFecha"]()

    $.datepicker.setDefaults($.datepicker.regional['es'])
    $("#txtFechaInicio").datepicker({dateFormat:"dd/mm/yy"})
    $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })
})


$("#cboBuscarPor").change(function () {
    if ($("#cboBuscarPor").val() == "fecha") {
        VISTA_BUSQUEDA["busquedaFecha"]()
    } else {
        VISTA_BUSQUEDA["busquedaReserva"]()
    }
})


$("#btnBuscar").click(function () {

    if ($("#cboBuscarPor").val() == "fecha") {
        if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {
            toastr.warning("", "Debe ingresar fecha de inicio y fin");
            return;
        }
    } else {
        if ($("#txtNumeroReserva").val().trim() == "" ) {
            toastr.warning("", "Debe ingresar el número de reserva");
            return;
        }
    }

    let numeroMovimiento = $("#txtNumeroReserva").val();
    let fechaInicio = $("#txtFechaInicio").val();
    let fechaFin = $("#txtFechaFin").val();
    let buscarPorTipo = $("#cboTipoMovimiento").val();

    $(".card-body").find("div.row").LoadingOverlay("show");

    fetch(`/Booking/HistoryBooking?bookingNumber=${numeroMovimiento}&searchBy=${buscarPorTipo}&dateIni=${fechaInicio}&dateFin=${fechaFin}`)
        .then(response => {
            $(".card-body").find("div.row").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            $("#tbventa tbody").html("");       
            if (responseJson.length > 0) {
                responseJson.forEach((venta) => {
                    $("#tbventa tbody").append(
                        $("<tr>").append(
                            $("<td>").text(venta.creationDate),
                            $("<td>").text(venta.idMovimientoNavigation.numeroMovimiento),
                            $("<td>").text(venta.idMovimientoNavigation.tipoDocumentoMovimiento),
                            $("<td>").text(venta.idMovimientoNavigation.documentoCliente),
                            $("<td>").text(venta.idMovimientoNavigation.nombreCliente),
                            $("<td>").text(venta.idMovimientoNavigation.total),
                            $("<td>").append(
                                $("<button>").addClass("btn btn-secondary btn-sm").append(
                                    $("<i>").addClass("fas fa-eye").text("  Ver Detalle")
                                ).data("venta",venta)
                            ),

                        )
                    )
                })
            }
        })
})

$("#tbventa tbody").on("click", ".btn-secondary", function () {
    let d = $(this).data("venta");

    $("#txtFechaRegistro").val(d.creationDate);
    $("#txtNumMovimiento").val(d.idMovimientoNavigation.numeroMovimiento);
    $("#txtUsuarioRegistro").val(d.idMovimientoNavigation.idUsuario);
    $("#txtTipoDocumento").val(d.idMovimientoNavigation.tipoDocumentoMovimiento);
    $("#txtDocumentoCliente").val(d.idMovimientoNavigation.documentoCliente);
    $("#txtNombreCliente").val(d.idMovimientoNavigation.nombreCliente);
    $("#txtSubTotal").val(d.idMovimientoNavigation.subTotal);
    $("#txtIGV").val(d.idMovimientoNavigation.impuestoTotal);
    $("#txtTotal").val(d.idMovimientoNavigation.total);

    $("#tbProductos tbody").html("");       

    d.detailBook.forEach((item) => {

        $("#tbProductos tbody").append(
            $("<tr>").append(
                $("<td>").text(item.idBook),
                $("<td>").text(item.idGuest),
                $("<td>").text(item.idRoom),
                $("<td>").text(item.price),
            )
        )
    })

    $("#linkImprimir").attr("href", `/Movimiento/MostrarPDFMovimiento?numeroMovimiento=${d.numeroMovimiento}`)
    
    $("#modalData").modal("show");
})
