--changeset dlnuckolls:1 labels:create table company context:initial creation
--comment: Creating the initial table
create table company (
    id int primary key identity not null,
    name varchar(50) not null,
    address1 varchar(50),
    address2 varchar(50),
    city varchar(30)
)
--rollback DROP TABLE company;