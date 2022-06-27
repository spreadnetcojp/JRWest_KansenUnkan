DROP TABLE S_MST_DLL_STS;
CREATE TABLE S_MST_DLL_STS
(
    INSERT_DATE datetime,
    INSERT_USER_ID varchar(10),
    INSERT_MACHINE_ID varchar(10),
    UPDATE_DATE datetime,
    UPDATE_USER_ID varchar(10),
    UPDATE_MACHINE_ID varchar(10),
    MODEL_CODE varchar(1) NOT NULL,
    FILE_KBN varchar(3) NOT NULL,
    DATA_KIND varchar(3) NOT NULL,
    DATA_SUB_KIND varchar(2) NOT NULL,
    DATA_VERSION varchar(8) NOT NULL,
    VERSION varchar(8) NOT NULL,
    RAIL_SECTION_CODE varchar(3) NOT NULL,
    STATION_ORDER_CODE varchar(3) NOT NULL,
    CORNER_CODE int NOT NULL,
    UNIT_NO int NOT NULL,
    DELIVERY_START_TIME varchar(14),
    DELIVERY_END_TIME varchar(14),
    DELIVERY_STS int
);

ALTER TABLE S_MST_DLL_STS
ADD CONSTRAINT pk_S_MST_DLL_STS PRIMARY KEY ( MODEL_CODE,FILE_KBN,DATA_KIND,DATA_SUB_KIND,DATA_VERSION,VERSION,RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,UNIT_NO);





