// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// função para carregar logo da filial
var inputFileLogo = document.getElementById("Homelogo");
if (inputFileLogo != null) {
    inputFileLogo.addEventListener('change', function (e) {
        const inputTarget = e.target;
        const file = inputTarget.files[0];
        if (file) {
            const reader = new FileReader;
            reader.onload = function (event) {
                const pictureImagem = document.getElementById('logoHome');
                pictureImagem.src = event.target.result;
            }
            reader.readAsDataURL(file);
            const btnCarregar = document.getElementById("btnlogoHome");
            btnCarregar.innerHTML = "Imagem carregada";
        }
    })
}
// função para carregar logo da filial 2
var inputFileLogoCardapio = document.querySelector("#logoCardapio");
if (inputFileLogoCardapio != null) {
    inputFileLogoCardapio.addEventListener('change', function (e) {
        const inputTarget = e.target;
        const file = inputTarget.files[0];
        if (file) {
            const reader = new FileReader;
            reader.onload = function (event) {
                const pictureImagem = document.getElementById('CardapioLogo');
                pictureImagem.src = event.target.result;
            }
            reader.readAsDataURL(file);
            const btnCarregar = document.getElementById("btnCardapioLogo");
            btnCarregar.innerHTML = "Imagem carregada";
        }
    })
}
//carregar img do usuario
var inputFile = document.getElementById("ImgUsu");
if (inputFile != null) {
    inputFile.addEventListener('change', function (e) {
        const inputTarget = e.target;
        const file = inputTarget.files[0];
        if (file) {
            const reader = new FileReader;
            reader.onload = function (event) {
                const pictureImagem = document.getElementById('ImgPerfil');
                pictureImagem.src = event.target.result;
            }
            reader.readAsDataURL(file);
            const btnCarregar = document.getElementById("btnImagem");
            btnCarregar.innerHTML = "Imagem carregada";
        }
    })
}

//funcao para carregar produtos na pagina criar campanha
$('#codFilial').on('change', function () {
    let idSelecionado = $(this).val(); //construir o id
    if (idSelecionado != "" && document.getElementById('Produtos')) {
        $.ajax({
            url: "../../Campanhas/ViewProduto",
            type: "GET",
            data: { id: idSelecionado },
            beforeSend: function () {
                //mostrando a tela de loading
                var divPai = document.getElementById('Produtos');
                if (document.getElementById('ProdutoFilho')) {
                    var node = document.getElementById('ProdutoFilho');
                    node.parentNode.removeChild(node);
                }
                divPai.className = "d-flex justify-content-center";
                var div = document.createElement('div');
                div.className = "spinner-border tam-spin";
                div.role = "status";
                var span = document.createElement('span');
                span.innerHTML = "Loading...";
                span.className = "sr-only";
                divPai.appendChild(div);
            },
            success: function (data) {
                var dados = data;
                if (document.querySelector(".spinner-border")) {
                    var node = document.querySelector(".spinner-border");
                    node.parentNode.removeChild(node);
                }
                if (document.getElementById('ProdutoFilho')) {
                    var node = document.getElementById('ProdutoFilho');
                    node.parentNode.removeChild(node);
                }
                var div = document.createElement('div');
                let id = document.createAttribute('id');
                id.value = 'ProdutoFilho';
                div.setAttributeNode(id);
                div.innerHTML = dados;
                var divPai = document.getElementById('Produtos');
                divPai.className = '';;
                divPai.appendChild(div);
            },
            complete: function () {
                //retirando o loading
                if (document.querySelector(".spinner-border tam-spin")) {
                    var node = document.querySelector(".spinner-border tam-spin");
                    node.parentNode.removeChild(node);
                }
            }
        })
    }
    if (idSelecionado == "" && document.getElementById('ProdutoFilho')) {
        var node = document.getElementById('ProdutoFilho');
        node.parentNode.removeChild(node);
    }

});
$('#codFilial').trigger("change");

$(document).ready(function () {
    $('.checar').on('change', function () {
        $('.checar').not(this).prop('checked', false);
    });
});

//botão de rolar até o fim
function scrollfim() {
    window.scrollTo({
        top: document.body.scrollHeight,
        behavior: "smooth"
    });
}
//busca de produtos nas partialview
$(document).on('keyup', '#pesquisar', function (e) {
    var termo = $(this).val().toUpperCase();

    $('table #table').each(function () {
        if ($(this).html().toUpperCase().indexOf(termo) === -1) {
            $(this).fadeOut(500); // Oculta com uma animação suave de fadeOut
        } else {
            $(this).fadeIn(500);  // Mostra com uma animação suave de fadeIn
        }
    });
});

//botão deletar
// script.js

$(document).on('click', '#deleteBtn', function (e) {
    const cod=this.getAttribute('data-id');
    const popup = document.getElementById('popup'+cod);
    popup.style.display = 'flex';
});
// Esconde o pop-up ao clicar no botão de cancelar
$(document).on('click', '#cancelBtn', function (e) {
    const cod = this.getAttribute('data-id');
    const popup = document.getElementById('popup'+cod);
    popup.style.display = 'none';
});

