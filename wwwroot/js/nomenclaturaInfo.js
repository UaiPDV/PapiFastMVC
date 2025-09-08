/**
 * Nomenclatura Dinâmica para a Página de Ingressos (Index.cshtml)
 * v3.2 - Corrigido e com Melhor Diagnóstico
 *
 * Este script adapta toda a terminologia da página (incluindo o título da aba)
 * com base na categoria do evento, tipo de local e se é gratuito.
 */
document.addEventListener('DOMContentLoaded', () => {
	// Mapeamento central de todas as nomenclaturas possíveis.
	// A prioridade de aplicação é: Tipo de Local > Categoria do Evento > Padrão.
	const nomenclaturas = {
		// --- MAPEAMENTOS POR CATEGORIA DO EVENTO ---
		Aniversário: {
			ingresso: 'Convite',
			ingressos: 'Convites',
			lote: 'Grupo',
			tipoVendaHeader: 'Tipo de Distribuição',
			vendaAcao: 'Distribuir',
		},
		Casamento: {
			ingresso: 'Convite',
			ingressos: 'Convites',
			lote: 'Mesa',
			tipoVendaHeader: 'Tipo de Confirmação',
			vendaAcao: 'Distribuir',
		},
		Bodas: {
			ingresso: 'Convite',
			ingressos: 'Convites',
			lote: 'Mesa',
			tipoVendaHeader: 'Tipo de Confirmação',
			vendaAcao: 'Distribuir',
		},
		'Chá de Bebê': {
			ingresso: 'Convite',
			ingressos: 'Convites',
			lote: 'Turma',
			tipoVendaHeader: 'Tipo de Distribuição',
			vendaAcao: 'Distribuir',
		},
		'Chá de Panela': {
			ingresso: 'Convite',
			ingressos: 'Convites',
			lote: 'Turma',
			tipoVendaHeader: 'Tipo de Distribuição',
			vendaAcao: 'Distribuir',
		},
		'Congresso, Conferência, Seminário': {
			ingresso: 'Inscrição',
			ingressos: 'Inscrições',
			lote: 'Categoria',
			tipoVendaHeader: 'Tipo de Inscrição',
			vendaAcao: 'Inscrever',
		},
		'Curso, Workshop': {
			ingresso: 'Inscrição',
			ingressos: 'Inscrições',
			lote: 'Turma',
			tipoVendaHeader: 'Tipo de Inscrição',
			vendaAcao: 'Inscrever',
		},
		'Show, Música, Festas': {
			ingresso: 'Ingresso',
			ingressos: 'Ingressos',
			lote: 'Setor',
			tipoVendaHeader: 'Tipo de Venda',
			vendaAcao: 'Vender',
		},
		Esportivo: {
			ingresso: 'Ingresso',
			ingressos: 'Ingressos',
			lote: 'Setor',
			tipoVendaHeader: 'Tipo de Venda',
			vendaAcao: 'Vender',
		},
		'E-Sports': {
			ingresso: 'Ingresso',
			ingressos: 'Ingressos',
			lote: 'Categoria',
			tipoVendaHeader: 'Tipo de Venda',
			vendaAcao: 'Vender',
		},

		// --- MAPEAMENTOS POR TIPO DE LOCAL (TÊM PRIORIDADE) ---
		Hotel: {
			ingresso: 'Reserva',
			ingressos: 'Reservas',
			lote: 'Pacote',
			tipoVendaHeader: 'Tipo de Reserva',
			vendaAcao: 'Reservar',
		},
		'Hotel Fazenda': {
			ingresso: 'Hospedagem',
			ingressos: 'Hospedagens',
			lote: 'Pacote',
			tipoVendaHeader: 'Tipo de Hospedagem',
			vendaAcao: 'Reservar',
		},
		Pousada: {
			ingresso: 'Reserva',
			ingressos: 'Reservas',
			lote: 'Acomodação',
			tipoVendaHeader: 'Tipo de Reserva',
			vendaAcao: 'Reservar',
		},
		Restaurante: {
			ingresso: 'Reserva',
			ingressos: 'Reservas',
			lote: 'Mesa',
			tipoVendaHeader: 'Tipo de Reserva',
			vendaAcao: 'Reservar',
		},
		'Bar/Pub': {
			ingresso: 'Reserva',
			ingressos: 'Reservas',
			lote: 'Mesa',
			tipoVendaHeader: 'Tipo de Reserva',
			vendaAcao: 'Reservar',
		},
		'Day Use': {
			ingresso: 'Reserva',
			ingressos: 'Reservas',
			lote: 'Pacote',
			tipoVendaHeader: 'Tipo de Reserva',
			vendaAcao: 'Reservar',
		},
		'Salão de festas': {
			ingresso: 'Convite',
			ingressos: 'Convites',
			lote: 'Mesa',
			tipoVendaHeader: 'Tipo de Distribuição',
			vendaAcao: 'Distribuir',
		},
		'Casa/Condomínio': {
			ingresso: 'Convite',
			ingressos: 'Convites',
			lote: 'Grupo',
			tipoVendaHeader: 'Tipo de Distribuição',
			vendaAcao: 'Distribuir',
		},
		Cinema: {
			ingresso: 'Ingresso',
			ingressos: 'Ingressos',
			lote: 'Sessão',
			tipoVendaHeader: 'Tipo de Venda',
			vendaAcao: 'Vender',
		},
		Anfiteatro: {
			ingresso: 'Ingresso',
			ingressos: 'Ingressos',
			lote: 'Setor',
			tipoVendaHeader: 'Tipo de Venda',
			vendaAcao: 'Vender',
		},
		'Auditório/Sala de Conferência': {
			ingresso: 'Inscrição',
			ingressos: 'Inscrições',
			lote: 'Categoria',
			tipoVendaHeader: 'Tipo de Inscrição',
			vendaAcao: 'Inscrever',
		},

		// --- NOMENCLATURA PADRÃO (FALLBACK) ---
		default: {
			ingresso: 'Ingresso',
			ingressos: 'Ingressos',
			lote: 'Lote',
			tipoVendaHeader: 'Tipo de Venda',
			vendaAcao: 'Vender',
		},
	};

	// 1. Obtém o container principal e os dados do evento.
	const pageContainer = document.getElementById('tickets-page');
	if (!pageContainer) {
		console.error(
			'ERRO: Container #tickets-page não encontrado. O script não pode ser executado.'
		);
		return;
	}

	// Adicionado .trim() para remover espaços extras que possam vir do banco de dados.
	const categoria = (pageContainer.dataset.categoriaEvento || '').trim();
	const tipoLocal = (pageContainer.dataset.tipoEvento || '').trim();
	const isGratuito = pageContainer.dataset.eventoGratuito === 'true';

	// Log de diagnóstico melhorado.
	console.log(
		`[Nomenclatura] DADOS RECEBIDOS -> Categoria: "${categoria}", Tipo de Local: "${tipoLocal}", Gratuito: ${isGratuito}`
	);

	// 2. Define os termos corretos com base na prioridade.
	const termos =
		nomenclaturas[tipoLocal] ||
		nomenclaturas[categoria] ||
		nomenclaturas.default;
	console.log('[Nomenclatura] TERMOS APLICADOS:', termos);

	// 3. Atualiza o título da aba do navegador para "papifast".
	document.title = `${
		termos.ingressos.charAt(0).toUpperCase() + termos.ingressos.slice(1)
	} - PapiFast`;

	// 4. Atualiza todos os elementos marcados com 'data-nomenclatura'.
	document.querySelectorAll('[data-nomenclatura]').forEach((el) => {
		const key = el.dataset.nomenclatura;
		const newText = termos[key];

		if (newText) {
			// Mantém a capitalização original do elemento (primeira letra maiúscula).
			const isCapitalized =
				el.textContent.length > 0 &&
				el.textContent[0] === el.textContent[0].toUpperCase();
			el.textContent = isCapitalized
				? newText.charAt(0).toUpperCase() + newText.slice(1)
				: newText;
		}
	});

	// 5. Aplica lógicas específicas para eventos gratuitos (sobrescreve o passo 4 se necessário).
	if (isGratuito) {
		const spanAcao = document.querySelector(
			'[data-nomenclatura="vendaAcao"]'
		);
		if (spanAcao) {
			spanAcao.textContent = 'Distribuir'; // Ação principal vira "Distribuir"
		}

		const tipoVendaHeader = document.querySelector(
			'[data-nomenclatura="tipoVendaHeader"]'
		);
		if (tipoVendaHeader) {
			tipoVendaHeader.textContent = 'Tipo de Distribuição'; // Cabeçalho da tabela
		}
	}
});
