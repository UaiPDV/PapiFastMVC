var lista = []; 0
var listamod = [];
window.onload = function () {
    var url_string = window.location.href;
    var url = new URL(url_string);
    var data = url.searchParams.get("Id");
    if (data != null) {
        var mesa = document.querySelector("#NumMesa");
        mesa.innerHTML += data;
    } else {
        data = prompt("Digite o número da mesa!");
        var mesa = document.querySelector("#NumMesa");
        mesa.innerHTML += data;
    }
};
function modSoma(id) {
    var div = document.getElementById('adicional' + id);
    var panel = document.getElementById('mod' + id + "-1");
    if (!div.classList.contains("active")) {
        div.classList.add('active');
    }
    var mudar = document.querySelector("#t" + id);
    var valor = mudar.innerHTML;
    if (valor >= 2) {
        var clone = panel.cloneNode(true);
        var titulofoFilho = clone.getElementsByTagName('h3')[0];
        var nome = document.querySelector(".nome" + id);
        titulofoFilho.innerHTML = "Adicional " + nome.innerHTML + " n" + valor;
        var butons = clone.getElementsByTagName('button');
        qtdadd = clone.querySelectorAll('.qtdmod');
        for (var i = 0; i < qtdadd.length; i++) {
            var string = qtdadd[i].id.replaceAll("-1", "-" + valor);
            qtdadd[i].id = string;
            qtdadd[i].innerHTML = "0";
        }
        for (var i = 0; i < butons.length; i++) {
            var string = butons[i].id.replaceAll("1", valor);
            butons[i].id = string;
        }
        var string = clone.id.replaceAll("-1", "-" + valor);
        clone.id = string;
        div.appendChild(clone);
    }
}
function modSub(id) {
    var value = document.getElementById('t' + id).innerHTML;
    var panel = document.getElementById('adicional' + id);
    if (value == 0) {
        if (panel.classList.contains("active")) {
            qtdadd = panel.querySelectorAll('.qtdmod');
            count = 0;
            for (var i = 0; i < qtdadd.length; i++) {
                if (count < qtdadd[i].innerHTML) {
                    count = qtdadd[i].innerHTML;
                }
                btn = panel.querySelectorAll('.sub');
                for (var c = 0; c < btn.length; c++) {
                    for (var k = 0; k < count; k++) {
                        var btn
                        btn[c].click();
                    }
                }
            }
            panel.classList.remove('active')
        }

    }
    else {
        var div = document.getElementById('mod' + id + "-" + (parseFloat(value) + 1));
        qtdadd = div.querySelectorAll('.qtdmod');
        count = 0;
        for (var i = 0; i < qtdadd.length; i++) {
            if (count < qtdadd[i].innerHTML) {
                count = qtdadd[i].innerHTML;
            }
            btn = div.querySelectorAll('.sub');
            for (var c = 0; c < btn.length; c++) {
                for (var k = 0; k < count; k++) {
                    var btn
                    btn[c].click();
                }
            }
        }
        div.remove();
    }
}