// Chamar a função de deletar a campanha
$(document).on('click', '#confirmBtn', function (e) {
    
    const confirmBtn = document.getElementById('confirmBtn');
    const itemId = confirmBtn.getAttribute('data-id');
    const popup = document.getElementById('popup'+itemId);
    $.ajax({
        url: "../Campanhas/Delete",
        type: "POST",
        data: { id: itemId },
        beforeSend: function () {
            //mostrando a tela de loading
            const loadingEl = document.createElement("div");
            document.body.prepend(loadingEl);
            loadingEl.classList.add("page-loader");
            loadingEl.classList.add("flex-column");
            loadingEl.innerHTML = `<span class="spinner-border text-primary" role="status"></span>
            <span class="text-muted fs-6 fw-semibold mt-5">Loading...</span>`;
            // Show page loading
            KTApp.showPageLoading();
        },
        success: function (data) {
            popup.style.display = 'none';
            //retirando o loading
            KTApp.hidePageLoading();

            if (data == true) {
                Swal.fire({
                    text: "Item deletado com Sucesso!",
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: "Ok, Entendi!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        var redirectUrl = "../Campanhas/Index";
                        if (redirectUrl) {
                            location.href = redirectUrl;
                        }
                    }
                });
            } else {
                Swal.fire({
                    text: "Ocorreu um erro ao deletar o item.",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Ok, Entendi!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        var redirectUrl = "../Campanhas/Index";
                        if (redirectUrl) {
                            location.href = redirectUrl;
                        }
                    }
                });
            }
        }
    })
});
// Função para deletar produto da campanha
function deletarProdCamp(codProdCamp,codCamp) {
    const popup = document.getElementById('popup'+codProdCamp);
    $.ajax({
        url: "../Delete",
        type: "POST",
        data: { id: codProdCamp },
        beforeSend: function () {
            //mostrando a tela de loading
            const loadingEl = document.createElement("div");
            document.body.prepend(loadingEl);
            loadingEl.classList.add("page-loader");
            loadingEl.classList.add("flex-column");
            loadingEl.innerHTML = `<span class="spinner-border text-primary" role="status"></span>
            <span class="text-muted fs-6 fw-semibold mt-5">Loading...</span>`;
            // Show page loading
            KTApp.showPageLoading();
        },
        success: function (data) {
            popup.style.display = 'none';
            //retirando o loading
            KTApp.hidePageLoading();

            if (data == true) {
                Swal.fire({
                    text: "Item deletado com Sucesso!",
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: "Ok, Entendi!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        var redirectUrl = codCamp;
                        if (redirectUrl) {
                            location.href = redirectUrl;
                        }
                    }
                });
            } else {
                Swal.fire({
                    text: "Ocorreu um erro ao deletar o item.",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Ok, Entendi!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        var redirectUrl =codCamp;
                        if (redirectUrl) {
                            location.href = redirectUrl;
                        }
                    }
                });
            }
        }
    })
}
//funcão que carregar os produtos baseado na categoria
if (document.getElementById('Produto_Categoria_codCategoria') != null) {
    document.getElementById('Produto_Categoria_codCategoria').addEventListener('change', function () {
        var id = this.value;
        if (id == 0) {
            var subCategoryDropdown = document.getElementById('codProduto');
        } else {

            var subCategoryDropdown = document.getElementById('codProduto');

            fetch('../GetProdutos/' + id)
                .then(response => response.json())
                .then(data => {
                    data.forEach(item => {
                        var option = new Option(item.nomeProduto, item.codProduto);
                        subCategoryDropdown.options.add(option);
                    });
                });
            subCategoryDropdown.removeAttribute("disabled");
        }
        // Limpar opções anteriores
        
    });
}
//função para mostrar anuncio carregado
var inputFileLogoCardapio = document.querySelector("#logoCardapio");
if (inputFileLogoCardapio != null) {
    inputFileLogoCardapio.addEventListener('change', function (e) {
        const inputTarget = e.target;
        const file = inputTarget.files[0];
        if (file) {
            const reader = new FileReader;
            reader.onload = function (event) {
                const pictureImagem = document.getElementById('CardapioLogo');
                pictureImagem.src = event.target.result;
            }
            reader.readAsDataURL(file);
            const btnCarregar = document.getElementById("btnCardapioLogo");
            btnCarregar.innerHTML = "Imagem carregada";
        }
    })
}
// adicionar lotes
function gerarLotes(valor) {


    // Obtém o valor do input de quantidade de lotes
    var quantidadeLotes = parseInt(document.getElementById('qtdlotesEvento').value);
    if (valor!=0) {
        quantidadeLotes = quantidadeLotes - valor;
    }
    // Pega o contêiner onde os lotes serão adicionados
    var lotesContainer = document.getElementById('Lotes');

    // Limpa os lotes anteriores, se houver
    lotesContainer.innerHTML = '';

    // Verifica se a quantidade de lotes é maior que 0
    if (quantidadeLotes > 0) {
        for (var i = 0; i < quantidadeLotes; i++) {
            // Cria o HTML para cada lote
            if (valor != 0) {
                i=i+valor;
            }
            var loteHTML = `
            <div class="divC${i} col-md-12 row" style="margin-top:2%">
                <div class="col-md-1">
                    <label class="form-label" for="Lotes_${i}_numLote">Lote:</label>
                    <input class="Disabled form-control text-box single-line" data-val="true" data-val-number="O campo de lote deve ser um número." data-val-required="O campo Lote é obrigatório." id="Lotes_${i}_numLote" name="Lotes[${i}].numLote" type="number" value="${i + 1}">
                    <span class="field-validation-valid text-danger" data-valmsg-for="Lotes[${i}].numLote" data-valmsg-replace="true"></span>
                </div>
                <div class="col-md-2">
                    <label class="form-label" for="Lotes_${i}_valorLote">Valor dos Ingressos:</label>
                    <input class="form-control text-box single-line" data-val="true" data-val-number="O campo Valor deve ser um número." data-val-required="O campo Valor é obrigatório." id="Lotes_${i}_valorLote" name="Lotes[${i}].valorLote" type="text" value="">
                    <span class="field-validation-valid text-danger" data-valmsg-for="Lotes[${i}].valorLote" data-valmsg-replace="true"></span>
                </div>
                <div class="col-md-3">
                    <label class="form-label" for="Lotes_${i}_qtdIngLote">Quantidade de Ingressos:</label>
                    <input class="form-control text-box single-line" data-val="true" data-val-number="O campo quantidade ingressos deve ser um número." data-val-required="O campo da quantidade de ingressos é obrigatório." id="Lotes_${i}_qtdIngLote" name="Lotes[${i}].qtdIngLote" type="number" value="">
                    <span class="field-validation-valid text-danger" data-valmsg-for="Lotes[${i}].qtdIngLote" data-valmsg-replace="true"></span>
                </div>
                <div class="col-md-3">
                    <label class="form-label" for="Lotes_${i}_dataVendaLote">Início das vendas do Lote:</label>
                    <input class="form-control text-box single-line" data-val="true" data-val-date="O campo data de vendas deve ser uma data." data-val-required="O campo Data de Vendas é obrigatório." id="Lotes_${i}_dataVendaLote" name="Lotes[${i}].dataVendaLote" type="datetime-local" value="">
                    <span class="field-validation-valid text-danger" data-valmsg-for="Lotes[${i}].dataVendaLote" data-valmsg-replace="true"></span>
                </div>
            </div>
            `;

            // Adiciona o lote gerado ao contêiner
            lotesContainer.innerHTML += loteHTML;
        }
    }
}
//carregar endereco 
$(document).ready(function () {
    $('#EnderecoFilial').click(function (event) {  //on click 
        if (this.checked) { // check select status
            CodFilial = document.getElementById('codFilial').value;
            if (CodFilial == null || CodFilial == 0) {
                var erro = document.querySelector('[data-valmsg-for="codFilial"]');
                erro.innerHTML = "Você deve selecionar uma Filial";
                var selectElement = document.getElementById("nomeEvento");
                selectElement.scrollIntoView({
                    behavior: 'smooth', // Rolagem suave
                    block: 'center'     // Posição do elemento no centro da viewport
                });
                setTimeout(function () {
                    $('#EnderecoFilial').each(function () { //loop through each checkbox
                        this.checked = false; //deselect all checkboxes with class "checkbox1" 
                    }); //clear above interval after 5 seconds
                }, 500);

            } else {
                EnderecoFilial(CodFilial);
            }

        } else {
            elementList = document.querySelectorAll(".Disabled.Endereco");
            for (var i = 0; i < elementList.length; i++) {
                elementList[i].value = "";
                elementList[i].classList.remove('Disabled');
            }
        }
    });
});
function EnderecoFilial(CodFilial) {
    $.ajax({
        url: "Endereco",
        type: "GET",
        data: { id: CodFilial },
        beforeSend: function () {
            //mostrando a tela de loading
            var divPai = document.getElementById('Endereco');
            if (document.getElementById('EnderecoFilho')) {
                var node = document.getElementById('EnderecoFilho');
                node.parentNode.removeChild(node);
            }
            divPai.className = "row";
            var div = document.createElement('div');
            div.className = "spinner-border tam-spin";
            div.role = "status";
            var span = document.createElement('span');
            span.innerHTML = "Loading...";
            span.className = "sr-only";
            divPai.appendChild(div);
        },
        success: function (data) {
            var dados = data;
            if (document.querySelector(".spinner-border")) {
                var node = document.querySelector(".spinner-border");
                node.parentNode.removeChild(node);
            }
            if (document.getElementById('EnderecoFilho')) {
                var node = document.getElementById('EnderecoFilho');
                node.parentNode.removeChild(node);
            }
            var div = document.createElement('div');
            let id = document.createAttribute('id');
            id.value = 'EnderecoFilho';
            div.setAttributeNode(id);
            div.className = 'row';
            div.innerHTML = dados;
            var divPai = document.getElementById('Endereco');
            divPai.appendChild(div);
        },
        complete: function () {
            //retirando o loading
            if (document.querySelector(".spinner-border tam-spin")) {
                var node = document.querySelector(".spinner-border tam-spin");
                node.parentNode.removeChild(node);
            }
        }
    })
}
//verificando evento gratuito
$(document).ready(function () {
    $('#eventoGratuito').click(function (event) {  //on click 
        if (this.checked) { // check select status
            elementList = document.querySelector("#qtdlotesEvento");
            elementList.classList.add('Disabled');
            elementList.value = "1";
            var loteHTML = `
            <div class="divC0 col-md-12 row" style="margin-top:2%">
                <div class="col-md-1">
                    <label class="form-label" for="Lotes_0_numLote">Lote:</label>
                    <input class="Disabled form-control text-box single-line" data-val="true" data-val-number="O campo de lote deve ser um número." data-val-required="O campo Lote é obrigatório." id="Lotes_0_numLote" name="Lotes[0].numLote" type="number" value="1">
                    <span class="field-validation-valid text-danger" data-valmsg-for="Lotes[0].numLote" data-valmsg-replace="true"></span>
                </div>
                <div class="col-md-2">
                    <label class="form-label" for="Lotes_0_valorLote">Valor dos Ingressos:</label>
                    <input class="Disabled form-control text-box single-line" data-val="true" data-val-number="O campo Valor deve ser um número." data-val-required="O campo Valor é obrigatório." id="Lotes_0_valorLote" name="Lotes[0].valorLote" type="text" value="0">
                    <span class="field-validation-valid text-danger" data-valmsg-for="Lotes[0].valorLote" data-valmsg-replace="true"></span>
                </div>
                <div class="col-md-3">
                    <label class="form-label" for="Lotes_0_qtdIngLote">Quantidade de Ingressos:</label>
                    <input class="form-control text-box single-line" data-val="true" data-val-number="O campo quantidade ingressos deve ser um número." data-val-required="O campo da quantidade de ingressos é obrigatório." id="Lotes_0_qtdIngLote" name="Lotes[0].qtdIngLote" type="number" value="">
                    <span class="field-validation-valid text-danger" data-valmsg-for="Lotes[0].qtdIngLote" data-valmsg-replace="true"></span>
                </div>
                <div class="col-md-3">
                    <label class="form-label" for="Lotes_0_dataVendaLote">Início das vendas do Lote:</label>
                    <input class="form-control text-box single-line" data-val="true" data-val-date="O campo data de vendas deve ser uma data." data-val-required="O campo Data de Vendas é obrigatório." id="Lotes_0_dataVendaLote" name="Lotes[0].dataVendaLote" type="datetime-local" value="">
                    <span class="field-validation-valid text-danger" data-valmsg-for="Lotes[0].dataVendaLote" data-valmsg-replace="true"></span>
                </div>
            </div>
            `;
            if (document.getElementById('Lotes')) {
                // Pega o contêiner onde os lotes serão adicionados
                var lotesContainer = document.getElementById('Lotes');

                // Limpa os lotes anteriores, se houver
                lotesContainer.innerHTML = '';
                lotesContainer.innerHTML += loteHTML;
            } else {
                // Pega o contêiner onde os lotes serão adicionados
                var lotesContainer = document.getElementById('Lotes');

                // Limpa os lotes anteriores, se houver
                lotesContainer.innerHTML = '';
            }

        } else {
            elementList = document.getElementById("qtdlotesEvento");
            elementList.classList.remove('Disabled');
            elementList.value = "";
            var elementos = document.querySelectorAll('.valor');

            if (elementos.length > 0) {
                console.log('Os elementos existem!');
            } else {
                console.log('Nenhum elemento encontrado.');
            }
            var lotesContainer = document.getElementById('Lotes');

            // Limpa os lotes anteriores, se houver
            lotesContainer.innerHTML = '';
        }
    });
});
//formando link para evento
$(document).ready(function () {
    $("#nomeEvento").on("input", function () {
        var valorDigitado = $(this).val();
        // Substituir espaços por hífens
        var valorComHifens = valorDigitado.replace(/ /g, "-");

        // Realizar a requisição AJAX para verificar se o valor existe no banco de dados
        $.ajax({
            url: "VerificarValor", // Substitua pelo URL do seu script PHP que verifica o valor no banco de dados
            method: "POST",
            data: { valor: valorComHifens },
            success: function (response) {
                if (response === true) {
                    var erro = document.querySelector('[data-valmsg-for="linkEvento"]');
                    erro.innerText = "Link já existente! Digite outro...";
                    $("#linkEvento").val(valorComHifens);
                } else {
                    var erro = document.querySelector('[data-valmsg-for="linkEvento"]');
                    erro.innerText = "";
                    $("#linkEvento").val(valorComHifens);
                }
            }
        });
    });
});
//carregar eventos recentes
document.addEventListener('DOMContentLoaded', function () {
    if (document.getElementById('ListEvento')) {
        $.ajax({
            url: "../DetailsPartial",
            type: "GET",
            beforeSend: function () {
                // Mostra o indicador de carregamento
                var divPai = document.getElementById('ListEvento');
                divPai.innerHTML = ''; // Limpa o conteúdo existente
                var li = document.createElement('li');
                var div = document.createElement('div');
                li.id = 'carregar'
                li.classList.add('d-flex', 'justify-content-center');
                div.className = "spinner-border tam-spin";
                div.role = "status";
                var span = document.createElement('span');
                span.className = "sr-only";
                div.appendChild(span);
                li.appendChild(div);
                divPai.appendChild(li);
            },
            success: function (data) {
                // Remove o indicador de carregamento e exibe os dados
                var div = document.getElementById('carregar');
                if (div) {
                    div.parentNode.removeChild(div);
                }
                var divPai = document.getElementById('ListEvento');
                divPai.innerHTML = '';
                divPai.innerHTML = data;
            },
            error: function () {
                // Lida com erros na solicitação AJAX
                hideLoadingIndicator();
                displayErrorMessage('Ocorreu um erro ao carregar os produtos.');
            }
        })
    }
});
//funcao para verificar valor no editEvento
function validarValor(input) {
    var quantidadeLotes = document.getElementById('qtdlotesEvento');
    // Se o valor for menor que o valor inicial, reseta para o valor inicial
    if (input > quantidadeLotes.value) {
        quantidadeLotes.value = input;
    } else {
        gerarLotes(input);
    }
}
// Função para deletar produto da evento/desativar
function deletarEvento(evento) {
    const popup = document.getElementById('popup'+evento);
    $.ajax({
        url: "../Eventos/Delete",
        type: "POST",
        data: { id: evento },
        beforeSend: function () {
            //mostrando a tela de loading
            const loadingEl = document.createElement("div");
            document.body.prepend(loadingEl);
            loadingEl.classList.add("page-loader");
            loadingEl.classList.add("flex-column");
            loadingEl.innerHTML = `<span class="spinner-border text-primary" role="status"></span>
            <span class="text-muted fs-6 fw-semibold mt-5">Loading...</span>`;
            // Show page loading
            KTApp.showPageLoading();
        },
        success: function (data) {
            popup.style.display = 'none';
            //retirando o loading
            KTApp.hidePageLoading();

            if (data == true) {
                Swal.fire({
                    text: "Evento Delatado, caso algum ingresso foi vendido o evento foi desativo!",
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: "Ok, Entendi!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        var redirectUrl = "Eventos/Lista";
                        if (redirectUrl) {
                            location.href = redirectUrl;
                        }
                    }
                });
            } else {
                Swal.fire({
                    text: "Ocorreu um erro ao deletar o item.",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Ok, Entendi!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        var redirectUrl = "Eventos/Lista";
                        if (redirectUrl) {
                            location.href = redirectUrl;
                        }
                    }
                });
            }
        }
    })
}
//display do cupom 
$(document).ready(function () {
    $("#tokenCupom").on("input", function () {
        var valorDigitado = $(this).val();
        var erro = document.getElementById("desconto");
        erro.innerText = valorDigitado;

    });
});
//display da data do cupom e verifica sé uma data valida
$(document).ready(function () {
    $("#validadeCupom").on("input", function () {

        var valorDigitado = $(this).val();
        const novaData = new Date(valorDigitado);
        var erro = document.getElementById("validate");
        erro.innerText = "Valido até " + novaData.toLocaleDateString('pt-BR');

    });
});
// valida a data
function validarData(input) {
    // Obter a data atual e adicionar 1 dia para garantir que seja o próximo dia
    var hoje = new Date();
    hoje.setDate(hoje.getDate() + 1); // Amanhã

    // Formatar para o padrão 'yyyy-MM-ddTHH:MM' (igual ao tipo datetime-local)
    var diaSeguinte = hoje.toISOString().slice(0, 16);

    // Obter a data inserida no input
    var dataInserida = input.value;

    // Comparar as datas: a inserida deve ser maior ou igual ao próximo dia
    if (dataInserida < diaSeguinte) {
        var erro = document.querySelector('[data-valmsg-for="validadeCupom"]');
        erro.innerHTML = "A data deve ser superior ao dia atual.";
        input.value = ''; // Limpar o valor do input
    }
}
//selecionar produtos para o cupom 
function produtosSelecao() {
    var campanha = document.getElementById("codCampanha").value;
    var filial = document.getElementById("codFilial").value;
    var span = document.querySelector('span[data-valmsg-for="codFilial"]');

    if (filial == null || filial == 0) {
        document.getElementById('valor').scrollIntoView({
            behavior: 'smooth',
            block: 'start'
        });
        span.textContent = 'Você deve selecionar uma Filial';
        span.classList.add('error-message'); // Adiciona uma classe para exibir a mensagem de erro
    } else {
        $.ajax({
            url: "ViewProduto",
            type: "GET",
            data: { codCampanha: campanha, codFilial: filial },
            beforeSend: function () {
                // Mostra o indicador de carregamento
                showLoadingIndicator();
            },
            success: function (data) {
                // Remove o indicador de carregamento e exibe os dados
                hideLoadingIndicator();
                displayProducts(data);
            },
            error: function () {
                // Lida com erros na solicitação AJAX
                hideLoadingIndicator();
                displayErrorMessage('Ocorreu um erro ao carregar os produtos.');
            }
        })
    }
}
function showLoadingIndicator() {
    var divPai = document.getElementById('produtos');
    divPai.innerHTML = ''; // Limpa o conteúdo existente
    divPai.classList.add('d-flex', 'justify-content-center');
    var div = document.createElement('div');
    div.className = "spinner-border tam-spin";
    div.role = "status";
    var span = document.createElement('span');
    span.className = "sr-only";
    div.appendChild(span);
    divPai.appendChild(div);
}
function hideLoadingIndicator() {
    var div = document.querySelector(".spinner-border");
    if (div) {
        div.parentNode.removeChild(div);
    }
}
function displayProducts(data) {
    var div = document.getElementById('produtos');
    div.innerHTML = data;
    div.classList.remove('justify-content-center');
    div.classList.remove('d-flex');
    div.classList.add('row');
    div.style.margin = "2% auto";
}
function displayErrorMessage(message) {
    var span = document.querySelector('span[data-valmsg-for="codFilial"]');
    span.textContent = message;
    span.classList.add('error-message'); // Adiciona uma classe para exibir a mensagem de erro
}
//função para desativar Convites
function deletarConvite(convite) {
    const popup = document.getElementById('popup'+convite);
    $.ajax({
        url: "../Convites/Desativar",
        type: "POST",
        data: { id: convite },
        beforeSend: function () {
            //mostrando a tela de loading
            const loadingEl = document.createElement("div");
            document.body.prepend(loadingEl);
            loadingEl.classList.add("page-loader");
            loadingEl.classList.add("flex-column");
            loadingEl.innerHTML = `<span class="spinner-border text-primary" role="status"></span>
            <span class="text-muted fs-6 fw-semibold mt-5">Loading...</span>`;
            // Show page loading
            KTApp.showPageLoading();
        },
        success: function (data) {
            popup.style.display = 'none';
            //retirando o loading
            KTApp.hidePageLoading();

            if (data == true) {
                Swal.fire({
                    text: "Convite desativado com sucesso!",
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: "Ok, Entendi!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        var redirectUrl = "../Convites/Index";
                        if (redirectUrl) {
                            location.href = redirectUrl;
                        }
                    }
                });
            } else {
                Swal.fire({
                    text: "Ocorreu um erro ao deletar o item.",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Ok, Entendi!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        var redirectUrl = "../Convites/Index";
                        if (redirectUrl) {
                            location.href = redirectUrl;
                        }
                    }
                });
            }
        }
    })
}
//função para desativar Funcionarios
function desativarFun(codUsuario) {
    const popup = document.getElementById('popup'+codUsuario);
    $.ajax({
        url: "../Usuarios/Desativar",
        type: "POST",
        data: { id: codUsuario },
        beforeSend: function () {
            //mostrando a tela de loading
            const loadingEl = document.createElement("div");
            document.body.prepend(loadingEl);
            loadingEl.classList.add("page-loader");
            loadingEl.classList.add("flex-column");
            loadingEl.innerHTML = `<span class="spinner-border text-primary" role="status"></span>
            <span class="text-muted fs-6 fw-semibold mt-5">Loading...</span>`;
            // Show page loading
            KTApp.showPageLoading();
        },
        success: function (data) {
            popup.style.display = 'none';
            //retirando o loading
            KTApp.hidePageLoading();

            if (data == true) {
                Swal.fire({
                    text: "Funcionário desativado com sucesso!",
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: "Ok, Entendi!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        var redirectUrl = "../Usuarios/Funcionarios";
                        if (redirectUrl) {
                            location.href = redirectUrl;
                        }
                    }
                });
            } else {
                Swal.fire({
                    text: "Ocorreu um erro ao desativar o funcionário.",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Ok, Entendi!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        var redirectUrl = "../Usuarios/Funcionarios";
                        if (redirectUrl) {
                            location.href = redirectUrl;
                        }
                    }
                });
            }
        }
    })
}
//função para desativar Clientes
function desativarCli(codUsuario) {
    const popup = document.getElementById('popup'+codUsuario);
    $.ajax({
        url: "../Usuarios/Desativar",
        type: "POST",
        data: { id: codUsuario },
        beforeSend: function () {
            //mostrando a tela de loading
            const loadingEl = document.createElement("div");
            document.body.prepend(loadingEl);
            loadingEl.classList.add("page-loader");
            loadingEl.classList.add("flex-column");
            loadingEl.innerHTML = `<span class="spinner-border text-primary" role="status"></span>
            <span class="text-muted fs-6 fw-semibold mt-5">Loading...</span>`;
            // Show page loading
            KTApp.showPageLoading();
        },
        success: function (data) {
            popup.style.display = 'none';
            //retirando o loading
            KTApp.hidePageLoading();

            if (data == true) {
                Swal.fire({
                    text: "Cliente desativado com sucesso!",
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: "Ok, Entendi!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        var redirectUrl = "../Usuarios/Clientes";
                        if (redirectUrl) {
                            location.href = redirectUrl;
                        }
                    }
                });
            } else {
                Swal.fire({
                    text: "Ocorreu um erro ao desativar o cliente.",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Ok, Entendi!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        var redirectUrl = "../Usuarios/Clientes";
                        if (redirectUrl) {
                            location.href = redirectUrl;
                        }
                    }
                });
            }
        }
    })
}
//função para desativar Filial
function desativarFilial(codFilial) {
    const popup = document.getElementById('popup' + codFilial);
    $.ajax({
        url: "../Filial/Desativar",
        type: "POST",
        data: { id: codFilial },
        beforeSend: function () {
            //mostrando a tela de loading
            const loadingEl = document.createElement("div");
            document.body.prepend(loadingEl);
            loadingEl.classList.add("page-loader");
            loadingEl.classList.add("flex-column");
            loadingEl.innerHTML = `<span class="spinner-border text-primary" role="status"></span>
            <span class="text-muted fs-6 fw-semibold mt-5">Loading...</span>`;
            // Show page loading
            KTApp.showPageLoading();
        },
        success: function (data) {
            popup.style.display = 'none';
            //retirando o loading
            KTApp.hidePageLoading();

            if (data == true) {
                Swal.fire({
                    text: "Filial desativado com sucesso!",
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: "Ok, Entendi!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        var redirectUrl = "../Filial/index";
                        if (redirectUrl) {
                            location.href = redirectUrl;
                        }
                    }
                });
            } else {
                Swal.fire({
                    text: "Ocorreu um erro ao desativar a filial.",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Ok, Entendi!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        var redirectUrl = "../Filial/index";
                        if (redirectUrl) {
                            location.href = redirectUrl;
                        }
                    }
                });
            }
        }
    })
}
//função para desativar ProdutoFilial
function DeletarProdutoFilial(codProduto) {
    const popup = document.getElementById('popup' + codProduto);
    const codFilial = document.getElementById('codFilial');
    $.ajax({
        url: "../../Precos/Delete",
        type: "POST",
        data: { id: codProduto },
        beforeSend: function () {
            //mostrando a tela de loading
            const loadingEl = document.createElement("div");
            document.body.prepend(loadingEl);
            loadingEl.classList.add("page-loader");
            loadingEl.classList.add("flex-column");
            loadingEl.innerHTML = `<span class="spinner-border text-primary" role="status"></span>
            <span class="text-muted fs-6 fw-semibold mt-5">Carregando...</span>`;
            // Show page loading
            KTApp.showPageLoading();
        },
        success: function (data) {
            popup.style.display = 'none';
            //retirando o loading
            KTApp.hidePageLoading();

            if (data == true) {
                Swal.fire({
                    text: "Produto deletado com sucesso!",
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: "Ok, Entendi!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        var redirectUrl = "../../Precos/index/"+codFilial.value;
                        if (redirectUrl) {
                            location.href = redirectUrl;
                        }
                    }
                });
            } else {
                Swal.fire({
                    text: "Ocorreu um erro ao deletar o produto.",
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "Ok, Entendi!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                }).then(function (result) {
                    if (result.isConfirmed) {
                        var redirectUrl = "../../Preco/index/"+codFilial.value;
                        if (redirectUrl) {
                            location.href = redirectUrl;
                        }
                    }
                });
            }
        }
    })
}
document.addEventListener('DOMContentLoaded', function () {
    var telefones = document.querySelectorAll('.telefone');
    telefones.forEach(function (telefone) {
        telefone.addEventListener('input', function (e) {
            let input = e.target.value;

            // Remove tudo que não é número
            input = input.replace(/\D/g, '');

            // Aplica a máscara conforme o tamanho do valor inserido
            if (input.length > 10) {
                input = input.replace(/^(\d{2})(\d{5})(\d{4}).*/, '($1) $2-$3');
            } else if (input.length > 5) {
                input = input.replace(/^(\d{2})(\d{4})(\d{0,4}).*/, '($1) $2-$3');
            } else if (input.length > 2) {
                input = input.replace(/^(\d{2})(\d{0,5})/, '($1) $2');
            } else {
                input = input.replace(/^(\d*)/, '($1');
            }

            e.target.value = input;
        });
    });
});

