# MedLab AInsights

MedLab AInsights is a modern **medical laboratory management and insight platform** designed to assist lab technicians and healthcare professionals in managing patient members, recording test results, and generating intelligent lab report summaries.

The system provides a structured workflow for **patient registration, visit tracking, vitals recording, test panel analysis, and AI-assisted health summaries**, helping reduce manual interpretation time and improving report readability.

This project focuses on building a **clean, scalable medical dashboard interface** that simulates a real-world healthcare SaaS product.

---

# Project Overview

MedLab AInsights allows technicians to:

• Register and manage patient members
• Record patient visits and vitals
• Perform laboratory tests (CBC, Thyroid, etc.)
• View structured lab reports
• Generate AI-assisted health summaries
• Track patient report history

The application follows a **dashboard-based workflow similar to real hospital lab systems.**

---

# Key Features

### Member Management

* Create and manage patient profiles
* Automatically calculate **Age from Date of Birth**
* Store gender, blood group, contact details, and address

### Visit & Vitals Tracking

* Record patient vitals during each visit
* Capture weight, height, blood pressure, and other medical metrics

### Lab Report Generation

* View structured report summaries
* Display panel name, test results, and visit date
* Organized report sections for clarity

### AI Health Summary

* Generates a readable health interpretation
* Helps technicians understand abnormal patterns quickly
* Designed to simulate **AI-assisted diagnostic insights**

### Medical Dashboard UI

* Clean **industry-style medical SaaS interface**
* Consistent forms, tables, and profile layouts
* Professional typography and spacing

---

# Tech Stack

Frontend:

* Angular
* TypeScript
* HTML / SCSS
* Angular CLI

UI & Styling:

* Modern dashboard design
* Responsive layout
* Clean SaaS-style component structure

Backend:

* REST API (provided by mentor system)
* Member, Visit, and Report endpoints

---

# Application Workflow

1. Technician creates a **Member Profile**
2. Member visits the lab and a **Visit entry is created**
3. Technician records **Vitals**
4. Tests are conducted and stored
5. System generates a **Lab Report Summary**
6. AI produces a **Health Insight Summary**

---

# Project Structure

src/
components/
members/
member-details/
members-list/
reports/
add-member/

services/
models/

The structure follows Angular best practices by separating **components, services, and models.**

---

# Development Server

To start the development server:

```bash
ng serve
```

Then open:

http://localhost:4200

The application automatically reloads when source files change.

---

# Build

To build the project:

```bash
ng build
```

Production build files will be generated inside the `dist/` directory.

---

# Generate Components

Angular CLI provides scaffolding tools.

Example:

```bash
ng generate component component-name
```

To view all available generators:

```bash
ng generate --help
```

---

# Testing

Run unit tests using:

```bash
ng test
```

End-to-end testing:

```bash
ng e2e
```

---

# Future Improvements

• Advanced AI diagnostic insights
• Machine learning model integration
• Medical trend visualization
• Patient health history analytics
• Secure authentication system

---

# Author

Shagul Rafiq
Full-Stack Developer | Python Developer | AI Enthusiast

LinkedIn:
[www.linkedin.com/in/shagulrafiq](http://www.linkedin.com/in/shagulrafiq)

GitHub:
https://github.com/shagul-hameed-rafiq

Location: Tamil Nadu, India

---

# License

This project is created for **educational and research purposes** as part of a healthcare analytics learning initiative.
