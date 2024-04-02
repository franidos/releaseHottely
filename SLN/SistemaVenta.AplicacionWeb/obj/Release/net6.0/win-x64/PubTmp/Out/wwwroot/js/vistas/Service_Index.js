const MODELO_BASE = {
    idService: 0,
    serviceName: "",
    serviceInfo: "",
    serviceInfoQuantity: "",
    serviceMaximumAmount: 0,
    serviceConditions: 0,
    servicePrice: "",
    serviceUrlImage: "",
    isAdditionalValue: 0,
    serviceIsActive: 1,
}

let tablaData;

$(document).ready(function () {

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Services/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idService", "visible": false, "searchable": false },
            { "data": "serviceUrlImage", render: function (data) { return `<img style="height:60px" src=${data} class="rounded mx-auto d-block"/>` } },
            { "data": "serviceName" },
            { "data": "serviceInfo" },
            { "data": "serviceInfoQuantity" },
            { "data": "serviceMaximumAmount" },
            { "data": "serviceConditions" },
            { "data": "servicePrice" },
            { "data": "isAdditionalValue" },
            {
                "data": "serviceIsActive", render: function (data) {
                    if (data == 1) return '<span class="badge badge-success">Activo</span>';
                    else return '<span class="badge badge-secondary">No Activo</span>';
                }
            },
            {
                "defaultContent": '<button class="btn btn-outline-secondary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-outline-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "80px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                className: '',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Services',
                exportOptions: {
                    columns: [2, 3, 4, 5, 6, 7]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });


});


function MostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.idService);
    $("#txtServiceName").val(modelo.serviceName);
    $("#txtServiceInfo").val(modelo.serviceInfo);
    $("#txtServiceInfoQuantity").val(modelo.serviceInfoQuantity);
    $("#txtServiceMaximumAmount").val(modelo.serviceMaximumAmount);
    $("#txtServiceConditions").val(modelo.serviceConditions);
    $("#txtServicePrice").val(modelo.servicePrice);
    $("#cboEstado").val(modelo.serviceIsActive);
    $("#cboAdditional").val(modelo.isAdditionalValue);
    $("#txtImagen").val("");
    $("#imgService").attr("src", modelo.serviceUrlImage);

    $("#modalData").modal("show");
}

$("#btnNuevo").click(function () {
    MostrarModal();
});

$("#btnGuardar").click(function () {
    //Validaciones
    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")

    console.log(inputs_sin_valor);

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Desbes completar el campo "${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje);
        $(`input[name = "${inputs_sin_valor[0].name}"]`).focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idService"] = parseInt($("#txtId").val());
    modelo["serviceName"] = $("#txtServiceName").val();
    modelo["serviceInfo"] = $("#txtServiceInfo").val();
    modelo["serviceInfoQuantity"] = $("#txtServiceInfoQuantity").val();
    modelo["idServiceConditions"] = $("#txtServiceConditions").val();
    modelo["servicePrice"] = $("#txtServicePrice").val();
    modelo["isAdditionalValue"] = $("#cboAdditional").val();
    modelo["serviceIsActive"] = $("#cboEstado").val();
    modelo["serviceMaximumAmount"] = $("#txtServiceMaximumAmount").val();

    console.log("modelo " + modelo);

    const inputImagen = document.getElementById("txtImagen");
    const formData = new FormData();
    formData.append("imagen", inputImagen.files[0])
    formData.append("modelo", JSON.stringify(modelo))

    console.log("formData " + formData);

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idService == 0) {
        fetch("/Services/Crear", {
            method: "POST",
            body: formData,
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false);
                    $("#modalData").modal("hide");
                    swal("Listo!", "El Services fue Creado", "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/Services/Editar", {
            method: "PUT",
            body: formData,
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null;
                    $("#modalData").modal("hide");
                    swal("Listo!", "El Services fue modificado", "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    }
});

let filaSeleccionada;

$("#tbdata tbody").on("click", ".btn-editar", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();
    console.log(data);
    MostrarModal(data);

})

$("#tbdata tbody").on("click", ".btn-eliminar", function () {
    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaData.row(fila).data();

    swal({
        title: "¿Está seguro?",
        text: `Eliminar al service "${data.descripcion}"`,
        type: "warning",
        showCancelButton: true,
        showConfirmButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar",
        closeOnConf‌irm: false,
        closeOnCancel: true
    },
        function (respuesta) {
            if (respuesta) {
                $("#modalData").find("div.modal-content").LoadingOverlay("show");
                fetch(`/Services/Eliminar?idService=${data.idService}`, {
                    method: "DELETE",
                })
                    .then(response => {
                        $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                        if (responseJson.estado) {                           
                            tablaData.row(fila).remove().draw();
                            swal("Listo!", "El service fue eliminado", "success")
                        } else {
                            swal("Lo sentimos!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    );

})
