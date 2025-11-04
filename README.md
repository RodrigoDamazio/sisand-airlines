# ✈️ Sisand Airlines – Sistema de Compra de Passagens

## 🧾 Sobre o Projeto
O **Sisand Airlines** é um sistema completo de reservas aéreas desenvolvido como parte de um teste de aptidão técnica.  
O sistema permite ao usuário visualizar voos disponíveis, reservar assentos, realizar login e acompanhar suas reservas.

---

## 🧩 Tecnologias Utilizadas

### 🔹 **Frontend**
- **Angular** (SPA - LTS)
- TypeScript
- HTML / SCSS modular
- RxJS
- JWT Authentication
- HTTP Interceptors
- UI responsiva e moderna

### 🔹 **Backend**
- **.NET 9 (C#)**
- ASP.NET Core MVC (sem minimal API)
- Dapper
- PostgreSQL
- Repository Pattern + Unit of Work
- JWT Authentication
- Envio de e-mails via SMTP (Mailhog)

### 🔹 **Infraestrutura**
- Docker e Docker Compose
- Mailhog (simulador de e-mail)
- PostgreSQL (banco principal)
- Volume persistente para dados

---

## ⚙️ Estrutura do Projeto

```bash
Sisand.Airlines/
├── sisand-airlines-frontend/       # Aplicação Angular SPA
├── Sisand.Airlines.Api/            # API principal .NET
├── Sisand.Airlines.Application/    # Camada de aplicação
├── Sisand.Airlines.Domain/         # Entidades e contratos (DDD)
├── Sisand.Airlines.Infrastructure/ # Repositórios e persistência (Dapper)
├── init_v1.1.sql                   # Script de criação e carga inicial do banco
├── docker-compose.yml              # Orquestração completa (API, Front, DB, Mailhog)
└── README.md


---

## ▶️ Como Executar o Projeto

### 📦 Pré-requisitos
- Docker e Docker Compose instalados
- Portas disponíveis:
  - API: **5300**
  - Frontend: **6540**
  - Mailhog: **8025**
  - Banco: **5432**

### 🧠 Passos

```bash
# Subir containers
docker-compose up --build -d

Aguarde até que todos os containers (api, frontend, db, mailhog) estejam prontos.

🌐 URLs Principais
Serviço	URL	Descrição
🧩 Frontend	http://localhost:6540
	Interface Angular SPA
⚙️ Backend (Swagger)	http://localhost:5300/swagger
	API e documentação
📬 Mailhog	http://localhost:8025
	Simulador de e-mails

👥 Funcionalidades Principais
✅ Fluxo do Cliente

Busca de voos por data e número de passageiros

Seleção de assento e reserva

Cadastro/login via e-mail e senha

Checkout com envio de e-mail de confirmação

Visualização e cancelamento de reservas

✅ Regras de Negócio

7 assentos por avião (5 econômicos + 2 primeira classe)

Voos a cada 3 horas, duração de 1h

Preços fixos: R$ 159,97 (econômica) e R$ 399,93 (primeira)

Restrição de reserva duplicada para mesmo assento

🧱 Padrões de Arquitetura

Domain-Driven Design (DDD)

Repository Pattern

Unit of Work Pattern

JWT Authentication

Clean Architecture

SPA (Single Page Application)