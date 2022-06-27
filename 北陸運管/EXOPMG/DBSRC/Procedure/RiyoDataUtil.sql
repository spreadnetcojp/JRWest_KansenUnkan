/***********************************************************************
   �V�X�e�����F�ԓ����D��� �� �Z�L�����e�B�T�[�o �V�X�e��

   Copyright Toshiba Solutions Corporation 2017 All rights reserved.

 ----------------------------------------------------------------------
   �ύX����:
   Ver      ���t        �S��       �R�����g
   0.0      2017/04/23  (NES)����  �V�K�쐬
 ***********************************************************************/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/***********************************************************************/

IF OBJECT_ID(N'[dbo].[ufnDiscountCodesForRiyoData]') IS NOT NULL
	DROP FUNCTION [dbo].[ufnDiscountCodesForRiyoData]
GO

CREATE FUNCTION [dbo].[ufnDiscountCodesForRiyoData]
(
	@���ǎ���P���w�L�� BINARY(1),
	@���ǎ���P�W�v���� BINARY(1),
	@���ǎ���P���� BINARY(1),
	@���ǎ���Q���w�L�� BINARY(1),
	@���ǎ���Q�W�v���� BINARY(1),
	@���ǎ���Q���� BINARY(1),
	@���ǎ���R���w�L�� BINARY(1),
	@���ǎ���R�W�v���� BINARY(1),
	@���ǎ���R���� BINARY(1),
	@���ǎ���S���w�L�� BINARY(1),
	@���ǎ���S�W�v���� BINARY(1),
	@���ǎ���S���� BINARY(1)
) RETURNS BINARY(2)
AS
BEGIN
	DECLARE @���� BINARY(1);
	DECLARE @���� BINARY(1);
	DECLARE @�����D��x TINYINT;
	DECLARE @�����D��x TINYINT;
	DECLARE @����敪 TINYINT;
	DECLARE @�D��x TINYINT;

	SET @���� = NULL;
	SET @���� = NULL;

	IF @���ǎ���P�W�v���� <> 0x00
	BEGIN
		SELECT @����敪 = ����敪 FROM dbo.M_RIYO_TICKET_KIND WHERE ���� = @���ǎ���P�W�v����;
		IF @����敪 = 1 OR (@����敪 IN (3, 4) AND @���ǎ���P���w�L�� = 0x01)
		BEGIN
			SELECT @�D��x = �D��x FROM dbo.M_RIYO_DISCOUNT_CODE WHERE �����R�[�h = @���ǎ���P����;
			IF @�D��x IS NOT NULL
			BEGIN
				SET @���� = @���ǎ���P����;
				SET @�����D��x = @�D��x;
			END;
		END
		ELSE IF @����敪 = 2 AND @���ǎ���P���w�L�� = 0x01
		BEGIN
			SELECT @�D��x = �D��x FROM dbo.M_RIYO_DISCOUNT_CODE WHERE �����R�[�h = @���ǎ���P����;
			IF @�D��x IS NOT NULL
			BEGIN
				SET @���� = @���ǎ���P����;
				SET @�����D��x = @�D��x;
			END;
		END;
	END;

	IF @���ǎ���Q�W�v���� <> 0x00
	BEGIN
		SELECT @����敪 = ����敪 FROM dbo.M_RIYO_TICKET_KIND WHERE ���� = @���ǎ���Q�W�v����;
		IF @����敪 = 1 OR (@����敪 IN (3, 4) AND @���ǎ���Q���w�L�� = 0x01)
		BEGIN
			SELECT @�D��x = �D��x FROM dbo.M_RIYO_DISCOUNT_CODE WHERE �����R�[�h = @���ǎ���Q����;
			IF @�D��x IS NOT NULL
			BEGIN
				IF @���� IS NULL
				BEGIN
					SET @���� = @���ǎ���Q����;
					SET @�����D��x = @�D��x;
				END
				ELSE IF (@�D��x < @�����D��x) OR (@�D��x = @�����D��x AND @���ǎ���Q���� < @����)
				BEGIN
					SET @���� = @���ǎ���Q����;
					SET @�����D��x = @�D��x;
				END;
			END;
		END
		ELSE IF @����敪 = 2 AND @���ǎ���Q���w�L�� = 0x01
		BEGIN
			SELECT @�D��x = �D��x FROM dbo.M_RIYO_DISCOUNT_CODE WHERE �����R�[�h = @���ǎ���Q����;
			IF @�D��x IS NOT NULL
			BEGIN
				IF @���� IS NULL
				BEGIN
					SET @���� = @���ǎ���Q����;
					SET @�����D��x = @�D��x;
				END
				ELSE IF (@�D��x < @�����D��x) OR (@�D��x = @�����D��x AND @���ǎ���Q���� < @����)
				BEGIN
					SET @���� = @���ǎ���Q����;
					SET @�����D��x = @�D��x;
				END;
			END;
		END;
	END;

	IF @���ǎ���R�W�v���� <> 0x00
	BEGIN
		SELECT @����敪 = ����敪 FROM dbo.M_RIYO_TICKET_KIND WHERE ���� = @���ǎ���R�W�v����;
		IF @����敪 = 1 OR (@����敪 IN (3, 4) AND @���ǎ���R���w�L�� = 0x01)
		BEGIN
			SELECT @�D��x = �D��x FROM dbo.M_RIYO_DISCOUNT_CODE WHERE �����R�[�h = @���ǎ���R����;
			IF @�D��x IS NOT NULL
			BEGIN
				IF @���� IS NULL
				BEGIN
					SET @���� = @���ǎ���R����;
					SET @�����D��x = @�D��x;
				END
				ELSE IF (@�D��x < @�����D��x) OR (@�D��x = @�����D��x AND @���ǎ���R���� < @����)
				BEGIN
					SET @���� = @���ǎ���R����;
					SET @�����D��x = @�D��x;
				END;
			END;
		END
		ELSE IF @����敪 = 2 AND @���ǎ���R���w�L�� = 0x01
		BEGIN
			SELECT @�D��x = �D��x FROM dbo.M_RIYO_DISCOUNT_CODE WHERE �����R�[�h = @���ǎ���R����;
			IF @�D��x IS NOT NULL
			BEGIN
				IF @���� IS NULL
				BEGIN
					SET @���� = @���ǎ���R����;
					SET @�����D��x = @�D��x;
				END
				ELSE IF (@�D��x < @�����D��x) OR (@�D��x = @�����D��x AND @���ǎ���R���� < @����)
				BEGIN
					SET @���� = @���ǎ���R����;
					SET @�����D��x = @�D��x;
				END;
			END;
		END;
	END;

	IF @���ǎ���S�W�v���� <> 0x00
	BEGIN
		SELECT @����敪 = ����敪 FROM dbo.M_RIYO_TICKET_KIND WHERE ���� = @���ǎ���S�W�v����;
		IF @����敪 = 1 OR (@����敪 IN (3, 4) AND @���ǎ���S���w�L�� = 0x01)
		BEGIN
			SELECT @�D��x = �D��x FROM dbo.M_RIYO_DISCOUNT_CODE WHERE �����R�[�h = @���ǎ���S����;
			IF @�D��x IS NOT NULL
			BEGIN
				IF @���� IS NULL
				BEGIN
					SET @���� = @���ǎ���S����;
					SET @�����D��x = @�D��x;
				END
				ELSE IF (@�D��x < @�����D��x) OR (@�D��x = @�����D��x AND @���ǎ���S���� < @����)
				BEGIN
					SET @���� = @���ǎ���S����;
					SET @�����D��x = @�D��x;
				END;
			END;
		END
		ELSE IF @����敪 = 2 AND @���ǎ���S���w�L�� = 0x01
		BEGIN
			SELECT @�D��x = �D��x FROM dbo.M_RIYO_DISCOUNT_CODE WHERE �����R�[�h = @���ǎ���S����;
			IF @�D��x IS NOT NULL
			BEGIN
				IF @���� IS NULL
				BEGIN
					SET @���� = @���ǎ���S����;
					SET @�����D��x = @�D��x;
				END
				ELSE IF (@�D��x < @�����D��x) OR (@�D��x = @�����D��x AND @���ǎ���S���� < @����)
				BEGIN
					SET @���� = @���ǎ���S����;
					SET @�����D��x = @�D��x;
				END;
			END;
		END;
	END;

	IF @���� IS NULL
		SET @���� = 0x00;

	IF @���� IS NULL
		SET @���� = @����;

	RETURN @���� + @����
END
GO
