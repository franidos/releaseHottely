$(document).ready(function () {
    // Hacer una solicitud al controlador para obtener los datos de los niveles (Levels)
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
            // Manejar el error en caso de que falle la solicitud
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
            return 'bg-c-blue'; // Color por defecto para cualquier otro estado
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
                    const cardColor = getCardColor(room.idRoomStatus); // Obtener el color de la tarjeta
                    return `<div class="col-md-4 col-xl-3">
                              <div class="card ${cardColor} order-card"> <!-- Aquí aplicamos la clase de color -->
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
                              <div class="card ${cardColor} order-card"> <!-- Aquí aplicamos la clase de color -->
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
            },
            error: function () {
                // Manejar el error en caso de que falle la solicitud
            }
        });
    }
}
