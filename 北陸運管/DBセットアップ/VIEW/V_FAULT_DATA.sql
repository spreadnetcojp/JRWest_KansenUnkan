USE [EXOPMG]
GO

/****** Object:  View [dbo].[V_FAULT_DATA]    Script Date: 02/05/2014 10:10:28 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_FAULT_DATA]'))
DROP VIEW [dbo].[V_FAULT_DATA]
GO

USE [EXOPMG]
GO

/****** Object:  View [dbo].[V_FAULT_DATA]    Script Date: 02/05/2014 10:10:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[V_FAULT_DATA]
AS
SELECT                  TOP (100) PERCENT dbo.D_FAULT_DATA.RAIL_SECTION_CODE + dbo.D_FAULT_DATA.STATION_ORDER_CODE AS STATION_CODE, 
                                  dbo.v_station_mast.STATION_NAME, dbo.D_FAULT_DATA.CORNER_CODE, dbo.v_corner_mast.CORNER_NAME, dbo.D_FAULT_DATA.MODEL_CODE, 
                                  dbo.v_model_mast.MODEL_NAME, dbo.D_FAULT_DATA.UNIT_NO, dbo.D_FAULT_DATA.OCCUR_DATE, CASE WHEN M_PASSAGE2.KIND IS NULL 
                                  THEN '' ELSE M_PASSAGE2.KIND END AS PASSAGE_FLG, CASE WHEN M_PASSAGE2.NAME IS NULL 
                                  THEN '[' + CAST(D_FAULT_DATA.PASSAGE_DIRECTION AS varchar) + ']' ELSE M_PASSAGE2.NAME END AS PASSAGE_NAME, 
                                  dbo.D_FAULT_DATA.ERROR_TYPE, dbo.D_FAULT_DATA.ACT_STEP, dbo.D_FAULT_DATA.ERR_CODE, dbo.D_FAULT_DATA.ERR_ITEM, 
                                  dbo.D_FAULT_DATA.ERROR_KIND, dbo.D_FAULT_DATA.DTL_INFO, dbo.D_FAULT_DATA.RES_INFO
FROM                     dbo.D_FAULT_DATA INNER JOIN
                                  dbo.v_station_mast ON 
                                  dbo.D_FAULT_DATA.RAIL_SECTION_CODE + dbo.D_FAULT_DATA.STATION_ORDER_CODE = dbo.v_station_mast.STATION_CODE INNER JOIN
                                  dbo.v_corner_mast ON 
                                  dbo.D_FAULT_DATA.RAIL_SECTION_CODE + dbo.D_FAULT_DATA.STATION_ORDER_CODE = dbo.v_corner_mast.STATION_CODE AND 
                                  dbo.D_FAULT_DATA.CORNER_CODE = dbo.v_corner_mast.CORNER_CODE INNER JOIN
                                  dbo.v_model_mast ON 
                                  dbo.D_FAULT_DATA.RAIL_SECTION_CODE + dbo.D_FAULT_DATA.STATION_ORDER_CODE = dbo.v_model_mast.STATION_CODE AND 
                                  dbo.D_FAULT_DATA.CORNER_CODE = dbo.v_model_mast.CORNER_CODE AND 
                                  dbo.D_FAULT_DATA.MODEL_CODE = dbo.v_model_mast.MODEL_CODE LEFT OUTER JOIN
                                  dbo.M_PASSAGE2 ON dbo.D_FAULT_DATA.MODEL_CODE = dbo.M_PASSAGE2.MODEL_CODE AND 
                                  dbo.D_FAULT_DATA.PASSAGE_DIRECTION = dbo.M_PASSAGE2.KIND
ORDER BY           STATION_CODE, dbo.D_FAULT_DATA.CORNER_CODE

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[58] 4[13] 2[28] 3) )"
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
         Begin Table = "D_FAULT_DATA"
            Begin Extent = 
               Top = 6
               Left = 12
               Bottom = 270
               Right = 220
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "v_station_mast"
            Begin Extent = 
               Top = 0
               Left = 585
               Bottom = 89
               Right = 752
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "v_corner_mast"
            Begin Extent = 
               Top = 90
               Left = 588
               Bottom = 209
               Right = 755
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "v_model_mast"
            Begin Extent = 
               Top = 244
               Left = 587
               Bottom = 363
               Right = 754
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "M_PASSAGE2"
            Begin Extent = 
               Top = 78
               Left = 787
               Bottom = 301
               Right = 984
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
      Begin ColumnWidths = 9
         Width = 284
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
         Column = 6990
      ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_FAULT_DATA'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'   Alias = 1965
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_FAULT_DATA'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_FAULT_DATA'
GO


