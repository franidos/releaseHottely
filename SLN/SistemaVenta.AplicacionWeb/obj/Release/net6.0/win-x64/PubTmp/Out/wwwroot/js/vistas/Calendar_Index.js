const MODELO_BASE = {
    descripcion: "",
    idCategoria: 0,
    precio: 0,
    esActivo: 1,
    start: new Date(),
    end: new Date(),
}

let valorImpuesto = 0;
let valorImpuesto2 = 0;
let checkIn;
let checkOut;
let rooms = [];
let productosParaMovimiento = [];
let formsCreated = 0;
let countAdultsInput = 0;
let countChildrenInput = 0;
let skipCompanionsForm = false;
var calendar;
const MAXIMUM_COUNTER = 8;
var adults = "";
var ageChildren = "";
var tempRoom = [];
var totalRoom = 0;
var uniqueIdRooms = [];
var idRoomSelect = 0;
var dateCalendarCurrent = "";

$(document).ready(function () {

    //Carga Levels y Rooms del establecimiento para Combos y luego inicia calendario con el 1er Room x default
    loadLevelOptions();
    loadRoomOptions($(`#roomSelectRoom`));
    initCalendar(document);

    /** Area Daterange picker **/

    $(function () {
        $('input[name="daterange"]').daterangepicker({
            "autoApply": true,
            "opens": "left",
            "locale": {
                "format": "YYYY-MM-DD",
                "separator": " - ",
                "applyLabel": "Aplicar",
                "cancelLabel": "Cancelar",
                "fromLabel": "Desde",
                "toLabel": "Hasta",
                "customRangeLabel": "Personalizado",
                "daysOfWeek": [
                    "Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"
                ],
                "monthNames": [
                    "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
                ],
                "firstDay": 1
            }
        });
        $('input[name="daterange"]').attr("placeholder", "Ingrese fechas de entrada y salida");
        $('input[name="daterange"]').on('apply.daterangepicker', function (ev, picker) {

            checkIn = new Date(picker.startDate.format('YYYY-MM-DD'));
            checkOut = new Date(picker.endDate.format('YYYY-MM-DD'));
        });
    });

    /** Area Buscar Cliente Existente **/

    var showSearchButton = $("#showInputSearch");

    showSearchButton.on("click", function () {
        var searchInput = $("#txt-search");

        if (searchInput.length) {
            searchInput.remove();
            return;
        }

        searchInput = $("<input>", {
            id: "txt-search",
            class: "form-control",
            type: "text",
            placeholder: "Search",
            "aria-label": "Search",
        }).insertAfter(showSearchButton);

        searchInput.focus().addClass("active");
    });

    function buscarClientesPorAjax(searchTerm, callback) {
        $.ajax({
            url: "/Booking/getGuests",
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            data: {
                busqueda: searchTerm
            },
            success: function (data) {
                var results = data.map((item) => ({
                    id: item.idGuest,
                    documentInfo: item.documentType + ' ' + item.document,
                    documentType: item.documentType,
                    document: item.document,
                    name: item.name,
                    lastName: item.lastName,
                    originCity: item.originCity,
                    RecidenceCity: item.RecidenceCity,
                    phoneNumber: item.phoneNumber,
                    text: item.name + ' ' + item.lastName

                }));
                callback(results);
            }
        });
    }

    $(document).on("keyup", "#txt-search", function () {
        var searchField = $(this).val();
        if (searchField === '') {
            $('#filter-records').html('');
            return;
        }

        buscarClientesPorAjax(searchField, function (results) {
            var output = '';
            $.each(results, function (index, result) {
                output += '<li id="' + result.id + '" class="li-search">' + result.documentInfo + ' | ' + result.text + '</li>';
            });
            $('#filter-records').html(output);
        });
    });

    $(document).on("click", ".li-search", function () {
        $("#txt-search").val($(this).html());
        setFormFields($(this).attr("id"));
        $("#filter-records").html("");
        $(".next").prop("disabled", false);
    });

    function setFormFields(id) {
        if (id !== false) {
            // FILL STEP 2 FORM FIELDS
            buscarClientesPorAjax(id, function (results) {
                var d = results[0]; // Se asume que el resultado es un solo cliente con el ID específico
                $("#DocumentType").val(d.documentType);
                $('#Document').val(d.document);
                $('#Name').val(d.name);
                $('#LastName').val(d.lastName);
                $('#OriginCity').val(d.originCity);
                $('#RecidenceCity').val(d.address);
                $('#PhoneNumber').val(d.phoneNumber);
                $('#Email').val(d.email);

            });
        } else {
            $("#txt-search").val('');
            $('#Name').val('');
            $('#LastName').val('');
            $('#team').val('');
            $('#RecidenceCity').val('');
            $('#tel').val('');
        }
    }

    /**  Area counters Room, Adult and Children  **/

    let countRoomsInput = $('#countRoomsInput');
    let minusRoomsBtn = $('#minusRoomsBtn');
    let plusRoomsBtn = $('#plusRoomsBtn');

    minusRoomsBtn.click(function () {
        let count = parseInt(countRoomsInput.val());
        count = calculateCounter(count, 'substract', 'room');
        countRoomsInput.val(count);
        generateRoomSelects(count);
    });

    plusRoomsBtn.click(function () {
        let count = parseInt(countRoomsInput.val());
        count = calculateCounter(count, 'add', 'room');
        countRoomsInput.val(count);
        generateRoomSelects(count);
    });

    // Contadores (2): Adults
    countAdultsInput = $('#countAdultsInput');
    let minusAdultsBtn = $('#minusAdultsBtn');
    let plusAdultsBtn = $('#plusAdultsBtn');

    minusAdultsBtn.click(function () {
        let count = parseInt(countAdultsInput.val());
        count = calculateCounter(count, 'substract', 'adult');
        countAdultsInput.val(count);
    });

    plusAdultsBtn.click(function () {
        let count = parseInt(countAdultsInput.val());
        count = calculateCounter(count, 'add', 'adult');
        countAdultsInput.val(count);
    });

    // Contadores (3): Children
    countChildrenInput = $('#countChildrenInput');
    let minusChildrenBtn = $('#minusChildrenBtn');
    let plusChildrenBtn = $('#plusChildrenBtn');

    minusChildrenBtn.click(function () {
        let count = parseInt(countChildrenInput.val());
        count = calculateCounter(count, 'substract', 'child');
        countChildrenInput.val(count);
        generateAgeSelects(count);
    });

    plusChildrenBtn.click(function () {
        let count = parseInt(countChildrenInput.val());
        count = calculateCounter(count, 'add', 'child');
        countChildrenInput.val(count);
        generateAgeSelects(count);
    });

    const calculateCounter = (counter, operation = 'add', field) => {

        let value = 0;
        // TODO: documentar
        if (operation === 'add') {
            if (counter < MAXIMUM_COUNTER) {
                value = counter + 1;
            } else {
                value = counter;
            }
        } else {

            // Perform substract
            if (counter >= 1) {
                value = counter - 1;
            }
        }

        // 
        if (field == 'room' && value == 1) {
            $("#minusRoomsBtn").attr("disabled", true);
            value = 1;
        } else {
            if (field == 'room' && value != 1) {
                $("#minusRoomsBtn").attr("disabled", false);
            }
        }

        if (field == 'adult' && value == 1) {
            $("#minusAdultsBtn").attr("disabled", true);
            value = 1;
        } else {
            if (field == 'adult' && value != 1) {
                $("#minusAdultsBtn").attr("disabled", false);
            }
        }

        if (field == 'child' && value == 0) {
            $("#minusChildrenBtn").attr("disabled", true);
        } else {
            if (field == 'child' && value != 0) {
                $("#minusChildrenBtn").attr("disabled", false);
            }
        }

        // Max counter
        if (value === MAXIMUM_COUNTER && field == 'room') {
            $("#plusRoomsBtn").attr("disabled", true);
        } else {
            if (value != MAXIMUM_COUNTER && field == 'room') {
                $("#plusRoomsBtn").attr("disabled", false);
            }
        }

        if (value === MAXIMUM_COUNTER && field == 'adult') {
            $("#plusAdultsBtn").attr("disabled", true);
        } else {
            if (value != MAXIMUM_COUNTER && field == 'adult') {
                $("#plusAdultsBtn").attr("disabled", false);
            }
        }

        if (value === MAXIMUM_COUNTER && field == 'child') {
            $("#plusChildrenBtn").attr("disabled", true);
        } else {
            if (value != MAXIMUM_COUNTER && field == 'child') {
                $("#plusChildrenBtn").attr("disabled", false);
            }
        }

        return value;
    }

});


