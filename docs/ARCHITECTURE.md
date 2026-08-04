# Architecture

## Components

### Web UI
Responsibilities:
- Dashboard
- Endpoint management

### Backend API
Responsibilities:
- Health checks
- Scheduling
- Statistics

### Monitoring Engine
Responsibilities:
- HTTP checks
- TCP checks
- DNS checks

### Storage
Responsibilities:
- Endpoint definitions
- Historical results

## Data Flow

User -> API -> Monitoring Engine -> Storage -> Dashboard