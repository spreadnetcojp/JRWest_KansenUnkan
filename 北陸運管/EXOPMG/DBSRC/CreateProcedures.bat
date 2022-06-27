FOR /F %%a IN (ProcedureList.txt) DO (
   sqlcmd -S %1 -d %2 -U %3 -P %4 -i .\Procedure\%%a
)
