let valorImpuesto = 0;
let estadoCaja = 1;
let movimientoRel = null;
$(document).ready(function () {
    ////Validacion de Apertura de Caja
    $(".container-fluid").LoadingOverlay("show");

    fetch("/Home/ObtenerUsuario")
        .then(response => {
            $(".container-fluid").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const o = responseJson.objeto

                // Mostrar información del usuario
                $("#nombreUsuario").text(o.nombre);

                if (o.abrirCaja) {

                    // Obtener las áreas fisicas disponibles del establecimiento
                    fetch("/Movimiento/GetAreaFisicaList")
                        .then(response => {
                            return response.ok ? response.json() : Promise.reject(response);
                        })
                        .then(responseJson => {
                            if (responseJson.length > 0) {
                                responseJson.forEach((item) => {
                                    $("#cboAreaFisica").append(
                                        $("<option>").val(item.idAreaFisica).text(item.nombre)
                                    )
                                });                               
                            }
                            $('#modalOpenCash').modal('show'); 
                        });                   

                    estadoCaja = 0;
                }
            } else {
                swal("Lo sentimos!", responseJson.mensaje, "error")
            }
        })

    //// Fin Validacion apertura de Caja

    fetch("/Movimiento/ListaTipoDocumentoMovimiento")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    if (item.naturaleza == 'S') {
                        $("#cboTipoDocumentoMovimiento").append(
                            $("<option>").val(item.idTipoDocumentoMovimiento).text(item.descripcion)
                        )
                    }
                })
            }
        })

    fetch("/Establishment/Obtener")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const d = responseJson.objeto;
                console.log(d);
                $("#inputGroupSubTotal").text(`Sub Total - ${d.currency}`)
                $("#inputGroupIGV").text(`IMP(${d.tax}%) - ${d.currency}`)
                $("#inputGroupTotal").text(`Total - ${d.currency}`)
                valorImpuesto = parseFloat(d.tax);
            }
    })

    $("#cboBuscarProducto").select2({
        ajax: {
            url: "/Movimiento/ObtenerProductos",
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            delay: 250,
            data: function (params) {
                return {
                    busqueda: params.term
                };
            },
            processResults: function (data) {
                return {
                    results: data.map((item) => (
                        {
                            id: item.idProducto,
                            text: item.descripcion,
                            marca: item.marca,
                            categoria: item.nombreCategoria,
                            urlImagen: item.urlImagen,
                            precio: item.precio
                        }

                    ))
                };
            }
        },
        language: 'es',
        placeholder: 'Buscar producto',
        minimumInputLength: 1,
        templateResult: formatoResultado,
    });

    $("#cboBuscarRoom").select2({
        ajax: {
            url: "/Booking/GetBookingsByRoomNumber",
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            delay: 250,
            data: function (params) {
                return {
                    number: params.term
                };
            },
            processResults: function (data) {
                return {    
                    results: data.map((item) => (
                    {
                        id: item?.roomMain.idRoom,
                        text: item?.roomMain.number,
                        roomTitle: item?.roomMain.roomTitle,
                        description: item?.roomMain.description,
                        categoryName: item?.roomMain.categoryName,
                        idMovimientoRel: item?.booking.idMovimiento,
                        documentoCliente: item?.booking?.idMovimientoNavigation?.documentoCliente,
                        nombreCliente: item?.booking?.idMovimientoNavigation?.nombreCliente
                    }
                 ))
                };
            }
        },
        language: 'es',
        placeholder: 'Buscar habitación (Ingreso)',
        minimumInputLength: 1,
        templateResult: formatoResultadoRoom,
    });

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
                    if (item.idMedioPago != 5 && item.idMedioPago != 6) {
                        firstImageUrl = firstImageUrl == '' ? item.urlImagen : firstImageUrl;
                        payMethods += `<div class="row">                   
                                        <div class="form-check">
                                                <input class="form-check-input" onchange="changePay('${item.idMedioPago}', '${item.urlImagen}')" data-urlimagen="${item.urlImagen}" data-desc="${item.descripcion}" type="radio" name="payMethod" id="pay${item.idMedioPago}" value="${item.idMedioPago}" ${item.idMedioPago == 1 ? "checked" : ''} >
                                                    <label class="form-check-label" for="selectPay1">${item.descripcion}</label>
                                            </div>
                                        </div>                                         
                                    </div>`;
                    }                
                    else {
                        firstImageUrlMvto = firstImageUrlMvto == '' ? item.urlImagen : firstImageUrlMvto;
                        mvtoMethods += `<div class="row">                   
                                        <div class="form-check">
                                                <input class="form-check-input" onchange="changePay('${item.idMedioPago}', '${item.urlImagen}')" data-urlimagen="${item.urlImagen}" data-desc="${item.descripcion}" type="radio" name="payMethodMvto" id="pay${item.idMedioPago}" value="${item.idMedioPago}" ${item.idMedioPago == 5 ? "checked" : ''} >
                                                    <label class="form-check-label" for="selectPay1">${item.descripcion}</label>
                                            </div>
                                        </div>                                         
                                    </div><br/>`;
                    }
                    
                });
              
                //Setea valores en las modales venta y entrada/salida
                $("#imgPayMethod").attr("src", firstImageUrl);
                $(".payMethods").html('<div>' + payMethods + '</div><br/>');   

                $("#imgMethodMvto").attr("src", firstImageUrlMvto);
                $(".payMethodsMvto").html('<div>' + mvtoMethods + '</div><br/>');  

            }
        });

})

