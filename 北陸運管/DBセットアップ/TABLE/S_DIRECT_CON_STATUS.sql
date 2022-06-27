DROP TABLE S_DIRECT_CON_STATUS;
CREATE TABLE S_DIRECT_CON_STATUS
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
    PORT_KBN varchar(1) NOT NULL,
    CONNECT_DATE datetime NOT NULL
);

ALTER TABLE S_DIRECT_CON_STATUS
ADD CONSTRAINT pk_S_DIRECT_CON_STATUS PRIMARY KEY ( MODEL_CODE,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,UNIT_NO,PORT_KBN);





