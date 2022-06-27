DROP TABLE D_COLLECTED_DATA_TYPO;
CREATE TABLE D_COLLECTED_DATA_TYPO
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
    UNIT_NO int NOT NULL,
    DATA_KIND varchar(40) NOT NULL,
    PROCESSING_TIME varchar(14) NOT NULL,
    ERROR_INFO varchar(80)
);


CREATE INDEX idx_D_COLLECTED_DATA_TYPO ON D_COLLECTED_DATA_TYPO(RAIL_SECTION_CODE,STATION_ORDER_CODE,CORNER_CODE,MODEL_CODE,UNIT_NO,DATA_KIND,PROCESSING_TIME);

CREATE INDEX idx2_D_COLLECTED_DATA_TYPO ON D_COLLECTED_DATA_TYPO(UPDATE_DATE);



