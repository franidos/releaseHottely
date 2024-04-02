let valorImpuesto = 0;
let valorImpuesto2 = 0;
let establishmentParaPedido = {};
let checkIn;
let checkOut;
let urlLogo = "";
let rooms = [];
let formsCreated = 0;

$(document).ready(function () {

    fetch("/Establishment/Obtener")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const d = responseJson.objeto;
                $("#inputGroupSubTotal").text(`Sub Total - ${d.currency}`)
                $("#inputGroupIGV").text(`IMP(${d.tax}%) - ${d.currency}`)
                $("#inputGroupTotal").text(`Total - ${d.currency}`)
                valorImpuesto = parseFloat(d.tax);
            }
        })

    fetch("/Movimiento/ListaTipoDocumentoMovimiento")
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    if (item.naturaleza == 'S') {
                        $("#cboTipoDocumentoMovimiento").append(
                            $("<option>").val(item.idTipoDocumentoMovimiento).text(item.descripcion)
                        )
                    }
                })
            }
        })

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

    //fetch("/Establishment/List")
    //    .then(response => {
    //        return response.ok ? response.json() : Promise.reject(response);
    //    })
    //    .then(responseJson => {
    //        if (responseJson.data.length > 0) {
    //            responseJson.data.forEach((item) => {
    //                $("#cboSearchEstablishment").append(
    //                    $("<option>").val(item.idEstablishment).text(item.establishmentName).data("additional-info", item.urlImage)
    //                )
    //            })
    //        }
    //    })
})

$('#btnNextRoom').on('click', function () {

    const additionalInfo = $("#cboSearchEstablishment option:selected");

    establishmentParaPedido = {
        idEstablishment: additionalInfo.val(),
        urlImage: additionalInfo.data("additional-info"),
        nombreEstablishment: additionalInfo.text(),
    }

   //updateLogo(establishmentParaPedido.urlImage);

    var dateRange = $('input[name="daterange"]').val();
    var dates = dateRange.split(" - ");

    var RequestRooms = {};

    //RequestRooms.IdEstablishment = establishmentParaPedido.idEstablishment;
    RequestRooms.CheckIn = dates[0];
    RequestRooms.CheckOut = dates[1];
    RequestRooms.CkeckDates = true;

    $.ajax({
        url: '/Booking/GetRooms',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(RequestRooms),
        success: function (data) {
            mostrarRoom_Precios(data);
        },
        error: function (error) {
            console.log(error);
        }
    });
})
//function updateLogo(url) {
//    $('#imgLogoEstablishment').removeClass('d-none');
//    $('#imgLogoEstablishment img').attr('src', url);
//}
function limpiarEstablishment() {

    $("#tbEstablishment tbody").html("")

    establishmentParaPedido.forEach((item) => {
        $("#tbEstablishment tbody").append(
            $("<tr>").append(
                $("<td>").text(item.nitEstablishment),
                $("<td>").text(item.nombreEstablishment),
                $("<td>").text(item.descripcionEstablishment),
                $("<td>").text(item.cantidad),

            )
        )
    })

}

$(document).on("select2:open", function () {
    document.querySelector(".select2-search__field").focus();
})

let roomsParaMovimiento = [];

