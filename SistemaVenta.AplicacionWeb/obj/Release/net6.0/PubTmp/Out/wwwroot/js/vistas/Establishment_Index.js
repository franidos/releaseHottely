let indexImg = 0;

$(document).ready(function () {
    $(".card-body").LoadingOverlay("show");

    /*ini images*/
    $("#input-images").sortable({
        axis: "x",
        handle: "img",
        update: function (event, ui) {
            $("#input-images li").each(function (index) {
                $(this).attr("id", index + 1);
            });
        }
    });

    $("#file-input").on("change", function () {
        var files = this.files;
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            renderImage(file);
        }

    });


    $("#input-images").on("click", ".delete-button", function (event) {
        event.preventDefault();
        event.stopPropagation();
        $(this).parent().parent().hide();
    });

    function renderImage(file) {
        var reader = new FileReader();
        reader.onload = function (e) {

            indexImg++;
            $("#input-images").append('<li id="' + indexImg + '"><div><img src="' + e.target.result + '"><button class="delete-button" data-id="' + file.name + '">X</button></div></li>');
        };
        reader.readAsDataURL(file);
    }
    /*fin imagnes*/

    //handleImages();

    fetch("/Establishment/Obtener")
        .then(response => {
            $(".card-body").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {

            if (responseJson.estado) {
                const o = responseJson.objeto

                $("#txtNumeroDocumento").val(o.nit)
                $("#txtRazonSocial").val(o.establishmentName)
                $("#txtCorreo").val(o.email)
                $("#txtContact").val(o.contact)
                $("#txtDireccion").val(o.address)
                $("#txTelefono").val(o.phoneNumber)
                $("#txtRnt").val(o.rnt)
                $("#txtToken").val(o.token)
                $("#txtCheckInTime").val(o.checkInTime)
                $("#txtCheckOutTime").val(o.checkOutTime)
                $("#txtEstablishmentType").val(o.establishmentType)
                $("#txtImpuesto").val(o.tax)
                $("#txtSimboloMoneda").val(o.currency)
                $("#imgLogo").attr("src", o.urlImage)

                o.imagesEstablishment.forEach((item, index) => {

                    indexImg = index + 1;
                    $("#input-images").append('<li id="' + indexImg + '"><div><img src="' + item.urlImage + '"><button class="delete-button" data-id="' + item.imageNumber + '">X</button></div></li>');

                });


            } else {
                swal("Lo sentimos!", responseJson.mensaje, "error")
            }
        })
});


$("#btnGuardarCambios").click(function () {
    //Validaciones
    const inputs = $("input.input-validar").serializeArray();
    const inputs_sin_valor = inputs.filter((item) => item.value.trim() == "")

    if (inputs_sin_valor.length > 0) {
        const mensaje = `Desbes completar el campo "${inputs_sin_valor[0].name}"`;
        toastr.warning("", mensaje);
        $(`input[name = "${inputs_sin_valor[0].name}"]`).focus();
        return;
    }

    const modelo = {
        nit: $("#txtNumeroDocumento").val(),
        establishmentName: $("#txtRazonSocial").val(),
        contact: $("#txtContact").val(),
        address: $("#txtDireccion").val(),
        email: $("#txtCorreo").val(),
        phoneNumber: $("#txTelefono").val(),
        token: $("#txtToken").val(),
        rnt: $("#txtRnt").val(),
        establishmentType: $("#txtEstablishmentType").val(),
        tax: $("#txtImpuesto").val(),
        checkInTime: $("#txtCheckInTime").val(),
        checkOutTime: $("#txtCheckOutTime").val(),
        currency: $("#txtSimboloMoneda").val(),
    }

    const inputFoto = document.getElementById("txtLogo");
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

   // formData.append("oldImagesUrl", listImgs);


    formData.append("logo", inputFoto.files[0])
    formData.append("modelo", JSON.stringify(modelo))


    $(".card-body").LoadingOverlay("show");

    fetch("/Establishment/GuardarCambios", {
        method: "POST",
        body: formData,
    })
        .then(response => {
            $(".card-body").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const o = responseJson.objeto
                $("#imgLogo").attr("src", o.urlLogo)
                swal("Listo!", "Los cambios han sido guardados", "success")
            } else {
                swal("Lo sentimos!", responseJson.mensaje, "error")
            }
        })




});

