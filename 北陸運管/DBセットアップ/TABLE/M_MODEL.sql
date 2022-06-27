DROP TABLE M_MODEL;
CREATE TABLE M_MODEL
(
    INSERT_DATE datetime,
    INSERT_USER_ID varchar(10),
    INSERT_MACHINE_ID varchar(10),
    UPDATE_DATE datetime,
    UPDATE_USER_ID varchar(10),
    UPDATE_MACHINE_ID varchar(10),
    MODEL_CODE varchar(2) NOT NULL,
    MST_SND_FLAG varchar(1),
    PRG_SND_FLAG varchar(1),
    KADO_RCV_FLAG varchar(1),
    FAULT_RCV_FLAG varchar(1),
    MODEL_NAME varchar(40)
);

ALTER TABLE M_MODEL
ADD CONSTRAINT pk_M_MODEL PRIMARY KEY ( MODEL_CODE);





