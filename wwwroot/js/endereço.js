/**
 * Módulo para busca automática de endereço por CEP
 * Utiliza a API ViaCEP para preenchimento automático dos campos
 */

class EnderecoViaCEP {
	constructor() {
		this.cepInput = null;
		this.estadoInput = null;
		this.cidadeInput = null;
		this.bairroInput = null;
		this.logradouroInput = null;
		this.enderecoSwitch = null;
	}

	/**
	 * Inicializa o módulo de busca de endereço
	 */
	init() {
		this.cepInput = document.querySelector('[name="Endereco.cep"]');
		this.estadoInput = document.querySelector('[name="Endereco.estado"]');
		this.cidadeInput = document.querySelector('[name="Endereco.cidade"]');
		this.bairroInput = document.querySelector('[name="Endereco.bairro"]');
		this.logradouroInput = document.querySelector(
			'[name="Endereco.logradouro"]'
		);
		this.enderecoSwitch = document.getElementById('EnderecoFilial');

		if (this.cepInput) {
			this.setupEventListeners();
		}
	}

	/**
	 * Configura os event listeners
	 */
	setupEventListeners() {
		// Busca automática quando sair do campo CEP
		this.cepInput.addEventListener('blur', (e) => {
			// Só busca o CEP se o switch de endereço da filial não estiver marcado
			if (!this.enderecoSwitch || !this.enderecoSwitch.checked) {
				this.buscarCEP(e.target.value);
			}
		});

		// Formatação automática do CEP
		this.cepInput.addEventListener('input', (e) => {
			let valor = e.target.value.replace(/\D/g, '');
			if (valor.length <= 8) {
				valor = valor.replace(/^(\d{5})(\d)/, '$1-$2');
				e.target.value = valor;
			}
		});
	}

	/**
	 * Limpa os campos de endereço
	 */
	limparEndereco() {
		if (this.estadoInput) this.estadoInput.value = '';
		if (this.cidadeInput) this.cidadeInput.value = '';
		if (this.bairroInput) this.bairroInput.value = '';
		if (this.logradouroInput) this.logradouroInput.value = '';
	}

	/**
	 * Preenche os campos com os dados do endereço
	 * @param {Object} dados - Dados retornados da API ViaCEP
	 */
	preencherEndereco(dados) {
		if (this.estadoInput) this.estadoInput.value = dados.uf || '';
		if (this.cidadeInput) this.cidadeInput.value = dados.localidade || '';
		if (this.bairroInput) this.bairroInput.value = dados.bairro || '';
		if (this.logradouroInput)
			this.logradouroInput.value = dados.logradouro || '';
	}

	/**
	 * Valida se o CEP tem o formato correto
	 * @param {string} cep - CEP a ser validado
	 * @returns {boolean} - True se válido
	 */
	validarCEP(cep) {
		const cepLimpo = cep.replace(/\D/g, '');
		return cepLimpo.length === 8;
	}

	/**
	 * Aplica indicadores visuais no campo CEP
	 * @param {string} tipo - Tipo do indicador: 'loading', 'success', 'error'
	 */
	aplicarIndicadorVisual(tipo) {
		if (!this.cepInput) return;

		// Remove classes anteriores
		this.cepInput.style.borderColor = '';
		this.cepInput.style.backgroundColor = '';

		switch (tipo) {
			case 'loading':
				this.cepInput.style.borderColor = '#ffc107';
				this.cepInput.style.backgroundColor = '#fff9e6';
				break;
			case 'success':
				this.cepInput.style.borderColor = '#28a745';
				this.cepInput.style.backgroundColor = '#d4edda';
				break;
			case 'error':
				this.cepInput.style.borderColor = '#dc3545';
				this.cepInput.style.backgroundColor = '#f8d7da';
				break;
		}

		// Remove indicadores visuais após 2 segundos (exceto loading)
		if (tipo !== 'loading') {
			setTimeout(() => {
				this.cepInput.style.borderColor = '';
				this.cepInput.style.backgroundColor = '';
			}, 2000);
		}
	}

	/**
	 * Busca o endereço pelo CEP na API ViaCEP
	 * @param {string} cep - CEP a ser pesquisado
	 */
	async buscarCEP(cep) {
		const cepLimpo = cep.replace(/\D/g, '');

		if (!this.validarCEP(cepLimpo)) {
			this.limparEndereco();
			return;
		}

		try {
			// Mostra indicador de carregamento
			this.aplicarIndicadorVisual('loading');

			const response = await fetch(
				`https://viacep.com.br/ws/${cepLimpo}/json/`
			);
			const dados = await response.json();

			if (dados.erro) {
				this.limparEndereco();
				this.aplicarIndicadorVisual('error');
				console.warn('CEP não encontrado');
			} else {
				this.preencherEndereco(dados);
				this.aplicarIndicadorVisual('success');
				console.log('Endereço encontrado:', dados);
			}
		} catch (error) {
			console.error('Erro ao buscar CEP:', error);
			this.limparEndereco();
			this.aplicarIndicadorVisual('error');
		}
	}
}

// Instância global para uso nos formulários
const enderecoViaCEP = new EnderecoViaCEP();

// Inicializa automaticamente quando o DOM estiver carregado
document.addEventListener('DOMContentLoaded', function () {
	enderecoViaCEP.init();
});
