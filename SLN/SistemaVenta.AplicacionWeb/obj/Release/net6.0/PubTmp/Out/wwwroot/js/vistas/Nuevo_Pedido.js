let valorImpuesto = 0;
let valorImpuesto2 = 0;
let proveedorParaPedido = [];

$(document).ready(function () {

    ////***** proveedores

    $("#cboBuscarProveedor").select2({
        ajax: {
            url: "/Pedido/ObtenerProveedor",
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
                            id: item.idProveedor,
                            nit: item.nit,
                            nombre: item.nombre,
                            contacto: item.contacto,
                            telefono: item.telefono,
                            correo: item.correo
                        }

                    ))
                };
            }
        },
        language: 'es',
        placeholder: 'Buscar Proveedor',
        minimumInputLength: 1,
        templateResult: formatoResultadoProveedor,
    });

    function formatoResultadoProveedor(data) {
        if (data.loading) {
            return data.text;
        }

        var contenedor = $(
            `<table width="100%">
                <tr>
                    <td>
                        <td style="width:60px">
                            <img style="height:60px;width:60px; margin-right:20px" src="${data.urlImagen}" />
                        </td>
                        <p style="font-weight:bold; margin:2px">${data.nombre}</p
                        <p style="margin:2px">${data.contacto}</p>
                    </td>
                </tr>
            </table>`
        );

        return contenedor;
    }

    $(document).on("select2:open", function () {
        document.querySelector(".select2-search__field").focus();
    })

    $("#cboBuscarProveedor").on("select2:select", function (e) {
        const data = e.params.data;

        let proveedor_encontrado = proveedorParaPedido.filter(P => P.idProveedor == data.id)
        if (proveedor_encontrado.length > 0) {
            $("#cboBuscarProveedor").val("").trigger("change")
            toastr.warning("", "El proveedor ya fue agregado");
            return false;
        }

        swal({

            title: data.nombre,
            text: data.nombre,
            imageUrl: data.urlImagen,
            showCancelButton: true,
            type: "input",
            closeOnConf‌irm: false,
            inputPlaceholder: "Digite Numero de Pedido"
        },
            function (valor) {

                if (valor === false) { return false }
                if (valor === "") {
                    toastr.warning("", "Nesecita ingresar N. Pedido");
                    return false;
                }
                if (isNaN(parseInt(valor))) {
                    toastr.warning("", "Debe ingresar un valor");
                    return false;
                }

                var val = valor;

                let proveedor = {
                    idProveedor: data.id,
                    nitProveedor: data.nit,
                    nombreProveedor: data.nombre,
                    descripcionProveedor: data.contacto,
                    cantidad: val,
                    //precio: data.precio.toString(),
                    //total: (parseFloat(valor)*data.precio).toString()
                }

                proveedorParaPedido.push(proveedor);

                mostrarProveedor_Precios();
                $("#cboBuscarProveedor").val("").trigger("change");
                swal.close();

                if (data.nit != null) {

                    $.ajax({
                        url: '/Pedido/ObtenerProductos',
                        type: 'GET',
                        data: {
                            busqueda: data.id
                        },
                        success: function (data) {

                            mostrarProducto_Precios(data);
                        },
                        error: function (error) {
                            console.log(error);
                        }
                    });
                }
            }
        );

    })

    function mostrarProveedor_Precios() {

        $("#tbProveedor tbody").html("")

        proveedorParaPedido.forEach((item) => {
            $("#tbProveedor tbody").append(
                $("<tr>").append(
                    $("<td>").text(item.nitProveedor),
                    $("<td>").text(item.nombreProveedor),
                    $("<td>").text(item.descripcionProveedor),
                    $("<td>").text(item.cantidad),

                )
            )
        })

    }

    ////******** fin proveedor

    fetch("/Movimiento/ListaTipoDocumentoMovimiento")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    if (item.naturaleza == 'E') {
                        $("#cboTipoDocumentoMovimiento").append(
                            $("<option>").val(item.idTipoDocumentoMovimiento).text(item.descripcion)
                        )
                    }
                })
            }
        })

    fetch("/Negocio/Obtener")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const d = responseJson.objeto;
                console.log(d);
                $("#inputGroupSubTotal").text(`Sub Total - ${d.simboloMoneda}`)
                $("#inputGroupIGV").text(`IMP(${d.porcentajeImpuesto}%) - ${d.simboloMoneda}`)
                $("#inputGroupTotal").text(`Total - ${d.simboloMoneda}`)
                valorImpuesto = parseFloat(d.porcentajeImpuesto);
            }
        })
})


function limpiarProveedor() {

    $("#tbProveedor tbody").html("")

    proveedorParaPedido.forEach((item) => {
        $("#tbProveedor tbody").append(
            $("<tr>").append(
                $("<td>").text(item.nitProveedor),
                $("<td>").text(item.nombreProveedor),
                $("<td>").text(item.descripcionProveedor),
                $("<td>").text(item.cantidad),

            )
        )
    })

}

$(document).on("select2:open", function () {
    document.querySelector(".select2-search__field").focus();
})

