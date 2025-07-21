# Quiz Platform - Local Development Setup

This guide will help you set up the Quiz Platform for local development using Docker Compose.

## Architecture Overview

The application consists of the following services:
- **Quiz API**: .NET 8 Web API with C# quiz functionality
- **PostgreSQL**: Database for storing quiz data, user answers, and progress
- **Keycloak**: Authentication and authorization server
- **Traefik**: Reverse proxy and load balancer with automatic HTTPS

## Prerequisites

- Docker and Docker Compose installed
- Text editor for configuration files
- Administrator access to modify hosts file

## Quick Start

### 1. Environment Configuration

Each service requires a `.env` file in its respective directory. Create the following files:

#### `infra/postgres/.env`
```env
POSTGRES_DB=postgres
POSTGRES_USER=admin
POSTGRES_PASSWORD=admin123
DBCONFIGS=csharp-quiz:root:example,keycloak:keycloak:keycloak123
```

#### `infra/keycloak/.env`
```env
IMAGE_TAG=quay.io/keycloak/keycloak:25.0.6
KC_HOSTNAME=auth.localhost.uz
KC_DB=postgres
KC_DB_URL=jdbc:postgresql://postgres:5432/keycloak
KC_DB_USERNAME=keycloak
KC_DB_PASSWORD=keycloak123
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin123
KC_HEALTH_ENABLED=true
KC_METRICS_ENABLED=true
KC_PROXY_HEADERS=xforwarded
KC_HTTP_ENABLED=true
```

#### `infra/traefik/.env`
```env
IMAGE_TAG=traefik:v3.1
TRAEFIK_LOG_LEVEL=INFO
TRAEFIK_HOSTNAME=traefik.localhost.uz
TRAEFIK_ACME_EMAIL=your-email@example.com
TRAEFIK_BASIC_AUTH=admin:$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi
```

#### `infra/quiz/.env`
```env
QUIZ_HOSTNAME=api.localhost.uz
```

### 2. Hosts File Configuration

Add the following entries to your system's hosts file to enable local domain access:

#### Windows
Edit `C:\Windows\System32\drivers\etc\hosts` as Administrator:
```
127.0.0.1    auth.localhost.uz
127.0.0.1    api.localhost.uz  
127.0.0.1    traefik.localhost.uz
```

#### macOS/Linux
Edit `/etc/hosts` with sudo privileges:
```bash
sudo nano /etc/hosts
```
Add these lines:
```
127.0.0.1    auth.localhost.uz
127.0.0.1    api.localhost.uz
127.0.0.1    traefik.localhost.uz
```

### 3. Start the Application

From the `infra` directory, run:

```bash
docker-compose up -d
```

This will start all services in the background. Monitor the logs:
```bash
docker-compose logs -f
```

### 4. Verify Services

Once all containers are running, verify access:

- **Traefik Dashboard**: http://traefik.localhost.uz (admin/admin)
- **Keycloak Admin**: http://auth.localhost.uz (admin/admin123)
- **Quiz API**: http://api.localhost.uz/swagger
- **Health Check**: http://api.localhost.uz/health

## Keycloak Configuration

### Initial Setup

1. **Access Keycloak Admin Console**
   - URL: http://auth.localhost.uz
   - Username: `admin`
   - Password: `admin123`

### Create Realm

1. Click on the dropdown next to "Master" in the top-left corner
2. Click "Create Realm"
3. Set Realm name: `ilmhub`
4. Click "Create"

### Create Client

1. Navigate to **Clients** in the left sidebar
2. Click "Create client"
3. Configure the client:
   - **Client type**: OpenID Connect
   - **Client ID**: `quiz-api`
   - **Name**: `Quiz Platform API`
   - Click "Next"

4. **Capability config**:
   - ✅ Client authentication
   - ✅ Authorization
   - ✅ Standard flow
   - ✅ Direct access grants
   - Click "Next"

5. **Login settings**:
   - **Valid redirect URIs**: 
     - `http://api.localhost.uz/*`
     - `http://localhost:5173/*` (for React frontend)
   - **Web origins**: 
     - `http://api.localhost.uz`
     - `http://localhost:5173`
   - Click "Save"

### Create Client Roles

