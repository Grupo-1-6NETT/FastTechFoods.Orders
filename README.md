# Fast Tech Foods - Orders

Este é um microsserviço desenvolvido em .NET Core 8 para gerenciar o cadastro de Pedidos. 
Este projeto é parte da solução para o Hackathon da Fase 5 do curso de pós graduação 6NETT na FIAP.

## Índice
- [Pré-requisitos](#pré-requisitos)
- [Configuração do Projeto](#configuração-do-projeto)
- [Eventos](#eventos)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)

## Pré-requisitos

- [Docker](https://www.docker.com/get-started/) e [Docker Compose](https://docs.docker.com/compose/install/) (necessário para executar o projeto)
- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) (somente para executar local)

## Configuração do Projeto

**1. Clone o repositório:**

   ```bash
   git https://github.com/Grupo-1-6NETT/FastTechFoods.Orders.git
   cd FastTechFoods.Orders
   ```

**2. Adicione configurações necessárias**

Adicione as credenciais do Postgres no appsettings do ambiente que estiver rodando.  
Por exempo, em ambiente de desenvolvimento, adicione a propriedade em `appsettings.Development.json`.  

```json
 "ConnectionStrings": {
    "DefaultConnection": "Host=batcave-server;Database=batbase;Username=alfred;Password=******;SSL Mode=Require;Trust Server Certificate=true"
  }
``` 

Adicione as credenciais do RabbitMQ

``` json
  "RabbitMQ": {
    "Hostname": "rabbitmq",
    "Username": "guest",
    "Password": "guest"    
  },
```


**3. Crie a imagem Docker:**

```bash
docker-compose up --build
```

---
## Eventos

### Pedido 
|Evento|Descrição|
|---|---|
|CriarPedido|Cria um pedido novo|
|GetByCliente|Consulta pedidos de um cliente|
|AlterarPedido|Altera itens um pedido novo|
|Confirmar|Confirma o pedido e envia para cozinha|
|Cancelar|Cancela um pedido não iniciado preparo|
|GetById|Localiza pedido informando ID|

### Produto Catálogo
|Evento|Descrição|
|---|---|
|ObterTodosProdutos|Obtem lista de produtos disponiveis|
|ObterProdutosPorCategoria|Obtem lista de produtos disponiveis|


---
## Tecnologias Utilizadas
- **ASP.NET Core 8** - Framework principal para desenvolvimento do serviço
- **Entity Framework Core** - ORM para manipulação do banco de dados
- **Postgres** - Banco de dados
- **RabbitMQ** - Message Broker
- **MassTransit** - Transporte de mensagens
- **Docker** - Criação de conteiners