function loading() {
    const loadingEl = document.createElement("div");
    document.body.prepend(loadingEl);
    loadingEl.classList.add("page-loader");
    loadingEl.classList.add("flex-column");
    loadingEl.innerHTML = `<span class="spinner-border text-primary" role="status"></span>
            <span class="text-muted fs-6 fw-semibold mt-5">Carregando...</span>`;
    // Show page loading
    KTApp.showPageLoading();
}

$(document).ready(function () {
    // Adiciona um evento de change ao select #evento
    $('#evento').change(function () {
        var eventoId = $(this).val();

        if (eventoId) {
            // Mostra o spinner
            $('#loadingSpinner').html(`
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Carregando...</span>
                </div>
                <span class="ms-2">Carregando dados...</span>
            `).removeClass('d-none');
            $('#opacity').addClass('opacity');
            // Chama a função para carregar os dados do evento
            carregarDadosEvento(eventoId);
        } else {
            // Se nenhum evento foi selecionado, reseta os valores
            resetarValores();
        }
    });

    function carregarDadosEvento(eventoId) {
        // Faz uma requisição AJAX para obter os dados do evento
        $.ajax({
            url: '/ReciboPresentes/Convidados/', // Substitua pelo seu endpoint
            type: 'GET',
            data: { id: eventoId },
            success: function (response) {
                // Atualiza os valores na interface
                $('#total').text(response.total || '0');
                $('#pendente').text(response.pendente || '0');
                $('#recusado').text(response.recusado || '0');
                $('#confirmado').text(response.confirmado || '0');
            },
            error: function () {
                console.error('Erro ao carregar dados do evento');
                resetarValores();
            },
            complete: function () {
                // Esconde o spinner quando a requisição estiver completa (tanto success quanto error)
                $('#loadingSpinner').addClass('d-none');
                $('#opacity').removeClass('opacity');
            }
        });
    }

    function resetarValores() {
        $('#total').text('-');
        $('#pendente').text('--');
        $('#recusado').text('--');
        $('#confirmado').text('--');
    }
});

