# Show Biz II - TV Production Management App

Welcome to the Show Biz II repository! This advanced web application, developed for the WEB524 course assignment, is designed to streamline TV production management processes. It boasts rich text support, multimedia item uploads, and role-based access control, ensuring efficient and organized management of TV series production elements.

## 🚀 Key Features

- **User Roles**: Supports various roles - Executive, Coordinator, Clerk, and Admin - each with specific responsibilities and access levels in TV production.
- **Data Management**: Seamlessly manage actors, TV series (shows), and episodes. Includes functionalities for adding, editing, and viewing details.
- **Rich Text Support**: Enhances actor biographies, show premises, and episode descriptions with rich text capabilities.
- **Media Items**: Facilitates the uploading of media items, including PDF documents, images, audio, and videos.
- **Azure Integration**: The application is hosted on Microsoft Azure, ensuring robust and reliable access.

## 📚 Assignment Overview

This repository represents a comprehensive assignment from a web programming course, showcasing the creation of a web application using the ASP.NET framework. The application focuses on customizing appearance, designing model classes, configuring security, and deploying to Azure.

## 👨‍💻 Technology Stack

- **ASP.NET**: For building the web application framework.
- **C#**: Server-side logic and functionality.
- **Entity Framework**: Efficient data management and ORM.
- **Azure**: Hosting the application for accessibility and reliability.
- **Bootstrap**: Styling for a professional and responsive user interface.
- **AutoMapper**: Mapping between view models and design models for efficient data handling.

## 📂 Project Structure

- **Data Models**: Design model classes for entities like Actor, Show, Episode, and Genre.
- **Data Initialization**: Methods in the Manager class for initial data loading (roles, genres, actors, shows, episodes).
- **Controller Actions**: Varied controller actions for different functionalities, with proper validation and authorization.
- **Security**: Configured role claims and user authentication for secure access control.
- **Azure Deployment**: The application is live at [dbaksheev-a5.azurewebsites.net](https://dbaksheev-a5.azurewebsites.net).

## 🛠️ Role-Based Functionalities

- **Create Functionality**: Admin, Executive, and Coordinator roles can create actors, shows, and episodes.
- **Read/Details Functionality**: All roles have access to view details of actors, shows, and episodes.
- **Update/Edit Functionality**: Restricted to Admin, Executive, and Coordinator roles for editing data.
- **Delete Functionality**: Only Admin role has the authority to delete data.
