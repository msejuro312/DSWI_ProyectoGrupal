// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Validacion en vivo (verde/rojo) segun las data annotations del modelo
$(document).on('input change', 'input, select, textarea', function () {
    var $el = $(this);
    if (!$el.length) return;
    if ($el.is('[readonly], [disabled], [type=hidden], [type=submit], [type=button]')) return;

    var valor = $el.val();
    if (valor === undefined || valor === null || valor === '') {
        $el.removeClass('is-valid is-invalid');
        return;
    }

    var valido;
    try { valido = $el.valid(); }
    catch (e) { return; }

    $el.toggleClass('is-valid', valido);
    $el.toggleClass('is-invalid', !valido);
});
