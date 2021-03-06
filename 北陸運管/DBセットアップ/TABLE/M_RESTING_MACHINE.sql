DROP TABLE M_RESTING_MACHINE;
CREATE TABLE M_RESTING_MACHINE
(
    INSERT_DATE datetime,
    INSERT_USER_ID varchar(10),
    INSERT_MACHINE_ID varchar(10),
    UPDATE_DATE datetime,
    UPDATE_USER_ID varchar(10),
    UPDATE_MACHINE_ID varchar(10),
    RAIL_SECTION_CODE varchar(3) NOT NULL,
    STATION_ORDER_CODE varchar(3) NOT NULL,
    CORNER_CODE int NOT NULL,
    MODEL_CODE varchar(1) NOT NULL,
    UNIT_NO int NOT NULL
);

ALTER TABLE M_RESTING_MACHINE
ADD CONSTRAINT pk_M_RESTING_MACHINE PRIMARY KEY ( RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,MODEL_CODE,UNIT_NO);





