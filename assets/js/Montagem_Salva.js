document.addEventListener('DOMContentLoaded', () => {
  const buildsContainer = document.getElementById('builds-container');
  
  // Carrega montagens do LocalStorage
  function loadBuilds() {
    const savedBuilds = JSON.parse(localStorage.getItem('pcBuilds')) || [];
    
   if (savedBuilds.length === 0) {
  buildsContainer.innerHTML = `
    <div class="empty-state">
      <h3>Você ainda não tem montagens salvas</h3>
      <p>Comece criando sua primeira configuração personalizada</p>
      <a href="montagem.html" class="btn-primary">Criar nova montagem</a>
    </div>
  `;
  return;
}

    buildsContainer.innerHTML = savedBuilds.map((build, index) => `
      <div class="build-card">
        <h3>Montagem ${index + 1}</h3>
        <small>Salva em: ${new Date(build.date).toLocaleDateString()}</small>
        <ul class="build-parts">
          ${build.parts.map(part => `
            <li>${part.category}: <strong>${part.name}</strong></li>
          `).join('')}
        </ul>
        <button class="btn-delete" data-index="${index}">Excluir</button>
      </div>
    `).join('');

    // Adiciona eventos aos botões de excluir
    document.querySelectorAll('.btn-delete').forEach(btn => {
      btn.addEventListener('click', (e) => {
        const index = e.target.dataset.index;
        deleteBuild(index);
      });
    });
  }

  // Exclui uma montagem
  function deleteBuild(index) {
    const savedBuilds = JSON.parse(localStorage.getItem('pcBuilds')) || [];
    savedBuilds.splice(index, 1);
    localStorage.setItem('pcBuilds', JSON.stringify(savedBuilds));
    loadBuilds(); // Recarrega a lista
  }

  loadBuilds();
});