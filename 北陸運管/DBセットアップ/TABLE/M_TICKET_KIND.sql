DROP TABLE M_TICKET_KIND;
CREATE TABLE M_TICKET_KIND
(
    INSERT_DATE datetime,
    INSERT_USER_ID varchar(10),
    INSERT_MACHINE_ID varchar(10),
    UPDATE_DATE datetime,
    UPDATE_USER_ID varchar(10),
    UPDATE_MACHINE_ID varchar(10),
    KIND varchar(1) NOT NULL,
    NO int NOT NULL,
    NAME varchar(30)
);

ALTER TABLE M_TICKET_KIND
ADD CONSTRAINT pk_M_TICKET_KIND PRIMARY KEY ( KIND,NO);





