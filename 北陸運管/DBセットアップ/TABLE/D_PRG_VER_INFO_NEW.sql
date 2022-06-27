DROP TABLE D_PRG_VER_INFO_NEW;
CREATE TABLE D_PRG_VER_INFO_NEW
(
    INSERT_DATE datetime,
    INSERT_USER_ID varchar(10),
    INSERT_MACHINE_ID varchar(10),
    UPDATE_DATE datetime,
    UPDATE_USER_ID varchar(10),
    UPDATE_MACHINE_ID varchar(10),
    MODEL_CODE varchar(1) NOT NULL,
    RAIL_SECTION_CODE varchar(3) NOT NULL,
    STATION_ORDER_CODE varchar(3) NOT NULL,
    CORNER_CODE int NOT NULL,
    UNIT_NO int NOT NULL,
    ELEMENT_ID varchar(20) NOT NULL,
    ELEMENT_VERSION varchar(8) NOT NULL,
    ELEMENT_NAME varchar(64) NOT NULL
);

ALTER TABLE D_PRG_VER_INFO_NEW
ADD CONSTRAINT pk_D_PRG_VER_INFO_NEW PRIMARY KEY ( MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,UNIT_NO,ELEMENT_ID);





