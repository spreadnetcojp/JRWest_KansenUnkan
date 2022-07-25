USE [EXOPMG]
GO
/****** Object:  View [dbo].[V_KSB_CONFIG4]    Script Date: 03/18/2019 15:37:44 ******/
DROP VIEW [dbo].[V_KSB_CONFIG4]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_KSB_CONFIG4]
AS
SELECT                  T.RAIL_SECTION_CODE + T.STATION_ORDER_CODE AS STATION_CODE, V2.STATION_NAME, T.CORNER_CODE, 
                                  V2.CORNER_NAME, T.UNIT_NO, T.SYUSYU_DATE, (CASE T .DENGEN WHEN '01' THEN '入' WHEN '02' THEN '切' WHEN '50' THEN '未' END) 
                                  AS DENGEN, (CASE T .TURO_SET WHEN '1' THEN '改札' WHEN '2' THEN '集札' WHEN '3' THEN '両用' WHEN '5' THEN '未' END) AS TURO_SET, 
                                  (CASE T .TURO WHEN '0' THEN '未' WHEN '1' THEN '改札' WHEN '2' THEN '集札' WHEN '3' THEN '両用' WHEN '4' THEN '中止' END) AS TURO, 
                                  (CASE T .KAIMODE WHEN '00' THEN 'ＩＣ併用' WHEN '01' THEN '磁気専用' WHEN '02' THEN 'ＩＣ専用' WHEN '50' THEN '未接続' END) 
                                  AS KAIMODE, (CASE T .YUSOSYOUGAI_NOBORI WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS YUSOSYOUGAI_NOBORI, 
                                  (CASE T .YUSOSYOUGAI_KUDARI WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS YUSOSYOUGAI_KUDARI, 
                                  (CASE WHEN T .ZAIRAITOKYUHANTEI_NOBORI IS NULL 
                                  THEN '　' ELSE (CASE ZAIRAITOKYUHANTEI_NOBORI WHEN '00' THEN '切' WHEN '01' THEN '入' END) END) AS ZAIRAITOKYUHANTEI_NOBORI, 
                                  (CASE WHEN T .ZAIRAITOKYUHANTEI_KUDARI IS NULL 
                                  THEN '　' ELSE (CASE ZAIRAITOKYUHANTEI_KUDARI WHEN '00' THEN '切' WHEN '01' THEN '入' END) END) AS ZAIRAITOKYUHANTEI_KUDARI, 
                                  (CASE T .CHIBARAIPRT_NOBORI WHEN '00' THEN '切' WHEN '01' THEN '入' WHEN '50' THEN '未' END) AS CHIBARAIPRT_NOBORI, 
                                  (CASE T .CHIBARAIPRT_KUDARI WHEN '00' THEN '切' WHEN '01' THEN '入' WHEN '50' THEN '未' END) AS CHIBARAIPRT_KUDARI, 
                                  (CASE T .ORIKAESHI_NOBORI WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS ORIKAESHI_NOBORI, 
                                  (CASE T .ORIKAESHI_KUDARI WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS ORIKAESHI_KUDARI, 
                                  (CASE T .SYURESYA WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS SYURESYA, 
                                  (CASE T .ZENJITUKEN WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS ZENJITUKEN, 
                                  (CASE T .DANTAITSUKA_MODE WHEN '00' THEN '切' WHEN '01' THEN '入' WHEN '50' THEN '未' END) AS DANTAITSUKA_MODE, 
                                  (CASE WHEN T .SELFNYUJYO_KYUSAI IS NULL THEN '　' ELSE (CASE SELFNYUJYO_KYUSAI WHEN '00' THEN '切' WHEN '01' THEN '入' END) END) 
                                  AS SELFNYUJYO_KYUSAI, (CASE WHEN T .JIKIKAISYU_TYUSI IS NULL 
                                  THEN '　' ELSE (CASE JIKIKAISYU_TYUSI WHEN '00' THEN '切' WHEN '01' THEN '入' END) END) AS JIKIKAISYU_TYUSI, 
                                  (CASE WHEN T .JIKIRYOKYAKU_FREE IS NULL 
                                  THEN '　' ELSE (CASE JIKIRYOKYAKU_FREE WHEN '00' THEN '切' WHEN '01' THEN '入' WHEN '50' THEN '未' END) END) AS JIKIRYOKYAKU_FREE, 
                                  (CASE WHEN T .NYUJYOTYUSI IS NULL THEN '　' ELSE (CASE NYUJYOTYUSI WHEN '00' THEN '切' WHEN '01' THEN '入' END) END) 
                                  AS NYUJYOTYUSI,
                                  (CASE T .SYSTEMSYOGAI_IN WHEN '00' THEN '切り' WHEN '01' THEN 'EXIC' WHEN '02' THEN '全IC' ELSE '切り' END) AS SYSTEMSYOGAI_IN, 
                                  (CASE T .SYSTEMSYOGAI_OUT WHEN '00' THEN '切り' WHEN '01' THEN 'EXIC' WHEN '02' THEN '全IC' ELSE '切り' END) AS SYSTEMSYOGAI_OUT, 
                                  (CASE T .ICSYUKUTAIKAIJYO WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS ICSYUKUTAIKAIJYO, 
                                  (CASE T .ZAIRAI_ICSYUKUTAIKAIJYO WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS ZAIRAI_ICSYUKUTAIKAIJYO, 
                                  (CASE T .EX_ICSYUKUTAIKAIJYO WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS EX_ICSYUKUTAIKAIJYO, 
                                  (CASE T .ONSEI_ONSEIANNAI WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS ONSEI_ONSEIANNAI, 
                                  (CASE T .ONSEI_ONRYOKIRIKAE WHEN '00' THEN '小' WHEN '01' THEN '大' END) AS ONSEI_ONRYOKIRIKAE, 
                                  (CASE T .KAISATSU_AUTOSET_ON WHEN '00' THEN '無効' WHEN '01' THEN '有効' END) AS KAISATSU_AUTOSET_ON, 
                                  T.KAISATSU_AUTOSET_ON_HH, T.KAISATSU_AUTOSET_ON_MI, (CASE T .KAISATSU_AUTOSET_OFF WHEN '00' THEN '無効' WHEN '01' THEN '有効' END)
                                   AS KAISATSU_AUTOSET_OFF, T.KAISATSU_AUTOSET_OFF_HH, T.KAISATSU_AUTOSET_OFF_MI, 
                                  (CASE T .KAISATSU_AUTOSET_END WHEN '00' THEN '開' WHEN '01' THEN '閉' END) AS KAISATSU_AUTOSET_END, 
                                  (CASE T .KAISATSU_AUTOSET_START WHEN '00' THEN '開' WHEN '01' THEN '閉' END) AS KAISATSU_AUTOSET_START, 
                                  (CASE T .FREE_YUUKO1 WHEN '00' THEN '切' WHEN '01' THEN '定期' WHEN '02' THEN '全券' END) AS FREE_YUUKO1, T.FREE_BEGIN_YEAR1, 
                                  T.FREE_BEGIN_NONTH1, T.FREE_BEGIN_DAY1, T.FREE_BEGIN_HOUR1, T.FREE_END_YEAR1, T.FREE_END_NONTH1, T.FREE_END_DAY1, 
                                  T.FREE_END_HOUR1, (CASE T .FREE_YUUKO2 WHEN '00' THEN '切' WHEN '01' THEN '定期' WHEN '02' THEN '全券' END) AS FREE_YUUKO2, 
                                  T.FREE_BEGIN_YEAR2, T.FREE_BEGIN_NONTH2, T.FREE_BEGIN_DAY2, T.FREE_BEGIN_HOUR2, T.FREE_END_YEAR2, T.FREE_END_NONTH2, 
                                  T.FREE_END_DAY2, T.FREE_END_HOUR2, (CASE T .FREE_YUUKO3 WHEN '00' THEN '切' WHEN '01' THEN '定期' WHEN '02' THEN '全券' END) 
                                  AS FREE_YUUKO3, T.FREE_BEGIN_YEAR3, T.FREE_BEGIN_NONTH3, T.FREE_BEGIN_DAY3, T.FREE_BEGIN_HOUR3, T.FREE_END_YEAR3, 
                                  T.FREE_END_NONTH3, T.FREE_END_DAY3, T.FREE_END_HOUR3, 
                                  (CASE T .FREE_YUUKO4 WHEN '00' THEN '切' WHEN '01' THEN '定期' WHEN '02' THEN '全券' END) AS FREE_YUUKO4, T.FREE_BEGIN_YEAR4, 
                                  T.FREE_BEGIN_NONTH4, T.FREE_BEGIN_DAY4, T.FREE_BEGIN_HOUR4, T.FREE_END_YEAR4, T.FREE_END_NONTH4, T.FREE_END_DAY4, 
                                  T.FREE_END_HOUR4, (CASE T .FREE_YUUKO5 WHEN '00' THEN '切' WHEN '01' THEN '定期' WHEN '02' THEN '全券' END) AS FREE_YUUKO5, 
                                  T.FREE_BEGIN_YEAR5, T.FREE_BEGIN_NONTH5, T.FREE_BEGIN_DAY5, T.FREE_BEGIN_HOUR5, T.FREE_END_YEAR5, T.FREE_END_NONTH5, 
                                  T.FREE_END_DAY5, T.FREE_END_HOUR5, (CASE T .FREE_YUUKO6 WHEN '00' THEN '切' WHEN '01' THEN '定期' WHEN '02' THEN '全券' END) 
                                  AS FREE_YUUKO6, T.FREE_BEGIN_YEAR6, T.FREE_BEGIN_NONTH6, T.FREE_BEGIN_DAY6, T.FREE_BEGIN_HOUR6, T.FREE_END_YEAR6, 
                                  T.FREE_END_NONTH6, T.FREE_END_DAY6, T.FREE_END_HOUR6, 
                                  (CASE T .FREE_YUUKO7 WHEN '00' THEN '切' WHEN '01' THEN '定期' WHEN '02' THEN '全券' END) AS FREE_YUUKO7, T.FREE_BEGIN_YEAR7, 
                                  T.FREE_BEGIN_NONTH7, T.FREE_BEGIN_DAY7, T.FREE_BEGIN_HOUR7, T.FREE_END_YEAR7, T.FREE_END_NONTH7, T.FREE_END_DAY7, 
                                  T.FREE_END_HOUR7, (CASE T .FREE_YUUKO8 WHEN '00' THEN '切' WHEN '01' THEN '定期' WHEN '02' THEN '全券' END) AS FREE_YUUKO8, 
                                  T.FREE_BEGIN_YEAR8, T.FREE_BEGIN_NONTH8, T.FREE_BEGIN_DAY8, T.FREE_BEGIN_HOUR8, T.FREE_END_YEAR8, T.FREE_END_NONTH8, 
                                  T.FREE_END_DAY8, T.FREE_END_HOUR8, (CASE T .FREE_YUUKO9 WHEN '00' THEN '切' WHEN '01' THEN '定期' WHEN '02' THEN '全券' END) 
                                  AS FREE_YUUKO9, T.FREE_BEGIN_YEAR9, T.FREE_BEGIN_NONTH9, T.FREE_BEGIN_DAY9, T.FREE_BEGIN_HOUR9, T.FREE_END_YEAR9, 
                                  T.FREE_END_NONTH9, T.FREE_END_DAY9, T.FREE_END_HOUR9, 
                                  (CASE T .SHUTSUJYO_FREE_YUUKO1 WHEN '00' THEN '切' WHEN '01' THEN '定期' WHEN '02' THEN '全券' END) AS SHUTSUJYO_FREE_YUUKO1, 
                                  T.SHUTSUJYO_FREE_BEGIN_YEAR1, T.SHUTSUJYO_FREE_BEGIN_NONTH1, T.SHUTSUJYO_FREE_BEGIN_DAY1, T.SHUTSUJYO_FREE_BEGIN_HOUR1, 
                                  T.SHUTSUJYO_FREE_END_YEAR1, T.SHUTSUJYO_FREE_END_NONTH1, T.SHUTSUJYO_FREE_END_DAY1, T.SHUTSUJYO_FREE_END_HOUR1, 
                                  (CASE T .SHUTSUJYO_FREE_YUUKO2 WHEN '00' THEN '切' WHEN '01' THEN '定期' WHEN '02' THEN '全券' END) AS SHUTSUJYO_FREE_YUUKO2, 
                                  T.SHUTSUJYO_FREE_BEGIN_YEAR2, T.SHUTSUJYO_FREE_BEGIN_NONTH2, T.SHUTSUJYO_FREE_BEGIN_DAY2, T.SHUTSUJYO_FREE_BEGIN_HOUR2, 
                                  T.SHUTSUJYO_FREE_END_YEAR2, T.SHUTSUJYO_FREE_END_NONTH2, T.SHUTSUJYO_FREE_END_DAY2, T.SHUTSUJYO_FREE_END_HOUR2, 
                                  (CASE T .SHUTSUJYO_FREE_YUUKO3 WHEN '00' THEN '切' WHEN '01' THEN '定期' WHEN '02' THEN '全券' END) AS SHUTSUJYO_FREE_YUUKO3, 
                                  T.SHUTSUJYO_FREE_BEGIN_YEAR3, T.SHUTSUJYO_FREE_BEGIN_NONTH3, T.SHUTSUJYO_FREE_BEGIN_DAY3, T.SHUTSUJYO_FREE_BEGIN_HOUR3, 
                                  T.SHUTSUJYO_FREE_END_YEAR3, T.SHUTSUJYO_FREE_END_NONTH3, T.SHUTSUJYO_FREE_END_DAY3, T.SHUTSUJYO_FREE_END_HOUR3, 
                                  (CASE T .SHUTSUJYO_FREE_YUUKO4 WHEN '00' THEN '切' WHEN '01' THEN '定期' WHEN '02' THEN '全券' END) AS SHUTSUJYO_FREE_YUUKO4, 
                                  T.SHUTSUJYO_FREE_BEGIN_YEAR4, T.SHUTSUJYO_FREE_BEGIN_NONTH4, T.SHUTSUJYO_FREE_BEGIN_DAY4, T.SHUTSUJYO_FREE_BEGIN_HOUR4, 
                                  T.SHUTSUJYO_FREE_END_YEAR4, T.SHUTSUJYO_FREE_END_NONTH4, T.SHUTSUJYO_FREE_END_DAY4, T.SHUTSUJYO_FREE_END_HOUR4, 
                                  (CASE T .SHUTSUJYO_FREE_YUUKO5 WHEN '00' THEN '切' WHEN '01' THEN '定期' WHEN '02' THEN '全券' END) AS SHUTSUJYO_FREE_YUUKO5, 
                                  T.SHUTSUJYO_FREE_BEGIN_YEAR5, T.SHUTSUJYO_FREE_BEGIN_NONTH5, T.SHUTSUJYO_FREE_BEGIN_DAY5, T.SHUTSUJYO_FREE_BEGIN_HOUR5, 
                                  T.SHUTSUJYO_FREE_END_YEAR5, T.SHUTSUJYO_FREE_END_NONTH5, T.SHUTSUJYO_FREE_END_DAY5, T.SHUTSUJYO_FREE_END_HOUR5, 
                                  (CASE T .SHUTSUJYO_FREE_YUUKO6 WHEN '00' THEN '切' WHEN '01' THEN '定期' WHEN '02' THEN '全券' END) AS SHUTSUJYO_FREE_YUUKO6, 
                                  T.SHUTSUJYO_FREE_BEGIN_YEAR6, T.SHUTSUJYO_FREE_BEGIN_NONTH6, T.SHUTSUJYO_FREE_BEGIN_DAY6, T.SHUTSUJYO_FREE_BEGIN_HOUR6, 
                                  T.SHUTSUJYO_FREE_END_YEAR6, T.SHUTSUJYO_FREE_END_NONTH6, T.SHUTSUJYO_FREE_END_DAY6, T.SHUTSUJYO_FREE_END_HOUR6, 
                                  (CASE T .SHUTSUJYO_FREE_YUUKO7 WHEN '00' THEN '切' WHEN '01' THEN '定期' WHEN '02' THEN '全券' END) AS SHUTSUJYO_FREE_YUUKO7, 
                                  T.SHUTSUJYO_FREE_BEGIN_YEAR7, T.SHUTSUJYO_FREE_BEGIN_NONTH7, T.SHUTSUJYO_FREE_BEGIN_DAY7, T.SHUTSUJYO_FREE_BEGIN_HOUR7, 
                                  T.SHUTSUJYO_FREE_END_YEAR7, T.SHUTSUJYO_FREE_END_NONTH7, T.SHUTSUJYO_FREE_END_DAY7, T.SHUTSUJYO_FREE_END_HOUR7, 
                                  (CASE T .SHUTSUJYO_FREE_YUUKO8 WHEN '00' THEN '切' WHEN '01' THEN '定期' WHEN '02' THEN '全券' END) AS SHUTSUJYO_FREE_YUUKO8, 
                                  T.SHUTSUJYO_FREE_BEGIN_YEAR8, T.SHUTSUJYO_FREE_BEGIN_NONTH8, T.SHUTSUJYO_FREE_BEGIN_DAY8, T.SHUTSUJYO_FREE_BEGIN_HOUR8, 
                                  T.SHUTSUJYO_FREE_END_YEAR8, T.SHUTSUJYO_FREE_END_NONTH8, T.SHUTSUJYO_FREE_END_DAY8, T.SHUTSUJYO_FREE_END_HOUR8, 
                                  (CASE T .SHUTSUJYO_FREE_YUUKO9 WHEN '00' THEN '切' WHEN '01' THEN '定期' WHEN '02' THEN '全券' END) AS SHUTSUJYO_FREE_YUUKO9, 
                                  T.SHUTSUJYO_FREE_BEGIN_YEAR9, T.SHUTSUJYO_FREE_BEGIN_NONTH9, T.SHUTSUJYO_FREE_BEGIN_DAY9, T.SHUTSUJYO_FREE_BEGIN_HOUR9, 
                                  T.SHUTSUJYO_FREE_END_YEAR9, T.SHUTSUJYO_FREE_END_NONTH9, T.SHUTSUJYO_FREE_END_DAY9, T.SHUTSUJYO_FREE_END_HOUR9, 
                                  (CASE T .NINGENKENCHI WHEN '00' THEN '切' WHEN '01' THEN '入' WHEN '50' THEN '未' END) AS NINGENKENCHI, 
                                  (CASE T .HORYU_SYORIERR WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS HORYU_SYORIERR, 
                                  (CASE T .HORYU_FUSEI WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS HORYU_FUSEI, 
                                  (CASE T .CYOKUPRT WHEN '00' THEN '切' WHEN '01' THEN '改：切　集：入' WHEN '02' THEN '改：入　集：切' WHEN '03' THEN '入' WHEN '50' THEN
                                   '未' END) AS CYOKUPRT, 
                                  (CASE T .TENSYAPRT WHEN '00' THEN '切' WHEN '01' THEN '改：切　集：入' WHEN '02' THEN '改：入　集：切' WHEN '03' THEN '入' WHEN '50' THEN
                                   '未' END) AS TENSYAPRT, (CASE T .KYOSEI_DOKYU WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS KYOSEI_DOKYU, 
                                  (CASE WHEN T .KANIC_HNT_EKI_HANTEI IS NULL THEN '　' ELSE (CASE KANIC_HNT_EKI_HANTEI WHEN '00' THEN '切' WHEN '01' THEN '入' END) 
                                  END) AS KANIC_HNT_EKI_HANTEI, (CASE WHEN T .KANIC_HNT_TIME_HANTEI IS NULL 
                                  THEN '　' ELSE (CASE KANIC_HNT_TIME_HANTEI WHEN '00' THEN '切' WHEN '01' THEN '入' END) END) AS KANIC_HNT_TIME_HANTEI, 
                                  (CASE WHEN T .IC_ONE_PASS IS NULL THEN '　' ELSE (CASE IC_ONE_PASS WHEN '00' THEN '切' WHEN '01' THEN '入' END) END) 
                                  AS IC_ONE_PASS, (CASE WHEN T .SELF_GESYA_YUKOUHANTEI IS NULL 
                                  THEN '　' ELSE (CASE SELF_GESYA_YUKOUHANTEI WHEN '00' THEN '切' WHEN '01' THEN '入' END) END) AS SELF_GESYA_YUKOUHANTEI, 
                                  (CASE WHEN T .FUSEIGOUKI IS NULL THEN '　' ELSE (CASE FUSEIGOUKI WHEN '00' THEN '切' WHEN '01' THEN '入' WHEN '50' THEN '未' END) 
                                  END) AS FUSEIGOUKI, (CASE T .HNT_CYCLE_KANJIKI WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS HNT_CYCLE_KANJIKI, 
                                  (CASE T .HNT_CYCLE_KANIC WHEN '00' THEN '入' WHEN '01' THEN '切１' WHEN '02' THEN '切２' END) AS HNT_CYCLE_KANIC, 
                                  (CASE T .HNT_FUSEI_KAN WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS HNT_FUSEI_KAN, 
                                  (CASE T .SENYOUKEN_KAN WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS SENYOUKEN_KAN, 
                                  (CASE T .QR_CHOKAHANTEI_KAN WHEN '00' THEN '切' WHEN '01' THEN '入' ELSE '　' END) AS QR_CHOKAHANTEI_KAN, 
                                  (CASE T .HNT_NORI_CYCLE_ZAIJIKI WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS HNT_NORI_CYCLE_ZAIJIKI, 
                                  (CASE T .HNT_NORI_CYCLE_ZAIIC WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS HNT_NORI_CYCLE_ZAIIC, 
                                  (CASE T .HNT_NORI_FUSEI_ZAI WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS HNT_NORI_FUSEI_ZAI, 
                                  (CASE T .SETSUZOKU_ZAI WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS SETSUZOKU_ZAI, 
                                  (CASE T .NYUJYO_FREESET_MEI WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS NYUJYO_FREESET_MEI, 
                                  (CASE T .SHUTSUJYO_FREESET_MEI WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS SHUTSUJYO_FREESET_MEI, 
                                  (CASE T .SELF_SHUTSUJYO_FREESET_MEI WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS SELF_SHUTSUJYO_FREESET_MEI, 
                                  (CASE T .JYOTOKU_NYUJYO WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS JYOTOKU_NYUJYO, 
                                  (CASE T .JYOTOKU_SHUTSUJYO WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS JYOTOKU_SHUTSUJYO, 
                                  (CASE T .HNT_NORI_HEIYOU_KANZAI WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS HNT_NORI_HEIYOU_KANZAI, 
                                  (CASE T .HNT_NORI_HEIYOU_ZAIKAN WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS HNT_NORI_HEIYOU_ZAIKAN, 
                                  (CASE T .HNT_NORI_KARAKEN_KANIC WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS HNT_NORI_KARAKEN_KANIC, 
                                  (CASE T .HNT_NORI_TUKA WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS HNT_NORI_TUKA, 
                                  (CASE T .HNT_NORI_ZAIKANNORITUGI WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS HNT_NORI_ZAIKANNORITUGI, 
                                  (CASE T .HNT_NORI_KARAKEN_KANJIKI WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS HNT_NORI_KARAKEN_KANJIKI, 
                                  (CASE T .TORIWASHURE_MODE WHEN '00' THEN '切' WHEN '01' THEN '中' WHEN '02' THEN '完' WHEN '50' THEN '未' END) 
                                  AS TORIWASHURE_MODE, (CASE T .SHODENRYOKU_MODE WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS SHODENRYOKU_MODE, 
                                  (CASE T .TOUKI_MODE WHEN '00' THEN '切' WHEN '01' THEN '入' END) AS TOUKI_MODE, (CASE WHEN T .JISA_NYUJYO_FREE IS NULL 
                                  THEN '　' ELSE (CASE JISA_NYUJYO_FREE WHEN '00' THEN '切' WHEN '01' THEN '入' END) END) AS JISA_NYUJYO_FREE, 
                                  (CASE WHEN T .JISA_SYUTUJYO_FREE IS NULL THEN '　' ELSE (CASE JISA_SYUTUJYO_FREE WHEN '00' THEN '切' WHEN '01' THEN '入' END) END) 
                                  AS JISA_SYUTUJYO_FREE,
                                  (CASE T .SYSTEMSYOGAI WHEN '00' THEN '切' WHEN '01' THEN '入' WHEN '50' THEN '未' ELSE '-' END) AS SYSTEMSYOGAI,
                                  (CASE T .NYUJYOTORIKESHI_MODE WHEN '00' THEN '切' WHEN '01' THEN '入' ELSE '　' END) AS NYUJYOTORIKESHI_MODE
FROM                     dbo.D_KSB_CONFIG AS T INNER JOIN
                                  dbo.v_corner_mast AS V2 ON T.RAIL_SECTION_CODE + T.STATION_ORDER_CODE = V2.STATION_CODE AND T.CORNER_CODE = V2.CORNER_CODE
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[18] 4[12] 2[66] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = -480
         Left = 0
      End
      Begin Tables = 
         Begin Table = "T"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 125
               Right = 299
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V1"
            Begin Extent = 
               Top = 126
               Left = 38
               Bottom = 215
               Right = 205
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "V2"
            Begin Extent = 
               Top = 126
               Left = 243
               Bottom = 245
               Right = 410
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 11
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 5610
         Alias = 2490
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_KSB_CONFIG4'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_KSB_CONFIG4'
GO