function formatoResultado(data) {
    if (data.loading) {
        return data.text;
    }

    var contenedor = $(
        `<table width="100%">
                <tr>
                    <td style="width:60px">
                        <img style="height:60px;width:60px; margin-right:20px" src="${data.urlImagen}" />
                    </td>
                    <td>
                        <p style="font-weight:bold; margin:2px">${data.marca}</p
                        <p style="margin:2px">${data.text}</p>
                    </td>
                </tr>
            </table>`
    );

    return contenedor;
}
function formatoResultadoRoom(data) {
    if (data.loading) {
        return data.text;
    }

    var contenedor = $(
        `<table width="100%">
                <tr>
                    <td style="width:60px">                       
                         <p style="margin-right:20px">Habitación ${data.text}</p>
                    </td>
                    <td>
                        <p style="font-weight:bold; margin:2px">Nombre: ${data.nombreCliente} - ${data.documentoCliente}</p>
                        <p style="font-weight:bold; margin:2px">${data.roomTitle} - ${data.categoryName}</p>
                    </td>
                </tr>
            </table>`
    );

    return contenedor;
}


$(document).on("select2:open", function () {
    document.querySelector(".select2-search__field").focus();
})

let productosParaMovimiento = [];

$("#cboBuscarProducto").on("select2:select", function (e) {
    const data = e.params.data;

    let producto_encontrado = productosParaMovimiento.filter(P => P.idProducto == data.id)
    if (producto_encontrado.length > 0) {
        //$("#cboBuscarProducto").val("").trigger("change")
        toastr.warning("", "El producto ya fue agregado");
        return false;
    }

    swal({
        title: data.marca,
        text: data.text,
        imageUrl: data.urlImagen,
        showCancelButton: true,
        type: "input",
        //showConfirmButton: true,
        //confirmButtonClass: "btn-danger",
        //confirmButtonText: "Si, eliminar",
        //cancelButtonText: "No, cancelar",
        closeOnConf‌irm: false,
        inputPlaceholder: "Ingrese cantidad"
        //    closeOnCancel: true
    },
        function (valor) {
            if (valor === false) { return false }
            if (valor === "") {
                toastr.warning("", "Necesita ingresar la cantidad");
                return false;
            }
            if (isNaN(parseInt(valor))) {
                toastr.warning("", "Debe ingresar un valor numérico");
                return false;
            }

            let producto = {
                idProducto: data.id,
                marcaProducto: data.marca,
                descripcionProducto: data.marca,
                categoriaProducto: data.categoria,
                cantidad: parseInt(valor),
                precio: data.precio.toString(),
                total: (parseFloat(valor) * data.precio).toString()
            }

            productosParaMovimiento.push(producto);

            mostrarProducto_Precios();
            $("#cboBuscarProducto").val("").trigger("change");
            swal.close();
            console.log(producto);

        }
    );

})

