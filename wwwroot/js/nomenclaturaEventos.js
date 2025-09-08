/**
 * Nomenclatura Dinâmica para Eventos
 * Sistema que adapta a terminologia (Ingresso/Convite/Reserva, Lote/Mesa/Pacote)
 * baseado na categoria e tipo de local do evento
 */

// Mapeamento de nomenclaturas por categoria e tipo de local
const nomenclaturas = {
	// === NOMENCLATURAS POR CATEGORIA ===
	Aniversário: {
		ingresso: 'Convite',
		ingressos: 'Convites',
		lote: 'Grupo',
		lotes: 'Grupos',
		tipoItem: 'Tipo de Convite',
		qtdItem: 'Quantidade de Grupos',
		qtdIndividual: 'Quantidade de Convites',
		inicioVenda: 'Início das Inscrições',
	},
	Casamento: {
		ingresso: 'Convite',
		ingressos: 'Convites',
		lote: 'Mesa',
		lotes: 'Mesas',
		tipoItem: 'Tipo de Convite',
		qtdItem: 'Quantidade de Mesas',
		qtdIndividual: 'Quantidade de Convites',
		inicioVenda: 'Início das Confirmações',
	},
	'Chá de Bebê': {
		ingresso: 'Convite',
		ingressos: 'Convites',
		lote: 'Turma',
		lotes: 'Turmas',
		tipoItem: 'Tipo de Convite',
		qtdItem: 'Quantidade de Turmas',
		qtdIndividual: 'Quantidade de Convites',
		inicioVenda: 'Início das Confirmações',
	},
	'Chá de Panela': {
		ingresso: 'Convite',
		ingressos: 'Convites',
		lote: 'Turma',
		lotes: 'Turmas',
		tipoItem: 'Tipo de Convite',
		qtdItem: 'Quantidade de Turmas',
		qtdIndividual: 'Quantidade de Convites',
		inicioVenda: 'Início das Confirmações',
	},
	Bodas: {
		ingresso: 'Convite',
		ingressos: 'Convites',
		lote: 'Mesa',
		lotes: 'Mesas',
		tipoItem: 'Tipo de Convite',
		qtdItem: 'Quantidade de Mesas',
		qtdIndividual: 'Quantidade de Convites',
		inicioVenda: 'Início das Confirmações',
	},
	'Congresso, Conferência, Seminário': {
		ingresso: 'Inscrição',
		ingressos: 'Inscrições',
		lote: 'Categoria',
		lotes: 'Categorias',
		tipoItem: 'Tipo de Inscrição',
		qtdItem: 'Quantidade de Categorias',
		qtdIndividual: 'Quantidade de Inscrições',
		inicioVenda: 'Início das Inscrições',
	},
	'Curso, Workshop': {
		ingresso: 'Inscrição',
		ingressos: 'Inscrições',
		lote: 'Turma',
		lotes: 'Turmas',
		tipoItem: 'Tipo de Inscrição',
		qtdItem: 'Quantidade de Turmas',
		qtdIndividual: 'Quantidade de Inscrições',
		inicioVenda: 'Início das Inscrições',
	},
	'Show, Música, Festas': {
		ingresso: 'Ingresso',
		ingressos: 'Ingressos',
		lote: 'Setor',
		lotes: 'Setores',
		tipoItem: 'Tipo de Ingresso',
		qtdItem: 'Quantidade de Setores',
		qtdIndividual: 'Quantidade de Ingressos',
		inicioVenda: 'Início das Vendas',
	},
	Esportivo: {
		ingresso: 'Ingresso',
		ingressos: 'Ingressos',
		lote: 'Setor',
		lotes: 'Setores',
		tipoItem: 'Tipo de Ingresso',
		qtdItem: 'Quantidade de Setores',
		qtdIndividual: 'Quantidade de Ingressos',
		inicioVenda: 'Início das Vendas',
	},
	'E-Sports': {
		ingresso: 'Ingresso',
		ingressos: 'Ingressos',
		lote: 'Categoria',
		lotes: 'Categorias',
		tipoItem: 'Tipo de Ingresso',
		qtdItem: 'Quantidade de Categorias',
		qtdIndividual: 'Quantidade de Ingressos',
		inicioVenda: 'Início das Vendas',
	},

	// === NOMENCLATURAS POR TIPO DE LOCAL ===
	Hotel: {
		ingresso: 'Reserva',
		ingressos: 'Reservas',
		lote: 'Pacote',
		lotes: 'Pacotes',
		tipoItem: 'Tipo de Reserva',
		qtdItem: 'Quantidade de Pacotes',
		qtdIndividual: 'Quantidade de Reservas',
		inicioVenda: 'Início das Reservas',
	},
	'Hotel Fazenda': {
		ingresso: 'Hospedagem',
		ingressos: 'Hospedagens',
		lote: 'Pacote',
		lotes: 'Pacotes',
		tipoItem: 'Tipo de Hospedagem',
		qtdItem: 'Quantidade de Pacotes',
		qtdIndividual: 'Quantidade de Hospedagens',
		inicioVenda: 'Início das Reservas',
	},
	Pousada: {
		ingresso: 'Reserva',
		ingressos: 'Reservas',
		lote: 'Acomodação',
		lotes: 'Acomodações',
		tipoItem: 'Tipo de Reserva',
		qtdItem: 'Quantidade de Acomodações',
		qtdIndividual: 'Quantidade de Reservas',
		inicioVenda: 'Início das Reservas',
	},
	Restaurante: {
		ingresso: 'Reserva',
		ingressos: 'Reservas',
		lote: 'Mesa',
		lotes: 'Mesas',
		tipoItem: 'Tipo de Reserva',
		qtdItem: 'Quantidade de Mesas',
		qtdIndividual: 'Quantidade de Reservas',
		inicioVenda: 'Início das Reservas',
	},
	'Bar/Pub': {
		ingresso: 'Reserva',
		ingressos: 'Reservas',
		lote: 'Mesa',
		lotes: 'Mesas',
		tipoItem: 'Tipo de Reserva',
		qtdItem: 'Quantidade de Mesas',
		qtdIndividual: 'Quantidade de Reservas',
		inicioVenda: 'Início das Reservas',
	},
	'Day Use': {
		ingresso: 'Reserva',
		ingressos: 'Reservas',
		lote: 'Pacote',
		lotes: 'Pacotes',
		tipoItem: 'Tipo de Reserva',
		qtdItem: 'Quantidade de Pacotes',
		qtdIndividual: 'Quantidade de Reservas',
		inicioVenda: 'Início das Reservas',
	},
	'Salão de festas': {
		ingresso: 'Convite',
		ingressos: 'Convites',
		lote: 'Mesa',
		lotes: 'Mesas',
		tipoItem: 'Tipo de Convite',
		qtdItem: 'Quantidade de Mesas',
		qtdIndividual: 'Quantidade de Convites',
		inicioVenda: 'Início das Confirmações',
	},
	'Casa/Condomínio': {
		ingresso: 'Convite',
		ingressos: 'Convites',
		lote: 'Grupo',
		lotes: 'Grupos',
		tipoItem: 'Tipo de Convite',
		qtdItem: 'Quantidade de Grupos',
		qtdIndividual: 'Quantidade de Convites',
		inicioVenda: 'Início das Confirmações',
	},
	Cinema: {
		ingresso: 'Ingresso',
		ingressos: 'Ingressos',
		lote: 'Sessão',
		lotes: 'Sessões',
		tipoItem: 'Tipo de Ingresso',
		qtdItem: 'Quantidade de Sessões',
		qtdIndividual: 'Quantidade de Ingressos',
		inicioVenda: 'Início das Vendas',
	},
	Anfiteatro: {
		ingresso: 'Ingresso',
		ingressos: 'Ingressos',
		lote: 'Setor',
		lotes: 'Setores',
		tipoItem: 'Tipo de Ingresso',
		qtdItem: 'Quantidade de Setores',
		qtdIndividual: 'Quantidade de Ingressos',
		inicioVenda: 'Início das Vendas',
	},
	'Auditório/Sala de Conferência': {
		ingresso: 'Inscrição',
		ingressos: 'Inscrições',
		lote: 'Categoria',
		lotes: 'Categorias',
		tipoItem: 'Tipo de Inscrição',
		qtdItem: 'Quantidade de Categorias',
		qtdIndividual: 'Quantidade de Inscrições',
		inicioVenda: 'Início das Inscrições',
	},

	// === NOMENCLATURA PADRÃO (FALLBACK) ===
	default: {
		ingresso: 'Ingresso',
		ingressos: 'Ingressos',
		lote: 'Lote',
		lotes: 'Lotes',
		tipoItem: 'Tipo de Ingresso',
		qtdItem: 'Quantidade de Lotes',
		qtdIndividual: 'Quantidade de Ingressos',
		inicioVenda: 'Início das Vendas',
	},
};

