/**
 * Nomenclatura Dinâmica para a Página de Ingressos (Index.cshtml)
 * v3.4 - Simplificado o cabeçalho da tabela de tipo de venda.
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
			tipoVendaHeader: 'Distribuição',
			vendaAcao: 'Distribuir',
		},
		Casamento: {
			ingresso: 'Convite',
			ingressos: 'Convites',
			lote: 'Mesa',
			tipoVendaHeader: 'Confirmação',
			vendaAcao: 'Distribuir',
		},
		Bodas: {
			ingresso: 'Convite',
			ingressos: 'Convites',
			lote: 'Mesa',
			tipoVendaHeader: 'Confirmação',
			vendaAcao: 'Distribuir',
		},
		'Chá de Bebê': {
			ingresso: 'Convite',
			ingressos: 'Convites',
			lote: 'Turma',
			tipoVendaHeader: 'Distribuição',
			vendaAcao: 'Distribuir',
		},
		'Chá de Panela': {
			ingresso: 'Convite',
			ingressos: 'Convites',
			lote: 'Turma',
			tipoVendaHeader: 'Distribuição',
			vendaAcao: 'Distribuir',
		},
		'Congresso, Conferência, Seminário': {
			ingresso: 'Inscrição',
			ingressos: 'Inscrições',
			lote: 'Categoria',
			tipoVendaHeader: 'Inscrição',
			vendaAcao: 'Inscrever',
		},
		'Curso, Workshop': {
			ingresso: 'Inscrição',
			ingressos: 'Inscrições',
			lote: 'Turma',
			tipoVendaHeader: 'Inscrição',
			vendaAcao: 'Inscrever',
		},
		'Show, Música, Festas': {
			ingresso: 'Ingresso',
			ingressos: 'Ingressos',
			lote: 'Setor',
			tipoVendaHeader: 'Venda',
			vendaAcao: 'Vender',
		},
		Esportivo: {
			ingresso: 'Ingresso',
			ingressos: 'Ingressos',
			lote: 'Setor',
			tipoVendaHeader: 'Venda',
			vendaAcao: 'Vender',
		},
		'E-Sports': {
			ingresso: 'Ingresso',
			ingressos: 'Ingressos',
			lote: 'Categoria',
			tipoVendaHeader: 'Venda',
			vendaAcao: 'Vender',
		},

		// --- MAPEAMENTOS POR TIPO DE LOCAL (TÊM PRIORIDADE) ---
		Hotel: {
			ingresso: 'Reserva',
			ingressos: 'Reservas',
			lote: 'Pacote',
			tipoVendaHeader: 'Reserva',
			vendaAcao: 'Reservar',
		},
		'Hotel Fazenda': {
			ingresso: 'Hospedagem',
			ingressos: 'Hospedagens',
			lote: 'Pacote',
			tipoVendaHeader: 'Hospedagem',
			vendaAcao: 'Reservar',
		},
		Pousada: {
			ingresso: 'Reserva',
			ingressos: 'Reservas',
			lote: 'Acomodação',
			tipoVendaHeader: 'Reserva',
			vendaAcao: 'Reservar',
		},
		Restaurante: {
			ingresso: 'Reserva',
			ingressos: 'Reservas',
			lote: 'Mesa',
			tipoVendaHeader: 'Reserva',
			vendaAcao: 'Reservar',
		},
		'Bar/Pub': {
			ingresso: 'Reserva',
			ingressos: 'Reservas',
			lote: 'Mesa',
			tipoVendaHeader: 'Reserva',
			vendaAcao: 'Reservar',
		},
		'Day Use': {
			ingresso: 'Reserva',
			ingressos: 'Reservas',
			lote: 'Pacote',
			tipoVendaHeader: 'Reserva',
			vendaAcao: 'Reservar',
		},
		'Salão de festas': {
			ingresso: 'Convite',
			ingressos: 'Convites',
			lote: 'Mesa',
			tipoVendaHeader: 'Distribuição',
			vendaAcao: 'Distribuir',
		},
		'Casa/Condomínio': {
			ingresso: 'Convite',
			ingressos: 'Convites',
			lote: 'Grupo',
			tipoVendaHeader: 'Distribuição',
			vendaAcao: 'Distribuir',
		},
		Cinema: {
			ingresso: 'Ingresso',
			ingressos: 'Ingressos',
			lote: 'Sessão',
			tipoVendaHeader: 'Venda',
			vendaAcao: 'Vender',
		},
		Anfiteatro: {
			ingresso: 'Ingresso',
			ingressos: 'Ingressos',
			lote: 'Setor',
			tipoVendaHeader: 'Venda',
			vendaAcao: 'Vender',
		},
		'Auditório/Sala de Conferência': {
			ingresso: 'Inscrição',
			ingressos: 'Inscrições',
			lote: 'Categoria',
			tipoVendaHeader: 'Inscrição',
			vendaAcao: 'Inscrever',
		},

		// --- NOMENCLATURA PADRÃO (FALLBACK) ---
		default: {
			ingresso: 'Ingresso',
			ingressos: 'Ingressos',
			lote: 'Lote',
			tipoVendaHeader: 'Venda',
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

	const categoria = (pageContainer.dataset.categoriaEvento || '').trim();
	const tipoLocal = (pageContainer.dataset.tipoEvento || '').trim();
	const isGratuito = pageContainer.dataset.eventoGratuito === 'true';

	console.log(
		`[Nomenclatura] DADOS RECEBIDOS -> Categoria: "${categoria}", Tipo de Local: "${tipoLocal}", Gratuito: ${isGratuito}`
	);

	// 2. Define os termos corretos com base na prioridade.
	const termos =
		nomenclaturas[tipoLocal] ||
		nomenclaturas[categoria] ||
		nomenclaturas.default;
	console.log('[Nomenclatura] TERMOS APLICADOS:', termos);

	// 3. Atualiza o título da aba do navegador.
	document.title = `${
		termos.ingressos.charAt(0).toUpperCase() + termos.ingressos.slice(1)
	} - PapiFast`;

	// 4. Atualiza todos os elementos marcados com 'data-nomenclatura'.
	document.querySelectorAll('[data-nomenclatura]').forEach((el) => {
		const key = el.dataset.nomenclatura;
		const newText = termos[key];

		if (newText) {
			const isCapitalized =
				el.textContent.length > 0 &&
				el.textContent[0] === el.textContent[0].toUpperCase();
			el.textContent = isCapitalized
				? newText.charAt(0).toUpperCase() + newText.slice(1)
				: newText;
		}
	});

	// 5. Aplica lógicas específicas para eventos gratuitos.
	if (isGratuito) {
		const spanAcao = document.querySelector(
			'[data-nomenclatura="vendaAcao"]'
		);
		if (spanAcao) {
			spanAcao.textContent = 'Distribuir';
		}

		const tipoVendaHeader = document.querySelector(
			'[data-nomenclatura="tipoVendaHeader"]'
		);
		if (tipoVendaHeader) {
			tipoVendaHeader.textContent = 'Distribuição';
		}

		// --- MUDANÇA APLICADA ---
		// Altera o status de "Aguardando Venda" para "Aguardando Distribuição"
		document
			.querySelectorAll('[data-nomenclatura="statusAguardando"]')
			.forEach((el) => {
				el.textContent = 'Aguardando Distribuição';
			});

		// Altera o tipo de venda "Não vendido!" para "Não distribuído!"
		document.querySelectorAll('.status-sale-type').forEach((el) => {
			if (el.textContent.trim() === 'Não vendido!') {
				el.textContent = 'Não distribuído!';
			}
		});
		// --- FIM DA MUDANÇA ---
	}
});
