/***********************************************************************
   システム名：車内改札情報 兼 セキュリティサーバ システム

   Copyright Toshiba Solutions Corporation 2017 All rights reserved.

 ----------------------------------------------------------------------
   変更履歴:
   Ver      日付        担当       コメント
   0.0      2017/04/23  (NES)小林  新規作成
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
	@券読取情報１当駅有効 BINARY(1),
	@券読取情報１集計券種 BINARY(1),
	@券読取情報１割引 BINARY(1),
	@券読取情報２当駅有効 BINARY(1),
	@券読取情報２集計券種 BINARY(1),
	@券読取情報２割引 BINARY(1),
	@券読取情報３当駅有効 BINARY(1),
	@券読取情報３集計券種 BINARY(1),
	@券読取情報３割引 BINARY(1),
	@券読取情報４当駅有効 BINARY(1),
	@券読取情報４集計券種 BINARY(1),
	@券読取情報４割引 BINARY(1)
) RETURNS BINARY(2)
AS
BEGIN
	DECLARE @割引 BINARY(1);
	DECLARE @特割 BINARY(1);
	DECLARE @割引優先度 TINYINT;
	DECLARE @特割優先度 TINYINT;
	DECLARE @券種区分 TINYINT;
	DECLARE @優先度 TINYINT;

	SET @割引 = NULL;
	SET @特割 = NULL;

	IF @券読取情報１集計券種 <> 0x00
	BEGIN
		SELECT @券種区分 = 券種区分 FROM dbo.M_RIYO_TICKET_KIND WHERE 券種 = @券読取情報１集計券種;
		IF @券種区分 = 1 OR (@券種区分 IN (3, 4) AND @券読取情報１当駅有効 = 0x01)
		BEGIN
			SELECT @優先度 = 優先度 FROM dbo.M_RIYO_DISCOUNT_CODE WHERE 割引コード = @券読取情報１割引;
			IF @優先度 IS NOT NULL
			BEGIN
				SET @割引 = @券読取情報１割引;
				SET @割引優先度 = @優先度;
			END;
		END
		ELSE IF @券種区分 = 2 AND @券読取情報１当駅有効 = 0x01
		BEGIN
			SELECT @優先度 = 優先度 FROM dbo.M_RIYO_DISCOUNT_CODE WHERE 割引コード = @券読取情報１割引;
			IF @優先度 IS NOT NULL
			BEGIN
				SET @特割 = @券読取情報１割引;
				SET @特割優先度 = @優先度;
			END;
		END;
	END;

	IF @券読取情報２集計券種 <> 0x00
	BEGIN
		SELECT @券種区分 = 券種区分 FROM dbo.M_RIYO_TICKET_KIND WHERE 券種 = @券読取情報２集計券種;
		IF @券種区分 = 1 OR (@券種区分 IN (3, 4) AND @券読取情報２当駅有効 = 0x01)
		BEGIN
			SELECT @優先度 = 優先度 FROM dbo.M_RIYO_DISCOUNT_CODE WHERE 割引コード = @券読取情報２割引;
			IF @優先度 IS NOT NULL
			BEGIN
				IF @割引 IS NULL
				BEGIN
					SET @割引 = @券読取情報２割引;
					SET @割引優先度 = @優先度;
				END
				ELSE IF (@優先度 < @割引優先度) OR (@優先度 = @割引優先度 AND @券読取情報２割引 < @割引)
				BEGIN
					SET @割引 = @券読取情報２割引;
					SET @割引優先度 = @優先度;
				END;
			END;
		END
		ELSE IF @券種区分 = 2 AND @券読取情報２当駅有効 = 0x01
		BEGIN
			SELECT @優先度 = 優先度 FROM dbo.M_RIYO_DISCOUNT_CODE WHERE 割引コード = @券読取情報２割引;
			IF @優先度 IS NOT NULL
			BEGIN
				IF @特割 IS NULL
				BEGIN
					SET @特割 = @券読取情報２割引;
					SET @特割優先度 = @優先度;
				END
				ELSE IF (@優先度 < @特割優先度) OR (@優先度 = @特割優先度 AND @券読取情報２割引 < @特割)
				BEGIN
					SET @特割 = @券読取情報２割引;
					SET @特割優先度 = @優先度;
				END;
			END;
		END;
	END;

	IF @券読取情報３集計券種 <> 0x00
	BEGIN
		SELECT @券種区分 = 券種区分 FROM dbo.M_RIYO_TICKET_KIND WHERE 券種 = @券読取情報３集計券種;
		IF @券種区分 = 1 OR (@券種区分 IN (3, 4) AND @券読取情報３当駅有効 = 0x01)
		BEGIN
			SELECT @優先度 = 優先度 FROM dbo.M_RIYO_DISCOUNT_CODE WHERE 割引コード = @券読取情報３割引;
			IF @優先度 IS NOT NULL
			BEGIN
				IF @割引 IS NULL
				BEGIN
					SET @割引 = @券読取情報３割引;
					SET @割引優先度 = @優先度;
				END
				ELSE IF (@優先度 < @割引優先度) OR (@優先度 = @割引優先度 AND @券読取情報３割引 < @割引)
				BEGIN
					SET @割引 = @券読取情報３割引;
					SET @割引優先度 = @優先度;
				END;
			END;
		END
		ELSE IF @券種区分 = 2 AND @券読取情報３当駅有効 = 0x01
		BEGIN
			SELECT @優先度 = 優先度 FROM dbo.M_RIYO_DISCOUNT_CODE WHERE 割引コード = @券読取情報３割引;
			IF @優先度 IS NOT NULL
			BEGIN
				IF @特割 IS NULL
				BEGIN
					SET @特割 = @券読取情報３割引;
					SET @特割優先度 = @優先度;
				END
				ELSE IF (@優先度 < @特割優先度) OR (@優先度 = @特割優先度 AND @券読取情報３割引 < @特割)
				BEGIN
					SET @特割 = @券読取情報３割引;
					SET @特割優先度 = @優先度;
				END;
			END;
		END;
	END;

	IF @券読取情報４集計券種 <> 0x00
	BEGIN
		SELECT @券種区分 = 券種区分 FROM dbo.M_RIYO_TICKET_KIND WHERE 券種 = @券読取情報４集計券種;
		IF @券種区分 = 1 OR (@券種区分 IN (3, 4) AND @券読取情報４当駅有効 = 0x01)
		BEGIN
			SELECT @優先度 = 優先度 FROM dbo.M_RIYO_DISCOUNT_CODE WHERE 割引コード = @券読取情報４割引;
			IF @優先度 IS NOT NULL
			BEGIN
				IF @割引 IS NULL
				BEGIN
					SET @割引 = @券読取情報４割引;
					SET @割引優先度 = @優先度;
				END
				ELSE IF (@優先度 < @割引優先度) OR (@優先度 = @割引優先度 AND @券読取情報４割引 < @割引)
				BEGIN
					SET @割引 = @券読取情報４割引;
					SET @割引優先度 = @優先度;
				END;
			END;
		END
		ELSE IF @券種区分 = 2 AND @券読取情報４当駅有効 = 0x01
		BEGIN
			SELECT @優先度 = 優先度 FROM dbo.M_RIYO_DISCOUNT_CODE WHERE 割引コード = @券読取情報４割引;
			IF @優先度 IS NOT NULL
			BEGIN
				IF @特割 IS NULL
				BEGIN
					SET @特割 = @券読取情報４割引;
					SET @特割優先度 = @優先度;
				END
				ELSE IF (@優先度 < @特割優先度) OR (@優先度 = @特割優先度 AND @券読取情報４割引 < @特割)
				BEGIN
					SET @特割 = @券読取情報４割引;
					SET @特割優先度 = @優先度;
				END;
			END;
		END;
	END;

	IF @割引 IS NULL
		SET @割引 = 0x00;

	IF @特割 IS NULL
		SET @特割 = @割引;

	RETURN @割引 + @特割
END
GO
