DROP TABLE M_PASSAGE;
CREATE TABLE M_PASSAGE
(
    INSERT_DATE datetime,
    INSERT_USER_ID varchar(10),
    INSERT_MACHINE_ID varchar(10),
    UPDATE_DATE datetime,
    UPDATE_USER_ID varchar(10),
    UPDATE_MACHINE_ID varchar(10),
    FLG varchar(1) NOT NULL,
    KIND int NOT NULL,
    NAME varchar(20)
);

ALTER TABLE M_PASSAGE
ADD CONSTRAINT pk_M_PASSAGE PRIMARY KEY ( FLG,KIND);





