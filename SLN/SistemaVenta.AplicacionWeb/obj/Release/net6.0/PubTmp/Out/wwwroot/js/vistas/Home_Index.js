$(document).ready(function () {


    $(".container-fluid").LoadingOverlay("show");

    // Obtener información del usuario
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

                // Obtener información de la subscripción
                return fetch("/Home/ObtenerServicios");
            } else {
                swal("Lo sentimos!", responseJson.mensaje, "error")
            }
        })
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const o = responseJson.objeto

                // Mostrar información de la subscripción
                $("#nombreSubscripcion").text(o.planDescription);
                const expiryDate = new Date(o.expiryDate);
                $("#fechaExpiracion").text(expiryDate.toLocaleDateString('en-US', { year: 'numeric', month: '2-digit', day: '2-digit' }));
            } else {
                swal("Lo sentimos!", responseJson.mensaje, "error")
            }
        });
});
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
