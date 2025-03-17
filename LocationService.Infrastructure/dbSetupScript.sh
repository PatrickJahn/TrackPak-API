#!/bin/bash

# Set variables
DB_NAME="trackpak-locationdb"
DB_USER="developer"
DB_PASSWORD="trackPak_dev_password"

echo "⏳ Waiting for PostgreSQL to be ready..."
until psql -U postgres -c '\q' 2>/dev/null; do
  sleep 1
done
echo "PostgreSQL is ready!"

# Ensure the user exists before assigning roles
psql -U postgres <<EOF
SELECT 'CREATE ROLE $DB_USER WITH LOGIN PASSWORD \'$DB_PASSWORD\';'
WHERE NOT EXISTS (SELECT FROM pg_roles WHERE rolname='$DB_USER')\gexec
EOF

# Ensure the database exists before trying to create it (MUST use double quotes for hyphen)
psql -U postgres <<EOF
SELECT 'CREATE DATABASE "$DB_NAME" OWNER $DB_USER;'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = '$DB_NAME')\gexec
EOF

# Apply permissions inside the new database (MUST use double quotes for hyphen)
psql -U postgres -d "$DB_NAME" <<EOF
ALTER USER $DB_USER CREATEDB;
GRANT USAGE ON SCHEMA public TO $DB_USER;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO $DB_USER;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL PRIVILEGES ON TABLES TO $DB_USER;
EOF

echo "Database '$DB_NAME' and user '$DB_USER' configured successfully!"