function formatarMoeda(valor) {
    return 'R$ ' + valor.toFixed(2).replace('.', ',');
}

// Função para converter valor monetário para número
function parseMoeda(valor) {
    return parseFloat(valor.replace('R$', '').replace('.', '').replace(',', '.').trim());
}

// Função para atualizar o valor total
function atualizarValorTotal() {
    // Pega o valor unitário e converte para número
    const valorUnitarioTexto = document.getElementById('valorUnitario').textContent.trim();
    const valorUnitario = parseMoeda(valorUnitarioTexto);

    // Pega a quantidade atual
    const quantidade = parseInt(document.getElementById('count-ingre').textContent);

    // Calcula o novo total
    const valorTotal = valorUnitario * quantidade;

    // Atualiza o campo de total
    document.getElementById('valorTotal').textContent = formatarMoeda(valorTotal);
}

// Função para somar 1 à quantidade
function somar() {
    const contador = document.getElementById('count-ingre');
    const qtdMax = parseInt(document.getElementById('qtdMax').value) ;
    const quantidade = document.getElementById('quantidade');
    let valorAtual = parseInt(contador.textContent);
    quantidade.value = valorAtual + 1;
    contador.textContent = valorAtual + 1;
    atualizarValorTotal();
}

// Função para subtrair 1 da quantidade (mínimo 1)
function subtrair() {
    const contador = document.getElementById('count-ingre');
    const quantidade = document.getElementById('quantidade');
    let valorAtual = parseInt(contador.textContent);

    if (valorAtual > 1) {
        contador.textContent = valorAtual - 1;
        quantidade.value = valorAtual - 1;
        atualizarValorTotal();
    }
}
function mascaraCPF(value) {
    // Remove todos os caracteres não numéricos
    value = value.replace(/\D/g, "");

    // Formata o valor para o padrão CPF: 000.000.000-00
    value = value.replace(/(\d{3})(\d)/, "$1.$2");
    value = value.replace(/(\d{3})(\d)/, "$1.$2");
    value = value.replace(/(\d{3})(\d{1,2})$/, "$1-$2");

    return value;
}
function cpfValido(cpf) {
    // Remove caracteres não numéricos
    cpf = cpf.replace(/[^\d]+/g, '');

    // CPF deve ter 11 dígitos
    if (cpf.length !== 11) return false;

    // Elimina CPFs com todos os dígitos iguais (ex: 111.111.111-11)
    if (/^(\d)\1+$/.test(cpf)) return false;

    // Validação do primeiro dígito verificador
    let soma = 0;
    for (let i = 0; i < 9; i++) {
        soma += parseInt(cpf.charAt(i)) * (10 - i);
    }
    let resto = (soma * 10) % 11;
    if (resto === 10 || resto === 11) resto = 0;
    if (resto !== parseInt(cpf.charAt(9))) return false;

    // Validação do segundo dígito verificador
    soma = 0;
    for (let i = 0; i < 10; i++) {
        soma += parseInt(cpf.charAt(i)) * (11 - i);
    }
    resto = (soma * 10) % 11;
    if (resto === 10 || resto === 11) resto = 0;
    if (resto !== parseInt(cpf.charAt(10))) return false;

    return true;
}