/**
 * Classe principal para gerenciar nomenclatura de eventos.
 * Esta classe agora apenas define a estrutura e os métodos.
 * A página principal (Create.cshtml) será responsável por criar uma instância e inicializá-la.
 */
class NomenclaturaEventos {
	constructor() {
		this.nomenclaturaAtual = { ...nomenclaturas.default };
	}

	/**
	 * Inicializa o sistema de nomenclatura.
	 * Esta função deve ser chamada depois que o DOM estiver totalmente carregado.
	 */
	init() {
		this.indicarStatusAtivo();
		this.configurarEventListeners();
		this.aplicarNomenclaturaInicial();
	}

	/**
	 * Adiciona um indicador visual para confirmar que o script está ativo.
	 */
	indicarStatusAtivo() {
		const statusIndicator = document.getElementById('scriptStatus');
		if (statusIndicator) {
			statusIndicator.classList.add('active');
			statusIndicator.title = 'Script de nomenclatura ativo!';
		}
	}

	/**
	 * Configura os event listeners para mudanças nos selects
	 */
	configurarEventListeners() {
		const categoriaSelect = document.getElementById('categoriaEvento');
		const tipoSelect = document.getElementById('tipoEvento');
		const eventoGratuitoSelect = document.getElementById(
			'eventoGratuitoSelect'
		);

		if (categoriaSelect) {
			categoriaSelect.addEventListener('change', () =>
				this.atualizarNomenclatura()
			);
		}

		if (tipoSelect) {
			tipoSelect.addEventListener('change', () =>
				this.atualizarNomenclatura()
			);
		}

		if (eventoGratuitoSelect) {
			eventoGratuitoSelect.addEventListener('change', () => {
				setTimeout(() => this.atualizarLotesGerados(), 100);
			});
		}
	}

