DROP TABLE M_KADOHOSYU_SET;
CREATE TABLE M_KADOHOSYU_SET
(
    INSERT_DATE datetime,
    INSERT_USER_ID varchar(10),
    INSERT_MACHINE_ID varchar(10),
    UPDATE_DATE datetime,
    UPDATE_USER_ID varchar(10),
    UPDATE_MACHINE_ID varchar(10),
    GROUP_NO int NOT NULL,
    MODEL_CODE varchar(1) NOT NULL,
    DATA_SYUBETU int NOT NULL,
    KOMOKU_NAME varchar(64),
    KOMOKU_NO int NOT NULL,
    KAISATUKIJUN float,
    SYUSATUKIJUN float,
    LAST_DATE datetime
);

ALTER TABLE M_KADOHOSYU_SET
ADD CONSTRAINT pk_M_KADOHOSYU_SET PRIMARY KEY ( GROUP_NO,MODEL_CODE,DATA_SYUBETU,KOMOKU_NO);





