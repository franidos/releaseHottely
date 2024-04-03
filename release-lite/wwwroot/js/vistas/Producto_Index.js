const MODELO_BASE = {
    idProducto: 0,
    codigoBarra: "",
    marca: "",
    descripcion: "",
    idCategoriaProducto: 0,
    stock: 0,
    urlImagen: "",
    precio: 0,
    esActivo: 1,
}

let tablaData;

$(document).ready(function () {

    fetch("/CategoriaProducto/Lista")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    $("#cboCategoria").append(
                        $("<option>").val(item.idCategoriaProducto).text(item.nombreCategoriaProducto)
                    )
                })
            }
        })

    fetch("/Proveedor/Lista")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    $("#cboProveedor").append(
                        $("<option>").val(item.idProveedor).text(item.nombre)
                    )
                })
            }
        })

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Producto/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idProducto", "visible": false, "searchable": false },
            { "data": "urlImagen", render: function (data) { return `<img style="height:60px" src=${data} class="rounded mx-auto d-block"/>` } },
            { "data": "codigoBarra" },
            { "data": "marca" },
            { "data": "descripcion" },
            { "data": "idCategoriaProducto" },
            { "data": "stock" },
            { "data": "precio" },
            {
                "data": "esActivo", render: function (data) {
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
                filename: 'Reporte Productos',
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
    $("#txtId").val(modelo.idProducto);
    $("#txtCodigoBarra").val(modelo.codigoBarra);
    $("#txtMarca").val(modelo.marca);
    $("#txtDescripcion").val(modelo.descripcion);
    $("#txtNombreCategoria").val(modelo.nombreCategoriaProducto);
    $("#cboCategoria").val(modelo.idCategoriaProducto == 0 ? $("#cboCategoria option:first").val() : modelo.idCategoriaProducto);
    $("#cboProveedor").val(modelo.idProveedor == 0 ? $("#cboProveedor option:first").val() : modelo.idProveedor);
    $("#txtStock").val(modelo.stock);
    $("#txtPrecio").val(modelo.precio);
    $("#cboEstado").val(modelo.esActivo);
    $("#txtImagen").val("");
    $("#imgProducto").attr("src", modelo.urlImagen);

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
        const mensaje = `Debes completar el campo "${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje);
        $(`input[name = "${inputs_sin_valor[0].name}"]`).focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idProducto"] = parseInt($("#txtId").val());
    modelo["codigoBarra"] = $("#txtCodigoBarra").val();
    modelo["marca"] = $("#txtMarca").val();
    modelo["descripcion"] = $("#txtDescripcion").val();
    modelo["nombreCategoriaProducto"] = $("#txtNombreCategoria").val();
    modelo["idCategoriaProducto"] = $("#cboCategoria").val();
    modelo["idProveedor"] = $("#cboProveedor").val();
    modelo["stock"] = 0;
    modelo["precio"] = $("#txtPrecio").val();
    modelo["esActivo"] = $("#cboEstado").val();

    console.log("modelo " + modelo);

    const inputImagen = document.getElementById("txtImagen");
    const formData = new FormData();
    formData.append("imagen", inputImagen.files[0])
    formData.append("modelo", JSON.stringify(modelo))

    console.log("formData " + formData);

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idProducto == 0) {
        fetch("/Producto/Crear", {
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
                    swal("Listo!", "El Producto fue Creado", "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/Producto/Editar", {
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
                    swal("Listo!", "El Producto fue modificado", "success")
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
        text: `Eliminar al producto "${data.descripcion}"`,
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
                fetch(`/Producto/Eliminar?idProducto=${data.idProducto}`, {
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
                            swal("Listo!", "El producto fue eliminado", "success")
                        } else {
                            swal("Lo sentimos!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    );

})

//// Categorias

let tablaDataP;
let tablaSel = false;

const MODELO_BASEP = {
    idCategoriaProducto: 0,
    descripcion: "",
    nombreCategoriaProducto: "",
    esActivo: 1,
}


$(".categoria").click(function () {

    if (!tablaSel) {

        tablaSel = true;
        tablaDataP = $('#tbdataP').DataTable({
            responsive: true,
            "ajax": {
                "url": '/CategoriaProducto/Lista',
                "type": "GET",
                "datatype": "json"
            },
            "columns": [
                { "data": "idCategoriaProducto", "visible": false, "searchable": false },
                { "data": "nombreCategoriaProducto"},
                { "data": "descripcion" },
                {
                    "data": "esActivo", render: function (data) {
                        if (data == 1) return '<span class="badge badge-success">Activo</span>';
                        else return '<span class="badge badge-secondary">No Activo</span>';
                    }
                },
                {
                    "defaultContent": '<button class="btn btn-outline-secondary btn-editarP btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
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


    }
});

function MostrarModalP(modelo = MODELO_BASEP) {
    $("#txtIdP").val(modelo.idCategoriaProducto);
    $("#txtDescripcionP").val(modelo.descripcion);
    $("#txtNombreCategoria").val(modelo.nombreCategoriaProducto);    
    $("#cboEstadoP").val(modelo.esActivo);

    $("#modalDataP").modal("show");
    console.log(modelo);
}

$("#btnNuevoP").click(function () {
    MostrarModalP();
});

$("#btnGuardarP").click(function () {

    if ($("#txtNombreCategoria").val().trim() == "") {
        toastr.warning("", "Debes completar el campo: Descripción");
        $("#txtNombreCategoria").focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASEP);
    modelo["idCategoriaProducto"] = parseInt($("#txtIdP").val());
    modelo["descripcion"] = $("#txtDescripcionP").val();
    modelo["nombreCategoriaProducto"] = $("#txtNombreCategoria").val();
    modelo["esActivo"] = $("#cboEstadoP").val();

    $("#modalDataP").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idCategoriaProducto == 0) {
        fetch("/CategoriaProducto/Crear", {
            method: "POST",
            headers: { "Content-type": "application/json; charset=utf-8" },
            body: JSON.stringify(modelo),
        })
            .then(response => {
                $("#modalDataP").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaDataP.row.add(responseJson.objeto).draw(false);
                    $("#modalDataP").modal("hide");
                    swal("Listo!", "La Categoria producto fue Creada", "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/CategoriaProducto/Editar", {
            method: "PUT",
            headers: { "Content-type": "application/json; charset=utf-8" },
            body: JSON.stringify(modelo),
        })
            .then(response => {
                $("#modalDataP").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaDataP.row(filaSeleccionadaP).data(responseJson.objeto).draw(false);
                    filaSeleccionadaP = null;
                    $("#modalDataP").modal("hide");
                    swal("Listo!", "La Categoria producto fue modificada", "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    }
});

let filaSeleccionadaP;

$("#tbdataP tbody").on("click", ".btn-editarP", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionadaP = $(this).closest("tr").prev();
    } else {
        filaSeleccionadaP = $(this).closest("tr");
    }

    const data = tablaDataP.row(filaSeleccionadaP).data();
    MostrarModalP(data);

})

$("#tbdataP tbody").on("click", ".btn-eliminar", function () {
    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaDataP.row(fila).data();

    swal({
        title: "¿Está seguro?",
        text: `Eliminar la Categoria "${data.descripcionP}"`,
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
                $("#modalDataP").find("div.modal-content").LoadingOverlay("show");
                fetch(`/CategoriaProducto/Eliminar?idCategoriaProducto=${data.idCategoriaProducto}`, {
                    method: "DELETE",
                })
                    .then(response => {
                        $("#modalDataP").find("div.modal-content").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        $("#modalDataP").find("div.modal-content").LoadingOverlay("hide");
                        if (responseJson.estado) {
                            tablaDataP.row(fila).remove().draw();
                            swal("Listo!", "La Categoria producto fue eliminada", "success")
                        } else {
                            swal("Lo sentimos!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    );

})

///// Proveedores

const MODELO_BASEV = {
    idProveedor: 0,
    nit: "",
    nombre: "",
    contacto: "",
    telefono: "",
    correo: 0,
    esActivo: 1,
}

let tablaDataV;
let tablaSelV = false;

//$(document).ready(function () {
$(".proveedor").click(function () {

    if (!tablaSelV) {
        tablaSelV = true;
        tablaDataV = $('#tbdataV').DataTable({
            responsive: true,
            "ajax": {
                "url": '/Proveedor/Lista',
                "type": "GET",
                "datatype": "json"
            },
            "columns": [
                { "data": "idProveedor", "visible": false, "searchable": false },
                { "data": "nit" },
                { "data": "nombre" },
                { "data": "contacto" },
                { "data": "telefono" },
                { "data": "correo" },
                {
                    "data": "esActivo", render: function (data) {
                        if (data == 1) return '<span class="badge badge-success">Activo</span>';
                        else return '<span class="badge badge-secondary">No Activo</span>';
                    }
                },
                {
                    "defaultContent": '<button class="btn btn-outline-secondary btn-editarV btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
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
                    filename: 'Reporte Proveedors',
                    exportOptions: {
                        columns: [2, 3, 4, 5, 6, 7]
                    }
                }, 'pageLength'
            ],
            language: {
                url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
            },
        });
    }
});

function MostrarModalV(modelo = MODELO_BASEV) {
    $("#txtIdV").val(modelo.idProveedor);
    $("#txtNITV").val(modelo.nit);
    $("#txtNombreV").val(modelo.nombre);
    $("#txtContactoV").val(modelo.contacto);
    $("#txtTelefonoV").val(modelo.telefono);
    $("#txtCorreoV").val(modelo.correo);
    $("#cboEstadoV").val(modelo.esActivo);

    $("#modalDataV").modal("show");
}

$("#btnNuevoV").click(function () {
    MostrarModalV();
});

$("#btnGuardarV").click(function () {
    //Validaciones
    const inputs = $("input.input-validarV").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")

    //  console.log(inputs_sin_valor);

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Debes completar el campo "${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje);
        $(`input[name = "${inputs_sin_valor[0].name}"]`).focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASEV);
    modelo["idProveedor"] = parseInt($("#txtIdV").val());
    modelo["nit"] = $("#txtNITV").val();
    modelo["nombre"] = $("#txtNombreV").val();
    modelo["contacto"] = $("#txtContactoV").val();
    modelo["telefono"] = $("#txtTelefonoV").val();
    modelo["correo"] = $("#txtCorreoV").val();
    modelo["esActivo"] = $("#cboEstadoV").val();


    //const inputImagen = document.getElementById("txtImagen");
    const formData = new FormData();
    // formData.append("imagen", inputImagen.files[0])
    formData.append("modelo", JSON.stringify(modelo))

    $("#modalDataV").find("div.modal-content").LoadingOverlay("show");

    if (modelo.idProveedor == 0) {
        fetch("/Proveedor/Crear", {
            method: "POST",
            body: formData,
        })
            .then(response => {
                $("#modalDataV").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                $("#modalDataV").modal("hide");
                if (responseJson.estado) {
                    tablaDataV.row.add(responseJson.objeto).draw(false);                  
                    swal("Listo!", "El Proveedor fue Creado", "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/Proveedor/Editar", {
            method: "PUT",
            body: formData,
        })
            .then(response => {
                $("#modalDataV").find("div.modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                $("#modalDataV").modal("hide");
                if (responseJson.estado) {
                    tablaDataV.row(filaSeleccionadaV).data(responseJson.objeto).draw(false);
                    filaSeleccionadaV = null;                  
                    swal("Listo!", "El Proveedor fue modificado", "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    }
});

let filaSeleccionadaV;

$("#tbdataV tbody").on("click", ".btn-editarV", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionadaV = $(this).closest("tr").prev();
    } else {
        filaSeleccionadaV = $(this).closest("tr");
    }

    const data = tablaDataV.row(filaSeleccionadaV).data();
    //console.log(data);
    MostrarModalV(data);

})

$("#tbdataV tbody").on("click", ".btn-eliminar", function () {
    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaDataV.row(fila).data();

    swal({
        title: "¿Está seguro?",
        text: `Eliminar al Proveedor "${data.Telefono}"`,
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
                $("#modalDataV").find("div.modal-content").LoadingOverlay("show");
                fetch(`/Proveedor/Eliminar?idProveedor=${data.idProveedor}`, {
                    method: "DELETE",
                })
                    .then(response => {
                        $("#modalDataV").find("div.modal-content").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        $("#modalDataV").find("div.modal-content").LoadingOverlay("hide");
                        if (responseJson.estado) {
                            tablaDataV.row(fila).remove().draw();
                            swal("Listo!", "El Proveedor fue eliminado", "success")
                        } else {
                            swal("Lo sentimos!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    );

})