function mostrarRoom_Precios(Data) {
    $("#collapseEstablishment").collapse('hide');
    $("#collapseRoom").collapse('show');
    let total = 0;
    $("#tbRoom tbody").html("");

    Data.forEach((item) => {
        total = total + parseFloat(item.total);
        $("#tbRoom tbody").append(
            $("<tr>").attr("id", item.idRoom).append(
                $("<td>").addClass("td-number").text(item.number),
                $("<td>").text(item.roomTitle),
                $("<td>").text(item.capacity),
                $("<td>").append($("<input>").addClass("form-control input-price").val(item.price).prop('readonly', true)),
                $("<td>").append($("<input>").addClass("form-control input-cantidad")),
            )
        )
    });

    $("#tbRoom").DataTable({
        searching: true, 
        paging: true, 
        responsive: true, 
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
    actualizarTotal();
}

function actualizarTotal() {
    let iva = 0;
    let subTotal = 0;
    let porcentaje = valorImpuesto / 100;
    let subtotal2 = 0;

    $(".input-cantidad, .input-price").each(function () {
        const cantidad = parseFloat($(this).closest("tr").find(".input-cantidad").val());
        const precio = parseFloat($(this).closest("tr").find(".input-price").val());
        const subtotal = cantidad * precio;


        if (isNaN(subtotal)) {
            const prov = 0;
            $(this).closest("tr").find(".input-subtotal").val(prov.toFixed(2));
        } else {
            subtotal2 = subtotal2 + subtotal;
            $(this).closest("tr").find(".input-subtotal").val(subtotal.toFixed(2));
        }
    });

    var diffTime = checkOut.getTime() - checkIn.getTime();
    var daysBook = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    const tott = (subtotal2 / 2) * daysBook;

    subTotal = tott / (1 + porcentaje);
    iva = tott - subTotal;

    var dateRange = $('input[name="daterange"]').val();
    var dates = dateRange.split(" - ");
    var ci = dates[0];
    var co = dates[1];

    $("#txtEntrada").val(ci);
    $("#txtSalida").val(co);
    $("#txtSubTotal").val(tott.toFixed(2));
    $("#txtIGV").val(iva.toFixed(2));
    $("#txtTotal").val(tott.toFixed(2));
}


$('#btnNextGuest').click(function () {
    formsCreated = 0;
    var hasNumericValue = false;
    $('#tbRoom tbody tr').each(function () {
        var cantHuespedes = $(this).find('.input-cantidad').val();

        if (!isNaN(parseInt(cantHuespedes))) {
            hasNumericValue = true;
            return false;
        }
    });

    if (hasNumericValue) {

        actualizarTotal();
        $("#IdRoom").empty();

        $("#collapseEstablishment").collapse('hide');
        $("#collapseRoom").collapse('hide');
        $("#collapseGuest").collapse('show');

        let cantidadTot = 0;

        $(".input-cantidad").each(function () {

            let cantidad = 0;
            const idRoom = parseFloat($(this).closest("tr").attr("id"));
            const room = parseFloat($(this).closest("tr").find(".td-number").text());
            const price = parseFloat($(this).closest("tr").find(".input-price").val());
            cantidad = parseFloat($(this).closest("tr").find(".input-cantidad").val());

            if (cantidad >= 1) {

                const newRoom = { id: idRoom, room: room, price: price};
                rooms.push(newRoom);
                cantidadTot = cantidadTot + cantidad;
            }
        });

        $.each(rooms, function (index, value) {
            var option = $('<option>', {
                value: value.id,
                text: value.room
            });
            option.data('price', value.price);
            $('#IdRoom').append(option);
        });

        $("#NumberCompanions").val(cantidadTot - 1).prop('readonly', true);

        $("#btnNextCompanions").toggleClass('disabled', cantidadTot - 1 == 0);

    } else {
        // ninguna fila tiene un valor numérico
        toastr.warning("", "Debe ingresar la cantidad de huéspedes para al menos una habitación.");
    }
})

$("#collapseGuest button[type='submit']").click(function (e) {
    e.preventDefault();
    var numAcompanantes = parseInt($("#NumberCompanions").val());

    if (formsCreated < 1) {

        // Crear un formulario adicional para cada acompañante
        for (var i = 1; i <= numAcompanantes; i++) {
            // Crear un elemento div para cada formulario adicional
            var nuevoForm = $("<div class='row childs'><div class= 'col-sm-12' >");

            // Agregar el formulario adicional dentro del elemento div
            nuevoForm.append(`
<div class='card shadow mb-4'><div class='card-header py-3 bg-gradient-info'><h6 class='m-0 font-weight-bold text-white'>Acompanante</h6>
        </div><div class="collapse show collapseCompanion">
        <div class="card-body">
          <form class="row g-3">
            <div class="col-md-4">
              <label for="DocumentType" class="form-label">Tipo Identificacion</label>
              <select id="DocumentType" class="form-select form-control" required>
                <option selected>Elige...</option>
                <option value="CC">CC</option>
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
            <div class="col-md-6">
             <label for="RecidenceCity" class="form-label">Ciudad de Residencia</label>
              <input type="text" class="form-control" id="RecidenceCity" required>
              </div>
            <div class="col-md-6">
               <label for="OriginCity" class="form-label">Ciudad de Procedencia</label>
               <input type="text" class="form-control" id="OriginCity" required>
            </div>
                <input type="hidden" id="IsMain" value="0">
          </form>
        </div>`);

            $("#masterDetail").append(nuevoForm);

            formsCreated = i;
        }
    }
    else {
        $(".childs .col-sm-12").empty();

    }

});

$('#btnPrevRooms').click(function (e) {

    e.preventDefault();
    $(".childs").remove()

    $("#collapseGuest").collapse('hide');
    $("#collapseRoom").collapse('show');

})

$('#btnPrevEstablishment').click(function () {

    $("#collapseEstablishment").collapse('show');
    $("#collapseRoom").collapse('hide');
    $("#collapseGuest").collapse('hide');

})

$(".btnSendData").click(function (e) {
    let formValid = true;
    $("#collapseGuest, .collapseCompanion").each(function (index) {
        var $form = $(this).find("form");
        var formId = $(this).attr("id");
        var formName = "Huésped principal";
        if (formId !== "collapseGuest") {
            formName = "Acompañante " + (index);
        }
        var formFields = ["#DocumentType",
            "#Document",
            "#IdRoom",
            "#Name",
            "#LastName",
            "#RecidenceCity",
            "#OriginCity",
            "#Reason",
            "#NumberCompanions",
            "#IsMain"];

        // Validar que todos los campos del formulario estén llenos
        $form.find(formFields.join(",")).each(function (i, e) {
            if ($(this).val() === "") {
                alert(formName + ": Por favor, complete todos los campos." + e);
                formValid = false;
                return false;
            }
        });
    });

    if (!formValid) {
        return false;
    }
    else {
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
                    $(".showSweetAlert").LoadingOverlay("show")
                    //sendData();
                    //function sendData() {
                    let movement = {};
                    let data = {
                        "Guests": [],
                        "Book": {},
                        "Movement": {}
                    };

                    movement.IdTipoDocumentoMovimiento = $("#cboTipoDocumentoMovimiento").val();
                    movement.documentoCliente = $("#Document").val();
                    movement.NombreCliente = $("#Name").val() + ' ' + $("#LastName").val();
                    movement.SubTotal = $("#txtSubTotal").val();
                    movement.ImpuestoTotal = $("#txtIGV").val();
                    movement.Total = $("#txtTotal").val();
                    // movement.numeroDocumentoExterno = establishmentParaPedido.cantidad

                    var book = {};

                    book.Reason = $("#Reason").val();
                    book.CheckIn = checkIn;
                    book.CheckOut = checkOut;
                    book.IdEstablishment = establishmentParaPedido.idEstablishment;

                    $("#masterDetail, .collapseCompanion").each(function (index) {

                        var guest = {};

                        var $form = $(this).find("form");

                        guest.DocumentType = $form.find("#DocumentType").val();
                        guest.Document = $form.find("#Document").val();
                        guest.RoomId = $form.find("#IdRoom").val();
                        guest.Room = $form.find("#IdRoom option:selected").text();
                        guest.Price = $form.find("#IdRoom option:selected").data('price');
                        guest.Name = $form.find("#Name").val();
                        guest.LastName = $form.find("#LastName").val();
                        guest.RecidenceCity = $form.find("#RecidenceCity").val();
                        guest.OriginCity = $form.find("#OriginCity").val();
                        guest.NumberCompanions = $form.find("#NumberCompanions").val();
                        guest.IsMain = $form.find("#IsMain").val();

                        data.Guests.push(guest);
                    });

                    data.Movement = movement;
                    data.Book = book;

                    // console.log("data " + JSON.stringify(data));
                    //$(".sweet-alert  .showSweetAlert .visible").LoadingOverlay("show");

                    fetch("/Booking/SaveBook", {
                        method: "POST",
                        headers: { "Content-type": "application/json; charset=utf-8" },
                        body: JSON.stringify(data),
                    })
                        .then(response => {

                           // $(".sweet-alert .showSweetAlert  .visible").LoadingOverlay("show");
                            $(".showSweetAlert").LoadingOverlay("hide")
                            return response.ok ? response.json() : Promise.reject(response);
                        })
                        .then(responseJson => {
                            if (responseJson.estado) {

                                roomsParaMovimiento = [];
                                establishmentParaPedido = [];

                                mostrarRoom_Precios(roomsParaMovimiento);

                                $("#collapseEstablishment").collapse('show');
                                $(".childs").remove()
                                $("#frmMainGuest")[0].reset();
                                $("#collapseGuest").collapse('hide');
                                // mostrarProducto_Precios(productosParaMovimiento);
                                limpiarEstablishment()
                                $("#collapseRoom").collapse('hide');
                                $("#txtDocumentoCliente").val("");
                                $("#txtNombreCliente").val("");
                                $("#cboTipoDocumentoMovimiento").val($("#cboTipoDocumentoMovimiento option:first").val())
                                $("#txtSubTotal").val("");
                                $("#txtIGV").val("");
                                $("#txtTotal").val("");
                                //$(".sweet-alert  .showSweetAlert .visible").LoadingOverlay("hide")
                                swal("Registrado", `Numero de Reserva:${responseJson.objeto.movement.numeroMovimiento}  `, "success")
                                $(".showSweetAlert").LoadingOverlay("hide");

                            } else {
                                $(".showSweetAlert").LoadingOverlay("hide");
                                swal("Error", responseJson.mensaje, "error")

                            }
                        });
                }

            });
    }
});
