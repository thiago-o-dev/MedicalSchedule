# MedicalSchedule
Sistema de agendamento de consultas veterinárias. Trabalho acadêmico com foco em DDD (Domain-Driven Design) e microsserviços.

## Informação de Arquitetura
Essa aplicação funciona com microsserviços utilizando Aspire .NET adjunto de gateway feito com YARP, mensagerias com EasyNetQ (rabbitMq com contratos), Redis para cache, e utilizando o Postgres como db.

Implementamos o Keycloak para autentificação e mailhog para visualização de notificações.

Nossos services são:
Registry, Scheduling, Notifications, Payments, WhatsappBot.

## Como rodar:
Iniciar o docker desktop e rodar a aplicação backend pelo AppHost.

O front end pode ser rodado por docker, tem um docker file dentro da pasta do app, ou utilizando o flutter no seu próprio sistema.

# English

# MedicalSchedule
Appointment system for veterinary consulations. Academic project with focus in DDD (Domain-Driven Design) and microservices.

## Architecture Information
This application works with microservices using Aspire .NET alongside a gateway built with YARP, messaging with EasyNetQ (RabbitMQ with contracts), Redis for caching, and using Postgres as the database.

We implemented Keycloak for authentication and Mailhog for displaying notifications.

Our services are:

Registry, Scheduling, Notifications, Payments, WhatsApp Bot.

## How to run:
Start docker desktop and run the aplication backend by the AppHost.

The front-end can be run with docker, as there is a dockerfile in the app folder, or by using flutter on your own system.
