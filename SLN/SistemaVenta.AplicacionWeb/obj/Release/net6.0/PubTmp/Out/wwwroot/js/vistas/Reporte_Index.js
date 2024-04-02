let tablaData;

$(document).ready(function () {

    $.datepicker.setDefaults($.datepicker.regional['es'])
    $("#txtFechaInicio").datepicker({ dateFormat: "dd/mm/yy" })
    $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": "/Reporte/ReporteMovimiento?fechaInicio=01/01/1991&fechaFin=02/01/1991",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "fechaRegistro" },
            { "data": "numeroMovimiento" },
            { "data": "tipoDocumento" },
            { "data": "documentoCliente" },
            { "data": "nombreCliente" },
            { "data": "subTotalMovimiento" },
            { "data": "impuestoTotalMovimiento" },
            { "data": "totalMovimiento" },
            { "data": "producto" },
            { "data": "cantidad" },
            { "data": "precio" },
            { "data": "total" },
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                className: '',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Movimientos',
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

    let nuevaUrl = `/Reporte/ReporteMovimiento?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`

    tablaData.ajax.url(nuevaUrl).load();
})