-- ==========================================================
--  SISAND AIRLINES - SCRIPT DE CRIAÇÃO DE BANCO DE DADOS v1.1
-- ==========================================================
-- Atualizações:
-- ✅ Adiciona coluna birth_date em users
-- ✅ Adiciona coluna is_available em seats
-- ✅ Adiciona ON DELETE CASCADE nas FKs
-- ✅ Corrige duplicação de voos (mantém apenas 3)
-- ✅ Corrige integridade e preços dos assentos
-- ==========================================================

-- Extensão UUID
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- ==========================================================
-- AVIÕES
-- ==========================================================
CREATE TABLE IF NOT EXISTS airplanes (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    model VARCHAR(100) NOT NULL,
    capacity_economica INT NOT NULL,
    capacity_primeira INT NOT NULL
);

-- Inserir 3 aviões Airbus A370
INSERT INTO airplanes (model, capacity_economica, capacity_primeira)
VALUES 
('Airbus A370-1', 5, 2),
('Airbus A370-2', 5, 2),
('Airbus A370-3', 5, 2)
ON CONFLICT DO NOTHING;

-- ==========================================================
-- VOOS
-- ==========================================================
CREATE TABLE IF NOT EXISTS flights (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    airplane_id UUID NOT NULL REFERENCES airplanes(id) ON DELETE CASCADE,
    departure_timestamp TIMESTAMP NOT NULL,
    arrival_timestamp TIMESTAMP NOT NULL,
    origin VARCHAR(100) NOT NULL,
    destination VARCHAR(100) NOT NULL
);

-- Três voos fixos (um para cada avião)
INSERT INTO flights (airplane_id, departure_timestamp, arrival_timestamp, origin, destination)
SELECT id, CURRENT_DATE + TIME '06:00', CURRENT_DATE + TIME '07:00', 'Curitiba', 'São Paulo' FROM airplanes
ON CONFLICT DO NOTHING;

-- ==========================================================
-- TIPOS E ASSENTOS
-- ==========================================================
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'seat_class') THEN
        CREATE TYPE seat_class AS ENUM ('ECONOMICA', 'PRIMEIRA');
    END IF;
END
$$;

CREATE TABLE IF NOT EXISTS seats (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    flight_id UUID NOT NULL REFERENCES flights(id) ON DELETE CASCADE,
    seat_number VARCHAR(10) NOT NULL,
    seat_class seat_class NOT NULL,
    price NUMERIC(10,2) NOT NULL,
    is_available BOOLEAN NOT NULL DEFAULT TRUE,
    UNIQUE(flight_id, seat_number)
);

-- Criação automática dos assentos (5 econômicos + 2 primeira classe por voo)
WITH labels AS (
    SELECT 'E1' AS seat_number, 'ECONOMICA'::seat_class AS seat_class, 159.97::numeric AS price UNION ALL
    SELECT 'E2','ECONOMICA',159.97 UNION ALL
    SELECT 'E3','ECONOMICA',159.97 UNION ALL
    SELECT 'E4','ECONOMICA',159.97 UNION ALL
    SELECT 'E5','ECONOMICA',159.97 UNION ALL
    SELECT 'P1','PRIMEIRA',399.93 UNION ALL
    SELECT 'P2','PRIMEIRA',399.93
)
INSERT INTO seats (flight_id, seat_number, seat_class, price, is_available)
SELECT f.id, l.seat_number, l.seat_class, l.price, TRUE
FROM flights f
CROSS JOIN labels l
LEFT JOIN seats s
  ON s.flight_id = f.id AND s.seat_number = l.seat_number
WHERE s.id IS NULL;

-- ==========================================================
-- USUÁRIOS
-- ==========================================================
CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    full_name VARCHAR(200) NOT NULL,
    email VARCHAR(200) NOT NULL UNIQUE,
    cpf VARCHAR(20) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    birth_date DATE NOT NULL DEFAULT '2000-01-01'
);

-- Usuário de teste
INSERT INTO users (full_name, email, cpf, password_hash)
VALUES ('Usuário Teste', 'teste@sisand.com', '12345678900', 
'$2a$11$yqS8aKk1cDYP4R9E2M0O7u1h2QeFZcRzHOMVpl4/BlRBuNmrcnNsW')
ON CONFLICT DO NOTHING;

-- ==========================================================
-- RESERVAS
-- ==========================================================
CREATE TABLE IF NOT EXISTS bookings (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    seat_id UUID NOT NULL REFERENCES seats(id) ON DELETE CASCADE,
    total_price NUMERIC(10,2) NOT NULL,
    confirmation_code VARCHAR(50) NOT NULL UNIQUE,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    UNIQUE(seat_id)
);

-- ==========================================================
-- FIM DO SCRIPT
-- ==========================================================