1. Go to **Clients** → **quiz-api** → **Roles**
2. Create the following roles:
   - `quiz-admin:read` - Read access to admin features
   - `quiz-admin:write` - Write access to admin features
   - `quiz-user` - Basic user access

### Create Users

#### Admin User
1. Navigate to **Users** → **Create new user**
2. Configure:
   - **Username**: `admin`
   - **Email**: `admin@localhost.uz`
   - **First name**: `Admin`
   - **Last name**: `User`
   - ✅ Email verified
   - ✅ Enabled
3. Click "Create"

4. Set password:
   - Go to **Credentials** tab
   - Set password: `admin123`
   - ❌ Temporary (uncheck this)
   - Click "Set password"

5. Assign roles:
   - Go to **Role mapping** tab
   - Click "Assign role"
   - Select **Filter by clients**
   - Assign: `quiz-admin:read`, `quiz-admin:write`

#### Regular User
1. Create another user:
   - **Username**: `testuser`
   - **Email**: `test@localhost.uz`
   - **First name**: `Test`
   - **Last name**: `User`

2. Set password: `test123`

3. Assign role: `quiz-user`

### Configure User Attributes (Optional)

For subscription-based access, add custom attributes:

1. Go to **Users** → Select user → **Attributes**
2. Add attribute:
   - **Key**: `ustoz-membership`
   - **Value**: `csharp-quiz` (or `premium`, `basic`, `admin`)

### Realm Roles (Optional)

Create realm-level roles for broader access control:

1. Navigate to **Realm roles**
2. Create roles:
   - `quiz-admin` - Administrative access
   - `quiz-user` - Standard user access
   - `premium-user` - Premium features access

## Application Configuration

The Quiz API is configured to work with the Keycloak setup above. Key configuration points:

- **Realm**: `ilmhub`
- **Client ID**: `quiz-api`
- **Auth Server URL**: `http://auth.localhost.uz/`
- **Database**: PostgreSQL with automatic migrations
- **CORS**: Configured for `http://localhost:5173` (React frontend)

## Troubleshooting

### Common Issues

1. **Services not accessible via custom domains**
   - Verify hosts file entries
   - Clear DNS cache: `ipconfig /flushdns` (Windows) or `sudo dscacheutil -flushcache` (macOS)

2. **Keycloak database connection errors**
   - Ensure PostgreSQL is fully started before Keycloak
   - Check database credentials in `.env` files

3. **Quiz API authentication errors**
   - Verify Keycloak realm and client configuration
   - Check that users have appropriate roles assigned

4. **Database migration issues**
   - Check PostgreSQL logs: `docker-compose logs postgres`
   - Verify connection string in application settings

### Useful Commands

```bash
# View all container logs
docker-compose logs

# View specific service logs
docker-compose logs keycloak
docker-compose logs quiz

# Restart a specific service
docker-compose restart keycloak

# Stop all services
docker-compose down

# Stop and remove volumes (⚠️ This will delete all data)
docker-compose down -v

# Rebuild and restart
docker-compose up --build -d
```

### Database Access

To connect directly to PostgreSQL:
```bash
docker-compose exec postgres psql -U admin -d postgres
```

## Development Workflow

1. **Making Code Changes**: The Quiz API runs from a Docker image. For development, you'll need to rebuild the image after code changes.

2. **Database Changes**: The application automatically runs Entity Framework migrations on startup.

3. **Frontend Development**: Configure your frontend to use:
   - **API Base URL**: `http://api.localhost.uz`
   - **Auth Server**: `http://auth.localhost.uz`

## Security Notes

⚠️ **This configuration is for local development only!**

- Uses HTTP instead of HTTPS
- Contains default/weak passwords
- Disables SSL requirements in Keycloak
- Uses basic authentication for Traefik dashboard

For production deployment, ensure:
- Enable HTTPS with proper certificates
- Use strong, unique passwords
- Enable SSL requirements in Keycloak
- Secure Traefik dashboard access
- Use environment-specific configuration files

## Next Steps

1. Set up your frontend application to connect to these services
2. Test the authentication flow with the created users
3. Explore the API documentation at `http://api.localhost.uz/swagger`
4. Review the quiz data seeding in the application logs 