	/**
	 * Aplica a nomenclatura inicial ao carregar a página
	 */
	aplicarNomenclaturaInicial() {
		this.atualizarNomenclatura();
	}

	/**
	 * Obtém a nomenclatura apropriada baseada na categoria e tipo selecionados
	 */
	obterNomenclatura() {
		const categoria =
			document.getElementById('categoriaEvento')?.value || '';
		const tipoLocal = document.getElementById('tipoEvento')?.value || '';

		// Prioridade: tipoLocal > categoria > default
		return (
			nomenclaturas[tipoLocal] ||
			nomenclaturas[categoria] ||
			nomenclaturas.default
		);
	}

	/**
	 * Atualiza toda a nomenclatura da interface
	 */
	atualizarNomenclatura() {
		this.nomenclaturaAtual = this.obterNomenclatura();
		this.atualizarTitulosSecoes();
		this.atualizarLabels();
		this.atualizarSubtitulos();
		this.atualizarLotesGerados();
	}

	/**
	 * Atualiza os títulos das seções
	 */
	atualizarTitulosSecoes() {
		const titulosSecao = document.querySelectorAll('.form-section-title');
		titulosSecao.forEach((titulo) => {
			if (titulo.textContent.includes('Configuração de')) {
				titulo.textContent = `Configuração de ${this.nomenclaturaAtual.ingressos}`;
			}
		});
	}

	/**
	 * Atualiza os labels dos campos
	 */
	atualizarLabels() {
		const labelTipoItem = document.querySelector(
			'label[for="eventoGratuito"]'
		);
		if (labelTipoItem) {
			labelTipoItem.textContent = this.nomenclaturaAtual.tipoItem;
		}

		const labelQtdLotes = document.querySelector(
			'label[for="qtdlotesEvento"]'
		);
		if (labelQtdLotes) {
			labelQtdLotes.textContent = this.nomenclaturaAtual.qtdItem;
		}
	}

	/**
	 * Atualiza os subtítulos explicativos
	 */
	atualizarSubtitulos() {
		const subtitulo = document.querySelector(
			'#ticketConfigSection .form-section-subtitle'
		);
		if (subtitulo) {
			subtitulo.textContent = `Configure se o evento é gratuito ou pago e defina os ${this.nomenclaturaAtual.lotes.toLowerCase()}.`;
		}
	}

	/**
	 * Atualiza os lotes que foram gerados dinamicamente
	 */
	atualizarLotesGerados() {
		const lotesContainer = document.getElementById('Lotes');
		if (!lotesContainer) return;

		lotesContainer
			.querySelectorAll('.lote-item')
			.forEach((loteItem, index) => {
				this.atualizarLoteIndividual(loteItem, index);
			});
	}

	/**
	 * Atualiza um lote individual
	 */
	atualizarLoteIndividual(loteItem, index) {
		const loteHeader = loteItem.querySelector('.lote-header h4');
		if (loteHeader) {
			const isGratuito =
				document.getElementById('eventoGratuitoSelect')?.value ===
				'true';
			loteHeader.textContent = isGratuito
				? `${this.nomenclaturaAtual.ingresso} (Gratuito)`
				: `${this.nomenclaturaAtual.lote} ${index + 1}`;
		}
		this.atualizarLabelsLote(loteItem);
	}

	/**
	 * Atualiza os labels dentro de um lote específico de forma mais robusta.
	 */
	atualizarLabelsLote(loteItem) {
		const labels = loteItem.querySelectorAll('label');
		labels.forEach((label) => {
			const textoLabel = label.textContent;

			if (textoLabel.includes('Quantidade de')) {
				label.textContent = this.nomenclaturaAtual.qtdIndividual;
			} else if (textoLabel.includes('Preço do')) {
				label.textContent = `Preço do ${this.nomenclaturaAtual.ingresso} (R$)`;
			} else if (textoLabel.includes('Início d')) {
				label.textContent = this.nomenclaturaAtual.inicioVenda;
			}
		});
	}

	/**
	 * Hook para ser chamado quando novos lotes são gerados
	 */
	onLotesGerados() {
		setTimeout(() => this.atualizarLotesGerados(), 50);
	}
}
