CREATE SCHEMA IF NOT EXISTS base;

CREATE TABLE IF NOT EXISTS base.passport_type (
	id SERIAL PRIMARY KEY,
	"name" VARCHAR(500) NOT null
);
COMMENT ON TABLE base.passport_type IS 'Таблица общего словаря данных';

COMMENT ON COLUMN base.passport_type.id IS 'Идентификатор';
COMMENT ON COLUMN base.passport_type."name" IS 'Название типа паспорта';


CREATE TABLE IF NOT EXISTS base.passport (
	id SERIAL PRIMARY KEY,
	passport_type_id INT NOT NULL,
	passport_number VARCHAR(50) NOT NULL,
	FOREIGN KEY (passport_type_id) REFERENCES base.passport_type(id)
);
COMMENT ON TABLE base.passport IS 'Таблица данных паспортов';

COMMENT ON COLUMN base.passport.id IS 'Идентификатор';
COMMENT ON COLUMN base.passport.passport_type_id IS 'Идентификатор типа паспорта';
COMMENT ON COLUMN base.passport.passport_number IS 'Номер паспорта';


CREATE TABLE IF NOT EXISTS base.company (
	id SERIAL PRIMARY KEY,
	"name" VARCHAR(500) NOT null
);
COMMENT ON TABLE base.company IS 'Таблица компаний';

COMMENT ON COLUMN base.company.id IS 'Идентификатор';
COMMENT ON COLUMN base.company."name" IS 'Название';


CREATE TABLE IF NOT EXISTS base.department (
	id SERIAL PRIMARY KEY,
	"name" VARCHAR(500) NOT NULL,
	phone VARCHAR(16) NULL,
	company_id INT NOT NULL,
	FOREIGN KEY (company_id) REFERENCES base.company(id)
);
COMMENT ON TABLE base.department IS 'Таблица отделов';

COMMENT ON COLUMN base.department.id IS 'Идентификатор';
COMMENT ON COLUMN base.department."name" IS 'Название';
COMMENT ON COLUMN base.department.phone IS 'Номер телефона';
COMMENT ON COLUMN base.department.company_id IS 'Идентификатор компании';


CREATE TABLE IF NOT EXISTS base.worker (
	id SERIAL PRIMARY KEY,
	"name" VARCHAR(200) NOT NULL,
	surname VARCHAR(200) NOT NULL,
	phone VARCHAR(16) NULL,
	passport_id INT NOT NULL,
	department_id INT NOT NULL,
	FOREIGN KEY (passport_id) REFERENCES base.passport(id),
	FOREIGN KEY (department_id) REFERENCES base.department(id)
);
COMMENT ON TABLE base.worker IS 'Таблица сотрудников';

COMMENT ON COLUMN base.worker.id IS 'Идентификатор';
COMMENT ON COLUMN base.worker."name" IS 'Имя';
COMMENT ON COLUMN base.worker.surname IS 'Фамилия';
COMMENT ON COLUMN base.worker.phone IS 'Номер телефона';
COMMENT ON COLUMN base.worker.passport_id IS 'Идентификатор паспортных данных';
COMMENT ON COLUMN base.worker.department_id IS 'Идентификатор отдела';