$("#cboBuscarRoom").on("select2:select", function (e) {
    const data = e.params.data;
    movimientoRel = data.idMovimientoRel;
    $("#txtDocumentoCliente").val(data.documentoCliente);
    $("#txtNombreCliente").val(data.nombreCliente);
    $("#tbventa tbody").html("");       
    //Obtener mvtos si es venta de reserva (Estado cuenta)
    if (movimientoRel != null) {
        fetch(`/Movimiento/GetMovimientosBooking?idMvtoRel=${movimientoRel}`)
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
                                $("<td>").text(venta.total),    
                            )
                        )
                    })
                }
            })
    }
})

function mostrarProducto_Precios() {
    let total = 0;
    let igv = 0;
    let subTotal = 0;
    let porcentaje = valorImpuesto / 100;

    $("#tbProducto tbody").html("")

    productosParaMovimiento.forEach((item) => {
        total = total + parseFloat(item.total);
        $("#tbProducto tbody").append(
            $("<tr>").append(
                $("<td>").append(
                    $("<button>").addClass("btn btn-danger btn-eliminar btn-sm").append(
                        $("<i>").addClass("fas fa-trash-alt")
                    ).data("idProducto", item.idProducto),
                ),
                $("<td>").text(item.descripcionProducto),
                $("<td>").text(item.cantidad),
                $("<td>").text(item.precio),
                $("<td>").text(item.total),

            )
        )
    })

    subTotal = total - (total * porcentaje);
    igv = total * porcentaje;
    var a = subTotal.toLocaleString("en");
    $("#txtSubTotal").val(subTotal.toFixed(2));
    $("#txtIGV").val(igv.toFixed(2));
    $("#txtTotal").val(total.toFixed(2));


}

$(document).on("click", "button.btn-eliminar", function () {
    const _idProducto = $(this).data("idProducto")
    productosParaMovimiento = productosParaMovimiento.filter(p => p.idProducto != _idProducto);
    mostrarProducto_Precios();
})

//-- Metodos Payments --//

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

$("#totalPay1").on("input", function () {
    var pay1 = parseFloat($(this).val());    
    var total = parseFloat($("#txtTotal").val());   
    $("#totalPay2").val(total - pay1);
});

$("#btnEntradaSalida").click(function () {
    // Validar si hay productos para el movimiento
    if (estadoCaja != 1) {
        toastr.warning("", "Antes de Registrar entradas o salidas debe Iniciar Caja");
        return;
    }

    $('#modalMvto').on('shown.bs.modal', function () {
        $('#totalMvto').trigger('focus')
    });
    $('#modalMvto').modal('show');

});


