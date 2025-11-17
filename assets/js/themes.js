let botaoTemas = document.getElementById('botao-temas');
let body = document.querySelector('body');
let heroImage = document.getElementById('hero-image');
let root = document.querySelector(':root');

// Verifica o tema salvo ao carregar a página
document.addEventListener('DOMContentLoaded', () => {
    const temaSalvo = localStorage.getItem('tema');
    if (temaSalvo === 'light') {
        ativarTemaLight();
    }
});

// Alterna o tema e salva a preferência
botaoTemas.addEventListener('click', () => {
    if (body.classList.contains('light')) {
        desativarTemaLight();
        localStorage.setItem('tema', 'dark');
    } else {
        ativarTemaLight();
        localStorage.setItem('tema', 'light');
    }
});

// Funções para ativar/desativar o tema light
function ativarTemaLight() {
    botaoTemas.classList.add('light');
    body.classList.add('light');
    root.classList.add('light');
}

function desativarTemaLight() {
    botaoTemas.classList.remove('light');
    body.classList.remove('light');
    root.classList.remove('light');
}

// Menu mobile
document.addEventListener('DOMContentLoaded', () => {
  const menuToggle = document.createElement('div');
  menuToggle.className = 'menu-toggle';
  menuToggle.innerHTML = '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="3" y1="12" x2="21" y2="12"></line><line x1="3" y1="6" x2="21" y2="6"></line><line x1="3" y1="18" x2="21" y2="18"></line></svg>';
  
  const header = document.querySelector('header .container');
  if (header) {
    header.prepend(menuToggle);
    
    const nav = document.querySelector('.nav-links');
    menuToggle.addEventListener('click', () => {
      nav.classList.toggle('active');
      menuToggle.classList.toggle('active');
    });
  }
});