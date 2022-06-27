USE [EXOPMG]
GO

/****** Object:  View [dbo].[V_FUSEI_JOSHA_DATA]    Script Date: 07/01/2013 13:50:50 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_FUSEI_JOSHA_DATA]'))
DROP VIEW [dbo].[V_FUSEI_JOSHA_DATA]
GO

USE [EXOPMG]
GO

/****** Object:  View [dbo].[V_FUSEI_JOSHA_DATA]    Script Date: 07/01/2013 13:50:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[V_FUSEI_JOSHA_DATA]
AS
SELECT                  TOP (100) PERCENT dbo.D_FUSEI_JOSHA_DATA.RAIL_SECTION_CODE + dbo.D_FUSEI_JOSHA_DATA.STATION_ORDER_CODE AS STATION_CODE, 
                                  dbo.v_station_mast.STATION_NAME, dbo.D_FUSEI_JOSHA_DATA.CORNER_CODE, dbo.v_corner_mast.CORNER_NAME, 
                                  dbo.D_FUSEI_JOSHA_DATA.PROCESSING_TIME, dbo.D_FUSEI_JOSHA_DATA.UNIT_NO, CASE WHEN M_PASSAGE.FLG IS NULL 
                                  THEN '' ELSE M_PASSAGE.FLG END AS PASSAGE_FLG, CASE WHEN M_PASSAGE.NAME IS NULL 
                                  THEN '[' + CAST(D_FUSEI_JOSHA_DATA.PASSAGE_DIRECTION AS varchar) + ']' ELSE M_PASSAGE.NAME END AS PASSAGE_NAME, 
                                  dbo.D_FUSEI_JOSHA_DATA.WRANG_TARGET_NO, CASE WHEN M_WARN_TICKET.WRANG_TARGET_NAME IS NULL 
                                  THEN '[' + CAST(D_FUSEI_JOSHA_DATA.WRANG_TARGET_NO AS varchar) 
                                  + ']' ELSE M_WARN_TICKET.WRANG_TARGET_NAME END AS WRANG_TARGET_NAME
FROM                     dbo.D_FUSEI_JOSHA_DATA LEFT OUTER JOIN
                                  dbo.M_PASSAGE ON dbo.D_FUSEI_JOSHA_DATA.PASSAGE_DIRECTION = dbo.M_PASSAGE.KIND LEFT OUTER JOIN
                                  dbo.M_WARN_TICKET ON dbo.M_WARN_TICKET.WRANG_TARGET_NO = dbo.D_FUSEI_JOSHA_DATA.WRANG_TARGET_NO INNER JOIN
                                  dbo.v_station_mast ON 
                                  dbo.D_FUSEI_JOSHA_DATA.RAIL_SECTION_CODE + dbo.D_FUSEI_JOSHA_DATA.STATION_ORDER_CODE = dbo.v_station_mast.STATION_CODE INNER
                                   JOIN
                                  dbo.v_corner_mast ON 
                                  dbo.D_FUSEI_JOSHA_DATA.RAIL_SECTION_CODE + dbo.D_FUSEI_JOSHA_DATA.STATION_ORDER_CODE = dbo.v_corner_mast.STATION_CODE AND 
                                  dbo.D_FUSEI_JOSHA_DATA.CORNER_CODE = dbo.v_corner_mast.CORNER_CODE
ORDER BY           STATION_CODE, dbo.D_FUSEI_JOSHA_DATA.CORNER_CODE, dbo.M_PASSAGE.FLG, dbo.D_FUSEI_JOSHA_DATA.PROCESSING_TIME, 
                                  dbo.D_FUSEI_JOSHA_DATA.UNIT_NO

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[27] 4[24] 2[36] 3) )"
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
         Begin Table = "M_PASSAGE"
            Begin Extent = 
               Top = 6
               Left = 584
               Bottom = 125
               Right = 781
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "M_WARN_TICKET"
            Begin Extent = 
               Top = 126
               Left = 38
               Bottom = 245
               Right = 244
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "v_station_mast"
            Begin Extent = 
               Top = 6
               Left = 284
               Bottom = 95
               Right = 451
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "v_corner_mast"
            Begin Extent = 
               Top = 96
               Left = 284
               Bottom = 215
               Right = 451
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "D_FUSEI_JOSHA_DATA"
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
         Column = 8895
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
         ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_FUSEI_JOSHA_DATA'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_FUSEI_JOSHA_DATA'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_FUSEI_JOSHA_DATA'
GO

