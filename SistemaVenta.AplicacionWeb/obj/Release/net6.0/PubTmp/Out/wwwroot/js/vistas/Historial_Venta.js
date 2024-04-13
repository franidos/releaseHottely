const VISTA_BUSQUEDA = {
    busquedaFecha: () => {
        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")
        $("#txtNumeroMovimiento").val("")

        $(".busqueda-fecha").show()
        $(".busqueda-venta").hide()
    },
    busquedaMovimiento: () => {
        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")
        $("#txtNumeroMovimiento").val("")

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
        VISTA_BUSQUEDA["busquedaMovimiento"]()
    }
})


$("#btnBuscar").click(function () {

    if ($("#cboBuscarPor").val() == "fecha") {
        if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {
            toastr.warning("", "Debe ingresar fecha de inicio y fin");
            return;
        }
    } else {
        if ($("#txtNumeroMovimiento").val().trim() == "" ) {
            toastr.warning("", "Debe ingresar el número de venta");
            return;
        }
    }

    let numeroMovimiento = $("#txtNumeroMovimiento").val();
    let fechaInicio = $("#txtFechaInicio").val();
    let fechaFin = $("#txtFechaFin").val();
    let buscarPorTipo = $("#cboTipoMovimiento").val();

    $(".card-body").find("div.row").LoadingOverlay("show");

    fetch(`/Movimiento/HistorialMovimiento?numeroMovimiento=${numeroMovimiento}&buscarPorTipo=${buscarPorTipo}&fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`)
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
                            $("<td>").text(venta.fechaRegistro),
                            $("<td>").text(venta.numeroMovimiento),
                            $("<td>").text(venta.tipoDocumentoMovimiento),
                            $("<td>").text(venta.documentoCliente),
                            $("<td>").text(venta.nombreCliente),
                            $("<td>").text(venta.total),
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

    $("#txtFechaRegistro").val(d.fechaRegistro);
    $("#txtNumMovimiento").val(d.numeroMovimiento);
    $("#txtUsuarioRegistro").val(d.usuario);
    $("#txtTipoDocumento").val(d.tipoDocumentoMovimiento);
    $("#txtDocumentoCliente").val(d.documentoCliente);
    $("#txtNombreCliente").val(d.nombreCliente);
    $("#txtSubTotal").val(d.subTotal);
    $("#txtIGV").val(d.impuestoTotal);
    $("#txtTotal").val(d.total);

    $("#tbProductos tbody").html("");       

    d.detalleMovimiento.forEach((item) => {

        $("#tbProductos tbody").append(
            $("<tr>").append(
                $("<td>").text(item.descripcionProducto + " " + item.marcaProducto),
                $("<td>").text(item.cantidad),
                $("<td>").text(item.precio),
                $("<td>").text(item.total),
            )
        )
    })

    $("#linkImprimir").attr("href", `/Movimiento/MostrarPDFMovimiento?numeroMovimiento=${d.numeroMovimiento}`)
    
    $("#modalData").modal("show");
})
