const MODELO_BASE = {
    idRoom: 0,
    number: "",
    description: "",
    roomTitle: "",
    sizeRoom: "",
    imagen: "",
    idCategoria: 0,
    nombreCategoria: "",
    idEstablishment: 0,
    capacity: 0,
    urlImage: "",
    isActive: 1,
}

let imgCounter = 0;
let tablaData;

$(document).ready(function () {

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Room/ListByIdEstablishment',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idRoom", "visible": false, "searchable": false },
            //{ "data": "urlImage", render: function (data) { return `<img style="height:60px" src=${data} class="rounded mx-auto d-block"/>` } },
            { "data": "number" },
            { "data": "roomTitle" },
            //{ "data": "description" },
            { "data": "categoryName" },
            { "data": "capacity" },
            { "data": "sizeRoom" },
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
                filename: 'Reporte Habitaciones',
                exportOptions: {
                    columns: [2, 3, 4, 5, 6, 7]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });


    fetch("/Categoria/Lista")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    $("#cboCategoria").append(
                        $("<option>").val(item.idCategoria).text(item.nombreCategoria)
                    )
                })
            }
        })

    fetch("/Level/Lista")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    $("#cboLevel").append(
                        $("<option>").val(item.idLevel).text(item.levelName)
                    )
                })
            }
        })

    fetch("/Room/ListarRoomStatus")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    $("#cboEstado").append(
                        $("<option>").val(item.idRoomStatus).text(item.title)
                    )
                })
            }
        })

});

function MostrarModal(modelo = MODELO_BASE) {

    const resetFileInput = () => {
        $("#file-input").val("");
        $("#file-input").wrap('<form>').closest('form').get(0).reset();
        $("#file-input").unwrap();
    };
    resetFileInput();
    $("#txtId").val(modelo.idRoom);
    $("#txtNumber").val(modelo.number);
    //  $("#txtCategoryName").val(modelo.categoryName);
    $("#txtDescription").val(modelo.description);
    $("#cboCategoria").val(modelo.idCategoria == 0 ? $("#cboCategoria option:first").val() : modelo.idCategoria);
    $("#cboLevel").val(modelo.idEstablishment == 0 ? $("#cboLevel option:first").val() : modelo.idLevel);
    $("#txtRoomTitle").val(modelo.roomTitle);
    $("#txtCapacity").val(modelo.capacity);
    $("#txtSizeRoom").val(modelo.sizeRoom);
    $("#cboEstado").val(modelo.isActive);
    $("#cboEstado").val(modelo.idRoomStatus);
    $("#txtImagen").val(""); // $("#imgRoom").attr("src", modelo.urlImage);

    /*ini imagenes*/
    $("#input-images").empty();
    $("#input-images li").remove(); // Elimina las imágenes existentes
    imgCounter = 0

    $("#input-images").sortable({
        axis: "x",
        handle: "img",
        update: function (event, ui) {
            $("#input-images li").each(function (index) {
                $(this).attr("id", index + 1);
            });
        }
    });

    if (modelo.imagesRooms != null) {
        modelo.imagesRooms.forEach((item, index) => {
            indexImg = index + 1;
            console.log("indexImg ", indexImg)
            $("#input-images").append('<li id="' + indexImg + '"><div><img src="' + item.urlImage + '"><button class="delete-button" data-id="' + item.imageNumber + '">X</button></div></li>');
        });
    }

    $("#file-input").on("change", function () {
        deletePreviousAdded()
        var files = this.files;
        imgCounter = imgCounter + 1
        indexImg = 0;
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            imgCounter == 1 ? renderImage(file) : "";
        }
    });

    function deletePreviousAdded() {
        var deletedImages = $("#input-images li[data-status='A1']").length;
        $("#input-images li[data-status='A1']").remove();

        if (deletedImages > 0) {
            imgCounter = 0;
        }
    }

    $("#input-images").on("click", ".delete-button", function (event) {
        event.preventDefault();
        event.stopPropagation();
        $(this).parent().parent().hide();
    });

    function renderImage(file) { // adicionar un "tag" que permita identificar los attach que estan listos para ser enviados a back
        var reader = new FileReader();
        reader.onload = function (e) {

            indexImg++;
            $("#input-images").append('<li id="' + indexImg + '" data-status="A1"><div><img src="' + e.target.result + '"><button class="delete-button" data-id="' + file.name + '">X</button></div></li>');
        };
        reader.readAsDataURL(file);
    }
    /*fin images*/


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
    modelo["idRoom"] = parseInt($("#txtId").val());
    modelo["number"] = $("#txtNumber").val();
    modelo["description"] = $("#txtDescription").val();
    modelo["roomTitle"] = $("#txtRoomTitle").val();
    modelo["capacity"] = $("#txtCapacity").val();
    modelo["sizeRoom"] = $("#txtSizeRoom").val();
    modelo["idCategoria"] = $("#cboCategoria").val();
    modelo["categoryName"] = $("#cboCategoria option:selected").text();;
    modelo["idLevel"] = $("#cboLevel").val();
    modelo["isActive"] = $("#cboEstado").val();
    modelo["nombresImagenes"] = [];
    modelo["idRoomStatus"] = $("#cboEstado option:selected").val();
    const formData = new FormData();

    // Agregar las imágenes existentes
    $("#input-images li").each(function () {
        const image = $(this);
        const imageId = image.attr("id");

        const isHidden = image.css("display") === "none";
        const iniOrdValue = isHidden ? "to-del" : image.find("div button").attr("data-id");

        const imageInfo = {
            currentOrd: imageId,
            iniOrd: iniOrdValue
        };

        formData.append("oldImagesUrl", image.find("div img").attr("src"));
        formData.append("imageIds", JSON.stringify(imageInfo));
    });


    // Agregar las imágenes nuevas
    const fileInput = document.getElementById("file-input");
    for (let i = 0; i < fileInput.files.length; i++) {
        formData.append("newImagenes", fileInput.files[i]);
    }


    formData.append("modelo", JSON.stringify(modelo));

    $("#modalData").find("div.modal-content").LoadingOverlay("show");


    if (modelo.idRoom == 0) {
        fetch("/Room/Create", {
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
                    swal("Listo!", "La Habitacion fue Creada", "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/Room/Edit", {
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
                    swal("Listo!", "La Habitacion fue modificada", "success")
                } else {
                    swal("Lo sentimos!", responseJson.mensaje, "error")
                }
            })
    }
});

let filaSeleccionada;

$("#tbdata tbody").on("click", ".btn-editar", function () {

    $(".divImages").empty();

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();

    $.ajax({
        url: '/Room/ListImagesById',
        type: 'GET',
        data: {
            roomId: data.idRoom
        },
        dataType: 'json',
        success: function (response) {

            const ImagesRooms = response.data;
            data.imagesRooms = ImagesRooms;
            //console.log(data);
            MostrarModal(data);

        },
        error: function () {
            console.log("Error gettting images");
        }
    });

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
        text: `Eliminar la habitacion "${data.descripcion}"`,
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

                fetch(`/Room/Delete?idRoom=${data.idRoom}`, {
                    method: "DELETE",
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw();
                            swal("Listo!", "La Habitacion fue eliminada", "success")
                        } else {
                            swal("Lo sentimos!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    );

})