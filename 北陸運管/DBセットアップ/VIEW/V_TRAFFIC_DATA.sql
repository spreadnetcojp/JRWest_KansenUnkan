USE [EXOPMG]
GO

/****** Object:  View [dbo].[V_TRAFFIC_DATA]    Script Date: 05/22/2013 19:39:01 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_TRAFFIC_DATA]'))
DROP VIEW [dbo].[V_TRAFFIC_DATA]
GO

USE [EXOPMG]
GO

/****** Object:  View [dbo].[V_TRAFFIC_DATA]    Script Date: 05/22/2013 19:39:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[V_TRAFFIC_DATA]
AS
SELECT                  TOP (100) PERCENT dbo.D_TRAFFIC_DATA.RAIL_SECTION_CODE + dbo.D_TRAFFIC_DATA.STATION_ORDER_CODE AS STATION_CODE, 
                                  dbo.v_station_mast.STATION_NAME, dbo.D_TRAFFIC_DATA.CORNER_CODE, dbo.v_corner_mast.CORNER_NAME, dbo.D_TRAFFIC_DATA.DATE, 
                                  dbo.D_TRAFFIC_DATA.TIME_ZONE, dbo.D_TRAFFIC_DATA.TICKET_NO, CASE WHEN M_TICKET_KIND.NAME IS NULL 
                                  THEN '[' + CAST(D_TRAFFIC_DATA.TICKET_NO AS varchar) + ']' ELSE M_TICKET_KIND.NAME END AS TICKET_NAME, dbo.D_TRAFFIC_DATA.STATION_IN, 
                                  dbo.D_TRAFFIC_DATA.STATION_OUT, dbo.D_TRAFFIC_DATA.STATION_SUM
FROM                     dbo.D_TRAFFIC_DATA INNER JOIN
                                  dbo.v_station_mast ON 
                                  dbo.D_TRAFFIC_DATA.RAIL_SECTION_CODE + dbo.D_TRAFFIC_DATA.STATION_ORDER_CODE = dbo.v_station_mast.STATION_CODE INNER JOIN
                                  dbo.v_corner_mast ON 
                                  dbo.D_TRAFFIC_DATA.RAIL_SECTION_CODE + dbo.D_TRAFFIC_DATA.STATION_ORDER_CODE = dbo.v_corner_mast.STATION_CODE AND 
                                  dbo.D_TRAFFIC_DATA.CORNER_CODE = dbo.v_corner_mast.CORNER_CODE LEFT OUTER JOIN
                                  dbo.M_TICKET_KIND ON dbo.D_TRAFFIC_DATA.TICKET_NO = dbo.M_TICKET_KIND.NO AND dbo.M_TICKET_KIND.KIND = '3'
ORDER BY           STATION_CODE, dbo.D_TRAFFIC_DATA.CORNER_CODE


GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[19] 4[4] 2[58] 3) )"
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
         Begin Table = "D_TRAFFIC_DATA"
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
               Left = 519
               Bottom = 95
               Right = 686
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "v_corner_mast"
            Begin Extent = 
               Top = 96
               Left = 519
               Bottom = 215
               Right = 686
            End
            DisplayFlags = 280
            TopColumn = 0
         End
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_TRAFFIC_DATA'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_TRAFFIC_DATA'
GO

