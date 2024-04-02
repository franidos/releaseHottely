const MODELO_BASE = {
    idGuest: 0,
    nombreGuest: "",
    descripcion: "",
    esActivo: 1,
}


$(document).ready(function () {

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Guest/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idGuest", "visible": false, "searchable": false },
            { "data": "document" },
            {
                "data": null,
                "render": function (data, type, full, meta) {
                    return full.name + ' ' + full.lastName;
                }
            },
            { "data": "nationality" },
            { "data": "email" },
            { "data": "phoneNumber" },
            //{ "data": "descripcion" },
            {
                "data": "isMain", render: function (data) {
                    if (data == 1) return '<span class="badge badge-success">Principal</span>';
                    else return '<span class="badge badge-secondary">Acompanante</span>';
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
                filename: 'Reporte Guests',
                exportOptions: {
                    columns: [1, 2]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });


});

function MostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.idGuest);
    $("#cboDocumentType").val(modelo.documentType);
    $("#txtDocument").val(modelo.document);
    $("#country").val(modelo.country);
    $("#nationality").val(modelo.nationality);
    $("#txtTratamiento").val(modelo.tratamiento);
    $("#txtName").val(modelo.name);
    $("#txtLastName").val(modelo.lastName);
    $("#txtEmail").val(modelo.email);
    $("#txtPhoneNumber").val(modelo.phoneNumber);
    $("#txtCodeCountry").val(modelo.codeCountry);
    $("#txtOriginCity").val(modelo.originCity);
    $("#txtRecidenceCity").val(modelo.recidenceCity);
    $("#txtIsMain").val(modelo.isMain);
    $("#txtAge").val(modelo.age);

    $("#modalData").modal("show");
}

$("#btnNuevo").click(function () {
    MostrarModal();
});

$("#btnGuardar").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")

    console.log(inputs_sin_valor);

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debes completar el campo "${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje);
        $(`input[name = "${inputs_sin_valor[0].name}"]`).focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idGuest"] = parseInt($("#txtId").val());
    modelo["documentType"] = $("#cboDocumentType").val();
    modelo["document"] = $("#txtDocument").val();
    modelo["country"] = $("#country").val();
    modelo["nationality"] = $("#nationality").val();
    modelo["tratamiento"] = $("#txtTratamiento").val();
    modelo["name"] = $("#txtName").val();
    modelo["lastName"] = $("#txtLastName").val();
    modelo["email"] = $("#txtEmail").val();
    modelo["phoneNumber"] = $("#txtPhoneNumber").val();
    modelo["codeCountry"] = $("#txtCodeCountry").val();
    modelo["originCity"] = $("#txtOriginCity").val();
    modelo["recidenceCity"] = $("#txtRecidenceCity").val();
    modelo["isMain"] = $("#txtIsMain").val();
    modelo["age"] = $("#txtAge").val();

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idGuest == 0) {
        fetch("/Guest/Crear", {
            method: "POST",
            headers: {"Content-type":"application/json; charset=utf-8"},
            body: JSON.stringify(modelo),
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false);
                    $("#modalData").modal("hide");
                    swal("Listo!", "El cliente fue Creado", "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/Guest/Editar", {
            method: "PUT",
            headers: { "Content-type": "application/json; charset=utf-8" },
            body: JSON.stringify(modelo),
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
                    swal("Listo!", "La Guest fue modificada", "success")
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
        text: `Eliminar la Guest "${data.nombreGuest}"`,
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
                $(".showSweetAlert").LoadingOverlay("show");

                fetch(`/Guest/Eliminar?IdGuest=${data.idGuest}`, {
                    method: "DELETE",
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw();
                            swal("Listo!", "La Guest fue eliminada", "success")
                        } else {
                            swal("Lo sentimos!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    );

})