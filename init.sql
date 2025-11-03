-- Extensão UUID
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- ====== AVIÕES ======
CREATE TABLE IF NOT EXISTS airplanes (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    model VARCHAR(100) NOT NULL,
    capacity_economica INT NOT NULL,
    capacity_primeira INT NOT NULL
);

INSERT INTO airplanes (model, capacity_economica, capacity_primeira)
VALUES 
('Airbus A370-1', 5, 2),
('Airbus A370-2', 5, 2),
('Airbus A370-3', 5, 2);

-- ====== VOOS ======
CREATE TABLE IF NOT EXISTS flights (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    airplane_id UUID NOT NULL REFERENCES airplanes(id),
    departure_timestamp TIMESTAMP NOT NULL,
    arrival_timestamp TIMESTAMP NOT NULL,
    origin VARCHAR(100) NOT NULL,
    destination VARCHAR(100) NOT NULL
);

-- Três voos manuais (um para cada avião)
INSERT INTO flights (airplane_id, departure_timestamp, arrival_timestamp, origin, destination)
SELECT id, CURRENT_DATE + TIME '06:00', CURRENT_DATE + TIME '07:00', 'Curitiba', 'São Paulo' FROM airplanes;
INSERT INTO flights (airplane_id, departure_timestamp, arrival_timestamp, origin, destination)
SELECT id, CURRENT_DATE + TIME '09:00', CURRENT_DATE + TIME '10:00', 'Curitiba', 'São Paulo' FROM airplanes;
INSERT INTO flights (airplane_id, departure_timestamp, arrival_timestamp, origin, destination)
SELECT id, CURRENT_DATE + TIME '12:00', CURRENT_DATE + TIME '13:00', 'Curitiba', 'São Paulo' FROM airplanes;

-- ====== TIPOS E ASSENTOS ======
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'seat_class') THEN
        CREATE TYPE seat_class AS ENUM ('ECONOMICA', 'PRIMEIRA');
    END IF;
END
$$;

CREATE TABLE IF NOT EXISTS seats (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    flight_id UUID NOT NULL REFERENCES flights(id),
    seat_number VARCHAR(10) NOT NULL,
    seat_class seat_class NOT NULL,
    price NUMERIC(10,2) NOT NULL,
    UNIQUE(flight_id, seat_number)
);

-- Cria manualmente 5 assentos econômicos e 2 primeira classe por voo
INSERT INTO seats (flight_id, seat_number, seat_class, price)
SELECT id, 'E1', 'ECONOMICA', 159.97 FROM flights;
INSERT INTO seats (flight_id, seat_number, seat_class, price)
SELECT id, 'E2', 'ECONOMICA', 159.97 FROM flights;
INSERT INTO seats (flight_id, seat_number, seat_class, price)
SELECT id, 'E3', 'ECONOMICA', 159.97 FROM flights;
INSERT INTO seats (flight_id, seat_number, seat_class, price)
SELECT id, 'E4', 'ECONOMICA', 159.97 FROM flights;
INSERT INTO seats (flight_id, seat_number, seat_class, price)
SELECT id, 'E5', 'ECONOMICA', 159.97 FROM flights;
INSERT INTO seats (flight_id, seat_number, seat_class, price)
SELECT id, 'P1', 'PRIMEIRA', 399.93 FROM flights;
INSERT INTO seats (flight_id, seat_number, seat_class, price)
SELECT id, 'P2', 'PRIMEIRA', 399.93 FROM flights;

-- ====== USUÁRIOS ======
CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    full_name VARCHAR(200) NOT NULL,
    email VARCHAR(200) NOT NULL UNIQUE,
    cpf VARCHAR(20) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL
);

INSERT INTO users (full_name, email, cpf, password_hash)
VALUES ('Usuário Teste', 'teste@sisand.com', '12345678900', '$2a$11$yqS8aKk1cDYP4R9E2M0O7u1h2QeFZcRzHOMVpl4/BlRBuNmrcnNsW');

-- ====== RESERVAS ======
CREATE TABLE IF NOT EXISTS bookings (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL REFERENCES users(id),
    seat_id UUID NOT NULL REFERENCES seats(id),
    total_price NUMERIC(10,2) NOT NULL,
    confirmation_code VARCHAR(50) NOT NULL UNIQUE,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    UNIQUE(seat_id)
);
