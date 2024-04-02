const MODELO_BASE = {
    idMap: 0,
    nombreChannel: "",
    descripcion: "",
    esActivo: 1,
}


$(document).ready(function () {

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Channel/ListChannels',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idRoomMap", "visible": false, "searchable": false },
            { "data": "originName" },
            { "data": "roomName" },
            { "data": "idEstablishmentOrigin" },
            { "data": "idRoomOrigin" },
            { "data": "urlCalendar" },
            {
                "data": "isActive", render: function (data) {
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
                filename: 'Reporte Channels',
                exportOptions: {
                    columns: [1, 2]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });


    fetch("/Room/Lista")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    $("#cboRoom").append(
                        $("<option>").val(item.idCategoria).text(item.nombreCategoria)
                    )
                })
            }
        })

    fetch("/Channel/ListOrigins")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    $("#cboOrigins").append(
                        $("<option>").val(item.idLevel).text(item.levelName)
                    )
                })
            }
        })

});

function MostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.idMap);
    $("#txtNombreChannel").val(modelo.nombreChannel);
    $("#txtDescripcion").val(modelo.descripcion);
    $("#cboEstado").val(modelo.esActivo);

    $("#modalData").modal("show");
    console.log(modelo);
}

$("#btnNuevo").click(function () {
    MostrarModal();
});

$("#btnGuardar").click(function () {

    if ($("#txtNombreChannel").val().trim() == "") {
        toastr.warning("", "Debes completar el campo: Nombre Channel");
        $("#txtNombreChannel").focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE); 
    modelo["idMap"] = parseInt($("#txtId").val());
    modelo["nombreChannel"] = $("#txtNombreChannel").val();
    modelo["descripcion"] = $("#txtDescripcion").val();
    modelo["esActivo"] = $("#cboEstado").val();

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idMap == 0) {
        fetch("/Channel/Crear", {
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
                    swal("Listo!", "La Channel fue Creada", "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/Channel/Editar", {
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
                    swal("Listo!", "La Channel fue modificada", "success")
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
        text: `Eliminar la Channel "${data.nombreChannel}"`,
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

                fetch(`/Channel/Eliminar?IdChannel=${data.idMap}`, {
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
                            swal("Listo!", "La Channel fue eliminada", "success")
                        } else {
                            swal("Lo sentimos!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    );

})