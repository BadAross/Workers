INSERT INTO base.passport_type (name) VALUES
('Паспорт гражданина'),
('Заграничный паспорт'),
('Временный паспорт');

INSERT INTO base.company (name) VALUES
('Adidas'),
('Casio'),
('HP'),
('Honda');

INSERT INTO base.department (name, phone, company_id) values
('Отдел продаж', '101-234-5678', 1),
('Отдел по работе с клиентами', '101-234-5679', 1),
('Отдел аналитики', '101-234-5680', 1),
('Маркетинговый отдел', '202-345-6789', 2),
('Отдел разработки продуктов', '202-345-6790', 2),
('Отдел логистики', '202-345-6791', 2),
('Отдел кадров', '303-456-7890', 3),
('Юридический отдел', '303-456-7891', 3),
('Финансовый отдел', '303-456-7892', 3),
('Отдел IT', '404-567-8901', 4),
('Отдел продаж и сервиса', '404-567-8902', 4),
('Отдел по связям с общественностью', '404-567-8903', 4);

INSERT INTO base.passport (passport_type_id, passport_number) VALUES
(1, 'AB 1234567'),
(2, '9876 543210'),
(1, 'XY 9876543'),
(2, 'AB 2345678'),
(3, 'XY 8765432'),
(1, 'AB 3456789'),
(2, '9876 543211'),
(3, 'XY 7654321'),
(1, 'AB 4567890'),
(2, '9876 543212'),
(1, 'AB 5678901'),
(2, '9876 543213'),
(3, 'XY 6543210'),
(1, 'AB 6789012'),
(2, '9876 543214'),
(1, 'AB 7890123'),
(2, '9876 543215'),
(3, 'XY 5432109'),
(1, 'AB 8901234'),
(2, '9876 543216');

INSERT INTO base.worker (name, surname, phone, passport_id, department_id) VALUES
('Алексей', 'Иванов', '111-222-3333', 1, 1),
('Мария', 'Петрова', '111-222-3334', 2, 2),
('Сергей', 'Сидоров', '111-222-3335', 3, 3),
('Елена', 'Кузнецова', '111-222-3336', 4, 1),
('Дмитрий', 'Федоров', '111-222-3337', 5, 2),
('Ирина', 'Смирнова', '222-333-4444', 6, 4),
('Андрей', 'Васильев', '222-333-4445', 7, 5),
('Ольга', 'Николаева', '222-333-4446', 8, 6),
('Владимир', 'Тихонов', '222-333-4447', 9, 4),
('Татьяна', 'Громова', '222-333-4448', 10, 5),
('Николай', 'Яковлев', '333-444-5555', 11, 7),
('Анна', 'Соловьева', '333-444-5556', 12, 8),
('Павел', 'Морозов', '333-444-5557', 13, 9),
('Ксения', 'Лебедева', '333-444-5558', 14, 7),
('Игорь', 'Ковалев', '333-444-5559', 15, 8),
('Евгений', 'Панин', '444-555-6666', 16, 10),
('Виктория', 'Смирнова', '444-555-6667', 17, 11),
('Роман', 'Кириллов', '444-555-6668', 18, 12),
('Дарья', 'Фролова', '444-555-6669', 19, 10),
('Станислав', 'Костин', '444-555-6670', 20, 11);