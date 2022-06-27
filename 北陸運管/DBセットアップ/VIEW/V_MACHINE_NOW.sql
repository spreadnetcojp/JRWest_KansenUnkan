USE [EXOPMG]
GO

/****** Object:  View [dbo].[V_MACHINE_NOW]    Script Date: 04/24/2014 16:46:26 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[V_MACHINE_NOW]'))
DROP VIEW [dbo].[V_MACHINE_NOW]
GO

USE [EXOPMG]
GO

/****** Object:  View [dbo].[V_MACHINE_NOW]    Script Date: 04/24/2014 16:46:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[V_MACHINE_NOW]
AS
SELECT                  CD.GROUP_NO, CD.BRANCH_OFFICE_CODE, NM.STATION_NAME, CD.RAIL_SECTION_CODE, CD.STATION_ORDER_CODE, NM.CORNER_NAME, 
                                  CD.CORNER_CODE, NM.MODEL_NAME, CD.MODEL_CODE, CD.UNIT_NO, NM.ADDRESS, NM.MONITOR_ADDRESS, NM.Y_AREA_CODE, 
                                  NM.G_AREA_CODE, NM.W_AREA_CODE
FROM                     (SELECT                  dbo.M_BRANCH_OFFICE.GROUP_NO, dbo.M_MACHINE.BRANCH_OFFICE_CODE, dbo.M_MACHINE.RAIL_SECTION_CODE, 
                                                                      dbo.M_MACHINE.STATION_ORDER_CODE, dbo.M_MACHINE.CORNER_CODE, dbo.M_MACHINE.MODEL_CODE, 
                                                                      dbo.M_MACHINE.UNIT_NO
                                    FROM                     dbo.M_MACHINE RIGHT OUTER JOIN
                                                                      dbo.M_BRANCH_OFFICE ON dbo.M_MACHINE.BRANCH_OFFICE_CODE = dbo.M_BRANCH_OFFICE.CODE
                                    WHERE                   (dbo.M_MACHINE.SETTING_START_DATE =
                                                                          (SELECT                  MAX(SETTING_START_DATE) AS Expr1
                                                                                FROM                     dbo.M_MACHINE AS M_MACHINE_1
                                                                                WHERE                   (SETTING_START_DATE <= CONVERT(VARCHAR, GETDATE(), 112))))
                                    GROUP BY          dbo.M_BRANCH_OFFICE.GROUP_NO, dbo.M_MACHINE.BRANCH_OFFICE_CODE, dbo.M_MACHINE.RAIL_SECTION_CODE, 
                                                                      dbo.M_MACHINE.STATION_ORDER_CODE, dbo.M_MACHINE.CORNER_CODE, dbo.M_MACHINE.MODEL_CODE, 
                                                                      dbo.M_MACHINE.UNIT_NO) AS CD LEFT OUTER JOIN
                                      (SELECT                  INSERT_DATE, INSERT_USER_ID, INSERT_MACHINE_ID, UPDATE_DATE, UPDATE_USER_ID, UPDATE_MACHINE_ID, 
                                                                              SETTING_START_DATE, SETTING_END_DATE, BRANCH_OFFICE_CODE, MONITOR_STATION_NAME, MONITOR_RAIL_SECTION_CODE, 
                                                                              MONITOR_STATION_ORDER_CODE, STATION_NAME, RAIL_SECTION_CODE, STATION_ORDER_CODE, CORNER_NAME, 
                                                                              CORNER_CODE, MODEL_NAME, MODEL_CODE, UNIT_NO, ADDRESS, SUBNET_MASK, DEFAULT_GW, MONITOR_MODEL_NAME, 
                                                                              MONITOR_ADDRESS, Y_AREA_CODE, G_AREA_CODE, W_AREA_CODE, NK_PORT_NO
                                            FROM                     dbo.M_MACHINE AS M_MACHINE_2
                                            WHERE                   (SETTING_START_DATE =
                                                                                  (SELECT                  MAX(SETTING_START_DATE) AS Expr1
                                                                                        FROM                     dbo.M_MACHINE AS M_MACHINE_1
                                                                                        WHERE                   (SETTING_START_DATE <= CONVERT(VARCHAR, GETDATE(), 112))))) AS NM ON 
                                  CD.BRANCH_OFFICE_CODE = NM.BRANCH_OFFICE_CODE AND CD.RAIL_SECTION_CODE = NM.RAIL_SECTION_CODE AND 
                                  CD.STATION_ORDER_CODE = NM.STATION_ORDER_CODE AND CD.CORNER_CODE = NM.CORNER_CODE AND 
                                  CD.MODEL_CODE = NM.MODEL_CODE AND CD.UNIT_NO = NM.UNIT_NO

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[25] 4[17] 2[44] 3) )"
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
         Begin Table = "NM"
            Begin Extent = 
               Top = 0
               Left = 438
               Bottom = 245
               Right = 700
            End
            DisplayFlags = 280
            TopColumn = 9
         End
         Begin Table = "CD"
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_MACHINE_NOW'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'V_MACHINE_NOW'
GO

