document.addEventListener('DOMContentLoaded', () => {
    // Elementos da interface
    const btnConnect = document.getElementById('btnConnect');
    const btnValidate = document.getElementById('btnValidate');
    const btnSave = document.getElementById('btnSave');
    const apiBaseInput = document.getElementById('apiBase');
    const grid = document.getElementById('grid');
    const issuesList = document.getElementById('issues');
    const priceElement = document.getElementById('price');
    
   // URL fixa do backend no Render
let apiBaseUrl = 'https://pcinx.onrender.com';
let selectedParts = [];

// Carregar peças automaticamente quando a página abrir
    loadParts(); 
    // Carregar peças da API
    async function loadParts() {
    try {
        console.log('Carregando peças de:', `${apiBaseUrl}/api/parts`);
        const response = await fetch(`${apiBaseUrl}/api/parts`);
        
        if (!response.ok) {
            const errorDetails = await response.text();
            throw new Error(`HTTP ${response.status}: ${errorDetails}`);
        }
        
        const parts = await response.json();
        console.log('Peças recebidas:', parts);
        renderPartsGrid(parts);
    } catch (error) {
        console.error('Erro completo:', error);
        alert('Erro ao carregar peças. Verifique o console para detalhes.');
    }
}

     const categoryIcons = {
    'CPU': 'https://img.icons8.com/ios-filled/100/ff2b8d/processor.png',
    'GPU': 'https://img.icons8.com/ios-filled/100/ff2b8d/video-card.png',
    'RAM': 'https://img.icons8.com/ios-filled/100/ff2b8d/memory-slot.png',
    'Motherboard': 'https://img.icons8.com/?size=100&id=35849&format=png&color=ff2b8d',
    'PSU': 'https://img.icons8.com/?size=100&id=9267&format=png&color=ff2b8d',
    'Case': 'https://img.icons8.com/?size=100&id=9928&format=png&color=ff2b8d',
    'Cooler': 'https://img.icons8.com/?size=100&id=tUNhVumwax7p&format=png&color=ff2b8d',
    'Storage': 'https://img.icons8.com/?size=100&id=9937&format=png&color=ff2b8d'
};
    
    // Renderizar grade de peças
    function renderPartsGrid(parts) {
    // Agrupar por categoria
    const categories = [...new Set(parts.map(p => p.category))];

    const categoryNames = {
    'CPU': 'Processador',
    'GPU': 'Placa de Vídeo', 
    'RAM': 'Memória RAM',
    'Motherboard': 'Placa-mãe',
    'PSU': 'Fonte',
    'Case': 'Gabinete',
    'Cooler': 'Cooler',
    'Storage': 'HD/SSD'
};
    
    grid.innerHTML = categories.map(category => {
        const categoryParts = parts.filter(p => p.category === category);
        const iconUrl = categoryIcons[category] || ''; // Pega o ícone correspondente à categoria
        
        return `
            <div class="slot">
                <div class="slot-header">
                    ${iconUrl ? `<img src="${iconUrl}" alt="${category}" class="category-icon">` : ''}
                    <h4>${categoryNames[category] || category}</h4>
                </div>
                <select id="select-${category}" data-category="${category}">
                    <option value="">Selecione...</option>
                    ${categoryParts.map(part => `
                        <option value="${part.id}">${part.name} (R$ ${part.price.toFixed(2)})</option>
                    `).join('')}
                </select>
                <div class="meta" id="meta-${category}"></div>
            </div>
        `;
    }).join('');
        
        // Adicionar listeners aos selects
        categories.forEach(category => {
            document.getElementById(`select-${category}`).addEventListener('change', (e) => {
                const partId = e.target.value;
                if (!partId) {
                    selectedParts = selectedParts.filter(p => p.category !== category);
                    updateSelection();
                    return;
                }
                
                const part = parts.find(p => p.id === partId);
                if (part) {
                    selectedParts = selectedParts.filter(p => p.category !== category);
                    selectedParts.push(part);
                    updateSelection();
                }
            });
        });
    }
    
    // Atualizar seleção e preço total
    function updateSelection() {
        // Calcular preço total
        const totalPrice = selectedParts.reduce((sum, part) => sum + part.price, 0);
        priceElement.textContent = `Total: R$ ${totalPrice.toFixed(2)}`;
        
        // Atualizar metadados
        selectedParts.forEach(part => {
            const metaElement = document.getElementById(`meta-${part.category}`);
            if (metaElement) {
                metaElement.innerHTML = `
                    <div>${part.brand || ''}</div>
                    ${part.attributes ? `
                        <div class="chips">
                            ${Object.entries(part.attributes).map(([key, value]) => `
                                <span class="chip">${key}: ${value}</span>
                            `).join('')}
                        </div>
                    ` : ''}
                `;
            }
        });
    }
    
    // Validar compatibilidade
    btnValidate.addEventListener('click', async () => {
        if (selectedParts.length === 0) {
            alert('Selecione pelo menos uma peça para validar');
            return;
        }
        
        try {
            const response = await fetch(`${apiBaseUrl}/api/build/validate`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ partIds: selectedParts.map(p => p.id) })
            });
            
            if (!response.ok) throw new Error('Falha na validação');
            
            const result = await response.json();
            renderValidationResults(result.messages);
        } catch (error) {
            console.error('Erro:', error);
            alert('Erro ao validar: ' + error.message);
        }
    });
    
    // Renderizar resultados da validação
    function renderValidationResults(messages) {
    issuesList.innerHTML = messages.map(msg => {
        let icon = '';
        if (msg.level === 'error') icon = '❌';
        else if (msg.level === 'warning') icon = '⚠️';
        else if (msg.level === 'ok') icon = '✅';
        
        return `<li class="${msg.level}">${icon} ${msg.text}</li>`;
    }).join('');
}
    
    // Salvar montagem
    function saveBuildToLocalStorage() {
  if (selectedParts.length === 0) return;

  const savedBuilds = JSON.parse(localStorage.getItem('pcBuilds')) || [];
  
  savedBuilds.push({
    date: new Date().toISOString(),
    parts: selectedParts.map(part => ({
      category: part.category,
      name: part.name,
      price: part.price
    }))
  });

  localStorage.setItem('pcBuilds', JSON.stringify(savedBuilds));
  alert('Montagem salva em "Montagens Salvas"!');
}

// evento do btnSave:
btnSave.addEventListener('click', () => {
  saveBuildToLocalStorage();
});
});
