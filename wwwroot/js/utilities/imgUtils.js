function handleImages() {
    let indexImg = 0;

    $("#input-images").sortable({
        axis: "x",
        handle: "img",
        update: function (event, ui) {
            // Actualiza los identificadores en función del nuevo orden
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
        event.stopPropagation();
        $(this).parent().parent().remove();
    });

    function renderImage(file) {
        var reader = new FileReader();
        reader.onload = function (e) {
            indexImg++;
            $("#input-images").append('<li id="' + indexImg + '"><div><img src="' + e.target.result + '"><button class="delete-button" data-id="' + file.name + '">X</button></div></li>');
        };
        reader.readAsDataURL(file);
    }
}