/** Area Calendar **/
function initCalendar(document) {

    var calendarEl = document.getElementById('calendar');
    $(".calendarView").LoadingOverlay("show");
    calendar = new FullCalendar.Calendar(calendarEl, {
        events: function (info, successCallback, failureCallback) {
            idRoomSelect = $("#roomSelectRoom").val() != '' ? $("#roomSelectRoom").val() : 0;
            dateCalendarCurrent = info ? info.startStr.toString().substring(0, 10) : moment().format('YYYY-MM-DD');
            $.ajax({
                url: '/Calendar/GetEvents?idRoom=' + idRoomSelect + "&dateCalendar=" + dateCalendarCurrent,
                dataType: 'json',
                type: 'GET',
                failure: function (err) {
                    $(".calendarView").LoadingOverlay("hide");
                    failureCallback(err);
                },
                success: function (response) {
                    idRoomSelect = $("#roomSelectRoom").val() != '' ? $("#roomSelectRoom").val() : 0;
                    dateCalendarCurrent = calendar.getDate().toISOString().slice(0, 10);
                    $(".calendarView").LoadingOverlay("hide");
                    successCallback(response);
                }
            });
        },
        locale: 'es',
        initialView: 'dayGridMonth',
        headerToolbar: {
            left: 'prev,next',
            center: 'title',
            right: 'dayGridMonth,dayGridWeek,dayGridDay'
        },
        selectable: true,
        editable: true,
        droppable: true,
        eventClick: function (info) {
            // Obtener el evento seleccionado
            var event = info.event;

            //Resetea los pasos o tabs de la reserva x default
            resetTabsView();

            // Abrir la modal con el formulario de edición y mostrar los detalles del evento si permite edicion
            if (event.allow)
                abrirModalEdicion(event);
        },
        eventDrop: function (info) {
            // Obtener el evento arrastrado a una nueva posición en el calendario
            var event = info.event;

            // Actualizar la fecha y hora del evento en la base de datos
            actualizarEvento(event);
        },
        eventResize: function (info) {
            // Obtener el evento cuyo tamaño ha cambiado
            var event = info.event;

            // Actualizar la duración del evento en la base de datos
            actualizarEvento(event);
        },
        eventDidMount: function (info) {
            if (info.event.extendedProps.desc1 != '' && info.event.extendedProps.desc1 != null) 
                $(info.el).find(".fc-event-title").append("<br/>-" + info.event.extendedProps.desc1);
            if (info.event.extendedProps.desc2 != '' && info.event.extendedProps.desc2 != null) 
                $(info.el).find(".fc-event-title").append("<br/>-" + info.event.extendedProps.desc2);
            if (info.event.extendedProps.desc3 != '' && info.event.extendedProps.desc3 != null) 
                $(info.el).find(".fc-event-title").append("<br/>-" + info.event.extendedProps.desc3);
            if (info.event.extendedProps.desc4 != '' && info.event.extendedProps.desc4 != null) 
                $(info.el).find(".fc-event-title").append("<br/>-" + info.event.extendedProps.desc4);

            //Permite mostrar un tooltip sobre cada evento creado
            var tooltip = new Tooltip(info.el, {
                title: info.event.title,
                placement: 'top',
                trigger: 'hover',
                container: 'body'
            });  
        },
        selectOverlap: function (event) {
            //Validar si ya existe un evento creado en el dia seleccionado para no permitir crear más eventos
            return event.id == null || event.id == undefined;
        },
        select: function (info) {
            // Obtener las fechas y horas seleccionadas en el calendario
            const data = {};
            data.start = info.start;
            data.end = info.end;
           
            //Resetea los pasos o tabs de la reserva x default
            resetTabsView();

            const today = new Date();
            today.setHours(0, 0, 0, 0);
            // Abrir la modal con el formulario de creación y mostrar las fechas y horas seleccionadas Si es mayor a hoy
            if (data.start >= today)
                MostrarModal(data, null);
        },

    });
    calendar.render();
}

