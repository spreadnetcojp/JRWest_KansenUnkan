USE [EXOPMG]
GO

/****** Object:  View [dbo].[V_FUNSHITSU_DATA]    Script Date: 07/01/2013 13:37:08 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_FUNSHITSU_DATA]'))
DROP VIEW [dbo].[V_FUNSHITSU_DATA]
GO

USE [EXOPMG]
GO

/****** Object:  View [dbo].[V_FUNSHITSU_DATA]    Script Date: 07/01/2013 13:37:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[V_FUNSHITSU_DATA]
AS
SELECT                  dbo.D_FUNSHITSU_DATA.RAIL_SECTION_CODE + dbo.D_FUNSHITSU_DATA.STATION_ORDER_CODE AS STATION_CODE, 
                                  dbo.v_station_mast.STATION_NAME, dbo.D_FUNSHITSU_DATA.CORNER_CODE, dbo.v_corner_mast.CORNER_NAME, 
                                  dbo.D_FUNSHITSU_DATA.PROCESSING_TIME, dbo.D_FUNSHITSU_DATA.UNIT_NO, CASE WHEN M_PASSAGE.FLG IS NULL 
                                  THEN '' ELSE M_PASSAGE.FLG END AS PASSAGE_FLG, CASE WHEN M_PASSAGE.NAME IS NULL 
                                  THEN '[' + CAST(D_FUNSHITSU_DATA.PASSAGE_DIRECTION AS varchar) + ']' ELSE M_PASSAGE.NAME END AS PASSAGE_NAME, 
                                  CASE WHEN M_ISSUE_ORGANIZE.ORGANIZE_NAME IS NULL THEN '[' + CAST(D_FUNSHITSU_DATA.RAIL_CODE AS varchar) 
                                  + ']' ELSE M_ISSUE_ORGANIZE.ORGANIZE_NAME END AS ORGANIZE_NAME, CASE WHEN M_TICKET_KIND.NAME IS NULL 
                                  THEN '[' + CAST(D_FUNSHITSU_DATA.TICKET_NO AS varchar) + ']' ELSE M_TICKET_KIND.NAME END AS TICKET_NAME, 
                                  dbo.D_FUNSHITSU_DATA.ID_NO, dbo.D_FUNSHITSU_DATA.TICKET_NO, dbo.D_FUNSHITSU_DATA.RAIL_CODE
FROM                     dbo.D_FUNSHITSU_DATA LEFT OUTER JOIN
                                  dbo.M_TICKET_KIND ON dbo.D_FUNSHITSU_DATA.TICKET_NO = dbo.M_TICKET_KIND.NO AND dbo.M_TICKET_KIND.KIND = '1' LEFT OUTER JOIN
                                  dbo.M_PASSAGE ON dbo.D_FUNSHITSU_DATA.PASSAGE_DIRECTION = dbo.M_PASSAGE.KIND LEFT OUTER JOIN
                                  dbo.M_ISSUE_ORGANIZE ON dbo.M_ISSUE_ORGANIZE.ORGANIZE_NO = dbo.D_FUNSHITSU_DATA.RAIL_CODE INNER JOIN
                                  dbo.v_station_mast ON 
                                  dbo.D_FUNSHITSU_DATA.RAIL_SECTION_CODE + dbo.D_FUNSHITSU_DATA.STATION_ORDER_CODE = dbo.v_station_mast.STATION_CODE INNER JOIN
                                  dbo.v_corner_mast ON 
                                  dbo.D_FUNSHITSU_DATA.RAIL_SECTION_CODE + dbo.D_FUNSHITSU_DATA.STATION_ORDER_CODE = dbo.v_corner_mast.STATION_CODE AND 
                                  dbo.D_FUNSHITSU_DATA.CORNER_CODE = dbo.v_corner_mast.CORNER_CODE

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[31] 4[4] 2[49] 3) )"
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
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "M_TICKET_KIND"
            Begin Extent = 
               Top = 6
               Left = 284
               Bottom = 125
               Right = 481
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "M_PASSAGE"
            Begin Extent = 
               Top = 6
               Left = 519
               Bottom = 125
               Right = 716
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "M_ISSUE_ORGANIZE"
            Begin Extent = 
               Top = 126
               Left = 338
               Bottom = 245
               Right = 535
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "v_station_mast"
            Begin Extent = 
               Top = 126
               Left = 38
               Bottom = 215
               Right = 205
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "v_corner_mast"
            Begin Extent = 
               Top = 126
               Left = 573
               Bottom = 245
               Right = 740
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "D_FUNSHITSU_DATA"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 125
               Right = 246
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
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
  ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_FUNSHITSU_DATA'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'       Column = 4665
         Alias = 900
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_FUNSHITSU_DATA'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_FUNSHITSU_DATA'
GO

