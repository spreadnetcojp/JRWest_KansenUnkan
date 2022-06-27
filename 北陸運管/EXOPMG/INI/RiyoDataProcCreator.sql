SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
${GO}

IF EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[uspPrepareToImportRiyoData${Sta}]'))
	DROP PROCEDURE [dbo].[uspPrepareToImportRiyoData${Sta}];
${GO}

CREATE PROCEDURE [dbo].[uspPrepareToImportRiyoData${Sta}]
AS
BEGIN
	TRUNCATE TABLE D_RIYO_DATA_C1_TEMP_${Sta};
	TRUNCATE TABLE D_RIYO_DATA_W1_TEMP_${Sta};
END;
${GO}


IF EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[uspImportRiyoData${Sta}]'))
	DROP PROCEDURE [dbo].[uspImportRiyoData${Sta}];
${GO}

CREATE PROCEDURE [dbo].[uspImportRiyoData${Sta}]
	@format_code VARCHAR(2),
	@data_file_path VARCHAR(128)
AS
BEGIN
	SET NOCOUNT ON;
	IF @format_code = 'C1'
		EXEC('INSERT INTO D_RIYO_DATA_C1_TEMP_${Sta} SELECT * FROM OPENROWSET(BULK ''' + @data_file_path + ''', FORMATFILE = ''${BasePath}\RiyoDataC1TempFormat.xml'') AS T')
	ELSE IF @format_code = 'W1'
		EXEC('INSERT INTO D_RIYO_DATA_W1_TEMP_${Sta} SELECT * FROM OPENROWSET(BULK ''' + @data_file_path + ''', FORMATFILE = ''${BasePath}\RiyoDataW1TempFormat.xml'') AS T');
END;
${GO}


IF EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[uspDispatchRiyoData${Sta}]'))
	DROP PROCEDURE [dbo].[uspDispatchRiyoData${Sta}];
${GO}

CREATE PROCEDURE [dbo].[uspDispatchRiyoData${Sta}]
AS
BEGIN
	DECLARE @CurTime DATETIME;

	DECLARE @��{�w�b�_�[�������� BINARY(7);
	DECLARE @��{�w�b�_�[�w�R�[�h BINARY(2);
	DECLARE @��{�w�b�_�[�R�[�i�[ BINARY(1);
	DECLARE @��{�w�b�_�[���@ BINARY(1);
	DECLARE @��{�w�b�_�[�V�[�P���XNo BINARY(4);
	DECLARE @�w�茔���w��P�w���� BINARY(4);
	DECLARE @�w�茔���w��P��Ԕԍ� BINARY(3);
	DECLARE @�w�茔���w��P��Ԕԍ�Dst BINARY(2);
	DECLARE @�w�茔���w��P���Ԕԍ� BINARY(1);
	DECLARE @�w�茔���w��P�Ȕ� BINARY(2);
	DECLARE @�w�茔���w��P�Ȕ�Dst BINARY(1);
	DECLARE @�w�茔���w��Q�w���� BINARY(4);
	DECLARE @�w�茔���w��Q��Ԕԍ� BINARY(3);
	DECLARE @�w�茔���w��Q��Ԕԍ�Dst BINARY(2);
	DECLARE @�w�茔���w��Q���Ԕԍ� BINARY(1);
	DECLARE @�w�茔���w��Q�Ȕ� BINARY(2);
	DECLARE @�w�茔���w��Q�Ȕ�Dst BINARY(1);
	DECLARE @�w�茔���w��R�w���� BINARY(4);
	DECLARE @�w�茔���w��R��Ԕԍ� BINARY(3);
	DECLARE @�w�茔���w��R��Ԕԍ�Dst BINARY(2);
	DECLARE @�w�茔���w��R���Ԕԍ� BINARY(1);
	DECLARE @�w�茔���w��R�Ȕ� BINARY(2);
	DECLARE @�w�茔���w��R�Ȕ�Dst BINARY(1);
	DECLARE @���ǎ���P���ڏ��W�v���� BINARY(1);
	DECLARE @���ǎ���P���ڏ�񊄈� BINARY(1);
	DECLARE @���ǎ���Q���ڏ��W�v���� BINARY(1);
	DECLARE @���ǎ���Q���ڏ�񊄈� BINARY(1);
	DECLARE @���ǎ���R���ڏ��W�v���� BINARY(1);
	DECLARE @���ǎ���R���ڏ�񊄈� BINARY(1);
	DECLARE @���ǎ���S���ڏ��W�v���� BINARY(1);
	DECLARE @���ǎ���S���ڏ�񊄈� BINARY(1);
	DECLARE @���ǎ���P���ڏ��r�b�g BINARY(2);
	DECLARE @���ǎ���Q���ڏ��r�b�g BINARY(2);
	DECLARE @���ǎ���R���ڏ��r�b�g BINARY(2);
	DECLARE @���ǎ���S���ڏ��r�b�g BINARY(2);
	DECLARE @���ǎ���P���ڏ�񓖉w�L�� BINARY(1);
	DECLARE @���ǎ���Q���ڏ�񓖉w�L�� BINARY(1);
	DECLARE @���ǎ���R���ڏ�񓖉w�L�� BINARY(1);
	DECLARE @���ǎ���S���ڏ�񓖉w�L�� BINARY(1);
	DECLARE @�召�敪��l���� BINARY(1);
	DECLARE @�w��PBcd�Ȕԍ� INT;
	DECLARE @�w��P�ȋL�� INT;
	DECLARE @�w��QBcd�Ȕԍ� INT;
	DECLARE @�w��Q�ȋL�� INT;
	DECLARE @�w��RBcd�Ȕԍ� INT;
	DECLARE @�w��R�ȋL�� INT;
	DECLARE @����敪 INT;
	DECLARE @���� BINARY(2);

	--TODO: �w���@�킪����č�����i���Ȏw�茔�łȂ��̂ɍ�����j�w�茔���ɂ��Ă��A
	--�w���Ԃ̑S�o�C�g��0x00�ɂȂ邱�Ƃ͂Ȃ����ƍl���Ă悢���H
	--���̂悤�Ȏw�茔���𗘗p�f�[�^�̎w�茔���P�ɓ������ƁA
	--���Y���p�f�[�^���̑S�Ă̎w�茔�����̂ĂĂ��܂����ƂɂȂ�B
	--���̂��Ƃ́A���C�^�ǂɂ�����Ή��D���T�[�oI/F�ɂ�������B
	DECLARE SrcRowCursorC1 CURSOR STATIC READ_ONLY FORWARD_ONLY FOR
	 SELECT
	  [��{�w�b�_�[��������],
	  [��{�w�b�_�[�w�R�[�h],
	  [��{�w�b�_�[�R�[�i�[],
	  [��{�w�b�_�[���@],
	  [��{�w�b�_�[�V�[�P���XNo],
	  [�w�茔���w��P�w����],
	  [�w�茔���w��P��Ԕԍ�],
	  [�w�茔���w��P���Ԕԍ�],
	  [�w�茔���w��P�Ȕ�],
	  [�w�茔���w��Q�w����],
	  [�w�茔���w��Q��Ԕԍ�],
	  [�w�茔���w��Q���Ԕԍ�],
	  [�w�茔���w��Q�Ȕ�],
	  [�w�茔���w��R�w����],
	  [�w�茔���w��R��Ԕԍ�],
	  [�w�茔���w��R���Ԕԍ�],
	  [�w�茔���w��R�Ȕ�],
	  [���ǎ���P���ڏ��W�v����],
	  [���ǎ���P���ڏ�񊄈�],
	  [���ǎ���Q���ڏ��W�v����],
	  [���ǎ���Q���ڏ�񊄈�],
	  [���ǎ���R���ڏ��W�v����],
	  [���ǎ���R���ڏ�񊄈�],
	  [���ǎ���S���ڏ��W�v����],
	  [���ǎ���S���ڏ�񊄈�],
	  [���ǎ���P���ڏ��r�b�g],
	  [���ǎ���Q���ڏ��r�b�g],
	  [���ǎ���R���ڏ��r�b�g],
	  [���ǎ���S���ڏ��r�b�g],
	  [�召�敪��l����]
	  FROM D_RIYO_DATA_C1_TEMP_${Sta}
	  WHERE ([���茋��] = 0x0000) AND ([�ʉߕ���] = 0x02) AND ([�w�茔���w��P�w����] <> 0x00000000)
	   AND (CAST([���ǎ���P���ڏ��r�b�g] AS int) & 0x0040 = 0)
	   AND (CAST([���ǎ���Q���ڏ��r�b�g] AS int) & 0x0040 = 0)
	   AND (CAST([���ǎ���R���ڏ��r�b�g] AS int) & 0x0040 = 0)
	   AND (CAST([���ǎ���S���ڏ��r�b�g] AS int) & 0x0040 = 0);

	--TODO: �w���@�킪����č�����i���Ȏw�茔�łȂ��̂ɍ�����j�w�茔���ɂ��Ă��A
	--�w���Ԃ̑S�o�C�g��0x00�ɂȂ邱�Ƃ͂Ȃ����ƍl���Ă悢���H
	--���̂悤�Ȏw�茔���𗘗p�f�[�^�̎w�茔���P�ɓ������ƁA
	--���Y���p�f�[�^���̑S�Ă̎w�茔�����̂ĂĂ��܂����ƂɂȂ�B
	--���̂��Ƃ́A���C�^�ǂɂ�����Ή��D���T�[�oI/F�ɂ�������B
	DECLARE SrcRowCursorW1 CURSOR STATIC READ_ONLY FORWARD_ONLY FOR
	 SELECT
	  [��{�w�b�_�[��������],
	  [��{�w�b�_�[�w�R�[�h],
	  [��{�w�b�_�[�R�[�i�[],
	  [��{�w�b�_�[���@],
	  [��{�w�b�_�[�V�[�P���XNo],
	  [�w�茔���w��P�w����],
	  [�w�茔���w��P��Ԕԍ�],
	  [�w�茔���w��P���Ԕԍ�],
	  [�w�茔���w��P�Ȕ�],
	  [�w�茔���w��Q�w����],
	  [�w�茔���w��Q��Ԕԍ�],
	  [�w�茔���w��Q���Ԕԍ�],
	  [�w�茔���w��Q�Ȕ�],
	  [�w�茔���w��R�w����],
	  [�w�茔���w��R��Ԕԍ�],
	  [�w�茔���w��R���Ԕԍ�],
	  [�w�茔���w��R�Ȕ�],
	  [���ǎ���P���ڏ��W�v����],
	  [���ǎ���P���ڏ�񊄈�],
	  [���ǎ���Q���ڏ��W�v����],
	  [���ǎ���Q���ڏ�񊄈�],
	  [���ǎ���R���ڏ��W�v����],
	  [���ǎ���R���ڏ�񊄈�],
	  [���ǎ���S���ڏ��W�v����],
	  [���ǎ���S���ڏ�񊄈�],
	  [���ǎ���P���ڏ�񓖉w�L��],
	  [���ǎ���Q���ڏ�񓖉w�L��],
	  [���ǎ���R���ڏ�񓖉w�L��],
	  [���ǎ���S���ڏ�񓖉w�L��],
	  [�召�敪��l����]
	  FROM D_RIYO_DATA_W1_TEMP_${Sta}
	  WHERE ([���茋��] = 0x0000) AND ([�ʘH����] = 0x01 OR [�ʘH����] = 0x03) AND ([�w�茔���w��P�w����] <> 0x00000000);

	SET NOCOUNT ON;
	SET @CurTime = GETDATE();

	WITH cte AS
	(SELECT ROW_NUMBER() OVER
	 (PARTITION BY
	  [��{�w�b�_�[��������],
	  [��{�w�b�_�[�w�R�[�h],
	  [��{�w�b�_�[�R�[�i�[],
	  [��{�w�b�_�[���@],
	  [��{�w�b�_�[�V�[�P���XNo]
	  ORDER BY (SELECT NULL)) RN
	 FROM D_RIYO_DATA_C1_TEMP_${Sta})
	DELETE FROM cte WHERE RN > 1;

	--TODO: �@��\���ɉ��D�@�ʉ߃f�[�^�̍̎�ΏۂƂ���ׂ�����ȉw�i���悪'070'�łȂ��w�Ȃǁj��
	--�ǉ����ꂽ�ꍇ�́A���̏��������������ƁB
	--NOTE: �����A��B�V��������̉w�Ŕ��������iJR��B�́j���p�f�[�^�ɋN������
	--���D�@�ʉ߃f�[�^��~�ς��邱�ƂɂȂ�Ƃ��Ă��A���p�f�[�^�𒼐ڎ��W����Ƃ�
	--�l���ɂ������߁A�����Ő���'071'�͋��e���Ȃ����Ƃɂ��Ă���B
	--NOTE: ������̑����́A���܂̂Ƃ��둶�݂��Ȃ����A�O�̂��ߑΏۂɂ��Ă���B
	IF SUBSTRING('${Sta}',1,3) = '070' OR '${Sta}' = '119003'
	BEGIN
		OPEN SrcRowCursorC1;
		FETCH NEXT FROM SrcRowCursorC1 INTO
		 @��{�w�b�_�[��������,
		 @��{�w�b�_�[�w�R�[�h,
		 @��{�w�b�_�[�R�[�i�[,
		 @��{�w�b�_�[���@,
		 @��{�w�b�_�[�V�[�P���XNo,
		 @�w�茔���w��P�w����,
		 @�w�茔���w��P��Ԕԍ�,
		 @�w�茔���w��P���Ԕԍ�,
		 @�w�茔���w��P�Ȕ�,
		 @�w�茔���w��Q�w����,
		 @�w�茔���w��Q��Ԕԍ�,
		 @�w�茔���w��Q���Ԕԍ�,
		 @�w�茔���w��Q�Ȕ�,
		 @�w�茔���w��R�w����,
		 @�w�茔���w��R��Ԕԍ�,
		 @�w�茔���w��R���Ԕԍ�,
		 @�w�茔���w��R�Ȕ�,
		 @���ǎ���P���ڏ��W�v����,
		 @���ǎ���P���ڏ�񊄈�,
		 @���ǎ���Q���ڏ��W�v����,
		 @���ǎ���Q���ڏ�񊄈�,
		 @���ǎ���R���ڏ��W�v����,
		 @���ǎ���R���ڏ�񊄈�,
		 @���ǎ���S���ڏ��W�v����,
		 @���ǎ���S���ڏ�񊄈�,
		 @���ǎ���P���ڏ��r�b�g,
		 @���ǎ���Q���ڏ��r�b�g,
		 @���ǎ���R���ڏ��r�b�g,
		 @���ǎ���S���ڏ��r�b�g,
		 @�召�敪��l����;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @�w��PBcd�Ȕԍ� = CAST(SUBSTRING(@�w�茔���w��P�Ȕ�,1,1) AS INT);
			SET @�w��P�ȋL�� = CAST(SUBSTRING(@�w�茔���w��P�Ȕ�,2,1) AS INT) / 16;
			SET @�w�茔���w��P��Ԕԍ�Dst = CAST(@�w�茔���w��P��Ԕԍ� % 1000 AS BINARY(2));
			SET @�w�茔���w��P���Ԕԍ� = CAST(CAST(@�w�茔���w��P���Ԕԍ� AS INT) & 0x3F AS BINARY(1));
			SET @�w�茔���w��P�Ȕ�Dst = CAST((@�w��PBcd�Ȕԍ� / 16 * 10 + @�w��PBcd�Ȕԍ� % 16) * 8 + @�w��P�ȋL�� AS BINARY(1));
			IF SUBSTRING(@�w�茔���w��P�w����,1,1) = 0x00 OR
			   SUBSTRING(@�w�茔���w��P�w����,2,1) = 0x00 OR
			   SUBSTRING(@�w�茔���w��P�w����,3,1) = 0x00 OR
			   SUBSTRING(@�w�茔���w��P�w����,4,1) = 0x00 OR
			   @�w�茔���w��P��Ԕԍ�Dst = 0x0000 OR
			   @�w�茔���w��P���Ԕԍ� = 0x00 OR
			   @�w�茔���w��P�Ȕ�Dst = 0x00
			BEGIN
				SET @�w�茔���w��P�w���� = 0x00000000;
				SET @�w�茔���w��P��Ԕԍ�Dst = 0x0000;
				SET @�w�茔���w��P���Ԕԍ� = 0x00;
				SET @�w�茔���w��P�Ȕ�Dst = 0x00;
			END;

			SET @�w��QBcd�Ȕԍ� = CAST(SUBSTRING(@�w�茔���w��Q�Ȕ�,1,1) AS INT);
			SET @�w��Q�ȋL�� = CAST(SUBSTRING(@�w�茔���w��Q�Ȕ�,2,1) AS INT) / 16;
			SET @�w�茔���w��Q��Ԕԍ�Dst = CAST(@�w�茔���w��Q��Ԕԍ� % 1000 AS BINARY(2));
			SET @�w�茔���w��Q���Ԕԍ� = CAST(CAST(@�w�茔���w��Q���Ԕԍ� AS INT) & 0x3F AS BINARY(1));
			SET @�w�茔���w��Q�Ȕ�Dst = CAST((@�w��QBcd�Ȕԍ� / 16 * 10 + @�w��QBcd�Ȕԍ� % 16) * 8 + @�w��Q�ȋL�� AS BINARY(1));
			IF SUBSTRING(@�w�茔���w��Q�w����,1,1) = 0x00 OR
			   SUBSTRING(@�w�茔���w��Q�w����,2,1) = 0x00 OR
			   SUBSTRING(@�w�茔���w��Q�w����,3,1) = 0x00 OR
			   SUBSTRING(@�w�茔���w��Q�w����,4,1) = 0x00 OR
			   @�w�茔���w��Q��Ԕԍ�Dst = 0x0000 OR
			   @�w�茔���w��Q���Ԕԍ� = 0x00 OR
			   @�w�茔���w��Q�Ȕ�Dst = 0x00
			BEGIN
				SET @�w�茔���w��Q�w���� = 0x00000000;
				SET @�w�茔���w��Q��Ԕԍ�Dst = 0x0000;
				SET @�w�茔���w��Q���Ԕԍ� = 0x00;
				SET @�w�茔���w��Q�Ȕ�Dst = 0x00;
			END;

			SET @�w��RBcd�Ȕԍ� = CAST(SUBSTRING(@�w�茔���w��R�Ȕ�,1,1) AS INT);
			SET @�w��R�ȋL�� = CAST(SUBSTRING(@�w�茔���w��R�Ȕ�,2,1) AS INT) / 16;
			SET @�w�茔���w��R��Ԕԍ�Dst = CAST(@�w�茔���w��R��Ԕԍ� % 1000 AS BINARY(2));
			SET @�w�茔���w��R���Ԕԍ� = CAST(CAST(@�w�茔���w��R���Ԕԍ� AS INT) & 0x3F AS BINARY(1));
			SET @�w�茔���w��R�Ȕ�Dst = CAST((@�w��RBcd�Ȕԍ� / 16 * 10 + @�w��RBcd�Ȕԍ� % 16) * 8 + @�w��R�ȋL�� AS BINARY(1));
			IF SUBSTRING(@�w�茔���w��R�w����,1,1) = 0x00 OR
			   SUBSTRING(@�w�茔���w��R�w����,2,1) = 0x00 OR
			   SUBSTRING(@�w�茔���w��R�w����,3,1) = 0x00 OR
			   SUBSTRING(@�w�茔���w��R�w����,4,1) = 0x00 OR
			   @�w�茔���w��R��Ԕԍ�Dst = 0x0000 OR
			   @�w�茔���w��R���Ԕԍ� = 0x00 OR
			   @�w�茔���w��R�Ȕ�Dst = 0x00
			BEGIN
				SET @�w�茔���w��R�w���� = 0x00000000;
				SET @�w�茔���w��R��Ԕԍ�Dst = 0x0000;
				SET @�w�茔���w��R���Ԕԍ� = 0x00;
				SET @�w�茔���w��R�Ȕ�Dst = 0x00;
			END;

			IF @�w�茔���w��P�w���� <> 0x00000000 OR @�w�茔���w��Q�w���� <> 0x00000000 OR @�w�茔���w��R�w���� <> 0x00000000
			BEGIN
				SET @���ǎ���P���ڏ�񓖉w�L�� = CAST((@���ǎ���P���ڏ��r�b�g & 4) / 4 AS BINARY(1))
				SET @���ǎ���Q���ڏ�񓖉w�L�� = CAST((@���ǎ���Q���ڏ��r�b�g & 4) / 4 AS BINARY(1))
				SET @���ǎ���R���ڏ�񓖉w�L�� = CAST((@���ǎ���R���ڏ��r�b�g & 4) / 4 AS BINARY(1))
				SET @���ǎ���S���ڏ�񓖉w�L�� = CAST((@���ǎ���S���ڏ��r�b�g & 4) / 4 AS BINARY(1))
				SET @���� = dbo.ufnDiscountCodesForRiyoData(
				 @���ǎ���P���ڏ�񓖉w�L��, @���ǎ���P���ڏ��W�v����, @���ǎ���P���ڏ�񊄈�,
				 @���ǎ���Q���ڏ�񓖉w�L��, @���ǎ���Q���ڏ��W�v����, @���ǎ���Q���ڏ�񊄈�,
				 @���ǎ���R���ڏ�񓖉w�L��, @���ǎ���R���ڏ��W�v����, @���ǎ���R���ڏ�񊄈�,
				 @���ǎ���S���ڏ�񓖉w�L��, @���ǎ���S���ڏ��W�v����, @���ǎ���S���ڏ�񊄈�);

				INSERT INTO ${ShiteiDataDatabaseName}.dbo.D_SHITEI_DATA_${Sta}
				 (UPDATE_DATE,
				  GICA_GET_STATUS,
				  [��{�w�b�_�[��������],
				  [��{�w�b�_�[�w�R�[�h],
				  [��{�w�b�_�[�R�[�i�[],
				  [��{�w�b�_�[���@],
				  [��{�w�b�_�[�V�[�P���XNo],
				  [�w�茔���w��P�w����],
				  [�w�茔���w��P��Ԕԍ�],
				  [�w�茔���w��P���Ԕԍ�],
				  [�w�茔���w��P�Ȕ�],
				  [�w�茔���w��P����1],
				  [�w�茔���w��P����2],
				  [�w�茔���w��P�W�v����],
				  [�w�茔���w��Q�w����],
				  [�w�茔���w��Q��Ԕԍ�],
				  [�w�茔���w��Q���Ԕԍ�],
				  [�w�茔���w��Q�Ȕ�],
				  [�w�茔���w��Q����1],
				  [�w�茔���w��Q����2],
				  [�w�茔���w��Q�W�v����],
				  [�w�茔���w��R�w����],
				  [�w�茔���w��R��Ԕԍ�],
				  [�w�茔���w��R���Ԕԍ�],
				  [�w�茔���w��R�Ȕ�],
				  [�w�茔���w��R����1],
				  [�w�茔���w��R����2],
				  [�w�茔���w��R�W�v����],
				  [�召])
				 VALUES
				 (@CurTime,
				  0,
				  @��{�w�b�_�[��������,
				  @��{�w�b�_�[�w�R�[�h,
				  @��{�w�b�_�[�R�[�i�[,
				  @��{�w�b�_�[���@,
				  @��{�w�b�_�[�V�[�P���XNo,
				  @�w�茔���w��P�w����,
				  @�w�茔���w��P��Ԕԍ�Dst,
				  @�w�茔���w��P���Ԕԍ�,
				  @�w�茔���w��P�Ȕ�Dst,
				  SUBSTRING(@����,1,1),
				  SUBSTRING(@����,2,1),
				  0x00,
				  @�w�茔���w��Q�w����,
				  @�w�茔���w��Q��Ԕԍ�Dst,
				  @�w�茔���w��Q���Ԕԍ�,
				  @�w�茔���w��Q�Ȕ�Dst,
				  SUBSTRING(@����,1,1),
				  SUBSTRING(@����,2,1),
				  0x00,
				  @�w�茔���w��R�w����,
				  @�w�茔���w��R��Ԕԍ�Dst,
				  @�w�茔���w��R���Ԕԍ�,
				  @�w�茔���w��R�Ȕ�Dst,
				  SUBSTRING(@����,1,1),
				  SUBSTRING(@����,2,1),
				  0x00,
				  @�召�敪��l����);
			END;

			FETCH NEXT FROM SrcRowCursorC1 INTO
			 @��{�w�b�_�[��������,
			 @��{�w�b�_�[�w�R�[�h,
			 @��{�w�b�_�[�R�[�i�[,
			 @��{�w�b�_�[���@,
			 @��{�w�b�_�[�V�[�P���XNo,
			 @�w�茔���w��P�w����,
			 @�w�茔���w��P��Ԕԍ�,
			 @�w�茔���w��P���Ԕԍ�,
			 @�w�茔���w��P�Ȕ�,
			 @�w�茔���w��Q�w����,
			 @�w�茔���w��Q��Ԕԍ�,
			 @�w�茔���w��Q���Ԕԍ�,
			 @�w�茔���w��Q�Ȕ�,
			 @�w�茔���w��R�w����,
			 @�w�茔���w��R��Ԕԍ�,
			 @�w�茔���w��R���Ԕԍ�,
			 @�w�茔���w��R�Ȕ�,
			 @���ǎ���P���ڏ��W�v����,
			 @���ǎ���P���ڏ�񊄈�,
			 @���ǎ���Q���ڏ��W�v����,
			 @���ǎ���Q���ڏ�񊄈�,
			 @���ǎ���R���ڏ��W�v����,
			 @���ǎ���R���ڏ�񊄈�,
			 @���ǎ���S���ڏ��W�v����,
			 @���ǎ���S���ڏ�񊄈�,
			 @���ǎ���P���ڏ��r�b�g,
			 @���ǎ���Q���ڏ��r�b�g,
			 @���ǎ���R���ڏ��r�b�g,
			 @���ǎ���S���ڏ��r�b�g,
			 @�召�敪��l����;
		END;
		CLOSE SrcRowCursorC1;
		DEALLOCATE SrcRowCursorC1;
	END;

	MERGE
	 INTO ${RiyoDataDatabaseName}.dbo.D_RIYO_DATA_C1_${Sta} AS Target
	 USING D_RIYO_DATA_C1_TEMP_${Sta} AS Source
	 ON
	 (Target.[��{�w�b�_�[��������] = Source.[��{�w�b�_�[��������] AND
	  Target.[��{�w�b�_�[�w�R�[�h] = Source.[��{�w�b�_�[�w�R�[�h] AND
	  Target.[��{�w�b�_�[�R�[�i�[] = Source.[��{�w�b�_�[�R�[�i�[] AND
	  Target.[��{�w�b�_�[���@] = Source.[��{�w�b�_�[���@] AND
	  Target.[��{�w�b�_�[�V�[�P���XNo] = Source.[��{�w�b�_�[�V�[�P���XNo])
	 WHEN NOT MATCHED THEN
	 INSERT
	 (UPDATE_DATE,
	  [��{�w�b�_�[�f�[�^���],
	  [��{�w�b�_�[��������],
	  [��{�w�b�_�[�R�[�i�[],
	  [��{�w�b�_�[���@],
	  [��{�w�b�_�[�V�[�P���XNo],
	  [��{�w�b�_�[�o�[�W����],
	  [��{�w�b�_�[�w�R�[�h],
	  [�ʉߕ���],
	  [���b�`�`��],
	  [���茋��],
	  [��������Ԍ����w],
	  [��������Ԍ����w],
	  [���������}�����w],
	  [���������}�����w],
	  [�������̂��݋�Ԕ��w],
	  [�������̂��݋�Ԓ��w],
	  [�������O���[����Ԕ��w],
	  [�������O���[����Ԓ��w],
	  [�������IC��Ԕ��w],
	  [�������IC��Ԓ��w],
	  [�������t���[���],
	  [�������FREX��Ԕ��w],
	  [�������FREX��Ԓ��w],
	  [����w����Ԍ�����w],
	  [����w����Ԍ��R�[�i],
	  [����w����Ԍ����@],
	  [����w�����}������w],
	  [�����������Ԍ���������],
	  [������������}����������],
	  [���w��������Ԍ���ԉw],
	  [���w��������Ԍ��R�[�i],
	  [���w��������Ԍ����@],
	  [���w���������}����ԉw],
	  [���w���猔����Ԍ����w],
	  [�召�敪��l����],
	  [���ʋ敪�j������],
	  [IC���p�V����IC���p],
	  [IC���p�܂Ō�IC���p],
	  [IC���p���猔IC���p],
	  [�w�茔���w��P�w����],
	  [�w�茔���w��P��Ԕԍ�],
	  [�w�茔���w��P���Ԕԍ�],
	  [�w�茔���w��P�Ȕ�],
	  [�w�茔���w��Q�w����],
	  [�w�茔���w��Q��Ԕԍ�],
	  [�w�茔���w��Q���Ԕԍ�],
	  [�w�茔���w��Q�Ȕ�],
	  [�w�茔���w��R�w����],
	  [�w�茔���w��R��Ԕԍ�],
	  [�w�茔���w��R���Ԕԍ�],
	  [�w�茔���w��R�Ȕ�],
	  [�s������Ώۋ敪�r�b�g],
	  [�s������m�f����],
	  [��������],
	  [���p�p�^�[�����],
	  [���ǎ���P���ڏ��W�v����],
	  [���ǎ���P���ڏ���Ԍ����],
	  [���ǎ���P���ڏ����}�����],
	  [���ǎ���P���ڏ��t���[���],
	  [���ǎ���P���ڏ��搔],
	  [���ǎ���P���ڏ����o����],
	  [���ǎ���P���ڏ��r�b�g],
	  [���ǎ���P���ڏ�񊄈�],
	  [���ǎ���P���ڏ��EXIC����],
	  [���ǎ���P���ڏ�񏤕i�ԍ�],
	  [���ǎ���P���ڏ�񔭍s���],
	  [���ǎ���P���ڏ��L���J�n��],
	  [���ǎ���P���ڏ�񔭍s����],
	  [���ǎ���P���ڏ�񍆎Ԕԍ�],
	  [���ǎ���P���ڏ�񗿋����敪],
	  [���ǎ���Q���ڏ��W�v����],
	  [���ǎ���Q���ڏ���Ԍ����],
	  [���ǎ���Q���ڏ����}�����],
	  [���ǎ���Q���ڏ��t���[���],
	  [���ǎ���Q���ڏ��搔],
	  [���ǎ���Q���ڏ����o����],
	  [���ǎ���Q���ڏ��r�b�g],
	  [���ǎ���Q���ڏ�񊄈�],
	  [���ǎ���Q���ڏ��EXIC����],
	  [���ǎ���Q���ڏ�񏤕i�ԍ�],
	  [���ǎ���Q���ڏ�񔭍s���],
	  [���ǎ���Q���ڏ��L���J�n��],
	  [���ǎ���Q���ڏ�񔭍s����],
	  [���ǎ���Q���ڏ�񍆎Ԕԍ�],
	  [���ǎ���Q���ڏ�񗿋����敪],
	  [���ǎ���R���ڏ��W�v����],
	  [���ǎ���R���ڏ���Ԍ����],
	  [���ǎ���R���ڏ����}�����],
	  [���ǎ���R���ڏ��t���[���],
	  [���ǎ���R���ڏ��搔],
	  [���ǎ���R���ڏ����o����],
	  [���ǎ���R���ڏ��r�b�g],
	  [���ǎ���R���ڏ�񊄈�],
	  [���ǎ���R���ڏ��EXIC����],
	  [���ǎ���R���ڏ�񏤕i�ԍ�],
	  [���ǎ���R���ڏ�񔭍s���],
	  [���ǎ���R���ڏ��L���J�n��],
	  [���ǎ���R���ڏ�񔭍s����],
	  [���ǎ���R���ڏ�񍆎Ԕԍ�],
	  [���ǎ���R���ڏ�񗿋����敪],
	  [���ǎ���S���ڏ��W�v����],
	  [���ǎ���S���ڏ���Ԍ����],
	  [���ǎ���S���ڏ����}�����],
	  [���ǎ���S���ڏ��t���[���],
	  [���ǎ���S���ڏ��搔],
	  [���ǎ���S���ڏ����o����],
	  [���ǎ���S���ڏ��r�b�g],
	  [���ǎ���S���ڏ�񊄈�],
	  [���ǎ���S���ڏ��EXIC����],
	  [���ǎ���S���ڏ�񏤕i�ԍ�],
	  [���ǎ���S���ڏ�񔭍s���],
	  [���ǎ���S���ڏ��L���J�n��],
	  [���ǎ���S���ڏ�񔭍s����],
	  [���ǎ���S���ڏ�񍆎Ԕԍ�],
	  [���ǎ���S���ڏ�񗿋����敪],
	  [�h�c�ԍ�],
	  [�r�e��������z],
	  [�r�e���p��ԂP�E���p�w�P],
	  [�r�e���p��ԂP�E���p�w�Q],
	  [�r�e���p��ԂQ�E���p�w�P],
	  [�r�e���p��ԂQ�E���p�w�Q],
	  [��Ԏn�_�w],
	  [���ʂ��}�X�^�K�p�L��],
	  [�\��],
	  [�T���l],
	  [����m�f�R�[�h�P],
	  [����m�f�R�[�h�P�Y����],
	  [����m�f�R�[�h�Q],
	  [����m�f�R�[�h�Q�Y����],
	  [����m�f�R�[�h�R],
	  [����m�f�R�[�h�R�Y����],
	  [����m�f�R�[�h�S],
	  [����m�f�R�[�h�S�Y����],
	  [����m�f�R�[�h�T],
	  [����m�f�R�[�h�T�Y����],
	  [����m�f�R�[�h�U],
	  [����m�f�R�[�h�U�Y����],
	  [����m�f�R�[�h�V],
	  [����m�f�R�[�h�V�Y����],
	  [����m�f�R�[�h�W],
	  [����m�f�R�[�h�W�Y����],
	  [���G���R�[�h���P���ڏ��],
	  [���G���R�[�h���Q���ڏ��],
	  [���G���R�[�h���R���ڏ��],
	  [���G���R�[�h���S���ڏ��])
	 VALUES
	 (@CurTime,
	  Source.[��{�w�b�_�[�f�[�^���],
	  Source.[��{�w�b�_�[��������],
	  Source.[��{�w�b�_�[�R�[�i�[],
	  Source.[��{�w�b�_�[���@],
	  Source.[��{�w�b�_�[�V�[�P���XNo],
	  Source.[��{�w�b�_�[�o�[�W����],
	  Source.[��{�w�b�_�[�w�R�[�h],
	  Source.[�ʉߕ���],
	  Source.[���b�`�`��],
	  Source.[���茋��],
	  Source.[��������Ԍ����w],
	  Source.[��������Ԍ����w],
	  Source.[���������}�����w],
	  Source.[���������}�����w],
	  Source.[�������̂��݋�Ԕ��w],
	  Source.[�������̂��݋�Ԓ��w],
	  Source.[�������O���[����Ԕ��w],
	  Source.[�������O���[����Ԓ��w],
	  Source.[�������IC��Ԕ��w],
	  Source.[�������IC��Ԓ��w],
	  Source.[�������t���[���],
	  Source.[�������FREX��Ԕ��w],
	  Source.[�������FREX��Ԓ��w],
	  Source.[����w����Ԍ�����w],
	  Source.[����w����Ԍ��R�[�i],
	  Source.[����w����Ԍ����@],
	  Source.[����w�����}������w],
	  Source.[�����������Ԍ���������],
	  Source.[������������}����������],
	  Source.[���w��������Ԍ���ԉw],
	  Source.[���w��������Ԍ��R�[�i],
	  Source.[���w��������Ԍ����@],
	  Source.[���w���������}����ԉw],
	  Source.[���w���猔����Ԍ����w],
	  Source.[�召�敪��l����],
	  Source.[���ʋ敪�j������],
	  Source.[IC���p�V����IC���p],
	  Source.[IC���p�܂Ō�IC���p],
	  Source.[IC���p���猔IC���p],
	  Source.[�w�茔���w��P�w����],
	  Source.[�w�茔���w��P��Ԕԍ�],
	  Source.[�w�茔���w��P���Ԕԍ�],
	  Source.[�w�茔���w��P�Ȕ�],
	  Source.[�w�茔���w��Q�w����],
	  Source.[�w�茔���w��Q��Ԕԍ�],
	  Source.[�w�茔���w��Q���Ԕԍ�],
	  Source.[�w�茔���w��Q�Ȕ�],
	  Source.[�w�茔���w��R�w����],
	  Source.[�w�茔���w��R��Ԕԍ�],
	  Source.[�w�茔���w��R���Ԕԍ�],
	  Source.[�w�茔���w��R�Ȕ�],
	  Source.[�s������Ώۋ敪�r�b�g],
	  Source.[�s������m�f����],
	  Source.[��������],
	  Source.[���p�p�^�[�����],
	  Source.[���ǎ���P���ڏ��W�v����],
	  Source.[���ǎ���P���ڏ���Ԍ����],
	  Source.[���ǎ���P���ڏ����}�����],
	  Source.[���ǎ���P���ڏ��t���[���],
	  Source.[���ǎ���P���ڏ��搔],
	  Source.[���ǎ���P���ڏ����o����],
	  Source.[���ǎ���P���ڏ��r�b�g],
	  Source.[���ǎ���P���ڏ�񊄈�],
	  Source.[���ǎ���P���ڏ��EXIC����],
	  Source.[���ǎ���P���ڏ�񏤕i�ԍ�],
	  Source.[���ǎ���P���ڏ�񔭍s���],
	  Source.[���ǎ���P���ڏ��L���J�n��],
	  Source.[���ǎ���P���ڏ�񔭍s����],
	  Source.[���ǎ���P���ڏ�񍆎Ԕԍ�],
	  Source.[���ǎ���P���ڏ�񗿋����敪],
	  Source.[���ǎ���Q���ڏ��W�v����],
	  Source.[���ǎ���Q���ڏ���Ԍ����],
	  Source.[���ǎ���Q���ڏ����}�����],
	  Source.[���ǎ���Q���ڏ��t���[���],
	  Source.[���ǎ���Q���ڏ��搔],
	  Source.[���ǎ���Q���ڏ����o����],
	  Source.[���ǎ���Q���ڏ��r�b�g],
	  Source.[���ǎ���Q���ڏ�񊄈�],
	  Source.[���ǎ���Q���ڏ��EXIC����],
	  Source.[���ǎ���Q���ڏ�񏤕i�ԍ�],
	  Source.[���ǎ���Q���ڏ�񔭍s���],
	  Source.[���ǎ���Q���ڏ��L���J�n��],
	  Source.[���ǎ���Q���ڏ�񔭍s����],
	  Source.[���ǎ���Q���ڏ�񍆎Ԕԍ�],
	  Source.[���ǎ���Q���ڏ�񗿋����敪],
	  Source.[���ǎ���R���ڏ��W�v����],
	  Source.[���ǎ���R���ڏ���Ԍ����],
	  Source.[���ǎ���R���ڏ����}�����],
	  Source.[���ǎ���R���ڏ��t���[���],
	  Source.[���ǎ���R���ڏ��搔],
	  Source.[���ǎ���R���ڏ����o����],
	  Source.[���ǎ���R���ڏ��r�b�g],
	  Source.[���ǎ���R���ڏ�񊄈�],
	  Source.[���ǎ���R���ڏ��EXIC����],
	  Source.[���ǎ���R���ڏ�񏤕i�ԍ�],
	  Source.[���ǎ���R���ڏ�񔭍s���],
	  Source.[���ǎ���R���ڏ��L���J�n��],
	  Source.[���ǎ���R���ڏ�񔭍s����],
	  Source.[���ǎ���R���ڏ�񍆎Ԕԍ�],
	  Source.[���ǎ���R���ڏ�񗿋����敪],
	  Source.[���ǎ���S���ڏ��W�v����],
	  Source.[���ǎ���S���ڏ���Ԍ����],
	  Source.[���ǎ���S���ڏ����}�����],
	  Source.[���ǎ���S���ڏ��t���[���],
	  Source.[���ǎ���S���ڏ��搔],
	  Source.[���ǎ���S���ڏ����o����],
	  Source.[���ǎ���S���ڏ��r�b�g],
	  Source.[���ǎ���S���ڏ�񊄈�],
	  Source.[���ǎ���S���ڏ��EXIC����],
	  Source.[���ǎ���S���ڏ�񏤕i�ԍ�],
	  Source.[���ǎ���S���ڏ�񔭍s���],
	  Source.[���ǎ���S���ڏ��L���J�n��],
	  Source.[���ǎ���S���ڏ�񔭍s����],
	  Source.[���ǎ���S���ڏ�񍆎Ԕԍ�],
	  Source.[���ǎ���S���ڏ�񗿋����敪],
	  Source.[�h�c�ԍ�],
	  Source.[�r�e��������z],
	  Source.[�r�e���p��ԂP�E���p�w�P],
	  Source.[�r�e���p��ԂP�E���p�w�Q],
	  Source.[�r�e���p��ԂQ�E���p�w�P],
	  Source.[�r�e���p��ԂQ�E���p�w�Q],
	  Source.[��Ԏn�_�w],
	  Source.[���ʂ��}�X�^�K�p�L��],
	  Source.[�\��],
	  Source.[�T���l],
	  Source.[����m�f�R�[�h�P],
	  Source.[����m�f�R�[�h�P�Y����],
	  Source.[����m�f�R�[�h�Q],
	  Source.[����m�f�R�[�h�Q�Y����],
	  Source.[����m�f�R�[�h�R],
	  Source.[����m�f�R�[�h�R�Y����],
	  Source.[����m�f�R�[�h�S],
	  Source.[����m�f�R�[�h�S�Y����],
	  Source.[����m�f�R�[�h�T],
	  Source.[����m�f�R�[�h�T�Y����],
	  Source.[����m�f�R�[�h�U],
	  Source.[����m�f�R�[�h�U�Y����],
	  Source.[����m�f�R�[�h�V],
	  Source.[����m�f�R�[�h�V�Y����],
	  Source.[����m�f�R�[�h�W],
	  Source.[����m�f�R�[�h�W�Y����],
	  Source.[���G���R�[�h���P���ڏ��],
	  Source.[���G���R�[�h���Q���ڏ��],
	  Source.[���G���R�[�h���R���ڏ��],
	  Source.[���G���R�[�h���S���ڏ��]);

	WITH cte AS
	(SELECT ROW_NUMBER() OVER
	 (PARTITION BY
	  [��{�w�b�_�[��������],
	  [��{�w�b�_�[�w�R�[�h],
	  [��{�w�b�_�[�R�[�i�[],
	  [��{�w�b�_�[���@],
	  [��{�w�b�_�[�V�[�P���XNo]
	  ORDER BY (SELECT NULL)) RN
	 FROM D_RIYO_DATA_W1_TEMP_${Sta})
	DELETE FROM cte WHERE RN > 1;

	--TODO: �@��\���ɉ��D�@�ʉ߃f�[�^�̍̎�ΏۂƂ���ׂ�����ȉw�i���悪'070'�łȂ��w�Ȃǁj��
	--�ǉ����ꂽ�ꍇ�́A���̏��������������ƁB
	--NOTE: �����A��B�V��������̉w�Ŕ��������iJR��B�́j���p�f�[�^�ɋN������
	--���D�@�ʉ߃f�[�^��~�ς��邱�ƂɂȂ�Ƃ��Ă��A���p�f�[�^�𒼐ڎ��W����Ƃ�
	--�l���ɂ������߁A�����Ő���'071'�͋��e���Ȃ����Ƃɂ��Ă���B
	IF SUBSTRING('${Sta}',1,3) = '070' OR '${Sta}' = '119003'
	BEGIN
		OPEN SrcRowCursorW1;
		FETCH NEXT FROM SrcRowCursorW1 INTO
		 @��{�w�b�_�[��������,
		 @��{�w�b�_�[�w�R�[�h,
		 @��{�w�b�_�[�R�[�i�[,
		 @��{�w�b�_�[���@,
		 @��{�w�b�_�[�V�[�P���XNo,
		 @�w�茔���w��P�w����,
		 @�w�茔���w��P��Ԕԍ�,
		 @�w�茔���w��P���Ԕԍ�,
		 @�w�茔���w��P�Ȕ�,
		 @�w�茔���w��Q�w����,
		 @�w�茔���w��Q��Ԕԍ�,
		 @�w�茔���w��Q���Ԕԍ�,
		 @�w�茔���w��Q�Ȕ�,
		 @�w�茔���w��R�w����,
		 @�w�茔���w��R��Ԕԍ�,
		 @�w�茔���w��R���Ԕԍ�,
		 @�w�茔���w��R�Ȕ�,
		 @���ǎ���P���ڏ��W�v����,
		 @���ǎ���P���ڏ�񊄈�,
		 @���ǎ���Q���ڏ��W�v����,
		 @���ǎ���Q���ڏ�񊄈�,
		 @���ǎ���R���ڏ��W�v����,
		 @���ǎ���R���ڏ�񊄈�,
		 @���ǎ���S���ڏ��W�v����,
		 @���ǎ���S���ڏ�񊄈�,
		 @���ǎ���P���ڏ�񓖉w�L��,
		 @���ǎ���Q���ڏ�񓖉w�L��,
		 @���ǎ���R���ڏ�񓖉w�L��,
		 @���ǎ���S���ڏ�񓖉w�L��,
		 @�召�敪��l����;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @�w��PBcd�Ȕԍ� = CAST(SUBSTRING(@�w�茔���w��P�Ȕ�,1,1) AS INT);
			SET @�w��P�ȋL�� = CAST(SUBSTRING(@�w�茔���w��P�Ȕ�,2,1) AS INT) / 16;
			SET @�w�茔���w��P��Ԕԍ�Dst = CAST((CAST(@�w�茔���w��P��Ԕԍ� AS INT) / 0x100 & 0xF) * 100 + (CAST(@�w�茔���w��P��Ԕԍ� AS INT) / 0x10 & 0xF) * 10 + (CAST(@�w�茔���w��P��Ԕԍ� AS INT) & 0xF) AS BINARY(2));
			SET @�w�茔���w��P���Ԕԍ� = CAST(CAST(@�w�茔���w��P���Ԕԍ� AS INT) & 0x3F AS BINARY(1));
			SET @�w�茔���w��P�Ȕ�Dst = CAST((@�w��PBcd�Ȕԍ� / 16 * 10 + @�w��PBcd�Ȕԍ� % 16) * 8 + @�w��P�ȋL�� AS BINARY(1));
			IF SUBSTRING(@�w�茔���w��P�w����,1,1) = 0x00 OR
			   SUBSTRING(@�w�茔���w��P�w����,2,1) = 0x00 OR
			   SUBSTRING(@�w�茔���w��P�w����,3,1) = 0x00 OR
			   SUBSTRING(@�w�茔���w��P�w����,4,1) = 0x00 OR
			   @�w�茔���w��P��Ԕԍ�Dst = 0x0000 OR
			   @�w�茔���w��P���Ԕԍ� = 0x00 OR
			   @�w�茔���w��P�Ȕ�Dst = 0x00
			BEGIN
				SET @�w�茔���w��P�w���� = 0x00000000;
				SET @�w�茔���w��P��Ԕԍ�Dst = 0x0000;
				SET @�w�茔���w��P���Ԕԍ� = 0x00;
				SET @�w�茔���w��P�Ȕ�Dst = 0x00;
			END;

			SET @�w��QBcd�Ȕԍ� = CAST(SUBSTRING(@�w�茔���w��Q�Ȕ�,1,1) AS INT);
			SET @�w��Q�ȋL�� = CAST(SUBSTRING(@�w�茔���w��Q�Ȕ�,2,1) AS INT) / 16;
			SET @�w�茔���w��Q��Ԕԍ�Dst = CAST((CAST(@�w�茔���w��Q��Ԕԍ� AS INT) / 0x100 & 0xF) * 100 + (CAST(@�w�茔���w��Q��Ԕԍ� AS INT) / 0x10 & 0xF) * 10 + (CAST(@�w�茔���w��Q��Ԕԍ� AS INT) & 0xF) AS BINARY(2));
			SET @�w�茔���w��Q���Ԕԍ� = CAST(CAST(@�w�茔���w��Q���Ԕԍ� AS INT) & 0x3F AS BINARY(1));
			SET @�w�茔���w��Q�Ȕ�Dst = CAST((@�w��QBcd�Ȕԍ� / 16 * 10 + @�w��QBcd�Ȕԍ� % 16) * 8 + @�w��Q�ȋL�� AS BINARY(1));
			IF SUBSTRING(@�w�茔���w��Q�w����,1,1) = 0x00 OR
			   SUBSTRING(@�w�茔���w��Q�w����,2,1) = 0x00 OR
			   SUBSTRING(@�w�茔���w��Q�w����,3,1) = 0x00 OR
			   SUBSTRING(@�w�茔���w��Q�w����,4,1) = 0x00 OR
			   @�w�茔���w��Q��Ԕԍ�Dst = 0x0000 OR
			   @�w�茔���w��Q���Ԕԍ� = 0x00 OR
			   @�w�茔���w��Q�Ȕ�Dst = 0x00
			BEGIN
				SET @�w�茔���w��Q�w���� = 0x00000000;
				SET @�w�茔���w��Q��Ԕԍ�Dst = 0x0000;
				SET @�w�茔���w��Q���Ԕԍ� = 0x00;
				SET @�w�茔���w��Q�Ȕ�Dst = 0x00;
			END;

			SET @�w��RBcd�Ȕԍ� = CAST(SUBSTRING(@�w�茔���w��R�Ȕ�,1,1) AS INT);
			SET @�w��R�ȋL�� = CAST(SUBSTRING(@�w�茔���w��R�Ȕ�,2,1) AS INT) / 16;
			SET @�w�茔���w��R��Ԕԍ�Dst = CAST((CAST(@�w�茔���w��R��Ԕԍ� AS INT) / 0x100 & 0xF) * 100 + (CAST(@�w�茔���w��R��Ԕԍ� AS INT) / 0x10 & 0xF) * 10 + (CAST(@�w�茔���w��R��Ԕԍ� AS INT) & 0xF) AS BINARY(2));
			SET @�w�茔���w��R���Ԕԍ� = CAST(CAST(@�w�茔���w��R���Ԕԍ� AS INT) & 0x3F AS BINARY(1));
			SET @�w�茔���w��R�Ȕ�Dst = CAST((@�w��RBcd�Ȕԍ� / 16 * 10 + @�w��RBcd�Ȕԍ� % 16) * 8 + @�w��R�ȋL�� AS BINARY(1));
			IF SUBSTRING(@�w�茔���w��R�w����,1,1) = 0x00 OR
			   SUBSTRING(@�w�茔���w��R�w����,2,1) = 0x00 OR
			   SUBSTRING(@�w�茔���w��R�w����,3,1) = 0x00 OR
			   SUBSTRING(@�w�茔���w��R�w����,4,1) = 0x00 OR
			   @�w�茔���w��R��Ԕԍ�Dst = 0x0000 OR
			   @�w�茔���w��R���Ԕԍ� = 0x00 OR
			   @�w�茔���w��R�Ȕ�Dst = 0x00
			BEGIN
				SET @�w�茔���w��R�w���� = 0x00000000;
				SET @�w�茔���w��R��Ԕԍ�Dst = 0x0000;
				SET @�w�茔���w��R���Ԕԍ� = 0x00;
				SET @�w�茔���w��R�Ȕ�Dst = 0x00;
			END;

			IF @�w�茔���w��P�w���� <> 0x00000000 OR @�w�茔���w��Q�w���� <> 0x00000000 OR @�w�茔���w��R�w���� <> 0x00000000
			BEGIN
				SET @���� = dbo.ufnDiscountCodesForRiyoData(
				 @���ǎ���P���ڏ�񓖉w�L��, @���ǎ���P���ڏ��W�v����, @���ǎ���P���ڏ�񊄈�,
				 @���ǎ���Q���ڏ�񓖉w�L��, @���ǎ���Q���ڏ��W�v����, @���ǎ���Q���ڏ�񊄈�,
				 @���ǎ���R���ڏ�񓖉w�L��, @���ǎ���R���ڏ��W�v����, @���ǎ���R���ڏ�񊄈�,
				 @���ǎ���S���ڏ�񓖉w�L��, @���ǎ���S���ڏ��W�v����, @���ǎ���S���ڏ�񊄈�);

				INSERT INTO ${ShiteiDataDatabaseName}.dbo.D_SHITEI_DATA_${Sta}
				 (UPDATE_DATE,
				  GICA_GET_STATUS,
				  [��{�w�b�_�[��������],
				  [��{�w�b�_�[�w�R�[�h],
				  [��{�w�b�_�[�R�[�i�[],
				  [��{�w�b�_�[���@],
				  [��{�w�b�_�[�V�[�P���XNo],
				  [�w�茔���w��P�w����],
				  [�w�茔���w��P��Ԕԍ�],
				  [�w�茔���w��P���Ԕԍ�],
				  [�w�茔���w��P�Ȕ�],
				  [�w�茔���w��P����1],
				  [�w�茔���w��P����2],
				  [�w�茔���w��P�W�v����],
				  [�w�茔���w��Q�w����],
				  [�w�茔���w��Q��Ԕԍ�],
				  [�w�茔���w��Q���Ԕԍ�],
				  [�w�茔���w��Q�Ȕ�],
				  [�w�茔���w��Q����1],
				  [�w�茔���w��Q����2],
				  [�w�茔���w��Q�W�v����],
				  [�w�茔���w��R�w����],
				  [�w�茔���w��R��Ԕԍ�],
				  [�w�茔���w��R���Ԕԍ�],
				  [�w�茔���w��R�Ȕ�],
				  [�w�茔���w��R����1],
				  [�w�茔���w��R����2],
				  [�w�茔���w��R�W�v����],
				  [�召])
				 VALUES
				 (@CurTime,
				  0,
				  @��{�w�b�_�[��������,
				  @��{�w�b�_�[�w�R�[�h,
				  @��{�w�b�_�[�R�[�i�[,
				  @��{�w�b�_�[���@,
				  @��{�w�b�_�[�V�[�P���XNo,
				  @�w�茔���w��P�w����,
				  @�w�茔���w��P��Ԕԍ�Dst,
				  @�w�茔���w��P���Ԕԍ�,
				  @�w�茔���w��P�Ȕ�Dst,
				  SUBSTRING(@����,1,1),
				  SUBSTRING(@����,2,1),
				  0x00,
				  @�w�茔���w��Q�w����,
				  @�w�茔���w��Q��Ԕԍ�Dst,
				  @�w�茔���w��Q���Ԕԍ�,
				  @�w�茔���w��Q�Ȕ�Dst,
				  SUBSTRING(@����,1,1),
				  SUBSTRING(@����,2,1),
				  0x00,
				  @�w�茔���w��R�w����,
				  @�w�茔���w��R��Ԕԍ�Dst,
				  @�w�茔���w��R���Ԕԍ�,
				  @�w�茔���w��R�Ȕ�Dst,
				  SUBSTRING(@����,1,1),
				  SUBSTRING(@����,2,1),
				  0x00,
				  @�召�敪��l����);
			END;

			FETCH NEXT FROM SrcRowCursorW1 INTO
			 @��{�w�b�_�[��������,
			 @��{�w�b�_�[�w�R�[�h,
			 @��{�w�b�_�[�R�[�i�[,
			 @��{�w�b�_�[���@,
			 @��{�w�b�_�[�V�[�P���XNo,
			 @�w�茔���w��P�w����,
			 @�w�茔���w��P��Ԕԍ�,
			 @�w�茔���w��P���Ԕԍ�,
			 @�w�茔���w��P�Ȕ�,
			 @�w�茔���w��Q�w����,
			 @�w�茔���w��Q��Ԕԍ�,
			 @�w�茔���w��Q���Ԕԍ�,
			 @�w�茔���w��Q�Ȕ�,
			 @�w�茔���w��R�w����,
			 @�w�茔���w��R��Ԕԍ�,
			 @�w�茔���w��R���Ԕԍ�,
			 @�w�茔���w��R�Ȕ�,
			 @���ǎ���P���ڏ��W�v����,
			 @���ǎ���P���ڏ�񊄈�,
			 @���ǎ���Q���ڏ��W�v����,
			 @���ǎ���Q���ڏ�񊄈�,
			 @���ǎ���R���ڏ��W�v����,
			 @���ǎ���R���ڏ�񊄈�,
			 @���ǎ���S���ڏ��W�v����,
			 @���ǎ���S���ڏ�񊄈�,
			 @���ǎ���P���ڏ�񓖉w�L��,
			 @���ǎ���Q���ڏ�񓖉w�L��,
			 @���ǎ���R���ڏ�񓖉w�L��,
			 @���ǎ���S���ڏ�񓖉w�L��,
			 @�召�敪��l����;
		END;
		CLOSE SrcRowCursorW1;
		DEALLOCATE SrcRowCursorW1;
	END;

	MERGE
	 INTO ${RiyoDataDatabaseName}.dbo.D_RIYO_DATA_W1_${Sta} AS Target
	 USING D_RIYO_DATA_W1_TEMP_${Sta} AS Source
	 ON
	 (Target.[��{�w�b�_�[��������] = Source.[��{�w�b�_�[��������] AND
	  Target.[��{�w�b�_�[�w�R�[�h] = Source.[��{�w�b�_�[�w�R�[�h] AND
	  Target.[��{�w�b�_�[�R�[�i�[] = Source.[��{�w�b�_�[�R�[�i�[] AND
	  Target.[��{�w�b�_�[���@] = Source.[��{�w�b�_�[���@] AND
	  Target.[��{�w�b�_�[�V�[�P���XNo] = Source.[��{�w�b�_�[�V�[�P���XNo])
	 WHEN NOT MATCHED THEN
	 INSERT
	 (UPDATE_DATE,
	  [��{�w�b�_�[�f�[�^���],
	  [��{�w�b�_�[�w�R�[�h],
	  [��{�w�b�_�[��������],
	  [��{�w�b�_�[�R�[�i�[],
	  [��{�w�b�_�[���@],
	  [��{�w�b�_�[�V�[�P���XNo],
	  [��{�w�b�_�[�o�[�W����],
	  [�ʘH����],
	  [���茋��],
	  [��������Ԍ����w],
	  [��������Ԍ����w],
	  [���������}�����w],
	  [���������}�����w],
	  [�������̂��݋�Ԕ��w],
	  [�������̂��݋�Ԓ��w],
	  [�������O���[����Ԕ��w],
	  [�������O���[����Ԓ��w],
	  [�������FREX��Ԕ��w],
	  [�������FREX��Ԓ��w],
	  [����w����Ԍ�����w],
	  [����w�����}������w],
	  [�����������Ԍ���������],
	  [������������}����������],
	  [���w��������Ԍ���ԉw],
	  [���w���������}����ԉw],
	  [���w���猔����Ԍ����w],
	  [�召�敪��l����],
	  [���ʋ敪�j������],
	  [�ʗp��ʂP���ڏ�񌔎�],
	  [�ʗp��ʂP���ڏ��L���J�n��],
	  [�ʗp��ʂP���ڏ��L���I����],
	  [�ʗp��ʂQ���ڏ�񌔎�],
	  [�ʗp��ʂQ���ڏ��L���J�n��],
	  [�ʗp��ʂQ���ڏ��L���I����],
	  [�ʗp��ʂR���ڏ�񌔎�],
	  [�ʗp��ʂR���ڏ��L���J�n��],
	  [�ʗp��ʂR���ڏ��L���I����],
	  [�w�茔���w��P�w����],
	  [�w�茔���w��P��Ԕԍ�],
	  [�w�茔���w��P���Ԕԍ�],
	  [�w�茔���w��P�Ȕ�],
	  [�w�茔���w��Q�w����],
	  [�w�茔���w��Q��Ԕԍ�],
	  [�w�茔���w��Q���Ԕԍ�],
	  [�w�茔���w��Q�Ȕ�],
	  [�w�茔���w��R�w����],
	  [�w�茔���w��R��Ԕԍ�],
	  [�w�茔���w��R���Ԕԍ�],
	  [�w�茔���w��R�Ȕ�],
	  [�s������Ώۋ敪�r�b�g],
	  [��������],
	  [���p�p�^�[�����],
	  [���ǎ���P���ڏ����],
	  [���ǎ���P���ڏ��W�v����],
	  [���ǎ���P���ڏ�񊄈�],
	  [���ǎ���P���ڏ�񏤕i�ԍ�],
	  [���ǎ���P���ڏ�񔭍s���],
	  [���ǎ���P���ڏ��L���J�n��],
	  [���ǎ���P���ڏ�񔭍s����],
	  [���ǎ���Q���ڏ����],
	  [���ǎ���Q���ڏ��W�v����],
	  [���ǎ���Q���ڏ�񊄈�],
	  [���ǎ���Q���ڏ�񏤕i�ԍ�],
	  [���ǎ���Q���ڏ�񔭍s���],
	  [���ǎ���Q���ڏ��L���J�n��],
	  [���ǎ���Q���ڏ�񔭍s����],
	  [���ǎ���R���ڏ����],
	  [���ǎ���R���ڏ��W�v����],
	  [���ǎ���R���ڏ�񊄈�],
	  [���ǎ���R���ڏ�񏤕i�ԍ�],
	  [���ǎ���R���ڏ�񔭍s���],
	  [���ǎ���R���ڏ��L���J�n��],
	  [���ǎ���R���ڏ�񔭍s����],
	  [���ǎ���S���ڏ����],
	  [���ǎ���S���ڏ��W�v����],
	  [���ǎ���S���ڏ�񊄈�],
	  [���ǎ���S���ڏ�񏤕i�ԍ�],
	  [���ǎ���S���ڏ�񔭍s���],
	  [���ǎ���S���ڏ��L���J�n��],
	  [���ǎ���S���ڏ�񔭍s����],
	  [���ǎ���P���ڏ�񓖉w�L��],
	  [���ǎ���Q���ڏ�񓖉w�L��],
	  [���ǎ���R���ڏ�񓖉w�L��],
	  [���ǎ���S���ڏ�񓖉w�L��],
	  [�r�e��������z],
	  [�r�e���p��ԂP�E���p�w�P],
	  [�r�e���p��ԂP�E���p�w�Q],
	  [�r�e���p��ԂQ�E���p�w�P],
	  [�r�e���p��ԂQ�E���p�w�Q],
	  [��Ԏn�_�w],
	  [���ʂ��}�X�^�K�p�L��],
	  [�\���P],
	  [�T���l],
	  [���g�p],
	  [���G���R�[�h���P���ڏ��],
	  [���G���R�[�h���Q���ڏ��],
	  [���G���R�[�h���R���ڏ��],
	  [���G���R�[�h���S���ڏ��])
	 VALUES
	 (@CurTime,
	  Source.[��{�w�b�_�[�f�[�^���],
	  Source.[��{�w�b�_�[�w�R�[�h],
	  Source.[��{�w�b�_�[��������],
	  Source.[��{�w�b�_�[�R�[�i�[],
	  Source.[��{�w�b�_�[���@],
	  Source.[��{�w�b�_�[�V�[�P���XNo],
	  Source.[��{�w�b�_�[�o�[�W����],
	  Source.[�ʘH����],
	  Source.[���茋��],
	  Source.[��������Ԍ����w],
	  Source.[��������Ԍ����w],
	  Source.[���������}�����w],
	  Source.[���������}�����w],
	  Source.[�������̂��݋�Ԕ��w],
	  Source.[�������̂��݋�Ԓ��w],
	  Source.[�������O���[����Ԕ��w],
	  Source.[�������O���[����Ԓ��w],
	  Source.[�������FREX��Ԕ��w],
	  Source.[�������FREX��Ԓ��w],
	  Source.[����w����Ԍ�����w],
	  Source.[����w�����}������w],
	  Source.[�����������Ԍ���������],
	  Source.[������������}����������],
	  Source.[���w��������Ԍ���ԉw],
	  Source.[���w���������}����ԉw],
	  Source.[���w���猔����Ԍ����w],
	  Source.[�召�敪��l����],
	  Source.[���ʋ敪�j������],
	  Source.[�ʗp��ʂP���ڏ�񌔎�],
	  Source.[�ʗp��ʂP���ڏ��L���J�n��],
	  Source.[�ʗp��ʂP���ڏ��L���I����],
	  Source.[�ʗp��ʂQ���ڏ�񌔎�],
	  Source.[�ʗp��ʂQ���ڏ��L���J�n��],
	  Source.[�ʗp��ʂQ���ڏ��L���I����],
	  Source.[�ʗp��ʂR���ڏ�񌔎�],
	  Source.[�ʗp��ʂR���ڏ��L���J�n��],
	  Source.[�ʗp��ʂR���ڏ��L���I����],
	  Source.[�w�茔���w��P�w����],
	  Source.[�w�茔���w��P��Ԕԍ�],
	  Source.[�w�茔���w��P���Ԕԍ�],
	  Source.[�w�茔���w��P�Ȕ�],
	  Source.[�w�茔���w��Q�w����],
	  Source.[�w�茔���w��Q��Ԕԍ�],
	  Source.[�w�茔���w��Q���Ԕԍ�],
	  Source.[�w�茔���w��Q�Ȕ�],
	  Source.[�w�茔���w��R�w����],
	  Source.[�w�茔���w��R��Ԕԍ�],
	  Source.[�w�茔���w��R���Ԕԍ�],
	  Source.[�w�茔���w��R�Ȕ�],
	  Source.[�s������Ώۋ敪�r�b�g],
	  Source.[��������],
	  Source.[���p�p�^�[�����],
	  Source.[���ǎ���P���ڏ����],
	  Source.[���ǎ���P���ڏ��W�v����],
	  Source.[���ǎ���P���ڏ�񊄈�],
	  Source.[���ǎ���P���ڏ�񏤕i�ԍ�],
	  Source.[���ǎ���P���ڏ�񔭍s���],
	  Source.[���ǎ���P���ڏ��L���J�n��],
	  Source.[���ǎ���P���ڏ�񔭍s����],
	  Source.[���ǎ���Q���ڏ����],
	  Source.[���ǎ���Q���ڏ��W�v����],
	  Source.[���ǎ���Q���ڏ�񊄈�],
	  Source.[���ǎ���Q���ڏ�񏤕i�ԍ�],
	  Source.[���ǎ���Q���ڏ�񔭍s���],
	  Source.[���ǎ���Q���ڏ��L���J�n��],
	  Source.[���ǎ���Q���ڏ�񔭍s����],
	  Source.[���ǎ���R���ڏ����],
	  Source.[���ǎ���R���ڏ��W�v����],
	  Source.[���ǎ���R���ڏ�񊄈�],
	  Source.[���ǎ���R���ڏ�񏤕i�ԍ�],
	  Source.[���ǎ���R���ڏ�񔭍s���],
	  Source.[���ǎ���R���ڏ��L���J�n��],
	  Source.[���ǎ���R���ڏ�񔭍s����],
	  Source.[���ǎ���S���ڏ����],
	  Source.[���ǎ���S���ڏ��W�v����],
	  Source.[���ǎ���S���ڏ�񊄈�],
	  Source.[���ǎ���S���ڏ�񏤕i�ԍ�],
	  Source.[���ǎ���S���ڏ�񔭍s���],
	  Source.[���ǎ���S���ڏ��L���J�n��],
	  Source.[���ǎ���S���ڏ�񔭍s����],
	  Source.[���ǎ���P���ڏ�񓖉w�L��],
	  Source.[���ǎ���Q���ڏ�񓖉w�L��],
	  Source.[���ǎ���R���ڏ�񓖉w�L��],
	  Source.[���ǎ���S���ڏ�񓖉w�L��],
	  Source.[�r�e��������z],
	  Source.[�r�e���p��ԂP�E���p�w�P],
	  Source.[�r�e���p��ԂP�E���p�w�Q],
	  Source.[�r�e���p��ԂQ�E���p�w�P],
	  Source.[�r�e���p��ԂQ�E���p�w�Q],
	  Source.[��Ԏn�_�w],
	  Source.[���ʂ��}�X�^�K�p�L��],
	  Source.[�\���P],
	  Source.[�T���l],
	  Source.[���g�p],
	  Source.[���G���R�[�h���P���ڏ��],
	  Source.[���G���R�[�h���Q���ڏ��],
	  Source.[���G���R�[�h���R���ڏ��],
	  Source.[���G���R�[�h���S���ڏ��]);
END;
${GO}