function selectRoom() {
    idRoomSelect = $("#roomSelectRoom").val() != '' ? $("#roomSelectRoom").val() : 0;
    initCalendar(document);
}

/**  Funciones que utilizan los counters al pulsarlos **/

function generateAgeOptions() {
    let ageOptions = '';
    for (let age = 0; age <= 17; age++) {
        ageOptions += `<option value="${age}">${age} años</option>`;
    }
    return ageOptions;
}

function generateAgeSelects(countChildren) {
    let $ageSelectContainer = $("#ageSelectContainer");
    $ageSelectContainer.empty();

    for (let i = 1; i <= countChildren; i++) {
        let ageSelectHtml = `
                <div class="col-md-6 py-2">
                    <label for="childAge${i}" class="form-label">Edad del Niño ${i}</label>
                    <select class="form-select form-control" id="childAge${i}" required>
                        <option value="">Elige...</option>
                        ${generateAgeOptions()}
                    </select>
                </div>
            `;
        $ageSelectContainer.append(ageSelectHtml);
    }
}

function generateRoomSelects(countRooms) {
    let $roomSelectContainer = $("#roomSelectContainer");
    let currentSelects = $roomSelectContainer.find("select").length;

    if (countRooms > currentSelects) {
        for (let i = currentSelects + 1; i <= countRooms; i++) {
            let roomSelectHtml = `
                <div class="col-md-12 py-2">
                    <label for="roomSelect${i}" class="form-label">Habitación ${i}</label>
                    <select class="form-select form-control" id="roomSelect${i}" required>
                        <option value="">Elige...</option>
                        <!-- Opciones de habitaciones (mismo contenido que el select por defecto) -->
                    </select>
                </div>
            `;
            $roomSelectContainer.append(roomSelectHtml);

            loadRoomOptions($(`#roomSelect${i}`));
        }

    } else if (countRooms < currentSelects) {
        $roomSelectContainer.find(`div:gt(${countRooms - 1})`).remove();
    }
}

