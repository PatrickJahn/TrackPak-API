#!/bin/bash

# Adjust to your environment
DB_NAME="trackpak-orderdb"
DB_USER="developer"
DB_PASSWORD="trackPak_dev_password"

psql -U postgres <<EOF
-- Create the database (fails silently if it already exists)
CREATE DATABASE $DB_NAME;

-- Create the user with the specified password
CREATE USER $DB_USER WITH PASSWORD '$DB_PASSWORD';

-- Grant all privileges on the database to the user
GRANT ALL PRIVILEGES ON DATABASE $DB_NAME TO $DB_USER;

-- Allow the user to create new databases (optional)
ALTER USER $DB_USER CREATEDB;

-- Grant usage on public schema
GRANT USAGE ON SCHEMA public TO $DB_USER;

-- Grant privileges on existing tables in public schema
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO $DB_USER;

-- Ensure the user can access future tables created by migrations
ALTER DEFAULT PRIVILEGES IN SCHEMA public
GRANT ALL PRIVILEGES ON TABLES TO $DB_USER;
EOF

echo "Database and user created successfully!"