function somarmod(obj, e, l, codproduto) {
    //para especifico
    valor = obj.id.replaceAll("c", "")
    var mudar = document.querySelector("#modt" + e + "-" + valor);
    var soma = mudar.innerHTML;
    //gera produto selecionado
    var nomeProduto = document.querySelector(".nomemod" + e).innerHTML;
    var ValorProduto = document.querySelector(".modt" + e).innerHTML;

    var indice;
    var lista = { codigo: e, quantidade: 1, NomeProd: nomeProduto, Valor: ValorProduto, codProduto: codproduto, Ordem: valor };
    l.push(lista);
    //soma itens
    soma = parseInt(soma) + 1;
    mudar.innerHTML = soma;
    //soma de itens para total
    mudar = document.querySelector("#total");
    soma = mudar.innerHTML;
    numbertotal = parseInt(soma) + 1;
    mudar.innerHTML = numbertotal;
    //chama funcao de valor a pagar
    somaValor(ValorProduto);
}
function somar(e, l) {
    //para especifico
    var mudar = document.querySelector("#t" + e);
    var soma = mudar.innerHTML;
    //gera produto selecionado
    var nomeProduto = document.querySelector(".nome" + e).innerHTML;
    var ValorProduto = document.querySelector(".t" + e).innerHTML;
    let nLista = l.length;
    var teste = false;
    var indice;
    for (var i = 0; i < nLista; i++) {
        if (l[i].codigo == e) {
            teste = true;
            indice = i;
        }
    }
    if (teste == true) {
        l[indice].quantidade = l[indice].quantidade + 1
    } else {
        var lista = { codigo: e, quantidade: 1, NomeProd: nomeProduto, Valor: ValorProduto };
        l.push(lista);
    }
    //soma itens
    soma = parseInt(soma) + 1;
    mudar.innerHTML = soma;
    //soma de itens para total
    mudar = document.querySelector("#total");
    soma = mudar.innerHTML;
    numbertotal = parseInt(soma) + 1;
    mudar.innerHTML = numbertotal;
    //chama funcao de valor a pagar
    somaValor(ValorProduto);
}
function categoria(cat) {
    var meuElemento = document.querySelector(cat);
    meuElemento.scrollIntoView({ behavior: "smooth" });
}
function subtrairmod(obj, e, l) {
    valor = obj.id.replaceAll("c", "")
    var mudar = document.querySelector("#modt" + e + "-" + valor);
    var soma = mudar.innerHTML;
    if (soma > "0") {
        soma = parseInt(soma) - 1;
        mudar.innerHTML = soma;
        mudar = document.querySelector("#total");
        Number = mudar.innerHTML;
        numbertotal = parseInt(Number) - 1;
        mudar.innerHTML = numbertotal;
        var ValorProduto = document.querySelector(".modt" + e).innerHTML;
        SubValor(ValorProduto);
        var nomeProduto = document.querySelector(".nomemod" + e).innerHTML;
        for (var i = 0; i < l.length; i++) {
            if (l[i].codigo == e) {
                if (l[i].quantidade == 1) {
                    l.splice(i, 1);
                } else {
                    l[i].quantidade = l[i].quantidade - 1;
                }
            }
        }
    }
}
function subtrair(e, l) {
    var mudar = document.querySelector("#t" + e);
    var soma = mudar.innerHTML;
    if (soma > "0") {
        soma = parseInt(soma) - 1;
        mudar.innerHTML = soma;
        mudar = document.querySelector("#total");
        Number = mudar.innerHTML;
        numbertotal = parseInt(Number) - 1;
        mudar.innerHTML = numbertotal;
        var ValorProduto = document.querySelector(".t" + e).innerHTML;
        SubValor(ValorProduto);
        var nomeProduto = document.querySelector(".nome" + e).innerHTML;
        for (var i = 0; i < l.length; i++) {
            if (l[i].codigo == e) {
                if (l[i].quantidade == 1) {
                    l.splice(i, 1);
                } else {
                    l[i].quantidade = l[i].quantidade - 1;
                }
            }
        }
    }

}
function somaValor(e) {


    var final1 = document.querySelector(".conta");
    var final = final1.innerHTML;
    var valor = e.replaceAll("R$ ", "");
    valor = valor.replace(",", ".");
    valor = parseFloat(valor);
    let n = valor;
    final = final.replaceAll("R$ ", "");
    final = final.replace(",", ".");
    let n2 = parseFloat(final);
    let soma = n2 + n;
    soma = soma.toFixed(2);
    soma = soma.replace(".", ",");
    final1.innerHTML = "R$ " + soma;
}
function SubValor(e) {
    var final1 = document.querySelector(".conta");
    var final = final1.innerHTML;
    var valor = e.replaceAll("R$", "");
    valor = valor.replace(",", ".");
    valor = parseFloat(valor);
    let n = valor;
    final = final.replaceAll("R$", "");
    final = final.replace(",", ".");
    let n2 = parseFloat(final);
    let soma = n2 - n;
    soma = soma.toFixed(2);
    soma = soma.replace(".", ",");
    final1.innerHTML = "R$ " + soma;
}
function enviar(ListaProd, Fonenumber, ListaMod) {
    var target;
    var mesa = document.querySelector("#NumMesa");
    var data = mesa.innerHTML;
    let isMobile = (function (a) {
        if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4))) {
            return true
        } else {
            return false
        }
    })(navigator.userAgent || navigator.vendor || window.opera);

    // checar
    if (isMobile) {
        let msg = `${data} \n\n`;

        for (var i = 0; i < ListaProd.length; i++) {
            msg += `Cod: ${ListaProd[i].codigo}\n${ListaProd[i].quantidade} - ${ListaProd[i].NomeProd} ...... ${ListaProd[i].Valor}\n`;
            for (var k = 1; k <= ListaProd[i].quantidade; k++) {
                msg += `Adicionais pedido ${k}:\n`;
                for (var j = 0; j < listamod.length; j++) {
                    if (listamod[j].codProduto == ListaProd[i].codigo && listamod[j].Ordem == (k)) {
                        msg += `${listamod[j].quantidade} - ${listamod[j].codigo} - ${listamod[j].NomeProd} ...... ${listamod[j].Valor}\n`;
                    }
                }
            }
            msg += `\n`;
        }
        var conta = document.querySelector(".conta").innerHTML;
        msg += `Total do pedido: ${conta}`;
        target = `whatsapp://send?phone=${encodeURIComponent(Fonenumber)}&text=${encodeURIComponent(msg)}`
    } else {
        let msg = `${data} %0A%0A`;
        for (var i = 0; i < ListaProd.length; i++) {
            msg += `Cod: ${ListaProd[i].codigo}%0A${ListaProd[i].quantidade} - ${ListaProd[i].NomeProd} ...... ${ListaProd[i].Valor}%0A`;
            for (var k = 1; k <= ListaProd[i].quantidade; k++) {
                msg += `Adicionais pedido ${k}:%0A`;
                for (var j = 0; j < listamod.length; j++) {
                    if (listamod[j].codProduto == ListaProd[i].codigo && listamod[j].Ordem == (k)) {
                        msg += `${listamod[j].quantidade} - ${listamod[j].codigo} - ${listamod[j].NomeProd} ...... ${listamod[j].Valor}%0A`;
                    }
                }
            }
            msg += `%0A`;
        }
        var conta = document.querySelector(".conta").innerHTML;
        msg += `Total do pedido: ${conta}`;
        target = `https://api.whatsapp.com/send?phone=${encodeURIComponent(Fonenumber)}&text=` + msg;
    }
    window.open(target);
}