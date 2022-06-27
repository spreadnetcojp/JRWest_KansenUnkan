DROP TABLE S_PRG_LIST;
CREATE TABLE S_PRG_LIST
(
    INSERT_DATE datetime,
    INSERT_USER_ID varchar(10),
    INSERT_MACHINE_ID varchar(10),
    UPDATE_DATE datetime,
    UPDATE_USER_ID varchar(10),
    UPDATE_MACHINE_ID varchar(10),
    FILE_NAME varchar(80) NOT NULL,
    RAIL_SECTION_CODE varchar(3),
    STATION_ORDER_CODE varchar(3),
    CORNER_CODE int,
    UNIT_NO int,
    APPLICABLE_DATE varchar(8)
);


CREATE INDEX idx_S_PRG_LIST ON S_PRG_LIST(FILE_NAME);