const inputCPF = document.querySelector('.cpf');
if (inputCPF) {
    document.querySelector('.cpf').addEventListener('input', function () {
        this.value = mascaraCPF(this.value);

        const spanErro = document.querySelector('.cpfInvalid');
        spanErro.textContent = ""; // Limpa mensagem
        if (this.value.length === 14) {
            if (cpfValido(this.value)) {
                verificarCpf(this.value);
            } else {
                spanErro.textContent = "CPF inválido";
            }
        }
    });
}
const emailInput = document.querySelector('.email');

if (emailInput) {
    emailInput.addEventListener('blur', function () {
        const email = this.value.trim();
        const cpf = document.querySelector('.cpf')?.value.trim();

        // Se não houver CPF preenchido, não faz nada
        if (!cpf) return;

        // Faz requisição para sua action C#
        fetch(`../../Usuarios/VerificarCpfEmail?cpf=${encodeURIComponent(cpf)}&email=${encodeURIComponent(email)}`)
            .then(response => response.json())
            .then(data => {
                const spanErro = document.querySelector('.cpfInvalid');
                if (!spanErro) return;

                if (!data.success) {
                    spanErro.textContent = "O CPF e o e-mail informados não correspondem";
                    const button = document.getElementById('clickS');
                    button.disabled = true; // Desabilita o botão de submit
                } else {
                    spanErro.textContent = "";
                    const button = document.getElementById('clickS');
                    button.disabled =false; // Desabilita o botão de submit
                }
            })
            .catch(error => {
                console.error('Erro na verificação de CPF e E-mail:', error);
            });
    });
}
function mascaraCNPJ(value) {
    // Remove tudo que não for número
    value = value.replace(/\D/g, '');

    // Aplica a máscara: 00.000.000/0000-00
    value = value.replace(/^(\d{2})(\d)/, '$1.$2');
    value = value.replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3');
    value = value.replace(/\.(\d{3})(\d)/, '.$1/$2');
    value = value.replace(/(\d{4})(\d)/, '$1-$2');

    return value;
}
function cnpjValido(cnpj) {
    cnpj = cnpj.replace(/[^\d]+/g, '');

    if (cnpj.length !== 14) return false;

    // Elimina CNPJs com todos os dígitos iguais
    if (/^(\d)\1+$/.test(cnpj)) return false;

    let tamanho = cnpj.length - 2;
    let numeros = cnpj.substring(0, tamanho);
    let digitos = cnpj.substring(tamanho);
    let soma = 0;
    let pos = tamanho - 7;

    for (let i = tamanho; i >= 1; i--) {
        soma += parseInt(numeros.charAt(tamanho - i)) * pos--;
        if (pos < 2) pos = 9;
    }

    let resultado = soma % 11 < 2 ? 0 : 11 - (soma % 11);
    if (resultado !== parseInt(digitos.charAt(0))) return false;

    tamanho += 1;
    numeros = cnpj.substring(0, tamanho);
    soma = 0;
    pos = tamanho - 7;

    for (let i = tamanho; i >= 1; i--) {
        soma += parseInt(numeros.charAt(tamanho - i)) * pos--;
        if (pos < 2) pos = 9;
    }

    resultado = soma % 11 < 2 ? 0 : 11 - (soma % 11);
    if (resultado !== parseInt(digitos.charAt(1))) return false;

    return true;
}

const inputCNPJ = document.getElementById('cnpj');

if (inputCNPJ) {
    document.getElementById('cnpj').addEventListener('input', function () {
        this.value = mascaraCNPJ(this.value);

        const spanErro = document.querySelector('.cnpjInvalid');
        spanErro.textContent = ""; // Limpa erro

        if (this.value.length === 18) { // CNPJ com máscara tem 18 caracteres
            if (!cnpjValido(this.value)) {
                spanErro.textContent = "CNPJ inválido";
            } else {
                verificarCNPJ(this.value);
            }
        }
    });
}
function verificarCNPJ(cnpj) {
    fetch(`VeryCNPJ/?cnpj=` + cnpj)
        .then(response => response.json())
        .then(data => {
            const spanErro = document.querySelector('.cnpjInvalid');
            if (!spanErro) return;
            if (data.success) {
                // CNPJ já cadastrado → mostra erro
                spanErro.textContent = "CNPJ já cadastrado";
            } else {
                // CNPJ disponível → limpa erro
                spanErro.textContent = "";
            }
        })
        .catch(error => {
            console.error('Erro na verificação do CNPJ:', error);
        });
}
