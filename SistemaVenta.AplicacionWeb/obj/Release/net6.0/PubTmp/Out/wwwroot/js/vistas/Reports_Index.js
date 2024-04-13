let tablaData;

$(document).ready(function () {

    $.datepicker.setDefaults($.datepicker.regional['es'])
    $("#txtFechaInicio").datepicker({ dateFormat: "dd/mm/yy" })
    $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": "/Reports/BookingReport?fechaInicio=01/01/1991&fechaFin=02/01/1991",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "creationDate" },
            { "data": "bookNumber" },
            { "data": "documentType" },
            { "data": "guestDocument" },
            { "data": "guestName" },
            { "data": "subTotal" },
            { "data": "totalTax" },
            { "data": "totalBook" },
            { "data": "roomNumber" },
            { "data": "checkIn" },
            { "data": "checkOut" },
            { "data": "establishmentName" },
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                className: '',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Reservas',
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
})

$("#btnBuscar").click(function () {
    if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {
        toastr.warning("", "Debe ingresar fecha de inicio y fin");
        return;
    }

    let fechaInicio = $("#txtFechaInicio").val().trim();
    let fechaFin = $("#txtFechaFin").val().trim();

    let nuevaUrl = `/Reports/BookingReport?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`

    tablaData.ajax.url(nuevaUrl).load();
})