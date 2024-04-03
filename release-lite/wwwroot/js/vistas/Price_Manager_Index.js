﻿const MODELO_BASE = {
    idRoomPrice: 0,
    idCategoria: 0,
    monday: "",
    tuesday: "",
    wednesday: "",
    thursday: "",
    friday: "",
    saturday: "",
    sunday: "",
    categoryName: "",
    isActive: 1,
}

const MODELO_BASE2 = {
    idRoomPrice: 0,
    name: "",
    dayOfTheWeek: 0,
    date: "",
    increment: "",
    isActive: 1,
}


const MODELO_BASE3 = {
    idRoomPrice: 0,
    name: "",
    dayOfTheWeek: 0,
    date: "",
    increment: "",
    isActive: 1,
}

const daysOfWeek = ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'];


$(document).ready(function () {

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/PriceManager/ListRegular',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idRoomPrice", "visible": false, "searchable": false },
            { "data": "idCategoria", "visible": false, "searchable": false },
            { "data": "nombreCategoria" },

            { "data": "monday", "render": $.fn.dataTable.render.number(',', '.', 2, '$') },
            { "data": "tuesday", "render": $.fn.dataTable.render.number(',', '.', 2, '$') },
            { "data": "wednesday", "render": $.fn.dataTable.render.number(',', '.', 2, '$') },
            { "data": "thursday", "render": $.fn.dataTable.render.number(',', '.', 2, '$') },
            { "data": "friday", "render": $.fn.dataTable.render.number(',', '.', 2, '$') },
            { "data": "saturday", "render": $.fn.dataTable.render.number(',', '.', 2, '$') },
            { "data": "sunday", "render": $.fn.dataTable.render.number(',', '.', 2, '$') },
            {
                "defaultContent": '<button class="btn btn-outline-secondary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>'
                    + '<button class="btn btn-outline-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
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
                filename: 'Reporte Categorias',
                exportOptions: {
                    columns: [1, 2]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

    tablaData2 = $('#tbdata2').DataTable({
        responsive: true,
        "ajax": {
            "url": '/PriceManager/ListSpecial',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idSeason", "visible": false, "searchable": false },
            { "data": "name" },
            {
                "data": "dayOfTheWeek", render: function (data) {
                    // Validar que el valor de dayOfTheWeek esté dentro del rango válido
                    if (data >= 1 && data <= 7) {
                        return daysOfWeek[data - 1]; // Restar 1 para mapear correctamente al índice del array
                    } else {
                        return 'Día inválido'; // Manejar el caso de un valor fuera de rango
                    }
                }
            },
            { "data": "date" },
            { "data": "increment" },
            { "data": "user" },
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
                filename: 'Reporte Temporadas',
                exportOptions: {
                    columns: [1, 2]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

    tablaData3 = $('#tbdata3').DataTable({
        responsive: true,
        "ajax": {
            "url": '/PriceManager/ListHolidays',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idHoliday", "visible": false, "searchable": false },
            { "data": "name" },
            {
                "data": "dayOfTheWeek", render: function (data) {
                    const date = new Date();
                    date.setDate(data); // Establecer el día del mes para el valor de data (1 al 31)
                    const options = { weekday: 'long' };
                    return date.toLocaleString('es-ES', options); // Obtener el nombre del día de la semana en español
                }
            },
            { "data": "date" },
            { "data": "increment" },
            { "data": "user" },
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
        order: [[4, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                className: '',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Festivos',
                exportOptions: {
                    columns: [1, 2]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

    $('#myTab a').on('click', function (e) {
        e.preventDefault()
        $(this).tab('show')
    })

    $("#txtDate").on("change", function () {
        const fechaSeleccionada = $(this).val();
        const dateObject = new Date(fechaSeleccionada);
        let dayOfWeek = dateObject.getDay(); 
        let day = dayOfWeek + 2
        if (day === 8) {
            day = 1; 
        }
        $("#txtDayOfTheWeek").val(day);
    });

});

function MostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.idRoomPrice);
    $("#txtIdCategory").val(modelo.idCategoria);
    $("#txtCategoryName").val(modelo.nombreCategoria);
    $("#txtMonday").val(modelo.monday);
    $("#txtTuesday").val(modelo.tuesday);
    $("#txtWednesday").val(modelo.wednesday);
    $("#txtThursday").val(modelo.thursday);
    $("#txtFriday").val(modelo.friday);
    $("#txtSaturday").val(modelo.saturday);
    $("#txtSunday").val(modelo.sunday);

    $("#cboEstado").val(modelo.isActive);

    $("#modalDataRegular").modal("show");
}

function MostrarModal2(modelo2 = MODELO_BASE2) {
    $("#txtId").val(modelo2.idRoomPrice);
    $("#txtName").val(modelo2.name);
    const diaSemana = obtenerDiaSemana(modelo2.date);
    $("#txtDayOfTheWeek").val(diaSemana);
    const fechaFormateada = modelo2.date.split('T')[0];
    $("#txtDate").val(fechaFormateada);
    $("#txtIncrement").val(modelo2.increment);

    $("#cboEstado").val(modelo2.isActive);

    $("#modalDataRegular2").modal("show");
}

function obtenerDiaSemana(fecha) {
    const date = new Date(fecha);
    let diaSemana = date.getDay();
    diaSemana = diaSemana === 0 ? 7 : diaSemana;
    return diaSemana;
}

function MostrarModal3(modelo3 = MODELO_BASE3) {
    $("#txtId").val(modelo3.idHoliday);
    $("#txtName3").val(modelo3.name);
    const diaSemana = obtenerDiaSemana(modelo3.date);
    $("#txtDayOfTheWeek3").val(diaSemana);
    const fechaFormateada = modelo3.date.split('T')[0];
    $("#txtDate3").val(fechaFormateada); $("#txtIncrement").val(modelo3.increment);
    $("#txtIncrement3").val(modelo3.increment);

    $("#cboEstado").val(modelo3.isActive);

    $("#modalDataRegular3").modal("show");
}

$("#btnNuevo").click(function () {
    MostrarModal();
});

$("#btnNuevo2").click(function () {
    MostrarModal2();
});

$("#btnNuevo3").click(function () {
    MostrarModal3();
});

$("#btnGuardarRegular").click(function () {

    var inputs = $("#modalDataRegular").find(".input-validar");
    var isValid = true;

    inputs.each(function () {
        var value = $(this).val();
        if ($.isNumeric(value)) {
            $(this).removeClass("is-invalid");
        } else {
            $(this).addClass("is-invalid");
            isValid = false;
        }
    });

    if (!isValid) {
        toastr.warning("", "Debes completar todos los Precios");
        $("#txtMonday").focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idRoomPrice"] = parseInt($("#txtId").val());
    modelo["idCategoria"] = parseInt($("#txtIdCategory").val());
    modelo["categoryName"] = $("#txtCategoryName").val();
    modelo["monday"] = $("#txtMonday").val();
    modelo["tuesday"] = $("#txtTuesday").val();
    modelo["wednesday"] = $("#txtWednesday").val();
    modelo["thursday"] = $("#txtThursday").val();
    modelo["friday"] = $("#txtFriday").val();
    modelo["saturday"] = $("#txtSaturday").val();
    modelo["sunday"] = $("#txtSunday").val();

    modelo["isActive"] = $("#cboEstado").val();

    $("#modalDataRegular").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idCategoria == 0) {
        fetch("/PriceManager/Crear", {
            method: "POST",
            headers: { "Content-type": "application/json; charset=utf-8" },
            body: JSON.stringify(modelo),
        })
            .then(response => {
                $("#modalDataRegular").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false);
                    $("#modalDataRegular").modal("hide");
                    swal("Listo!", "La Categoria fue Creada", "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/PriceManager/Editar", {
            method: "PUT",
            headers: { "Content-type": "application/json; charset=utf-8" },
            body: JSON.stringify(modelo),
        })
            .then(response => {
                $("#modalDataRegular").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null;
                    $("#modalDataRegular").modal("hide");
                    swal("Listo!", "El Precio fue modificado", "success")
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
        text: `Eliminar el Precio para "${data.nombreCategoria}"`,
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

                fetch(`/PriceManager/Eliminar?IdRoomPrice=${data.idRoomPrice}`, {
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
                            swal("Listo!", "El precio estandar fue eliminado", "success")
                        } else {
                            swal("Lo sentimos!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    );

})

$("#btnGuardarRegular2").click(function () {

    var inputs = $("#modalDataRegular2").find(".input-validar");
    var isValid = true;

    inputs.each(function () {
        var value = $(this).val();
        if ($.isNumeric(value)) {
            $(this).removeClass("is-invalid");
        } else {
            $(this).addClass("is-invalid");
            isValid = false;
        }
    });

    if (!isValid) {
        toastr.warning("", "Debes completar todos los Campos");
        $("#txtName").focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idSeason"] = parseInt($("#txtId").val());
    modelo["name"] = $("#txtName").val();
    modelo["dayOfTheWeek"] = $("#txtDayOfTheWeek").val();
    modelo["date"] = $("#txtDate").val();
    modelo["increment"] = $("#txtIncrement").val();

    modelo["isActive"] = $("#cboEstado").val();

    $("#modalDataRegular2").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idCategoria == 0) {
        fetch("/PriceManager/CreateSeason", {
            method: "POST",
            headers: { "Content-type": "application/json; charset=utf-8" },
            body: JSON.stringify(modelo),
        })
            .then(response => {
                $("#modalDataRegular2").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false);
                    $("#modalDataRegular2").modal("hide");
                    swal("Listo!", "El dia de temporada fue Creado", "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/PriceManager/EditSeason", {
            method: "PUT",
            headers: { "Content-type": "application/json; charset=utf-8" },
            body: JSON.stringify(modelo),
        })
            .then(response => {
                $("#modalDataRegular").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null;
                    $("#modalDataRegular").modal("hide");
                    swal("Listo!", "La Temporada fue modificada", "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    }
});

let filaSeleccionada2;

$("#tbdata2 tbody").on("click", ".btn-editar", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada2 = $(this).closest("tr").prev();
    } else {
        filaSeleccionada2 = $(this).closest("tr");
    }

    const data = tablaData2.row(filaSeleccionada2).data();
    MostrarModal2(data);

})

$("#tbdata2 tbody").on("click", ".btn-eliminar", function () {
    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaData2.row(fila).data();

    swal({
        title: "¿Está seguro?",
        text: `Eliminar el Precio para "${data.nombreCategoria}"`,
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

                fetch(`/PriceManager/DeleteSeason?IdSeason=${data.idSeason}`, {
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
                            swal("Listo!", "El precio estandar fue eliminado", "success")
                        } else {
                            swal("Lo sentimos!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    );

})


$("#btnGuardarRegular3").click(function () {

    var inputs = $(".input-validar");
    var isValid = true;

    inputs.each(function () {
        var value = $(this).val();
        if ($.isNumeric(value)) {
            $(this).removeClass("is-invalid"); // Remover clase 'is-invalid' si existe
        } else {
            $(this).addClass("is-invalid"); // Agregar clase 'is-invalid' para mostrar el error de validación
            isValid = false;
        }
    });

    if (!isValid) {
        toastr.warning("", "Debes completar todos los Precios");
        $("#txtMonday").focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idRoomPrice"] = parseInt($("#txtId").val());
    modelo["idCategoria"] = parseInt($("#txtIdCategory").val());
    modelo["categoryName"] = $("#txtCategoryName").val();
    modelo["monday"] = $("#txtMonday").val();
    modelo["tuesday"] = $("#txtTuesday").val();
    modelo["wednesday"] = $("#txtWednesday").val();
    modelo["thursday"] = $("#txtThursday").val();
    modelo["friday"] = $("#txtFriday").val();
    modelo["saturday"] = $("#txtSaturday").val();
    modelo["sunday"] = $("#txtSunday").val();

    modelo["isActive"] = $("#cboEstado").val();

    $("#modalDataRegular").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idCategoria == 0) {
        fetch("/PriceManager/CreateHoliday", {
            method: "POST",
            headers: { "Content-type": "application/json; charset=utf-8" },
            body: JSON.stringify(modelo),
        })
            .then(response => {
                $("#modalDataRegular").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false);
                    $("#modalDataRegular").modal("hide");
                    swal("Listo!", "La Categoria fue Creada", "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/PriceManager/EditHoliday", {
            method: "PUT",
            headers: { "Content-type": "application/json; charset=utf-8" },
            body: JSON.stringify(modelo),
        })
            .then(response => {
                $("#modalDataRegular").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    filaSeleccionada = null;
                    $("#modalDataRegular").modal("hide");
                    swal("Listo!", "El Precio fue modificado", "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    }
});

let filaSeleccionada3;

$("#tbdata3 tbody").on("click", ".btn-editar", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada3 = $(this).closest("tr").prev();
    } else {
        filaSeleccionada3 = $(this).closest("tr");
    }

    const data = tablaData3.row(filaSeleccionada3).data();
    MostrarModal3(data);

})

$("#tbdata3 tbody").on("click", ".btn-eliminar", function () {
    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaData.row(fila).data();

    swal({
        title: "¿Está seguro?",
        text: `Eliminar el dia de temporada "${data.name}"`,
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

                fetch(`/PriceManager/DeleteHoliday?IdHoliday=${data.idHoliday}`, {
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
                            swal("Listo!", "El precio estandar fue eliminado", "success")
                        } else {
                            swal("Lo sentimos!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    );

})