$("#btnTerminarMovimiento").click(function () {
    // Validar si hay productos para el movimiento
    if (estadoCaja != 1) {
        toastr.warning("", "Antes de Registrar Productos debe Iniciar Caja");
        return;
    }
    // Validar si hay productos para el movimiento
    if (productosParaMovimiento < 1) {
        toastr.warning("", "Debe ingresar un producto");
        return;
    }

    // Validar si se requieren datos de cliente
    if ($("#cboTipoDocumentoMovimiento").val() == 5 && $("#txtDocumentoCliente").val() == ''
        || $("#cboTipoDocumentoMovimiento").val() == 2 && $("#txtDocumentoCliente").val() == '') {
        toastr.warning("", "Debe ingresar datos de Cliente");
        return;
    }

    $("#totalCobro").html($("#txtTotal").val());
    
    $('#modalPay').on('shown.bs.modal', function () {
        $('#totalPayWith').trigger('focus')
    });
    $('#modalPay').modal('show');

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
        $(".payWith").css("display", parseInt(id) != 1 && parseInt(id) != 3? "none" : "");
        $(".payMixto").css("display", "none"); 
    }

}
function registerPay(print) {

    var selectPayMethod = $("input[type='radio'][name='payMethod']:checked");

    // Valida el medio pago seleccionado y registra la venta
    if (selectPayMethod) {
        const detalleMovimientoDto = productosParaMovimiento;

        const Movimiento = {
            idTipoDocumentoMovimiento: $("#cboTipoDocumentoMovimiento").val(),
            documentoCliente: $("#txtDocumentoCliente").val(),
            nombreCliente: $("#txtNombreCliente").val(),
            subTotal: $("#txtSubTotal").val(),
            impuestoTotal: $("#txtIGV").val(),
            total: $("#txtTotal").val(),
            DetalleMovimiento: detalleMovimientoDto,
            IdMovimientoRel: movimientoRel,
            IdMedioPago: selectPayMethod.val(),
            TotalEfectivoMixto: selectPayMethod.val() == 3 ? $("#totalPay1").val() : 0,
            TotalRecibido: selectPayMethod.val() == 1 || selectPayMethod.val() == 3 ? $("#totalPayWith").val() : 0,
            TotalCambio:   selectPayMethod.val() == 1 || selectPayMethod.val() == 3 ? $("#cambio").html() : 0,
            Observacion: $("#observacionVenta").val(),
            PrintTicket: print
        }
        $('#modalPay').modal('hide');
        $(".viewMvto").LoadingOverlay("show");
        // Registrar el movimiento
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
                    movimientoRel = null;
                    productosParaMovimiento = [];
                    mostrarProducto_Precios();
                    $("#txtDocumentoCliente").val("");
                    $("#txtNombreCliente").val("");
                    $("#cboTipoDocumentoMovimiento").val($("#cboTipoDocumentoMovimiento option:first").val());
                    $("#cboBuscarProducto").val("");
                    $("#cboBuscarRoom").val("");
                    $("#txtSubTotal").val("");
                    $("#txtIGV").val("");
                    $("#txtTotal").val("");
                    $("#observacionVenta").val("")
                    $("#tbventa tbody").html("");
                    $("#cboBuscarRoom").val("").trigger("change");
                    $("#totalPay1").val("");
                    $("#totalPay2").val("");
                    $("#totalPayWith").val("");
                    $("#cambio").html("0");
                    $("#txtSaldoReal").val("");
                    $("#totalMvto").val("");  
                    $("#observacionCierreCaja").val("")
                    swal("Registrado", `Numero de Movimiento:${responseJson.objeto.numeroMovimiento}  `, "success");
                } else {
                    swal("Error", responseJson.mensaje, "error");
                }
            });

    }
}

function registerMvto() {

    var selectPayMethod = $("input[type='radio'][name='payMethodMvto']:checked");

    // Valida el medio pago seleccionado y registra la venta
    if (selectPayMethod) {

        const Movimiento = {
                SubTotal: 0,
                ImpuestoTotal : 0,
                Total : $("#totalMvto").val(),         
                IdMedioPago : selectPayMethod.val(),
                TotalEfectivoMixto : 0,
                Observacion : $("#observacionMvto").val()
        }
        $('#modalMvto').modal('hide');
        $(".viewMvto").LoadingOverlay("show");
        // Registrar el movimiento
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
                    movimientoRel = null;
                    productosParaMovimiento = [];
                    mostrarProducto_Precios();
                    $("#txtDocumentoCliente").val("");
                    $("#txtNombreCliente").val("");
                    $("#cboTipoDocumentoMovimiento").val($("#cboTipoDocumentoMovimiento option:first").val());
                    $("#cboBuscarProducto").val("");
                    $("#cboBuscarRoom").val("");
                    $("#txtSubTotal").val("");
                    $("#txtIGV").val("");
                    $("#txtTotal").val("");
                    $("#observacionVenta").val("")
                    $("#tbventa tbody").html("");
                    $("#cboBuscarRoom").val("").trigger("change");
                    $("#totalPay1").val("");
                    $("#totalPay2").val("");
                    $("#totalPayWith").val("");
                    $("#cambio").html("0");
                    $("#txtSaldoReal").val("");
                    $("#totalMvto").val("");
                    $("#observacionCierreCaja").val("")
                    swal("Registrado", `Se registró el detalle en caja correctamente`, "success");
                } else {
                    swal("Error", responseJson.mensaje, "error");
                }
            });

    }
}


