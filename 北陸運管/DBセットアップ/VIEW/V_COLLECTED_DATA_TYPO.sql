USE [EXOPMG]
GO

/****** Object:  View [dbo].[V_COLLECTED_DATA_TYPO]    Script Date: 05/22/2013 19:37:03 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_COLLECTED_DATA_TYPO]'))
DROP VIEW [dbo].[V_COLLECTED_DATA_TYPO]
GO

USE [EXOPMG]
GO

/****** Object:  View [dbo].[V_COLLECTED_DATA_TYPO]    Script Date: 05/22/2013 19:37:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[V_COLLECTED_DATA_TYPO]
AS
SELECT                  TOP (100) PERCENT dbo.D_COLLECTED_DATA_TYPO.RAIL_SECTION_CODE + dbo.D_COLLECTED_DATA_TYPO.STATION_ORDER_CODE AS STATION_CODE, 
                                  dbo.v_station_mast.STATION_NAME, dbo.D_COLLECTED_DATA_TYPO.CORNER_CODE, dbo.v_corner_mast.CORNER_NAME, dbo.D_COLLECTED_DATA_TYPO.DATA_KIND, 
                                  dbo.D_COLLECTED_DATA_TYPO.PROCESSING_TIME, dbo.D_COLLECTED_DATA_TYPO.ERROR_INFO, dbo.D_COLLECTED_DATA_TYPO.MODEL_CODE
FROM                     dbo.D_COLLECTED_DATA_TYPO INNER JOIN
                                  dbo.v_station_mast ON 
                                  dbo.D_COLLECTED_DATA_TYPO.RAIL_SECTION_CODE + dbo.D_COLLECTED_DATA_TYPO.STATION_ORDER_CODE = dbo.v_station_mast.STATION_CODE INNER JOIN
                                  dbo.v_corner_mast ON 
                                  dbo.D_COLLECTED_DATA_TYPO.RAIL_SECTION_CODE + dbo.D_COLLECTED_DATA_TYPO.STATION_ORDER_CODE = dbo.v_corner_mast.STATION_CODE AND 
                                  dbo.D_COLLECTED_DATA_TYPO.CORNER_CODE = dbo.v_corner_mast.CORNER_CODE
ORDER BY           STATION_CODE, dbo.D_COLLECTED_DATA_TYPO.CORNER_CODE


GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[10] 2[31] 3) )"
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
         Begin Table = "D_COLLECTED_DATA_TYPO"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 125
               Right = 246
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
               Top = 6
               Left = 489
               Bottom = 125
               Right = 656
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
         Column = 1440
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_COLLECTED_DATA_TYPO'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_COLLECTED_DATA_TYPO'
GO

