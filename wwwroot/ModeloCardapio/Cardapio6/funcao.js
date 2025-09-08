$(function () {
    $("#txtBusca").keyup(function () {
        var texto = $(this).val();
        $(".bloco").each(function () {
            var resultado = $(this).text().toUpperCase().indexOf(' ' + texto.toUpperCase());

            if (resultado < 0) {
                $(this).fadeOut();
            } else {
                $(this).fadeIn();
            }
        });

    });

});
function filtrarPorCategoria(elemento,categoriaSelecionada) {
    // Obtém todas as divs com a classe 'item'
    var divsItens = document.querySelectorAll('.item');
    var efeito=document.querySelectorAll('.button');
    for (var i = 0; i < efeito.length; i++) {
        efeito[i].classList.remove('barraEfect');
    }
    var categoria = document.getElementById('categoria');
    categoria.innerHTML = categoriaSelecionada;
    elemento.classList.add('barraEfect')
    // Itera sobre as divs para mostrar ou ocultar com base na categoria selecionada
    divsItens.forEach(function (divItem) {
        var classes = divItem.id;

        // Verifica se a categoria selecionada é 'todas' ou se a div tem a classe correspondente
        if (categoriaSelecionada === 'Todos' || classes==categoriaSelecionada) {
            divItem.style.display = 'block';  // Mostra a div
        } else {
            divItem.style.display = 'none';   // Oculta a div
        }
    });
}