/**  Funciones automaticas que filtran habitaciones con las fechas del Calendar  **/

$('input[name="daterange"]').on('hide.daterangepicker', function () {

    var dateRange = $('input[name="daterange"]').val();
    var dates = dateRange.split(" - ");

    checkIn = dates[0];
    checkOut = dates[1];

    loadRoomOptions($("#roomSelect1"));
});

$("#btnAbrirModalCreacion").click(function () {
    MostrarModal();
});

/**  Area Tabs del step by step  **/

var currentTab = 0;
showTab(currentTab);

function showTab(n) {
    // This function will display the specified tab of the form...
    var x = document.getElementsByClassName("tab");
    x[n].style.display = "block";
    //... and fix the Previous/Next buttons:
    if (n == 0) {
        document.getElementById("prevBtn").style.display = "none";
    } else {
        document.getElementById("prevBtn").style.display = "inline";
    }
    if (n == (x.length - 1)) {
        document.getElementById("nextBtn").innerHTML = "Reservar";
    } else {
        document.getElementById("nextBtn").innerHTML = "<i class='fas fa-chevron-right'></i> Siguiente";
    }
    //... and run a function that will display the correct step indicator:
    fixStepIndicator(n)
}

function fixStepIndicator(n) {
    // This function removes the "active" class of all steps...
    var i, x = document.getElementsByClassName("step");
    for (i = 0; i < x.length; i++) {
        x[i].className = x[i].className.replace(" active", "");
    }
    //... and adds the "active" class on the current step:
    x[n].className += " active";
}

function skipCompanion(n) {
    skipCompanionsForm = true;
    nextPrev(n);
}

function nextPrev(n) {

    var x = document.getElementsByClassName("tab");
    if (n == 1 && !validateForm(n)) return false;

    x[currentTab].style.display = "none";

    currentTab = currentTab + n;

    if (currentTab == x.length - 3) {
        setSelectsRooms();
    }
     if (currentTab == x.length - 2) {

        setCompanions();
        getPrices();
        loadStatusBookOptions();
    }
     if (currentTab >= x.length - 1) {
        skipCompanionsForm = false;
    }
     if (currentTab >= x.length) {

        sendBook();
        return false;
    }
    showTab(currentTab);
}

function validateForm(step) {
    var x, y, i, valid = true;

    if (skipCompanionsForm) {

        valid = true;

        //toastr.warning("", "Debe ingresar la cantidad de huéspedes para al menos una habitación.");

    } else {

        x = document.getElementsByClassName("tab");
        y = x[currentTab].getElementsByTagName("input");

        for (i = 0; i < y.length; i++) {

            if (y[i].value == "") {
                y[i].className += " invalid";
                valid = false;
            }
        }
    }
    if (valid) {
        document.getElementsByClassName("step")[currentTab].className += " finish";
    }
    return valid;
}

function resetTabsView() {
    var i, x = document.getElementsByClassName("tab");
    for (i = 0; i < x.length; i++) {
        x[i].style.display = "none";
    }
    currentTab = 0;
    showTab(currentTab);
}

/**  Area Formularios de Acompananates los cuales se pueden saltar  **/

