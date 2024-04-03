const MODELO_BASE = {
    idRoom: 0,
    status: "",
    number: "",
    description: "",
    capacity: "",
    idRoomStatus: "",
    observation: "",
}

$(document).ready(function () {
    $.ajax({
        url: '/Room/GetAllLevels',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            const levels = response.niveles;

            // Generar las pestañas para cada nivel
            const tabs = levels.map(level => {
                return `<li class="nav-item" role="presentation">
                            <button class="nav-link" id="tab-piso${level.levelNumber}" data-bs-toggle="tab" data-bs-target="#tab-content-piso${level.levelNumber}" type="button" role="tab" aria-controls="tab-content-piso${level.levelNumber}" aria-selected="false" data-level-id="${level.levelNumber}">${level.levelName}</button>
                        </li>`;
            });

            // Agregar las pestañas generadas al elemento ul con id="myTab"
            $('#myTab').html(`<li class="nav-item" role="presentation">
                                <button class="nav-link active" id="tab-all" data-bs-toggle="tab" data-bs-target="#tab-content-all" type="button" role="tab" aria-controls="tab-content-all" aria-selected="true" data-level-id="0">Todos</button>
                            </li>` + tabs.join(''));

            // Cargar todas las habitaciones al inicio
            loadRoomsForLevel(0);

            // Asignar evento cuando se muestra una pestaña
            $('#myTab button[data-level-id]').on('click', function () {
                const lnum = $(this).data('level-id');
                $(this).tab('show');
                $('#myTabContent').empty();
                loadRoomsForLevel(lnum);
            });
        },
        error: function () {
            //Error
        }
    });
});

function getCardColor(status) {
    switch (status) {
        case 8: //'DISPONIBLE':
            return 'bg-c-green';
        case 1: //'OCUPADA':
            return 'bg-c-pink';
        case 6: //'MANTENIMIENTO':
            return 'bg-c-yellow';
        case 2: //'ASEO Prof':
            return 'bg-c-blue';
        case 3: //'ASEO Ligero':
            return 'bg-c-blue-light';
        case 4: //'Checkout tardio':
            return 'bg-c-red'
        default:
            return 'bg-c-blue';
    }
}


function loadRoomsForLevel(lnum) {

    if (lnum === 0) {
        // Cargar todas las habitaciones
        $.ajax({
            url: '/Room/Lista',
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                const rooms = response.data;

                const cards = rooms.map(room => {
                    const cardColor = getCardColor(room.idRoomStatus);
                    return `<div class="col-md-4 col-xl-3">
                              <div class="card ${cardColor} order-card pointer" data-room-id="${room.idRoom}">
                                <div class="card-block">
                                  <h6 class="m-b-20">${room.categoryName}</h6>
                                  <h2 class="text-right"><i class="fa fa-bed f-left"></i><span>${room.number}</span></h2>
                                  <p class="m-b-0">${room.statusName}<span class="f-right">${room.sizeRoom} m²</span></p>
                                </div>
                              </div>
                            </div>`;
                });

                $('#myTabContent').html(`<div class="row">${cards.join('')}</div>`);

                $('.card.pointer').click(function () {
                    const roomId = $(this).data('room-id');
                    const room = rooms.find(r => r.idRoom === roomId);
                    MostrarModal(room);
                });
            },
            error: function () {
                // Manejar el error en caso de que falle la solicitud
            }
        });
    } else {
        // Cargar las habitaciones para el nivel seleccionado
        $.ajax({
            url: '/Room/ListByLevel',
            type: 'GET',
            data: {
                levelNum: lnum
            },
            dataType: 'json',
            success: function (response) {
                const rooms = response.data;

                // Generar las tarjetas para las habitaciones
                // Generar las tarjetas para todas las habitaciones
                const cards = rooms.map(room => {
                    const cardColor = getCardColor(room.idRoomStatus); // Obtener el color de la tarjeta
                    return `<div class="col-md-4 col-xl-3">
                              <div class="card ${cardColor} order-card pointer" data-room-id="${room.idRoom}">
                                <div class="card-block">
                                  <h6 class="m-b-20">${room.categoryName}</h6>
                                  <h2 class="text-right"><i class="fa fa-bed f-left"></i><span>${room.number}</span></h2>
                                  <p class="m-b-0">${room.statusName}<span class="f-right">${room.sizeRoom} m²</span></p>
                                </div>
                              </div>
                            </div>`;
                });

                // Agregar las tarjetas generadas al contenido de la pestaña "Todos"
                $('#myTabContent').html(`<div class="row">${cards.join('')}</div>`);

                $('.card.pointer').click(function () {
                    const roomId = $(this).data('room-id');
                    const room = rooms.find(r => r.idRoom === roomId);
                    MostrarModal(room);
                });
            },
            error: function () {
                // Manejar el error en caso de que falle la solicitud
            }
        });
    }

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
}

function MostrarModal(modelo = MODELO_BASE) {

    $("#txtId").val(modelo.idRoom);
    $("#txtNumber").val(modelo.number);
    $("#txtRoomTitle").val(modelo.roomTitle);
    $("#txtDescription").val(modelo.description);
    $("#txtCapacity").val(modelo.capacity);
    $("#cboEstado").val(modelo.idRoomStatus);
    $("#txtObservations").val(modelo.observation);

    $("#modalData").modal("show");
    console.log(modelo);
}


$("#btnGuardar").click(function () {

    if ($("#txtObservations").val().trim() == "") {
        toastr.warning("", "Debes detallar el motivo u observacion");
        $("#cboEstado").focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["idRoom"] = parseInt($("#txtId").val());
    modelo["observation"] = $("#txtObservations").val();
    modelo["idRoomStatus"] = parseInt($("#cboEstado").val());

    $("#modalData").find("div.modal-content").LoadingOverlay("show");


    fetch("/Reception/CreateMovement", {
        method: "POST",
        headers: { "Content-type": "application/json; charset=utf-8" },
        body: JSON.stringify(modelo),
    })
        .then(response => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {

                const cardColor = getCardColor(modelo.idRoomStatus);
                const roomCard = $(`div.card[data-room-id="${modelo.idRoom}"]`);
                roomCard.removeClass().addClass(`card ${cardColor} order-card pointer`);
                roomCard.find('.m-b-0').html(`${responseJson.objeto.statusName}<span class="f-right">${responseJson.objeto.sizeRoom} m²</span>`);


                $("#modalData").modal("hide");
                swal("Listo!", "Actualizacion guardada exitosamente", "success")
            } else {
                swal("Lo sentimos!", responseJson.mensaje, "error")
            }
        })
});