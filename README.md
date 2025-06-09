# 🎓 Edu Trailblaze BE

[![Edu-trailblaze-CD-Main](https://github.com/Edu-Trailblaze/Edu-Trailblaze-BE/actions/workflows/main_edu-trailblaze.yml/badge.svg)](https://github.com/Edu-Trailblaze/Edu-Trailblaze-BE/actions/workflows/main_edu-trailblaze.yml)
![MIT License](https://img.shields.io/badge/License-MIT-yellow.svg)
[![CodeQL](https://github.com/Edu-Trailblaze/Edu-Trailblaze-BE/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/Edu-Trailblaze/Edu-Trailblaze-BE/actions/workflows/github-code-scanning/codeql)

![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-5C2D91?style=for-the-badge&logo=dot-net&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Identity](https://img.shields.io/badge/Identity-0078D4?style=for-the-badge&logo=asp.net&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white)
![Elasticsearch](https://img.shields.io/badge/Elasticsearch-005571?style=for-the-badge&logo=elasticsearch&logoColor=white)

---

## 🧠 Overview

**Edu Trailblaze** is an intelligent online course platform that supports **students**, **instructors**, and **administrators**. It follows a **monolith + microservices** hybrid architecture for flexibility and scalability.

### Key Highlights

- ✅ **Monolithic core** (.NET API with Identity, SignalR, Hangfire)
- 🚪 **Ocelot API Gateway** for routing/auth/authentication
- 📦 **Cart Microservice** (CQRS, MediatR, Redis)
- 🔄 **Event-Driven Architecture** with RabbitMQ
- 📚 **Reusable shared libraries**: Logging, Contracts, EventBus, Seedwork
- 🧠 **AI integrations** for intelligent recommendations
- 💬 **Real-time updates** via SignalR
- 🧵 **Background tasks** using Hangfire
- 🔍 **Search & analytics** powered by Elasticsearch

---

## 🧩 Features

- **Authentication & Authorization**
  - Secure login via Identity + JWT
  - Role-based access for admin, instructors, students

- **Course Management**
  - Create and manage courses, materials, pricing
  - Tagging system powered by AI
  - Real-time approval flow

- **AI-Powered Tagging**
  - Suggests course tags before publishing using custom-trained models

- **Cart System**
  - Microservice with independent DB, Redis cache, and event sourcing

- **Background Jobs**
  - Hangfire manages email, notifications, AI training, etc.

- **Real-Time Events**
  - SignalR for live notifications, chat, instructor alerts

- **Event Bus**
  - RabbitMQ used for pub/sub across services

---

## 👥 Roles & Capabilities

| Role        | Capabilities                                                                 |
|-------------|------------------------------------------------------------------------------|
| **Student**    | Enroll in courses, track progress, get real-time updates.                  |
| **Instructor** | Create courses, update materials, handle feedback.                         |
| **Admin**      | Manage users, approve tags, monitor platform-wide activities.              |

---

## 🛠️ Tech Stack Summary

| Layer                 | Technologies Used                                                   |
|-----------------------|---------------------------------------------------------------------|
| **API Gateway**       | Ocelot                                                              |
| **Monolithic Core**   | ASP.NET Core, EF Core, Hangfire, SignalR                           |
| **Microservices**     | ASP.NET Core, MediatR, CQRS, Redis                                  |
| **Search Engine**     | Elasticsearch                                                       |
| **Message Broker**    | RabbitMQ                                                            |
| **Storage**           | SQL Server, Redis                                                   |
| **AI Integration**    | Custom ML/LLM models (tag suggestion, prediction engines)          |
| **External Services** | Cloudinary, Payment Gateways, ClamAV                               |

---

## 📬 Contact

📧 [edutrailblaze@gmail.com](mailto:edutrailblaze@gmail.com)

---

© 2025 Edu Trailblaze – Open-source & MIT Licensed