function setCompanions() {
    var numAcompanantes = parseInt($("#NumberCompanions").val());

    if (formsCreated < 1) {

        for (var i = 1; i <= numAcompanantes; i++) {
            var nuevoForm = $("<div class='row childs'><div class= 'col-sm-12 p-3' >");
            nuevoForm.append(`
<div><h6 class='m-0 font-weight-bold'>Acompanante</h6>
        </div><div class="collapseCompanion mt-3 rounded border p-3">

          <form class="row g-3">
            <div class="col-md-4">
              <label for="DocumentType" class="form-label">Tipo Identificacion</label>
              <select id="DocumentType" class="form-select form-control" required>
                <option value="CC" selected>CC</option>
                <option value="TI">TI</option>
                <option value="CE">CE</option>
                <option value="PS">Pasaporte</option>
                <option value="DNT">DNI</option>
              </select>
            </div>
            <div class="col-md-6">
              <label for="Document" class="form-label">N. Identificacion</label>
              <input type="text" class="form-control" id="Document" required>
            </div>
            <div class="col-md-2">
                <label for="IdRoom" class="form-label">Habitación</label>
              <select class="form-select form-control input-room" id="IdRoom" required>
                <option selected>Elige...</option>
                ${rooms.map(room => `<option value="${room.id}" data-price="${room.price}" >${room.room}</option>`).join("")}
              </select>
            </div >
            <div class="col-md-6">
              <label for="Name" class="form-label">Nombres</label>
              <input type="text" class="form-control" id="Name" required>
            </div>
            <div class="col-md-6">
              <label for="LastName" class="form-label">Apellidos</label>
              <input type="text" class="form-control" id="LastName" required>
            </div>
            <div class="col-md-2">
             <label for="Age" class="form-label">Edad</label>
              <input type="number" class="form-control" id="Age" required>
              </div>
            <div class="col-md-5">
             <label for="RecidenceCity" class="form-label">Ciudad de Residencia</label>
              <input type="text" class="form-control" id="RecidenceCity" required>
              </div>
            <div class="col-md-5">
               <label for="OriginCity" class="form-label">Ciudad de Procedencia</label>
               <input type="text" class="form-control" id="OriginCity" required>
            </div>
                <input type="hidden" id="IsMain" value="0">
          </form>`);

            $("#masterDetail").append(nuevoForm);

            formsCreated = i;
        }
    }
    else {
        $(".childs .col-sm-12").empty();

    }

};

function setSelectsRooms() {
    $('#IdRoom').empty();
    $("#roomSelect1 option").each(function (index, option) {
        var $option = $(option);
        var id = $option.val();
        var text = $option.text();
        var price = $option.data('price');

        $('#IdRoom').append($('<option>', {
            value: id,
            text: text
        }).data('price', price));
    });
    if (idRoomSelect != 0 && idRoomSelect != null)
       $('#IdRoom').val(idRoomSelect);

    $("#NumberCompanions").val(parseInt(countChildrenInput.val()) + parseInt(countAdultsInput.val()) - 1).prop('readonly', true);

}

/**  Area Cargue de Selects de Rooms con Ajax  **/

function loadRoomOptions($select) {
    var RequestRooms = {};

    RequestRooms.CheckIn = checkIn != null ? checkIn : new Date();
    RequestRooms.CheckOut = checkOut != null ? checkOut : new Date();
    RequestRooms.CkeckDates = false;
    $.ajax({
        url: '/Booking/GetRooms',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(RequestRooms),
        success: function (data) {
            $select.empty();

            data.forEach((item) => {
                if ($select[0].id != "roomSelectRoom") {
                    $select.append(
                        $("<option>").val(item.idRoom).text(item.number).data("additional-info", item.price));
                }      
                else {
                    $select.append(
                        $("<option>").val(item.idRoom).text(item.number + " - " + item.roomTitle + " - " +  item.categoryName));
                }
            });
            if (idRoomSelect != 0 && idRoomSelect != null)
                $($select).val(idRoomSelect);
        },
        error: function (error) {
            console.log(error);
        }
    });

}

/**  Area Cargue de Select de Levels  **/
function loadLevelOptions($select) {
    $('#roomSelectLevel').empty();
    fetch("/Level/Lista")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    $("#roomSelectLevel").append(
                        $("<option>").val(item.idLevel).text(item.levelName)
                    )
                })
            }
        });
}

function loadStatusBookOptions($select) {
    $('#statusBookSelect').empty();
    fetch("/Booking/GetBookStatusList")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            var defaultValue = null;
            if (responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    if (defaultValue == null) defaultValue = item.idBookStatus;
                    $("#statusBookSelect").append(
                        $("<option>").val(item.idBookStatus).text(item.statusName)
                    )
                });
                $("#statusBookSelect").val(defaultValue);
            }
        });
}

function updateRoomCount() {
    let count = parseInt($("#countRoomsInput").val());
    generateRoomSelects(count);
}

/**  Area Calculos de Precios  **/

function getPrices() {

    var countAdult = parseInt(countAdultsInput.val());
    adults = 'A,'.repeat(countAdult).slice(0, -1); // Agregar comas y luego eliminar la última coma
    const selectsAge = document.querySelectorAll("#ageSelectContainer select");

    selectsAge.forEach((select) => {

        const selectedValue = select.value;

        if (ageChildren !== "") {
            ageChildren += ",";
        }
        ageChildren += selectedValue;
    });

    const selects = document.querySelectorAll("#roomSelectContainer select");

    tempRoom = [];

    selects.forEach((select, index) => {
        const selectedOption = select.options[select.selectedIndex];
        const idRoom = parseInt(selectedOption.value);
        const numberRoom = selectedOption.text;

        const tempRoomDTO = {
            Number: numberRoom,
            IdRoom: idRoom
        };

        tempRoom.push(tempRoomDTO);
    });

    const Model = {
        Adults: adults,
        AgeChildren: ageChildren,
        TempRoom: tempRoom,
        CheckIn: checkIn,
        CheckOut: checkOut
    };

    var data = JSON.stringify(Model);

    $.ajax({
        url: '/Booking/GetRangePrices',
        type: 'POST',
        contentType: 'application/json',
        data: data,
        success: function (data) {
            $("#priceSelectedRoom").empty();
            totalRoom = 0;
            $("#priceSelectedRoom").val(data);
            totalRoom = data;
            getExtraServices();
        },
        error: function (error) {
            console.log(error);
        }
    });
}