let productosParaMovimiento = [];

function mostrarProducto_Precios(Data) {

    let total = 0;
    let igv = 0;
    let subTotal = 0;
    let porcentaje = valorImpuesto / 100;

    $("#tbProducto tbody").html("");

    Data.forEach((item) => {
        total = total + parseFloat(item.total);
        $("#tbProducto tbody").append(
            $("<tr>").attr("id", item.idProducto).append(
                //$("<td>").append(
                //    $("<button>").addClass("btn btn-danger btn-eliminar btn-sm").append(
                //        $("<i>").addClass("fas fa-trash-alt")
                //    ).data("idProducto", item.idProducto),
                //),
                $("<td>").text(item.codigoBarra),
                $("<td>").text(item.descripcion),
                $("<td>").text(item.marca),
                $("<td>").append($("<input>").addClass("form-control input-precio").val(item.precio)),
                $("<td>").append($("<input>").addClass("form-control input-cantidad")),
                //  $("<td>").append($("<input>").addClass("form-control input-subtotal")),
                $("<td>").append($("<input>").addClass("form-control input-subtotal").val(0).prop('readonly', true)),

                //console.log("item.idProducto: " + item.idProducto)
            )
        )
    });

    // Escuchar cambios en los inputs de cantidad y precio
    $(".input-cantidad, .input-precio").on("change", function () {
        actualizarTotal();
    });

    function actualizarTotal() {
        let subtotal2 = 0;
        $(".input-cantidad, .input-precio").each(function () {
            const cantidad = parseFloat($(this).closest("tr").find(".input-cantidad").val());
            const precio = parseFloat($(this).closest("tr").find(".input-precio").val());
            const subtotal = cantidad * precio;

            if (isNaN(subtotal)) {
                subtotal2 = subtotal2 + 0;
            } else {
                subtotal2 = subtotal2 + subtotal;
            }
            $(this).closest("tr").find(".input-subtotal").val(subtotal.toFixed(2));
        });

        const tott = subtotal2 / 2

        subTotal = tott / (1 + porcentaje);
        igv = tott - subTotal;

        $("#txtSubTotal").val(tott.toFixed(2));
        $("#txtIGV").val(igv.toFixed(2));
        $("#txtTotal").val(tott.toFixed(2));
    }
}

$("#btnTerminarPedido").click(function () {

    productosParaMovimiento = [];

    $("#tbProducto tbody tr").each(function () {

        const codigoBarra = $(this).find("td:eq(1)").text();
        const descripcion = $(this).find("td:eq(2)").text();
        const precio = parseFloat($(this).find(".input-precio").val());
        const cantidad = parseFloat($(this).find(".input-cantidad").val());
        const subtotal = parseFloat($(this).find(".input-subtotal").val());
        const idProducto = $(this).attr("id");

        if (!isNaN(precio) && !isNaN(cantidad) && !isNaN(subtotal)
            && !(cantidad == 0) && !(cantidad == null) && !(cantidad == "")) {
            const producto = {
                codigoBarra: codigoBarra,
                descripcionProducto: descripcion,
                precio: precio,
                cantidad: cantidad,
                total: subtotal,
                idProducto: idProducto
            };
            productosParaMovimiento.push(producto);
        }
    });

    if (productosParaMovimiento.length < 1) {
        toastr.warning("", "Debe ingresar un producto");
        return;
    }

    ////*****

    const detallePedidoDto = productosParaMovimiento;

    const Movimiento = {
        idTipoDocumentoMovimiento: $("#cboTipoDocumentoMovimiento").val(),
        idProveedor: proveedorParaPedido[0].idProveedor,
        numeroDocumentoExterno: proveedorParaPedido[0].cantidad,
        documentoCliente: proveedorParaPedido[0].nitProveedor,
        nombreCliente: proveedorParaPedido[0].nombreProveedor,
        subTotal: $("#txtSubTotal").val(),
        impuestoTotal: $("#txtIGV").val(),
        total: $("#txtTotal").val(),
        DetalleMovimiento: detallePedidoDto
    }

    $("#btnTerminarPedido").LoadingOverlay("show");

    fetch("/Pedido/RegistrarPedido", {
        method: "POST",
        headers: { "Content-type": "application/json; charset=utf-8" },
        body: JSON.stringify(Movimiento),
    })
        .then(response => {
            $("#btnTerminarPedido").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {

                productosParaMovimiento = [];
                proveedorParaPedido = [];

                mostrarProducto_Precios(productosParaMovimiento);
                limpiarProveedor(proveedorParaPedido);

                $("#txtDocumentoCliente").val("");
                $("#txtNombreCliente").val("");
                $("#cboTipoDocumentoMovimiento").val($("#cboTipoDocumentoMovimiento option:first").val())
                $("#txtSubTotal").val("");
                $("#txtIGV").val("");
                $("#txtTotal").val("");

                swal("Registrado", `Numero de Pedido:${responseJson.objeto.numeroDocumentoExterno}  `, "success")
            } else {
                swal("Error", responseJson.mensaje, "error")

            }

        })

})