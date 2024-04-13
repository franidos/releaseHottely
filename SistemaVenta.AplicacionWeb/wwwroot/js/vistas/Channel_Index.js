const MODELO_BASE = {
    idRoomMap: 0,
    channelName: "",
    originName: "",
    idEstablishmentOrigin: "",
    idRoomOrigin: "",
    idRoom: 0,
    idOrigin: 0,
    urlCalendar: "",
    isActive: 1,
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
            { "data": "channelName" },
            { "data": "originName" },
            { "data": "roomName" },
            { "data": "idEstablishmentOrigin" },
            { "data": "idRoomOrigin" },
            { "data": "urlCalendar" },
            { "data": "idRoom", "visible": false, "searchable": false },
            { "data": "idOrigin", "visible": false, "searchable": false },
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
                        $("<option>").val(item.idRoom).text(item.number)
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
                        $("<option>").val(item.idOrigin).text(item.name)
                    )
                })
            }
        })

});

function MostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.idRoomMap);
    $("#txtChannelName").val(modelo.channelName);
    $("#cboRoom").val(modelo.idRoom);
    $("#cboOrigins").val(modelo.idOrigin);
    $("#cboEstado").val(modelo.isActive);
    $("#txtIdEstablishmentOrigin").val(modelo.idEstablishmentOrigin);
    $("#txtIdRoomOrigin").val(modelo.idRoomOrigin);
    $("#txtURLCalendar").val(modelo.urlCalendar);

    $("#modalData").modal("show");
    console.log(modelo);
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
    modelo["idRoomMap"] = parseInt($("#txtId").val());
    modelo["channelName"] = $("#txtChannelName").val();
    modelo["idRoom"] = $("#cboRoom").val();
    modelo["idOrigin"] = $("#cboOrigins").val();
    modelo["idEstablishmentOrigin"] = $("#txtIdEstablishmentOrigin").val();
    modelo["idRoomOrigin"] = $("#txtIdRoomOrigin").val();
    modelo["urlCalendar"] = $("#txtURLCalendar").val();
    modelo["isActive"] = $("#cboEstado").val();

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idRoomMap == 0) {
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
                    swal("Listo!", "El canal fue modificado", "success")
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
    console.log("data ", data);
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
        text: `Eliminar la Channel "${data.channelName}"`,
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