/**  Area Modal  **/

function MostrarModal(modelo = MODELO_BASE, book) {

    checkIn = getFormattedDate(modelo.start);
    checkOut = getFormattedDate(modelo.end);

    updateRoomCount(); // todo genera un error validar

    $("#cboBuscarCliente").select2('open');

    if (book != null) {
        uniqueIdRooms = [];
        let adultosCount = book.adults.split(',').length;
        let ageChildrenCount = book.ageChildren ? book.ageChildren.split(',').length : 0;
        let detailBookList = book.detailBook;
        detailBookList.forEach(detailBook => {
            uniqueIdRooms.push(detailBook.idRoom);
        });

        $("#countAdultsInput").val(adultosCount);
        $("#countChildrenInput").val(ageChildrenCount);
        var cantidadHabitaciones = uniqueIdRooms.length;
        $("#countRoomsInput").val(cantidadHabitaciones);
    }

    if (book == null) {
        loadRoomOptions($("#roomSelect1")) 
        //TODO: Mapear campos de la reserva existente
        //$("#DocumentType").val(book.documentType);........
    }
    else 
        generateRoomSelects(cantidadHabitaciones);

    $("#daterange").val(checkIn + " - " + checkOut);
    $("#modalCreacion").modal("show");
}

/**  Area Utilidades  **/

function getFormattedDate(date) {

    var year = date.getFullYear();
    var month = String(date.getMonth() + 1).padStart(2, '0'); // El mes empieza en 0, por eso se le suma 1
    var day = String(date.getDate()).padStart(2, '0');

    var formattedDate = year + '-' + month + '-' + day;
    return formattedDate;
}

function abrirModalCreacion(start, end) {

}

/**  Area Funciones de eventos del Calendar  **/

function abrirModalEdicion(event) {

    //if ($(this).closest("tr").hasClass("child")) {
    //    filaSeleccionada = $(this).closest("tr").prev();
    //} else {
    //    filaSeleccionada = $(this).closest("tr");
    //}

    //const data = tablaData.row(filaSeleccionada).data();

    // Cargar las habitaciones para el nivel seleccionado
    $.ajax({
        url: '/Booking/getBookbyId',
        type: 'GET',
        data: {
            eventId: event.id
        },
        dataType: 'json',
        success: function (response) {

            const book = response.data;

            var rr = event;
            console.log(event);
            MostrarModal(event, book[0]);

        },
        error: function () {
            // Manejar el error en caso de que falle la solicitud
        }
    });


}

function actualizarEvento(event) {
    // Lógica para realizar una llamada al controlador y actualizar el evento en la base de datos
    // Puedes usar AJAX o fetch para enviar la solicitud al controlador
    // Recuerda manejar las respuestas del controlador y actualizar el calendario en consecuencia
}

/**  Area Guardar / Editar Booking  **/

