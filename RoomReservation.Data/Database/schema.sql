PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS users (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    login TEXT NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    profile_info TEXT,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS rooms (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    capacity INTEGER NOT NULL CHECK (capacity > 0),
    equipment TEXT,
    max_reservation_duration_minutes INTEGER NOT NULL CHECK (max_reservation_duration_minutes > 0),
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS reservations (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    start_time DATETIME NOT NULL,
    end_time DATETIME NOT NULL,
    purpose TEXT NOT NULL,
    number_of_people INTEGER NOT NULL CHECK (number_of_people > 0),
    user_id INTEGER NOT NULL,
    room_id INTEGER NOT NULL,
    status INTEGER NOT NULL CHECK (status IN (1, 2)),
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CHECK (end_time > start_time),

    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (room_id) REFERENCES rooms(id)
);

CREATE TABLE IF NOT EXISTS reservation_history (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    reservation_id INTEGER NOT NULL,
    field_name TEXT NOT NULL,
    old_value TEXT,
    new_value TEXT,
    changed_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (reservation_id) REFERENCES reservations(id)
);

INSERT OR IGNORE INTO users (id, login, password_hash, profile_info)
VALUES 
(1, 'demo', 'demo', 'Demo user for testing');

INSERT OR IGNORE INTO rooms (id, name, capacity, equipment, max_reservation_duration_minutes)
VALUES
(1, 'Small Meeting Room', 4, 'TV, whiteboard', 120),
(2, 'Large Conference Room', 12, 'Projector, speakers, whiteboard', 240),
(3, 'Quiet Room', 2, 'Table, chairs', 60);