//-- Metodos Cerrar Caja --//
function confirmOpenCash() {
    let caja = {
        saldoInicial: $("#txtEfectivoInicial").val(),
        idAreaFisica: $("#cboAreaFisica option:selected").val()
    }
    if (caja.saldoInicial === "") {
        toastr.warning("", "Necesita ingresar la cantidad");
        return false;
    }
    if (isNaN(parseInt(caja.saldoInicial))) {
        toastr.warning("", "Debe ingresar un valor numérico");
        return false;
    }
    else {
        caja.saldoInicial = parseInt(caja.saldoInicial);
        caja.idAreaFisica = parseInt(caja.idAreaFisica);
    }       
    $("#modalOpenCash").LoadingOverlay("show");
    fetch("/Movimiento/OpenCash", {
        method: "POST",
        headers: { "Content-type": "application/json; charset=utf-8" },
        body: JSON.stringify(caja),
    }).then(response => {
        $("#modalOpenCash").LoadingOverlay("hide");
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {
        if (responseJson.estado) {
            swal("Listo!", "Se registró la caja con éxito", "success");
            $('#modalOpenCash').modal('hide'); 
            estadoCaja = 1;
        }
        else
            swal("Lo sentimos!", responseJson.mensaje, "error")
    });
}
$("#btnCerrarCaja").click(function () {

    fetch("/Movimiento/GetTotalCash", {
        method: "POST",
        headers: { "Content-type": "application/json; charset=utf-8" },
    }).then(response => {
        $(".viewMvto").LoadingOverlay("hide");
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {
        if (responseJson.estado) {
            $('#modalCloseCash').modal('show'); 
            
            $('#saldoFinal').val(responseJson.objeto?.saldoFinal); 
            $('#txtSaldoFinal').val(responseJson.mensaje); 
            $('#txtSaldoReal').val(responseJson.objeto?.saldoFinal); 
            return;
        }
        else
            swal("Lo sentimos!", responseJson.mensaje, "error")
    }); 


});

$("#txtSaldoReal").on("input", function () {
    var inputValue = $(this).val();
    var saldo = parseFloat($("#saldoFinal").val());
    var real = parseFloat(inputValue);

    if (!isNaN(real)) {
        var cambio = real - saldo;
        $("#diferenciaCierre").text(cambio);
    } else {
        $("#diferenciaCierre").text("0");
    }
});
function confirmCloseCash() {
    const close = {
        saldoReal: $("#txtSaldoReal").val(),
        observacion: $("#observacionCierreCaja").val()
    }   

    // Validar saldo
    if (isNaN(close.saldoReal)) {
        toastr.warning("", "Debe ingresar un saldo real válido");
        return;
    }
    $(".viewMvto").LoadingOverlay("show");

    fetch("/Movimiento/CloseCash", {
        method: "POST",
        headers: { "Content-type": "application/json; charset=utf-8" },
        body: JSON.stringify(close),
    }).then(response => {
        $(".viewMvto").LoadingOverlay("hide");
        return response.ok ? response.json() : Promise.reject(response);
    }).then(responseJson => {
        if (responseJson.estado) {
            $('#modalCloseCash').modal('hide'); 
            swal("Listo!", "Se cerró la caja con éxito", "success")
        }
        else
            swal("Lo sentimos!", responseJson.mensaje, "error")
    });
}