$("#btnGuardar").click(function () {

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
    modelo["idProducto"] = parseInt($("#txtId").val());
    modelo["codigoBarra"] = $("#txtCodigoBarra").val();
    modelo["marca"] = $("#txtMarca").val();
    modelo["descripcion"] = $("#txtDescripcion").val();
    modelo["idCategoria"] = $("#cboCategoria").val();
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

function sendBook() {

    let formValid = true;

    swal({
        title: 'Estas seguro de enviar la reserva?',
        text: "Confirmacion",
        icon: 'info',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si, Enviar!'
    },
        function (respuesta) {
            if (respuesta) {
                $("#modalCreacion").LoadingOverlay("show");

                let movement = {};
                let data = {
                    "Movement": {},
                    "Book": {},
                    "Guests": []
                };

                // movement.IdTipoDocumentoMovimiento = $("#cboTipoDocumentoMovimiento").val();
                movement.DocumentoCliente = $("#Document").val();
                movement.NombreCliente = $("#Name").val() + ' ' + $("#LastName").val();

                var totalSpanText = $("#totalSpan").text();
                // var totalNumber = parseFloat(totalSpanText.replace(/[^\d.-]/g, ""));

                // movement.SubTotal = parseFloat(totalSpanText.replace(/[^\d.-]/g, ""));
                //movement.TotalRoom = $("#totalRoomSpan").text();
                //movement.ImpuestoTotal = $("#impuestoValueSpan").text();
                movement.Total = parseFloat(totalSpanText.replace(/[^\d.-]/g, ""));

                var book = {};

                book.Reason = $("#Reason").val();
                book.CheckIn = checkIn;
                book.CheckOut = checkOut;
                book.Adults = adults;
                book.AgeChildren = ageChildren;
                book.IdBookStatus = $("#statusBookSelect option:selected").val();

                var guest = {};

                guest.DocumentType = $("#DocumentType").val();
                guest.Document = $("#Document").val();
                guest.IdRoom = $("#IdRoom").val();
                guest.Room = $("#IdRoom option:selected").text();
                guest.Price = $("#IdRoom option:selected").data('price');
                guest.Name = $("#Name").val();
                guest.LastName = $("#LastName").val();
                guest.RecidenceCity = $("#RecidenceCity").val();
                guest.OriginCity = $("#OriginCity").val();
                guest.NumberCompanions = $("#NumberCompanions").val();
                guest.IsMain = $("#IsMain").val();
                //guest.Age = $("#Age").val();
                guest.Email = $("#Email").val();
                guest.PhoneNumber = $("#PhoneNumber").val();

                data.Guests.push(guest);



                data.Movement = movement;
                data.Book = book;

                fetch("/Booking/SaveBook", {
                    method: "POST",
                    headers: { "Content-type": "application/json; charset=utf-8" },
                    body: JSON.stringify(data),
                })
                    .then(response => {
                        $("#modalCreacion").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {

                            roomsParaMovimiento = [];
                            establishmentParaPedido = [];

                            $("#regForm")[0].reset(); /// todo refrescar formulario
                            $('#modalCreacion').modal('hide');
                            calendar.refetchEvents();


                            // $("#cboTipoDocumentoMovimiento").val($("#cboTipoDocumentoMovimiento option:first").val())
                            //$("#txtSubTotal").val("");
                            //$("#txtIGV").val("");
                            //$("#txtTotal").val("");
                            swal("Registrado", `Numero de Reserva:${responseJson.objeto.movement.numeroMovimiento}  `, "success")
                            $("#modalCreacion").LoadingOverlay("hide");
                            
                        } else {
                            $("#modalCreacion").LoadingOverlay("hide");
                            swal("Error", responseJson.mensaje, "error")

                        }
                    });
            }
           currentTab = currentTab - 1;
        });
    //  }
};

/**  Area Eliminar Booking  **/

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
                //$(".showSweetAlert").LoadingOverlay("show");

                fetch(`/Producto/Eliminar?idProducto=${data.idProducto}`, {
                    method: "DELETE",
                })
                    .then(response => {
                        //$(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
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

/**  Area Servicios adicionales y counters **/

function getExtraServices() {
    $.ajax({
        url: '/Booking/GetServices',
        type: 'GET',
        datatype: "json",
        success: function (data) {
            loadServices(data);
        },
        error: function () {
            console.log('Error al cargar los servicios');
        }
    });
};

function loadServices(services) {
    const servicesContainer = $('#services-container');
    servicesContainer.empty();

    for (const service of services.data) {
        const serviceHTML = `
            <div class="col-md-4 services-parent">
                <div class="services-child p-2">
                    <div class="row">
                        <div class="col-md-12">
                            <span class="font-weight-bold">${service.serviceName}</span>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <span class="card-service__per-unit">${service.serviceInfoQuantity}</span>
                        </div>
                    </div>
                    <div class="row form__selector">
                        <div class="col-md-6">
                            <div class="card-service-details__counter-label pt-2">
                                ${service.servicePrice}
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="counter counter-section" data-idservice="${service.idService}">
                                <button type="button" id="minusServicesBtn" class="button button-secondary button--more minusBtn"
                                    data-idservice="${service.idService}" data-price="${service.serviceMaximumAmount}">
                                    -
                                </button>
                                <input type="text" id="countServicesInput" name="countServicesInput" class="form-control text-center input-counter countInput"
                                    value="0" readonly tabindex="-1" data-idservice="${service.idService}"
                                    data-maxservice="${service.serviceMaximumAmount}" data-price="${service.servicePrice}">
                                <button type="button" id="plusServicesBtn" class="button button-secondary button--more plusBtn"
                                    data-idservice="${service.idService}" data-maxservice="${service.serviceMaximumAmount}"
                                    data-price="${service.servicePrice}">
                                    +
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;
        servicesContainer.append(serviceHTML);
    }

    /**  Area counters Services y Calculos **/

    var objetoGuardado = {
        totalPorServicio: 0,
        total: 0,
        totalValue: 0,
        servicios: [],
    };

    //var totalServices = 0;

    // Valor total
    const priceSelectedRoom = $("#priceSelectedRoom").val();
    $("#totalSpan").text('$ ' + numberWithCommas(priceSelectedRoom) + ' COP');
    $("#totalRoomSpan").text('$ ' + numberWithCommas(priceSelectedRoom) + ' COP');

    $('#countServicesInput').val(0);
    $('#countAdultsInput').val(0);
    $('#countChildrenInput').val(0);

    function numberWithCommas(x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }

    function actualizarObjetoServicios(id, operacion, price) {
        const servicioEncontrado = objetoGuardado.servicios.find(servicio => servicio.id === id);

        if (operacion === "add") {
            if (servicioEncontrado) {
                servicioEncontrado.cont++;
                servicioEncontrado.value = parseFloat(servicioEncontrado.value) + parseFloat(price);
            } else {
                objetoGuardado.servicios.push({
                    id: id,
                    cont: 1,
                    value: price
                });
            }
        } else if (operacion === "substract") {
            if (servicioEncontrado) {

                if (servicioEncontrado.cont === 1) {
                    // Si la propiedad 'cont' es 1, quitar el objeto del array 'servicios'
                    objetoGuardado.servicios = objetoGuardado.servicios.filter(servicio => servicio.id !== id);
                } else {
                    // Restar en 1 la propiedad 'cont' del servicio encontrado
                    servicioEncontrado.cont--;
                }
            }
        }

        const totalPorServicio = objetoGuardado.servicios.length;
        objetoGuardado.totalPorServicio = totalPorServicio;

        let total = 0;
        let totalValue = 0;
        objetoGuardado.servicios.forEach(element => {
            if (element.cont) {
                total = total + element.cont;
            }
            if (element.value) {
                totalValue = totalValue + parseFloat(element.value);
            }
        });
        objetoGuardado.total = total;
        objetoGuardado.totalValue = totalValue;

        const priceSelectedRoom = $("#priceSelectedRoom").val();
        const totalToPay = totalValue + parseFloat(priceSelectedRoom);

        $("#serviceValueSpan").text('$ ' + totalValue + ' COP');
        $("#totalPriceServices").val(totalValue);
        $("#totalSpan").text('$ ' + numberWithCommas(totalToPay) + ' COP');
        totalServices = totalValue;
    }

    $(".minusBtn").on("click", function () {

        var input = $(this).siblings(".countInput");
        var contador = parseInt(input.val());
        if (contador > 0) {
            input.val(contador - 1);
        }

        let idBuscado = $(this).data('idservice');
        let price = $(this).data('price');

        actualizarObjetoServicios(idBuscado, 'substract', price);
    });

    // Evento para el botón Restar
    $(".plusBtn").on("click", function () {

        var input = $(this).siblings(".countInput");
        var contador = parseInt(input.val());
        let maximumService = $(this).data('maxservice');

        if (contador < maximumService) {
            input.val(contador + 1);

            let idBuscado = $(this).data('idservice');
            let price = $(this).data('price');

            actualizarObjetoServicios(idBuscado, 'add', price);
        }

    });

    const descuentoSelect = $('#Descuento');
    const cobroExtraInput = $('#CobroExtra');
    const adelantoInput = $('#Adelanto');
    const saldoTotalSpan = $('#totalSpan');
    const observacionesTextArea = $('#w3review');

    // Función para actualizar el saldo total
    function updateSaldoTotal() {
        // Obtener el precio seleccionado de la habitación
        let saldoTotal = parseFloat($('#priceSelectedRoom').val());

        // Validar que los campos sean números válidos
        const descuentoPorcentaje = parseFloat(descuentoSelect.val()) || 0;
        const cobroExtra = parseFloat(cobroExtraInput.val()) || 0;
        const adelanto = parseFloat(adelantoInput.val()) || 0;

        // Calcular descuento
        const descuento = (saldoTotal * descuentoPorcentaje) / 100;
        saldoTotal -= descuento;

        // Sumar cobro extra
        saldoTotal += cobroExtra;

        // Restar adelanto
        saldoTotal -= adelanto;

        // Actualizar saldo total en el HTML
        saldoTotalSpan.text('$ ' + saldoTotal.toFixed(2));

        // Habilitar o deshabilitar observaciones
        if (descuentoPorcentaje > 0 || cobroExtra > 0 || adelanto > 0) {
            observacionesTextArea.prop('required', true);
        } else {
            observacionesTextArea.prop('required', false);
        }
    }

    // Eventos de cambio en los campos
    descuentoSelect.change(updateSaldoTotal);
    cobroExtraInput.on('input', updateSaldoTotal);
    adelantoInput.on('input', updateSaldoTotal);

}