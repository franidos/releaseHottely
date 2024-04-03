
$(document).ready(function () {

    const MAXIMUM_COUNTER = 8;

    $('#countRoomsInput').val(1);
    $('#countAdultsInput').val(1);
    $('#countChildrenInput').val(0);

    $("#minusRoomsBtn").attr("disabled", true);
    $("#minusAdultsBtn").attr("disabled", true);
    $("#minusChildrenBtn").attr("disabled", true);

    // DatePicker
    $(function () {

        var fechaActual = new Date();
        var start = moment();
        var end = moment().add(1, 'days');
        $('input[name="daterange"]').daterangepicker({
            "startDate": start,
            "endDate": end,
            "minDate": fechaActual,
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
  

    // Seccion contadores
    // Contadores (1): Rooms
    let countRoomsInput = $('#countRoomsInput');
    let minusRoomsBtn   = $('#minusRoomsBtn');
    let plusRoomsBtn    = $('#plusRoomsBtn');

    minusRoomsBtn.click(function () {
        let count = parseInt(countRoomsInput.val());
        count = calculateCounter(count, 'substract', 'room');
        countRoomsInput.val(count);
    });

    plusRoomsBtn.click(function () {
        let count = parseInt(countRoomsInput.val());
        count = calculateCounter(count, 'add', 'room');
        countRoomsInput.val(count);
    });

    // Contadores (2): Adults
    let countAdultsInput = $('#countAdultsInput');
    let minusAdultsBtn = $('#minusAdultsBtn');
    let plusAdultsBtn = $('#plusAdultsBtn');
    var adults = "";
    var ageChildren = "";

    minusAdultsBtn.click(function () {
        let count = parseInt(countAdultsInput.val());
        count = calculateCounter(count, 'substract', 'adult');
        countAdultsInput.val(count);    
        calculateAdultChildren();
    });

    plusAdultsBtn.click(function () {
        let count = parseInt(countAdultsInput.val());
        count = calculateCounter(count, 'add', 'adult');
        countAdultsInput.val(count);
        calculateAdultChildren();
    });

    // Contadores (3): Children
    let countChildrenInput = $('#countChildrenInput');
    let minusChildrenBtn = $('#minusChildrenBtn');
    let plusChildrenBtn = $('#plusChildrenBtn');

    minusChildrenBtn.click(function () {
        let count = parseInt(countChildrenInput.val());
        count = calculateCounter(count, 'substract', 'child');
        countChildrenInput.val(count);
        generateAgeSelects(count);
        calculateAdultChildren();
    });

    plusChildrenBtn.click(function () {
        let count = parseInt(countChildrenInput.val());
        count = calculateCounter(count, 'add', 'child');
        countChildrenInput.val(count);
        generateAgeSelects(count);
         calculateAdultChildren();
        console.log("Hola + un niño");
    });

    //Calcula texto de Adultos y children
    function calculateAdultChildren(){
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
    }

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


    // Seleccion de Room
    $('.dynamic-button-room').click(function () {

        // Armar el link dinamico seleccion de room
        let idestabl = $(this).data('idestabl');
        let idroom = $(this).data('idroom');
        let checkin = $(this).data('checkin');
        let checkout = $(this).data('checkout');
        let nrooms = $(this).data('numrooms');
        let nadults = $(this).data('numadults');
        let nchildren = $(this).data('numchildren') === '' ? 0 : $(this).data('numchildren');


        // Parametros
        var url = '/MotorReservas/Servicios?idestablishment=' + idestabl + '&idroom=' + idroom + '&checkin=' + checkin +
            '&checkout=' + checkout + '&nrooms=' + nrooms + '&nadults=' + nadults + '&nchildren=' + nchildren;

        window.location.href = url;
    });

    // Modal de imagenes
    $('#imageModal2').on('show.bs.modal', function (event) {

        var button = $(event.relatedTarget);
        var roomId = button.data('roomid');

        $.ajax({
            url: "/MotorReservas/GetRoomImages",
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            data: { idRoom: roomId },

            success: function (data) {
                var carouselInner = $('.carousel-inner');
                carouselInner.empty();

                var firstImage = true;

                data.forEach(function (image, index) {
                    var activeClass = firstImage ? 'active' : '';
                    carouselInner.append('<div class="carousel-item ' + activeClass + '"><img src="' + image.urlImage + '" alt="' + image.nameImage + '" class="d-block w-100_ img-fluid" sizes="(max-width: 320px) 280px, (max-width: 480px) 440px, 800px,(max-width: 710px) 120px,(max-width: 991px) 193px,278px"></div>');
                    firstImage = false;
                });
            }
        });
    });


    // Imagenes establecimiento
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);
    const id = urlParams.get('establecimientoid');

    $.ajax({
        url: "/MotorReservas/GetEstablishmentImages",
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        data: { idEstablishment: id },

        success: function (data) {
            var carouselInner = $('.carousel-inner');
            carouselInner.empty();

            var firstImage = true;
            data.forEach(function (image, index) {
                var activeClass = firstImage ? 'active' : '';
                carouselInner.append('<div class="carousel-item ' + activeClass + '"><img src="' + image.urlImage + '" alt="' + image.nameImage + '" class="d-block w-100_ img-fluid" sizes="(max-width: 320px) 280px, (max-width: 480px) 440px, 800px,(max-width: 710px) 120px,(max-width: 991px) 193px,278px"></div>');
                firstImage = false;
            });
        }
    });

    //Servicios Info establecimiento
    $.ajax({
        url: "/MotorReservas/GetServicesInfoEstablishment",
        dataType: 'json',
        contentType: "application/json; charset=utf-8",
        data: { idEstablishment: id },
        success: function (data) {
            var list = $('.services-list');
            list.empty();
            data.forEach(function (d, index) {
                list.append(`<li><span><i class="fa ${d.icon}"></i>&nbsp;&nbsp;${d.descripcion}</span></li><br>`);
            });
           
        }
    });

    //// Age childern
    function generateAgeSelects(countChildren) {
        let $ageSelectContainer = $("#ageSelectContainer");
        $ageSelectContainer.empty();

        for (let i = 1; i <= countChildren; i++) {
            let ageSelectHtml = `
                <div class="col-md-12 py-2">
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

    /**  Funciones que utilizan los counters al pulsarlos **/
    function generateAgeOptions() {
        let ageOptions = '';
        for (let age = 0; age <= 17; age++) {
            ageOptions += `<option value="${age}">${age} años</option>`;
        }
        return ageOptions;
    }

    // submit
    $("#step1Form").submit(function (event) {
        // event.preventDefault();
    });
});