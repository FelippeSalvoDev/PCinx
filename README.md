# 🖥️ PCinx - Monte seu PC Gamer

PCinx é uma plataforma web para montagem de computadores gamers com verificação automática de compatibilidade entre componentes. O sistema permite escolher peças, validar compatibilidade e salvar montagens para consulta posterior.

## ✨ Funcionalidades

- **Catálogo de Componentes**: Mais de 18.1k peças disponíveis (CPU, GPU, RAM, Placa Mãe, Fontes, Gabinetes, Coolers, HD/SSD)
- **Montagem Personalizada**: Selecione as peças que desejar sem pré-seleções
- **Validação de Compatibilidade**: Verificação automática de:
  - Compatibilidade de socket (CPU x Placa Mãe)
  - Tipo de memória RAM (RAM x Placa Mãe)
  - Potência da fonte (PSU)
  - Componentes essenciais ausentes
- **Salvar Montagens**: Salve suas configurações e acesse depois com um código único
- **Visualizar Montagens Salvas**: Consulte suas montagens anteriores
- **Tema Escuro/Claro**: Alternância entre temas para melhor experiência
- **Agregação de Preços**: Preços agregados de várias lojas brasileiras

## 🛠️ Tecnologias

### Frontend
- **HTML5** - Estrutura das páginas
- **CSS3** - Estilização e responsividade
- **JavaScript (Vanilla)** - Interatividade e lógica do cliente

### Backend
- **.NET 8.0** - Framework da API
- **C#** - Linguagem de programação
- **ASP.NET Core** - Web API RESTful

### Infraestrutura
- **Docker** - Containerização para deploy

## 📁 Estrutura do Projeto

```
PCinx-main/
├── assets/
│   ├── css/           # Estilos CSS
│   │   ├── Styles.css
│   │   ├── montagem.css
│   │   ├── pecas.css
│   │   └── Montagem_Salva.css
│   ├── js/            # Scripts JavaScript
│   │   ├── themes.js
│   │   ├── pecas.js
│   │   ├── montagem.js
│   │   └── Montagem_Salva.js
│   └── img/           # Imagens e assets
├── Data/
│   ├── parts.json     # Catálogo de componentes
│   └── builds/        # Montagens salvas
├── index.html         # Página inicial
├── pecas.html         # Página de componentes
├── montagem.html      # Página de montagem
├── Montagem_Salva.html # Página de montagens salvas
├── Program.cs         # Código da API
├── pcinx-api.csproj   # Arquivo de projeto .NET
└── Dockerfile         # Configuração Docker
```

## 🚀 Como Executar

### Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Navegador web moderno
- (Opcional) Docker para containerização

### Executando Localmente

1. **Clone o repositório**
   ```bash
   git clone <url-do-repositorio>
   cd PCinx-main
   ```

2. **Execute a API**
   ```bash
   dotnet run --project pcinx-api.csproj
   ```
   
   A API estará disponível em `http://localhost:5000` ou `https://localhost:5001`

3. **Abra o Frontend**
   - Abra o arquivo `index.html` no navegador, ou
   - Configure um servidor web local (ex: Live Server no VS Code)

### Executando com Docker

1. **Build da imagem**
   ```bash
   docker build -t pcinx-api .
   ```

2. **Execute o container**
   ```bash
   docker run -p 8080:8080 pcinx-api
   ```

   A API estará disponível em `http://localhost:8080`

## 📡 API Endpoints

### `GET /`
Retorna informações básicas da API.

**Resposta:**
```json
{
  "name": "PCinx API",
  "parts": 18100
}
```

### `GET /api/parts?category={categoria}`
Retorna lista de componentes. Pode ser filtrada por categoria.

**Parâmetros:**
- `category` (opcional): Filtra por categoria (CPU, GPU, RAM, Motherboard, Storage, PSU, Case, etc.)

**Exemplo:**
```bash
GET /api/parts?category=CPU
```

**Resposta:**
```json
[
  {
    "id": "cpu-r5-5600",
    "category": "CPU",
    "name": "AMD Ryzen 5 5600",
    "brand": "AMD",
    "price": 799.9,
    "attributes": {
      "Socket": "AM4",
      "TdpW": "65",
      "Cores": "6",
      "Threads": "12",
      "Clock": "3.5GHz",
      "Boost": "4.4GHz"
    }
  }
]
```

### `POST /api/build/validate`
Valida a compatibilidade de uma montagem.

**Body:**
```json
{
  "partIds": ["cpu-r5-5600", "mobo-b450", "ram-ddr4-16gb"]
}
```

**Resposta:**
```json
{
  "messages": [
    {
      "level": "ok",
      "text": "✅ Compatibilidade perfeita!"
    }
  ]
}
```

**Níveis de mensagem:**
- `ok`: Compatibilidade perfeita
- `warning`: Avisos e observações
- `error`: Erros de compatibilidade

### `POST /api/build/save`
Salva uma montagem e retorna um código único.

**Body:**
```json
{
  "partIds": ["cpu-r5-5600", "mobo-b450", "ram-ddr4-16gb"]
}
```

**Resposta:**
```json
{
  "code": "A1B2C3D4"
}
```

## 🔍 Validações de Compatibilidade

O sistema verifica automaticamente:

1. **Componentes Essenciais**: CPU, Placa Mãe, RAM, Armazenamento e Fonte
2. **Socket CPU x Placa Mãe**: Verifica se o socket do processador é compatível com a placa mãe
3. **Tipo de RAM**: Verifica se o tipo de memória (DDR4, DDR5, etc.) é suportado pela placa mãe
4. **Potência da Fonte**: Calcula o consumo total (CPU + GPU + margem de segurança) e verifica se a fonte é suficiente

## 🎨 Páginas

- **Home (`index.html`)**: Página inicial com apresentação do projeto e categorias
- **Componentes (`pecas.html`)**: Catálogo completo de componentes com filtros
- **Montagem (`montagem.html`)**: Interface para montar seu PC e validar compatibilidade
- **Montagens Salvas (`Montagem_Salva.html`)**: Visualizar e gerenciar montagens salvas

## 🎯 Categorias de Componentes

- **CPU** - Processadores
- **GPU** - Placas de Vídeo
- **RAM** - Memórias RAM
- **Motherboard** - Placas Mãe
- **Storage** - HD/SSD
- **PSU** - Fontes de Alimentação
- **Case** - Gabinetes
- **Cooler** - Coolers

## 📝 Estrutura de Dados

### Componente (Part)
```json
{
  "id": "string",
  "category": "string",
  "name": "string",
  "brand": "string",
  "price": 0.0,
  "attributes": {
    "Socket": "string",
    "TdpW": "string",
    "Type": "string",
    "WattageW": "string"
  }
}
```

## 🔧 Configuração

### CORS
A API está configurada para aceitar requisições de qualquer origem (desenvolvimento). Para produção, ajuste as políticas CORS em `Program.cs`.

### Dados
Os componentes são carregados do arquivo `Data/parts.json`. As montagens salvas são armazenadas em `Data/builds/` com arquivos JSON nomeados pelo código da montagem.

## 🐳 Deploy

O projeto inclui um `Dockerfile` para facilitar o deploy em plataformas como:
- Render
- Heroku
- AWS
- Azure
- Outras plataformas que suportam Docker

## 📄 Licença

Este projeto está sob licença proprietária. Todos os direitos reservados.

## 👥 Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues e pull requests.

## 📧 Contato

- Instagram: @PCinx
- Discord: [Link do Discord]
- Website: [URL do site]

---

**PCinx** — Inspirado por caos e tecnologia 💻✨


